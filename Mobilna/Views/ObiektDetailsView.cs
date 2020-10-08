using Android.App;
using Android.OS;
using AndroidX.RecyclerView.Widget;
using Core.ViewModels;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Views;

namespace Mobilna.Views
{
    [Activity(MainLauncher = true, Label = "Szczegóły")]
    public class ObiektDetailsView : MvxActivity<ObiektDetailsViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.obiekt_details);
        }
    }
}