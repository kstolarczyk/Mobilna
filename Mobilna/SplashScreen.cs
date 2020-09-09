using Android.App;
using MvvmCross.Platforms.Android.Views;

namespace Mobilna
{
    [Activity(MainLauncher = true, NoHistory = true)]
    public class SplashScreen : MvxSplashScreenActivity
    {
        public SplashScreen() : base(Resource.Layout.splash_screen)
        {
            
        }
    }
}