using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Core.ViewModels;
using MvvmCross.Base;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.ViewModels;
using Environment = System.Environment;
using Path = System.IO.Path;

namespace Mobilna.Views
{
    [Activity(Label = "Szczegóły")]
    public class ObiektDetailsView : MvxActivity<ObiektDetailsViewModel>
    {
        private string _imagePath;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.obiekt_details);
            ViewModel.PopupImageInteraction.Requested += OnInteractionRequested;
        }

        private void OnInteractionRequested(object sender, MvxValueEventArgs<string> e)
        {
            _imagePath = e.Value;
            ShowPopup();
        }

        private void ShowPopup()
        {
            var dialog = new Dialog(this);
            var imgView = new ImageView(this);
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), _imagePath);
            imgView.SetImageBitmap(BitmapFactory.DecodeFile(path));
            dialog.SetContentView(imgView);
            dialog.Show();
        }
    }
}