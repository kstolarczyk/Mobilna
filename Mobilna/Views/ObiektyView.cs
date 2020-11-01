using Acr.UserDialogs;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Core.Interactions;
using Core.Models;
using Core.ViewModels;
using MvvmCross.Base;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.Platforms.Android.Views.Fragments;
using MvvmCross.ViewModels;
using Xamarin.Essentials;

namespace Mobilna.Views
{
    [Register("mobilna.custom.fragments.ObiektyFragment")]
    public class ObiektyView : MvxFragment<ObiektyViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignore = base.OnCreateView(inflater, container, savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.obiekty_recyclerview, null);
            return view;
        }
    }
}