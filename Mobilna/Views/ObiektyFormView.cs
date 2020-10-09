using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Core.Models;
using Core.Repositories;
using Core.ViewModels;
using Java.IO;
using Java.Text;
using Java.Util;
using Mobilna.ItemTemplateSelector;
using MvvmCross.Platforms.Android.Views;



namespace Mobilna.Views
{
    [Activity(Label = "@string/form_title")]
    public class ObiektFormView : MvxActivity<ObiektyFormViewModel>
    {
        ImageView imageView;
        protected override void OnCreate(Bundle bundle) {  
            base.OnCreate(bundle);           
            SetContentView(Resource.Layout.obiekt_form);
            var btnCamera = FindViewById<Button>(Resource.Id.btnCamera);  
            imageView = FindViewById<ImageView>(Resource.Id.imageView);  
            btnCamera.Click += BtnCamera_Click;  
        }  
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data) {  
            base.OnActivityResult(requestCode, resultCode, data);
            if (data != null)
            {
                Bitmap bitmap = (Bitmap)data.Extras.Get("data");
                imageView.SetImageBitmap(bitmap);
            }
        }  
        private void BtnCamera_Click(object sender, System.EventArgs e) {  
            Intent intent = new Intent(MediaStore.ActionImageCapture);  
            StartActivityForResult(intent, 0);  
        }  
    } 

}