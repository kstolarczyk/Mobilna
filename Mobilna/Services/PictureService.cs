using System;
using System.IO;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Core.Interfaces;
using Core.Messages;
using MvvmCross;
using MvvmCross.Plugin.Messenger;
using MvvmCross.Plugin.PictureChooser;
using Xamarin.Essentials;

namespace Mobilna.Services
{
    public class PictureService : IPictureService
    {
        private readonly IMvxMessenger _messenger;
        private readonly IMvxPictureChooserTask _chooserTask;
        private const int MaxImageDimenson = 3840;
        private const int Quality = 95;
        public PictureService(IMvxMessenger messenger, IMvxPictureChooserTask chooserTask)
        {
            _messenger = messenger;
            _chooserTask = chooserTask;
        }

        public async Task<byte[]> TakePictureAsync()
        {
            var status = await CheckOrRequestPermission().ConfigureAwait(false);
            if (status != PermissionStatus.Granted)
            {
                Mvx.IoCProvider.Resolve<IUserDialogs>().Toast("Brak uprawnień do aparatu!. Sprawdź ustawienia.", TimeSpan.FromSeconds(3));
                return default;
            }

            var stream = await _chooserTask.TakePictureAsync(MaxImageDimenson, Quality);
            if (stream == null || stream.Length == 0) return null;
            var bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes, 0, (int)stream.Length).ConfigureAwait(false);
            return bytes;
        }

        public async void TakePicture()
        {
            var status = await CheckOrRequestPermission().ConfigureAwait(false);
            if (status != PermissionStatus.Granted)
            {
                Mvx.IoCProvider.Resolve<IUserDialogs>().Toast("Brak uprawnień do aparatu!. Sprawdź ustawienia.", TimeSpan.FromSeconds(3));
                return;
            }
            _chooserTask.TakePicture(MaxImageDimenson, Quality, OnPictureTaken, () => { });
        }

        public async Task<byte[]> ChoosePictureAsync()
        {
            var stream = await _chooserTask.ChoosePictureFromLibraryAsync(MaxImageDimenson, Quality);
            if (stream == null || stream.Length == 0) return null;
            var bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes, 0, (int)stream.Length).ConfigureAwait(false);
            return bytes;
        }

        public void ChoosePicture()
        {
            _chooserTask.ChoosePictureFromLibrary(MaxImageDimenson, Quality, OnPictureTaken, () => { });
        }

        private async void OnPictureTaken(Stream stream)
        {
            var bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes, 0, (int)stream.Length).ConfigureAwait(false);
            _messenger.Publish(new PictureMessage(this, bytes));
        }

        protected async Task<PermissionStatus> CheckOrRequestPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>().ConfigureAwait(false);
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>().ConfigureAwait(false);
            }
            return status;
        }
    }
}