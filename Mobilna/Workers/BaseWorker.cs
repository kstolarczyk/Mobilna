using Android.Content;
using AndroidX.Work;
using Core;

namespace Mobilna.Workers
{
    public abstract class BaseWorker : Worker
    {
        static BaseWorker()
        {
            var initTask = App.InitializeDatabase();
            initTask.Wait();
        }

        protected BaseWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
        }
    }
}