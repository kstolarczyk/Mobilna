using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using Core.Services;
using Microsoft.EntityFrameworkCore;

namespace Core.Helpers
{
    public delegate void GlobalEvent();

    public delegate void GlobalEvent<in T>(T parameter);

    public class ObiektSynchronizer
    {
        public static bool IsSynchronizing { get; private set; }
        public static readonly object Mutex = new object();

        public static async Task SynchronizeObiekty()
        {
            lock (Mutex)
            {
                if (IsSynchronizing)
                {
                    return;
                }

                IsSynchronizing = true;
                SynchronizingChanged?.Invoke();
            }

            try
            {
                await using var context = new MyDbContext();
                var webService = new WebService(context);
                await SendChanges(context, webService).ConfigureAwait(false);
                await GetChanges(context, webService).ConfigureAwait(false);
            }
            finally
            {
                IsSynchronizing = false;
                SynchronizingChanged?.Invoke();
            }
        }

        private static async Task SendChanges(MyDbContext context, WebService webService)
        {
            var obiektyDodaj = context.Obiekty.Where(o => o.Status == 1)
                .Include(o => o.Parametry).AsAsyncEnumerable();
            var obiektyEdytuj = context.Obiekty.Where(o => o.Status == 2)
                .Include(o => o.Parametry).AsAsyncEnumerable();
            var obiektyUsun = context.Obiekty.Where(o => o.Status == 3).AsAsyncEnumerable();

            await DodajObiektyAsync(webService, obiektyDodaj).ConfigureAwait(false);
            await EdytujObiektyAsync(webService, obiektyEdytuj).ConfigureAwait(false);
            await UsunObiektyAsync(context, webService, obiektyUsun).ConfigureAwait(false);
            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private static async Task UsunObiektyAsync(MyDbContext context, WebService webService,
            IAsyncEnumerable<Obiekt> obiektyUsun)
        {
            await foreach (var obiekt in webService.SendDeleteObiektyAsync(obiektyUsun).ConfigureAwait(false))
            {
                context.Obiekty.Remove(obiekt);
            }
        }

        private static async Task EdytujObiektyAsync(WebService webService, IAsyncEnumerable<Obiekt> obiektyEdytuj)
        {
            using var sftp = new SftpService();
            var localDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            await foreach (var obiekt in webService.SendUpdateObiektyAsync(obiektyEdytuj).ConfigureAwait(false))
            {
                obiekt.Status = 0;
                if (obiekt.ZdjecieLokal == obiekt.Zdjecie) continue;
                if (!string.IsNullOrEmpty(obiekt.ZdjecieLokal) && !await sftp
                    .SendFileAsync(Path.Combine(localDir, obiekt.ZdjecieLokal), obiekt.ZdjecieLokal)
                    .ConfigureAwait(false))
                {
                    obiekt.Status = 2;
                }
                else
                {
                    obiekt.Zdjecie = obiekt.ZdjecieLokal;
                }
            }
        }

        private static async Task DodajObiektyAsync(WebService webService, IAsyncEnumerable<Obiekt> obiektyDodaj)
        {
            using var sftp = new SftpService();
            var localDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            await foreach (var obiekt in webService.SendNewObiektyAsync(obiektyDodaj).ConfigureAwait(false))
            {
                obiekt.Status = 0;
                if (!string.IsNullOrEmpty(obiekt.ZdjecieLokal) && !await sftp
                    .SendFileAsync(Path.Combine(localDir, obiekt.ZdjecieLokal), obiekt.ZdjecieLokal)
                    .ConfigureAwait(false))
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
                webService.GetObiektyAsync(lastUpdate,
                    context.GrupyObiektow.AsNoTracking().Select(g => g.GrupaObiektowId)).ConfigureAwait(false);
            if (obiekty.Count <= 0) return;
            if (!await CzySpojne(obiekty, context).ConfigureAwait(false))
            {
                await context.ClearExceptUserAsync().ConfigureAwait(false);
                await GrupaSynchronizer.SynchronizeGrupy().ConfigureAwait(false);
                context = new MyDbContext();
                obiekty = await webService
                    .GetObiektyAsync(null, context.GrupyObiektow.AsNoTracking().Select(g => g.GrupaObiektowId))
                    .ConfigureAwait(false);
            }
            await UpdateLocalDb(obiekty, context).ConfigureAwait(false);
            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync().ConfigureAwait(false);
            }

            await context.DisposeAsync().ConfigureAwait(false);
        }

        private static async Task<bool> CzySpojne(List<Obiekt> obiekty, MyDbContext context)
        {
            var remoteGrupyIds = obiekty.Select(o => o.GrupaObiektowId).Distinct().ToList();
            var localGrupy = await context.GrupyObiektow.AsNoTracking()
                .Where(g => remoteGrupyIds.Contains(g.GrupaObiektowId)).Include(g => g.TypyParametrow)
                .ToListAsync().ConfigureAwait(false);
            if (localGrupy.Count != remoteGrupyIds.Count) return false;
            var grupyParametryMap = localGrupy.ToDictionary(g => g.GrupaObiektowId,
                g => g.TypyParametrow.Select(t => t.TypParametrowId).ToList());
            return !obiekty.AsParallel().Any(o =>
                o.Parametry.Count != grupyParametryMap[o.GrupaObiektowId].Count || o.Parametry.Any(p =>
                    !grupyParametryMap[o.GrupaObiektowId].Contains(p.TypParametrowId)));
        }

        private static async Task UpdateLocalDb(List<Obiekt> obiekty, MyDbContext context)
        {
            var map = await context.Obiekty.AsNoTracking().Where(o => o.RemoteId != null)
                .ToDictionaryAsync(o => o.RemoteId, o => o.ObiektId).ConfigureAwait(false);
            obiekty.ForEach(o => o.ObiektId = map.ContainsKey(o.RemoteId!) ? map[o.RemoteId] : default);
            var nieUsuwane = obiekty.Where(o => !o.Usuniety).ToList();
            var usunieteId = obiekty.Where(o => o.Usuniety).Select(o => o.RemoteId).ToArray();

            using var sftpService = new SftpService();
            if (usunieteId.Length > 0)
            {
                await RemoveObiekty(context, context.Obiekty.AsNoTracking().Where(o => usunieteId.Contains(o.RemoteId)))
                    .ConfigureAwait(false);
            }

            if (nieUsuwane.Count <= 0) return;

            await UpdateObiekty(sftpService, context, nieUsuwane.Where(o => o.ObiektId != default))
                .ConfigureAwait(false);
            await AddObiekty(sftpService, context, nieUsuwane.Where(o => o.ObiektId == default)).ConfigureAwait(false);
        }

        private static async Task AddObiekty(SftpService sftpService, MyDbContext context, IEnumerable<Obiekt> obiekty)
        {
            var user = await context.Users.AsNoTracking().FirstAsync().ConfigureAwait(false);
            var list = obiekty.ToList();
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            foreach (var obiekt in list)
            {
                if (obiekt.UserId != user.UserId) obiekt.UserId = null;
                if (string.IsNullOrEmpty(obiekt.Zdjecie))
                {
                    context.Add(obiekt);
                    continue;
                }

                using var outStream = new FileStream(Path.Combine(localFolder, obiekt.Zdjecie), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                if (await sftpService.DownloadFileAsync(obiekt.Zdjecie, outStream).ConfigureAwait(false))
                {
                    obiekt.ZdjecieLokal = obiekt.Zdjecie;
                }

                context.Add(obiekt);
            }
        }

        private static async Task UpdateObiekty(SftpService sftpService, MyDbContext context,
            IEnumerable<Obiekt> obiekty)
        {
            var list = obiekty.ToList();
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            foreach (var obiekt in list)
            {
                var entity = await context.Obiekty.Include(o => o.Parametry)
                    .FirstOrDefaultAsync(o => o.ObiektId == obiekt.ObiektId).ConfigureAwait(false);
                if (entity == null) continue;
                if (string.IsNullOrEmpty(obiekt.Zdjecie) || entity.ZdjecieLokal == obiekt.Zdjecie)
                {
                    entity.Update(obiekt);
                    continue;
                }

                if (!string.IsNullOrEmpty(obiekt.ZdjecieLokal) &&
                    File.Exists(Path.Combine(localFolder, obiekt.ZdjecieLokal)))
                {
                    File.Delete(Path.Combine(localFolder, obiekt.ZdjecieLokal));
                }
                using var outStream = new FileStream(Path.Combine(localFolder, obiekt.Zdjecie), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                if (await sftpService.DownloadFileAsync(obiekt.Zdjecie, outStream).ConfigureAwait(false))
                {
                    obiekt.ZdjecieLokal = obiekt.Zdjecie;
                }

                entity.Update(obiekt);
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
        }

        public static event GlobalEvent SynchronizingChanged;
    }
}