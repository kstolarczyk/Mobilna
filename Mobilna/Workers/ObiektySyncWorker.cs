using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using AndroidX.Work;
using Core;
using Core.Models;
using Core.Services;
using Microsoft.EntityFrameworkCore;

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

        private void UpdateLocalDb(List<Obiekt> obiekty, MyDbContext context)
        {
            var map = context.Obiekty.AsNoTracking().Where(o => o.RemoteId != null).ToDictionary(o => o.RemoteId, o => o.ObiektId);
            obiekty.ForEach(o => o.ObiektId = map.ContainsKey(o.RemoteId!) ? map[o.RemoteId] : default);
            var nieUsuwane = obiekty.Where(o => !o.Usuniety).ToList();
            var usunieteId = obiekty.Where(o => o.Usuniety).Select(o => o.RemoteId).ToArray();

            context.Obiekty.RemoveRange(context.Obiekty.Where(o => usunieteId.Contains(o.RemoteId)));
            context.Obiekty.UpdateRange(nieUsuwane.Where(o => o.ObiektId != default));
            context.Obiekty.AddRange(nieUsuwane.Where(o => o.ObiektId == default));
            context.SaveChanges();
        }

        public ObiektySyncWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
        }
    }
}