using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Core.ViewModels;
using MvvmCross.Platforms.Android.Views;
using Xamarin.Essentials;


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
            Platform.Init(this, bundle);
            SetContentView(Resource.Layout.obiekt_form);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    } 

}