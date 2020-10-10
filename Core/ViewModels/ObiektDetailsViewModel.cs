using System.Threading.Tasks;
using Acr.UserDialogs;
using Core.Models;
using Core.Repositories;
using Core.Utility.ViewModel;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Core.ViewModels
{
    public class ObiektDetailsViewModel : BaseViewModel<int>
    {
        private readonly IObiektRepository _repository;
        private readonly IMvxNavigationService _navigationService;
        private Obiekt _obiekt;
        private int _obiektId;
        private MapViewModel _mapViewModel;
        private readonly MvxInteraction<string> _popupImageInteraction = new MvxInteraction<string>();
        public ObiektDetailsViewModel(IObiektRepository repository, IMvxNavigationService navigationService)
        {
            _repository = repository;
            _navigationService = navigationService;
            ShowMapCommand = new MvxAsyncCommand(ShowMap);
            DeleteObiektCommand = new MvxAsyncCommand(DeleteObiekt, CanDelete);
            ShowImageCommand = new MvxCommand( () => _popupImageInteraction.Raise(Obiekt.ZdjecieLokal), CanShowImage);
        }

        private bool CanShowImage()
        {
            return !string.IsNullOrEmpty(Obiekt?.ZdjecieLokal);
        }

        private bool CanDelete()
        {
            return Obiekt?.UserId != null;
        }

        public async Task ShowMap()
        {
            await _navigationService.Navigate(_mapViewModel);
        }

        public override void Prepare(int obiektId)
        {
            _obiektId = obiektId;
        }

        public override async Task Initialize()
        {
            Obiekt = await _repository.GetOneAsync(_obiektId);
            _mapViewModel = new MapViewModel(Obiekt);
        }
        private async Task DeleteObiekt()
        {
            if (await Mvx.IoCProvider.Resolve<IUserDialogs>().ConfirmAsync("Czy na pewno chcesz usunąć?",
                $"Potwierdź usunięcie {Obiekt.Symbol}", "Tak", "Nie"))
            {
                await _repository.DeleteInstantlyAsync(Obiekt);
                await _navigationService.Close(this);
            }
        }

        public Obiekt Obiekt { get => _obiekt; set => SetProperty(ref _obiekt, value); }
        public IMvxAsyncCommand ShowMapCommand { get; set; }
        public IMvxAsyncCommand DeleteObiektCommand { get; set; }
        public IMvxCommand ShowImageCommand { get; set; }
        public IMvxInteraction<string> PopupImageInteraction => _popupImageInteraction;
    }
}