using System;
using Android.App;
using Android.Runtime;
using AndroidX.Work;
using Core;
using Mobilna.Workers;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.Platforms.Android.Views;

namespace Mobilna
{
    [Application]
    public class MainApplication : MvxAndroidApplication<MvxAndroidSetup<App>, App>
    {
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            var request = PeriodicWorkRequest.Builder.From<DbSyncWorker>(TimeSpan.FromMinutes(1)).Build();
            WorkManager.GetInstance(this).CancelAllWork();
            WorkManager.GetInstance(this).Enqueue(request);
        }
    }
}