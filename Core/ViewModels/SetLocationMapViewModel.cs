using System.Threading.Tasks;
using Core.Models;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Core.ViewModels
{
    public class SetLocationMapViewModel : MvxViewModel<Obiekt, Obiekt>
    {
        private readonly IMvxNavigationService _navigationService;

        public SetLocationMapViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
            SaveCommand = new MvxAsyncCommand(Save);
            CancelCommand = new MvxAsyncCommand(async () => await _navigationService.Close(this, null));
        }

        public override void Prepare(Obiekt parameter)
        {
            Obiekt = parameter;
            Latitude = Obiekt.Latitude;
            Longitude = Obiekt.Longitude;
        }

        private async Task Save()
        {
            Obiekt.Latitude = Latitude;
            Obiekt.Longitude = Longitude;
            await _navigationService.Close(this, Obiekt);
        }
        
        private Obiekt _obiekt;
        public Obiekt Obiekt
        {
            get => _obiekt;
            set => SetProperty(ref _obiekt, value);
        }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public IMvxAsyncCommand CancelCommand { get; set; }
        public IMvxAsyncCommand SaveCommand { get; set; }
    }
}