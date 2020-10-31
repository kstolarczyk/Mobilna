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
    [Activity(Label = "Wybierz lokalizację")]
    public class SetLocationMapView : MvxActivity<SetLocationMapViewModel>, IOnMapReadyCallback
    {
        private SupportMapFragment _mapFragment;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.obiekt_set_location);
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
            var isEdit = ViewModel.Latitude != default || ViewModel.Longitude != default;
            var latlng = isEdit ? new LatLng(Convert.ToDouble(ViewModel.Latitude),
                Convert.ToDouble(ViewModel.Longitude)) : new LatLng(Resources.GetFloat(Resource.Dimension.default_latitude), Resources.GetFloat(Resource.Dimension.default_longitude));
            var camera = CameraUpdateFactory.NewLatLngZoom(latlng, 15);  
            googleMap.MoveCamera(camera);

            if (isEdit)
            {
                var markerOptions = new MarkerOptions()
                    .SetPosition(latlng)
                    .SetTitle(ViewModel.Obiekt.Nazwa);
                googleMap.AddMarker(markerOptions);
                googleMap.MapLongClick += (s, e) => AddMarker(googleMap, e.Point);
            }
            else
            {
                googleMap.MapClick += (s,e) => AddMarker(googleMap, e.Point);
            }
        }

        private void AddMarker(GoogleMap map, LatLng point)
        {
            var marker = new MarkerOptions().SetPosition(point).SetTitle(ViewModel.Obiekt.Nazwa);
            map.Clear();
            map.AddMarker(marker);
            ViewModel.Latitude = (decimal) point.Latitude;
            ViewModel.Longitude = (decimal) point.Longitude;
        }
    }
}