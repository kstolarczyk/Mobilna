using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using AndroidX.Work;
using Core;
using Core.Models;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using SshNet.Security.Cryptography;
using SHA256 = System.Security.Cryptography.SHA256;

namespace Mobilna.Workers
{
    public class ObiektySyncWorker : BaseWorker
    {
        public override Result DoWork()
        {
            try
            {
                var task = SynchronizeObiekty();
                task.Wait();
                return task.IsFaulted ? Result.InvokeFailure() : Result.InvokeSuccess();
            }
            catch (Exception e)
            {
                return Result.InvokeFailure();
            }
            
        }

        public static async Task SynchronizeObiekty()
        {
            await using var context = new MyDbContext();
            var webService = new WebService(context);
            await SendChanges(context, webService).ConfigureAwait(false);
            await GetChanges(context, webService).ConfigureAwait(false);
        }

        private static async Task SendChanges(MyDbContext context, WebService webService)
        {
            var obiektyDodaj = context.Obiekty.Where(o => o.Status == 1)
                .Include(o => o.Parametry).AsAsyncEnumerable();
            var obiektyEdytuj = context.Obiekty.Where(o => o.Status == 2)
                .Include(o => o.Parametry).AsAsyncEnumerable();
            var obiektyUsun = context.Obiekty.Where(o => o.Status == 3).AsAsyncEnumerable();

            await DodajObiektyAsync(context, webService, obiektyDodaj).ConfigureAwait(false);
            await EdytujObiektyAsync(context, webService, obiektyEdytuj).ConfigureAwait(false);
            await UsunObiektyAsync(context, webService, obiektyUsun).ConfigureAwait(false);
            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private static async Task UsunObiektyAsync(MyDbContext context, WebService webService, IAsyncEnumerable<Obiekt> obiektyUsun)
        {
            await foreach (var obiekt in webService.SendDeleteObiektyAsync(obiektyUsun).ConfigureAwait(false))
            {
                context.Obiekty.Remove(obiekt);
            }

        }

        private static async Task EdytujObiektyAsync(MyDbContext context, WebService webService, IAsyncEnumerable<Obiekt> obiektyEdytuj)
        {
            using var sftp = new SftpService();
            var localDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            await foreach (var obiekt in webService.SendUpdateObiektyAsync(obiektyEdytuj).ConfigureAwait(false))
            {
                obiekt.Status = 0;
                if (obiekt.ZdjecieLokal == obiekt.Zdjecie) continue;
                if (!string.IsNullOrEmpty(obiekt.ZdjecieLokal) && !await sftp.SendFileAsync(Path.Combine(localDir, obiekt.ZdjecieLokal), obiekt.ZdjecieLokal))
                {
                    obiekt.Status = 2;
                }
                else
                {
                    obiekt.Zdjecie = obiekt.ZdjecieLokal;
                }
            }

        }

        private static async Task DodajObiektyAsync(MyDbContext context, WebService webService, IAsyncEnumerable<Obiekt> obiektyDodaj)
        {
            using var sftp = new SftpService();
            var localDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            await foreach (var obiekt in webService.SendNewObiektyAsync(obiektyDodaj).ConfigureAwait(false))
            {
                obiekt.Status = 0;
                if (!string.IsNullOrEmpty(obiekt.ZdjecieLokal) && !await sftp.SendFileAsync(Path.Combine(localDir, obiekt.ZdjecieLokal), obiekt.ZdjecieLokal))
                {
                    obiekt.Status = 2;
                }
            }

        }

        private static async Task GetChanges(MyDbContext context, WebService webService)
        {
            var lastUpdate = await context.Obiekty.AsNoTracking().OrderByDescending(o => o.OstatniaAktualizacja)
                .Select(o => o.OstatniaAktualizacja).FirstOrDefaultAsync().ConfigureAwait(false);
            var obiekty = await
                webService.GetObiektyAsync(lastUpdate, context.GrupyObiektow.AsNoTracking().Select(g => g.GrupaObiektowId)).ConfigureAwait(false);
            await UpdateLocalDb(obiekty, context).ConfigureAwait(false);
            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private static async Task UpdateLocalDb(List<Obiekt> obiekty, MyDbContext context)
        {
            var map = await context.Obiekty.AsNoTracking().Where(o => o.RemoteId != null).ToDictionaryAsync(o => o.RemoteId, o => o.ObiektId).ConfigureAwait(false);
            obiekty.ForEach(o => o.ObiektId = map.ContainsKey(o.RemoteId!) ? map[o.RemoteId] : default);
            var nieUsuwane = obiekty.Where(o => !o.Usuniety).ToList();
            var usunieteId = obiekty.Where(o => o.Usuniety).Select(o => o.RemoteId).ToArray();

            using var sftpService = new SftpService();
            await RemoveObiekty(context, context.Obiekty.Where(o => usunieteId.Contains(o.RemoteId))).ConfigureAwait(false);
            await UpdateObiekty(sftpService, context, nieUsuwane.Where(o => o.ObiektId != default)).ConfigureAwait(false);
            await AddObiekty(sftpService, context, nieUsuwane.Where(o => o.ObiektId == default)).ConfigureAwait(false);
        }

        private static async Task AddObiekty(SftpService sftpService, MyDbContext context, IEnumerable<Obiekt> obiekty)
        {
            var user = await context.Users.FirstAsync().ConfigureAwait(false);
            var list = obiekty.ToList();
            var hashAlgoritm = HashAlgorithm.Create();
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            foreach (var obiekt in list)
            {
                if (string.IsNullOrEmpty(obiekt.Zdjecie))
                {
                    context.Add(obiekt);
                    continue;
                }
                var ext = obiekt.Zdjecie.Substring(obiekt.Zdjecie.LastIndexOf('.'));
                var hashBytes =
                    hashAlgoritm.ComputeHash(Encoding.ASCII.GetBytes($"{user.Username}{Guid.NewGuid():N}"));
                var localName = hashBytes.Aggregate(new StringBuilder(), (builder, hb) => builder.Append(hb.ToString("X"))).ToString();
                var localImg = $"{localName}{ext}";
                await using var outputStream = File.OpenWrite(Path.Combine(localFolder, localImg));
                if (await sftpService.DownloadFileAsync(obiekt.Zdjecie, outputStream).ConfigureAwait(false))
                {
                    obiekt.ZdjecieLokal = obiekt.Zdjecie = localImg;
                }
                context.Add(obiekt);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        private static async Task UpdateObiekty(SftpService sftpService, MyDbContext context, IEnumerable<Obiekt> obiekty)
        {
            var user = await context.Users.FirstAsync().ConfigureAwait(false);
            var list = obiekty.ToList();
            var hashAlgoritm = HashAlgorithm.Create();
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            foreach (var obiekt in list)
            {
                if (string.IsNullOrEmpty(obiekt.Zdjecie) || context.Obiekty.AsNoTracking().FirstOrDefault(o => o.RemoteId == obiekt.RemoteId)?.ZdjecieLokal == obiekt.Zdjecie)
                {
                    context.Update(obiekt);
                    continue;
                }
                var ext = obiekt.Zdjecie.Substring(obiekt.Zdjecie.LastIndexOf('.'));
                var hashBytes =
                    hashAlgoritm.ComputeHash(Encoding.ASCII.GetBytes($"{user.Username}{Guid.NewGuid():N}"));
                var localName = hashBytes.Aggregate(new StringBuilder(), (builder, hb) => builder.Append(hb.ToString("X"))).ToString();
                var localImg = $"{localName}{ext}";
                await using var outputStream = File.OpenWrite(Path.Combine(localFolder, localImg));
                if (!string.IsNullOrEmpty(obiekt.ZdjecieLokal) &&
                    File.Exists(Path.Combine(localFolder, obiekt.ZdjecieLokal)))
                {
                    File.Delete(Path.Combine(localFolder, obiekt.ZdjecieLokal));
                }
                if (await sftpService.DownloadFileAsync(obiekt.Zdjecie, outputStream).ConfigureAwait(false))
                {
                    obiekt.ZdjecieLokal = obiekt.Zdjecie = localImg;
                }
                context.Update(obiekt);
            }
        }

        private static async Task RemoveObiekty(MyDbContext context, IQueryable<Obiekt> query)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            await foreach (var obiekt in query.AsAsyncEnumerable().ConfigureAwait(false))
            {
                var imgPath = Path.Combine(path, obiekt.ZdjecieLokal);
                if (File.Exists(imgPath))
                {
                    File.Delete(imgPath);
                }
                context.Obiekty.Remove(obiekt);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public ObiektySyncWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
        }
    }
}