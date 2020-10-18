using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Services;
using Microsoft.EntityFrameworkCore;

namespace Core.Helpers
{
    public class GrupaSynchronizer
    {
        public static readonly object Mutext = new object();
        public static bool IsSynchronizing { get; private set; }
        public static event GlobalEvent SynchronizingChanged;

        public static async Task SynchronizeGrupy()
        {
            lock (Mutext)
            {
                if (IsSynchronizing) return;
                IsSynchronizing = true;
                SynchronizingChanged?.Invoke();
            }

            try
            {
                await using var context = new MyDbContext();
                var webService = new WebService(context);
                await GetChanges(context, webService).ConfigureAwait(false);
            }
            finally
            {
                IsSynchronizing = false;
                SynchronizingChanged?.Invoke();
            }
        }

        private static async Task GetChanges(MyDbContext context, WebService webService)
        {
            var lastUpdate = await context.GrupyObiektow.AsNoTracking().OrderByDescending(g => g.OstatniaAktualizacja)
                .Select(g => g.OstatniaAktualizacja)
                .FirstOrDefaultAsync().ConfigureAwait(false);
            var grupy = await webService.GetGrupyAsync(lastUpdate).ConfigureAwait(false);
            if (grupy.Count <= 0) return;
            grupy.ForEach(g => g.Obiekty.Clear());
            await UpdateLocalDb(grupy, context).ConfigureAwait(false);
            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private static async Task UpdateLocalDb(List<GrupaObiektow> grupy, MyDbContext context)
        {
            var nieUsuwane = grupy.Where(g => !g.Usunieta).ToList();
            var typy = nieUsuwane.SelectMany(g => g.TypyParametrow).Distinct().ToList();

            await UpdateTypy(context, typy).ConfigureAwait(false);
            await UpdateGrupy(context, grupy, nieUsuwane).ConfigureAwait(false);
        }

        private static async Task UpdateGrupy(MyDbContext context, List<GrupaObiektow> grupy, List<GrupaObiektow> nieUsuwane)
        {
            var localGrupy = context.GrupyObiektow.AsNoTracking();
            var usunieteId = grupy.Where(g => g.Usunieta).Select(g => g.GrupaObiektowId).ToArray();

            if (usunieteId.Length > 0)
            {
                context.GrupyObiektow.RemoveRange(localGrupy.Where(g => usunieteId.Contains(g.GrupaObiektowId)));
            }

            if (nieUsuwane.Count > 0)
            {
                nieUsuwane.ForEach(g =>
                    g.TypyParametrow = g.TypyParametrow.Select(t => context.TypyParametrow.Find(t.TypParametrowId)).ToList());
                var toAdd = nieUsuwane.Where(g => context.GrupyObiektow.Find(g.GrupaObiektowId)?.Update(g) == null);
                await context.GrupyObiektow.AddRangeAsync(toAdd).ConfigureAwait(false);
            }
        }

        private static async Task UpdateTypy(MyDbContext context, List<TypParametrow> typy)
        {
            var toAdd = typy.Where(t => context.TypyParametrow.Find(t.TypParametrowId) == null);
            await context.TypyParametrow.AddRangeAsync(toAdd).ConfigureAwait(false);

            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}