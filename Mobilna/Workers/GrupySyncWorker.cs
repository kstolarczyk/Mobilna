using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Runtime;
using AndroidX.Work;
using Core.Models;
using Core.Services;
using Microsoft.EntityFrameworkCore;

namespace Mobilna.Workers
{
    public class GrupySyncWorker : BaseWorker
    {
        public GrupySyncWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
        }

        public override Result DoWork()
        {
            try
            {
                var syncTask = SynchronizeGrupy();
                syncTask.Wait();
                return syncTask.IsFaulted ? Result.InvokeFailure() : Result.InvokeSuccess();
            }
            catch (Exception e)
            {
                return Result.InvokeFailure();
            }
        }

        public static async Task SynchronizeGrupy()
        {
            await using var context = new MyDbContext();
            var webService = new WebService(context);
            var lastUpdate = await context.GrupyObiektow.AsNoTracking().OrderByDescending(g => g.OstatniaAktualizacja).Select(g => g.OstatniaAktualizacja)
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
            var nieusuwaneTypy = typy.Where(t => !t.Usuniety).ToList();
            var usunieteTypy = typy.Where(t => t.Usuniety).Select(t => t.TypParametrowId).ToArray();

            if (usunieteTypy.Length > 0)
            {
                context.TypyParametrow.RemoveRange(context.TypyParametrow.AsNoTracking()
                    .Where(t => usunieteTypy.Contains(t.TypParametrowId)));
            }

            if (nieusuwaneTypy.Count > 0)
            {
                var typyAdd = nieusuwaneTypy.Where(t => context.TypyParametrow.Find(t.TypParametrowId)?.Update(t) == null);
                await context.TypyParametrow.AddRangeAsync(typyAdd).ConfigureAwait(false);
            }

            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}