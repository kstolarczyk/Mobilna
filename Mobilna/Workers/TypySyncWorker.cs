using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using AndroidX.Work;
using Core.Models;
using Core.Services;
using Microsoft.EntityFrameworkCore;

namespace Mobilna.Workers
{
    public class TypySyncWorker : BaseWorker
    {
        public TypySyncWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
        }

        public override Result DoWork()
        {
            using var context = new MyDbContext();
            var webService = new WebService(context);
            var lastUpdate = context.TypyParametrow.AsNoTracking().OrderByDescending(t => t.OstatniaAktualizacja)
                .FirstOrDefault()?.OstatniaAktualizacja;
            var typyTask = webService.GetTypyAsync(lastUpdate);
            typyTask.Wait();
            if(typyTask.IsFaulted) return Result.InvokeRetry();
            var typy = typyTask.Result;
            try
            {
                UpdateLocalDb(typy, context);
            }
            catch (Exception e)
            {
                return Result.InvokeFailure();
            }
            return Result.InvokeSuccess();
        }

        private void UpdateLocalDb(List<TypParametrow> typy, MyDbContext context)
        {
            typy.ForEach(t => t.TypParametrowId = context.TypyParametrow.AsNoTracking().Any(l => l.TypParametrowId == t.TypParametrowId) ? t.TypParametrowId : default);
            var nieUsuwane = typy.Where(t => !t.Usuniety).ToList();
            var usunieteId = typy.Where(t => t.Usuniety).Select(t => t.TypParametrowId).ToArray();
            
            context.TypyParametrow.UpdateRange(nieUsuwane.Where(t => t.TypParametrowId != default));
            context.TypyParametrow.AddRange(nieUsuwane.Where(t => t.TypParametrowId == default));
            context.TypyParametrow.RemoveRange(context.TypyParametrow.Where(t => usunieteId.Contains(t.TypParametrowId)));
            context.SaveChanges();
        }
    }
}