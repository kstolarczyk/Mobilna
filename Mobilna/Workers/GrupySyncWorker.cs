using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Runtime;
using AndroidX.Work;
using Core.Helpers;
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
                var syncTask = GrupaSynchronizer.SynchronizeGrupy();
                syncTask.Wait();
                return syncTask.IsFaulted ? Result.InvokeFailure() : Result.InvokeSuccess();
            }
            catch (Exception e)
            {
                return Result.InvokeFailure();
            }
        }

    }
}