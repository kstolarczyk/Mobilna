using Acr.UserDialogs;
using Core;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using Mobilna.Services;
using MvvmCross;
using MvvmCross.Platforms.Android.Core;
using LocationService = Mobilna.Services.LocationService;

namespace Mobilna
{
    public class MySetup : MvxAndroidSetup<App>
    {
        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();
            Mvx.IoCProvider.RegisterType(() => UserDialogs.Instance);
            Mvx.IoCProvider.RegisterType<ILocationService>(() => new LocationService());
            Mvx.IoCProvider.RegisterType<IPictureService>(() => Mvx.IoCProvider.IoCConstruct<PictureService>());
        }
    }
}