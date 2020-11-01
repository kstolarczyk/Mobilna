using System.IO;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Widget;
using Core.ViewModels;
using Java.Nio;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.Platforms.Android.Views.Base;
using MvvmCross.Plugin.PictureChooser;
using MvvmCross.Plugin.PictureChooser.Platforms.Android;
using Xamarin.Essentials;
using Orientation = Android.Media.Orientation;
using Path = Android.Graphics.Path;
using Stream = System.IO.Stream;


namespace Mobilna.Views
{
    [Activity(Label = "@string/form_title")]
    public class ObiektFormView : MvxActivity<ObiektFormViewModel>
    {
        internal static ObiektFormView Instance { get; private set; }
        protected override void OnCreate(Bundle bundle) {  
            Instance = this;
            Instance.SetTheme(Resource.Style.AppThemeNoActionBar);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.obiekt_form);
            Platform.Init(this, bundle);
        }


        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent intent) {  
            base.OnActivityResult(requestCode, resultCode, intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    } 

}