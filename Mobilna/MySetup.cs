using Acr.UserDialogs;
using Core;
using Core.Interfaces;
using Core.Models;
using Mobilna.Services;
using MvvmCross;
using MvvmCross.Platforms.Android.Core;

namespace Mobilna
{
    public class MySetup : MvxAndroidSetup<App>
    {
        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();
            Mvx.IoCProvider.RegisterType<IPickImageService>(() => new PhotoPickerService());
            Mvx.IoCProvider.RegisterType(() => UserDialogs.Instance);
        }
    }
}