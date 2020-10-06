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
using Core.Helpers;
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
                var task = ObiektSynchronizer.SynchronizeObiekty();
                task.Wait();
                return task.IsFaulted ? Result.InvokeFailure() : Result.InvokeSuccess();
            }
            catch (Exception e)
            {
                return Result.InvokeFailure();
            }
            
        }

        
        public ObiektySyncWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
        }
    }
}