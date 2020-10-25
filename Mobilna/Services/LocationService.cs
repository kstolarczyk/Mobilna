using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Android.Gms.Tasks;
using Core.Interfaces;
using Core.Messages;
using Core.Services;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Plugin.Location;
using MvvmCross.Plugin.Messenger;
using Xamarin.Essentials;

namespace Mobilna.Services
{
    public class LocationService : ILocationService
    {
        public async Task<(double, double)> GetLocation()
        {
            var location = await Geolocation.GetLocationAsync().ConfigureAwait(false);
            return (location.Latitude, location.Longitude);
        }
    }
}