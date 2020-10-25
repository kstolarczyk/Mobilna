using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Core.ViewModels;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.Platforms.Android.Views.Base;
using Xamarin.Essentials;

namespace Mobilna.Views
{
    [Activity(Label = "@string/obiekty_title")]
    public class LoginView : MvxActivity<LoginViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Platform.Init(this, bundle);
            SetContentView(Resource.Layout.login);
        }
    }
}