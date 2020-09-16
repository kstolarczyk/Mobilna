using Android.Content;
using AndroidX.Work;
using Core;

namespace Mobilna.Workers
{
    public abstract class BaseWorker : Worker
    {
        static BaseWorker()
        {
            lock (App.Mutex)
            {
                App.DbInitialization ??= App.InitializeDatabase();
            }
            App.DbInitialization.Wait();
        }

        protected BaseWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
        }
    }
}