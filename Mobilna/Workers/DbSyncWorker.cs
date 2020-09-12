using System;
using System.Linq;
using Android.Content;
using Android.Runtime;
using AndroidX.Work;
using Core.Models;
using Core.Services;
using Microsoft.EntityFrameworkCore.Internal;
using MvvmCross;
using SQLitePCL;

namespace Mobilna.Workers
{
    public class DbSyncWorker : Worker
    {
        private readonly WebService _webService;
        private readonly MyDbContext _context;

        public override Result DoWork()
        {
            var obiektyTask = _webService.GetObiektyAsync(_context.GrupyObiektow.Select(g => g.GrupaObiektowId));
            obiektyTask.Wait();
            if(obiektyTask.IsFaulted) return Result.InvokeRetry();
            var obiekty = obiektyTask.Result;
            return Result.InvokeSuccess();
        }

        public DbSyncWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
            _context = new MyDbContext();
            _webService ??= new WebService(_context);
        }
    }
}