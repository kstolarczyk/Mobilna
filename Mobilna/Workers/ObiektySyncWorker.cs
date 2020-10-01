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
            using var context = new MyDbContext();
            var webService = new WebService(context);
            var lastUpdate = context.Obiekty.AsNoTracking().OrderByDescending(o => o.OstatniaAktualizacja)
                .Select(o => o.OstatniaAktualizacja).FirstOrDefault();
            var obiektyTask = webService.GetObiektyAsync(lastUpdate,context.GrupyObiektow.AsNoTracking().Select(g => g.GrupaObiektowId));
            obiektyTask.Wait();
            if(obiektyTask.IsFaulted) return Result.InvokeRetry();
            var obiekty = obiektyTask.Result;
            try
            {
                UpdateLocalDb(obiekty, context);
            }
            catch (Exception e)
            {
                return Result.InvokeFailure();
            }
            return Result.InvokeSuccess();
        }

        private async void UpdateLocalDb(List<Obiekt> obiekty, MyDbContext context)
        {
            var map = context.Obiekty.AsNoTracking().Where(o => o.RemoteId != null).ToDictionary(o => o.RemoteId, o => o.ObiektId);
            obiekty.ForEach(o => o.ObiektId = map.ContainsKey(o.RemoteId!) ? map[o.RemoteId] : default);
            var nieUsuwane = obiekty.Where(o => !o.Usuniety).ToList();
            var usunieteId = obiekty.Where(o => o.Usuniety).Select(o => o.RemoteId).ToArray();

            var sftpService = new SftpService();
            await RemoveObiekty(context, context.Obiekty.Where(o => usunieteId.Contains(o.RemoteId)));
            await UpdateObiekty(sftpService, context, nieUsuwane.Where(o => o.ObiektId != default));
            await AddObiekty(sftpService, context, nieUsuwane.Where(o => o.ObiektId == default));
        }

        private async Task AddObiekty(SftpService sftpService, MyDbContext context, IEnumerable<Obiekt> obiekty)
        {
            var user = context.Users.First();
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
                var localImg = Path.Combine(localFolder, $"{localName}{ext}");
                await using var outputStream = File.OpenWrite(localImg);
                if (sftpService.DownloadFile(obiekt.Zdjecie, outputStream))
                {
                    obiekt.ZdjecieLokal = localImg;
                }
                context.Add(obiekt);
            }

            await context.SaveChangesAsync();
        }

        private async Task UpdateObiekty(SftpService sftpService, MyDbContext context, IEnumerable<Obiekt> obiekty)
        {
            var user = context.Users.First();
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
                var localImg = Path.Combine(localFolder, $"{localName}{ext}");
                await using var outputStream = File.OpenWrite(localImg);
                if (sftpService.DownloadFile(obiekt.Zdjecie, outputStream))
                {
                    obiekt.ZdjecieLokal = localImg;
                }
                context.Update(obiekt);
            }

            await context.SaveChangesAsync();
        }

        private async Task RemoveObiekty(MyDbContext context, IQueryable<Obiekt> query)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            await foreach (var obiekt in query.AsAsyncEnumerable())
            {
                var imgPath = Path.Combine(path, obiekt.ZdjecieLokal);
                if (File.Exists(imgPath))
                {
                    File.Delete(imgPath);
                }
                context.Obiekty.Remove(obiekt);
            }

            await context.SaveChangesAsync();
        }

        public ObiektySyncWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
        }
    }
}