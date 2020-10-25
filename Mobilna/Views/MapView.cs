using System;
using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Core.ViewModels;
using MvvmCross.Platforms.Android.Views;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Mobilna.Views
{
    [Activity(Label = "Obiekt na mapie", MainLauncher = true)]
    public class MapView : MvxActivity<MapViewModel>, IOnMapReadyCallback
    {
        private SupportMapFragment _mapFragment;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.obiekt_map);
            InitializeMap();
            _mapFragment?.GetMapAsync(this);
        }

        private void InitializeMap()
        {
            _mapFragment = SupportFragmentManager.FindFragmentById(Resource.Id.map).JavaCast<SupportMapFragment>();

            if (_mapFragment != null) return;
            var mapOptions = new GoogleMapOptions()
                .InvokeMapType(GoogleMap.MapTypeNormal)
                .InvokeZoomControlsEnabled(true)
                .InvokeCompassEnabled(true);

            var fragTx = SupportFragmentManager.BeginTransaction();
            _mapFragment = SupportMapFragment.NewInstance(mapOptions);
            fragTx.Add(Resource.Id.map, _mapFragment.JavaCast<Fragment>(), "map");
            fragTx.Commit();
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            var latlng = new LatLng(Convert.ToDouble(ViewModel.Obiekt.Latitude),
                Convert.ToDouble(ViewModel.Obiekt.Longitude));
            var markerOptions = new MarkerOptions()
                .SetPosition(latlng)
                .SetTitle(ViewModel.Obiekt.Nazwa);
            var camera = CameraUpdateFactory.NewLatLngZoom(latlng, 15);  
            googleMap.MoveCamera(camera);
            googleMap.AddMarker(markerOptions);
        }
    }
}