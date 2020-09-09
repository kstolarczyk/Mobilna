using Android.App;
using Android.OS;
using Core.ViewModels;
using MvvmCross.Platforms.Android.Views;

namespace Mobilna.Views
{
    [Activity(Label = "@string/obiekty_title", MainLauncher = true)]
    public class ObiektyView : MvxActivity<ObiektyViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.obiekty_list);
        }
    }
}