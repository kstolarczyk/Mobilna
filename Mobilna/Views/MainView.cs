using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Android.App;
using Android.OS;
using AndroidX.ViewPager.Widget;
using Core.Models;
using Core.Utility.ViewModel;
using Core.ViewModels;
using Google.Android.Material.Tabs;
using MvvmCross.Base;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.Platforms.Android.Views.ViewPager;
using MvvmCross.ViewModels;

namespace Mobilna.Views
{
    [Activity(Label = "Obiekty")]
    public class MainView : MvxActivity<MainViewModel>
    {
        private ViewPager _viewPager;
        private TabLayout _tablayout;
        private MvxCachingFragmentStatePagerAdapter _adapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.obiekty_list);
            _viewPager = FindViewById<ViewPager>(Resource.Id.obiekty_viewPager);
            _tablayout = FindViewById<TabLayout>(Resource.Id.tab_layout);
            var fragments = ViewModel.GrupyObiektow.Select(SetupFragment).ToList();
            _adapter = new MvxCachingFragmentStatePagerAdapter(this, SupportFragmentManager, fragments);
            _viewPager.Adapter = _adapter;
            _viewPager.OffscreenPageLimit = fragments.Count;
            _tablayout.SetupWithViewPager(_viewPager);
            ViewModel.GrupySyncDoneInteraction.Requested += OnInteractionRequested;
        }

        private void OnInteractionRequested(object sender, MvxValueEventArgs<IEnumerable<GrupaObiektow>> e)
        {
            _adapter.FragmentsInfo.Clear();
            _adapter.FragmentsInfo.AddRange(e.Value.Select(SetupFragment));
            RunOnUiThread(() => _adapter.NotifyDataSetChanged());
        }
        private MvxViewPagerFragmentInfo SetupFragment(GrupaObiektow grupa)
        {
            var bundle = new MvxBundle();
            bundle.Write(new ViewModelParameter<int>(grupa.GrupaObiektowId));
            return new MvxViewPagerFragmentInfo(grupa.Nazwa, $"GrupaObiektowTag{grupa.GrupaObiektowId}", typeof(ObiektyView), new MvxViewModelRequest<ObiektyViewModel>(bundle, bundle));
        }
    }
}