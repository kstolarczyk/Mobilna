using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Core.ViewModels;
using MvvmCross.Platforms.Android.Views;



namespace Mobilna.Views
{
    [Activity(Label = "@string/form_title")]
    public class ObiektFormView : MvxActivity<ObiektFormViewModel>
    {
        ImageView imageView;
        internal static ObiektFormView Instance { get; private set; }
        internal static int PickImageId = 1;
        public TaskCompletionSource<byte[]> PickImageTask { get; set; }

        protected override void OnCreate(Bundle bundle) {  
            Instance = this;
            base.OnCreate(bundle);           
            SetContentView(Resource.Layout.obiekt_form);
            imageView = FindViewById<ImageView>(Resource.Id.imageView);
        }


        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent intent) {  
            base.OnActivityResult(requestCode, resultCode, intent);
            if (requestCode == PickImageId && resultCode == Result.Ok && intent != null)
            {
                var uri = intent.Data;
                imageView.SetImageURI(uri);
                var stream = ContentResolver?.OpenInputStream(uri!);
                if (stream == null) return;
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                PickImageTask.SetResult(buffer);
            }
            else
            {
                PickImageTask.SetResult(null);
            }
        }
    } 

}