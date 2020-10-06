using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Core.ViewModels;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.Platforms.Android.Views.Base;

namespace Mobilna.Views
{
    [Activity(Label = "@string/obiekty_title", MainLauncher = true)]
    public class LoginView : MvxActivity<LoginViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.login);
        }
    }
}