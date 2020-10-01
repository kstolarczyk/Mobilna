using System;
using System.Collections.Generic;
using System.Linq;
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
            using var context = new MyDbContext();
            var webService = new WebService(context);
            var lastUpdate = context.GrupyObiektow.AsNoTracking().OrderByDescending(g => g.OstatniaAktualizacja)
                .FirstOrDefault()?.OstatniaAktualizacja;
            var grupyTask = webService.GetGrupyAsync(lastUpdate);
            grupyTask.Wait();
            if(grupyTask.IsFaulted) return Result.InvokeRetry();
            var grupy = grupyTask.Result;
            grupy.ForEach(g => g.Obiekty.Clear());
            try
            {
                UpdateLocalDb(grupy, context);
            }
            catch (Exception e)
            {
                return Result.InvokeFailure();
            }
            return Result.InvokeSuccess();
        }

        private void UpdateLocalDb(List<GrupaObiektow> grupy, MyDbContext context)
        {
            var localGrupy = context.GrupyObiektow.AsNoTracking();
            var nieUsuwane = grupy.Where(g => !g.Usunieta).ToList();
            var usunieteId = grupy.Where(g => g.Usunieta).Select(g => g.GrupaObiektowId).ToArray();
            var typy = nieUsuwane.SelectMany(g => g.TypyParametrow).Distinct().ToList();
            var nieusuwaneTypy = typy.Where(t => !t.Usuniety).ToList();
            var usunieteTypy = typy.Where(t => t.Usuniety).Select(t => t.TypParametrowId).ToArray();
            
            context.TypyParametrow.RemoveRange(context.TypyParametrow.AsNoTracking().Where(t => usunieteTypy.Contains(t.TypParametrowId)));
            var typyAdd = nieusuwaneTypy.Where(t => context.TypyParametrow.Find(t.TypParametrowId)?.Update(t) == null);
            context.TypyParametrow.AddRange(typyAdd);
            context.SaveChanges();

            context.GrupyObiektow.RemoveRange(localGrupy.Where(g => usunieteId.Contains(g.GrupaObiektowId)));
            nieUsuwane.ForEach(g => g.TypyParametrow = g.TypyParametrow.Select(t => context.TypyParametrow.Find(t.TypParametrowId)).ToList());
            var toAdd = nieUsuwane.Where(g => context.GrupyObiektow.Find(g.GrupaObiektowId)?.Update(g) == null);
            context.GrupyObiektow.AddRange(toAdd);
            context.SaveChanges();
        }
    }
}