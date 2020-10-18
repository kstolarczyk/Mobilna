using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Runtime;
using AndroidX.Work;
using Core;
using Java.Util.Concurrent;
using Mobilna.Workers;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.Platforms.Android.Views;

namespace Mobilna
{
    [Application]
    public class MainApplication : MvxAndroidApplication<MySetup, App>
    {
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override async void OnCreate()
        {
            base.OnCreate();
            var startGrupyRequest = OneTimeWorkRequest.Builder.From<GrupySyncWorker>().SetConstraints(new Constraints()
            {
                RequiredNetworkType = NetworkType.Connected
            }).Build();
            var startObiektyRequest = OneTimeWorkRequest.Builder.From<ObiektySyncWorker>().SetConstraints(
                new Constraints()
                {
                    RequiredNetworkType = NetworkType.Connected
                }).Build();
            var grupyRequest = PeriodicWorkRequest.Builder.From<GrupySyncWorker>(TimeSpan.FromHours(1))
                .SetConstraints(new Constraints()
            {
                RequiredNetworkType = NetworkType.Connected
            }).SetInitialDelay(1, TimeUnit.Hours)
                .Build();
            var obiektyRequest = Enumerable.Range(1, 3).Select(i => PeriodicWorkRequest.Builder.From<ObiektySyncWorker>(TimeSpan.FromMinutes(15)).SetConstraints(new Constraints()
            {
                RequiredNetworkType = NetworkType.Connected
            }).SetInitialDelay(i*5, TimeUnit.Minutes).Build());

            var workManager = WorkManager.GetInstance(this);
            
            workManager.CancelAllWork();
            while (!App.LoggedIn)
            {
                await Task.Delay(250);
            }

            workManager.BeginWith(startGrupyRequest).Then(startObiektyRequest).Enqueue();
            workManager.Enqueue(grupyRequest);
            workManager.Enqueue(obiektyRequest.ToList());
        }
    }
}