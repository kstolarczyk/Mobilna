using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Core.Messages;
using Core.Models;
using Core.Repositories;
using Core.Utility.ViewModel;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;

namespace Core.ViewModels
{
    public class ObiektDetailsViewModel : BaseViewModel<int>
    {
        private readonly IObiektRepository _repository;
        private readonly IMvxNavigationService _navigationService;
        private readonly IMvxMessenger _messenger;
        private readonly IUserDialogs _userDialogs;
        private Obiekt _obiekt;
        private int _obiektId;
        private readonly MvxInteraction<string> _popupImageInteraction = new MvxInteraction<string>();
        public ObiektDetailsViewModel(IObiektRepository repository, IMvxNavigationService navigationService, IMvxMessenger messenger, IUserDialogs userDialogs)
        {
            _repository = repository;
            _navigationService = navigationService;
            _messenger = messenger;
            _userDialogs = userDialogs;
            ShowMapCommand = new MvxAsyncCommand(ShowMap);
            DeleteObiektCommand = new MvxAsyncCommand(DeleteObiekt, CanDelete);
            EdytujObiektCommand = new MvxAsyncCommand(EdytujObiekt, CanEdytuj);
            BackArrowCommand = new MvxAsyncCommand(GoBack);
            ShowImageCommand = new MvxCommand( () => _popupImageInteraction.Raise(Obiekt.ZdjecieLokal), CanShowImage);
            _messenger.Subscribe<ToastMessage>(OnToast, MvxReference.Strong);
            _messenger.Subscribe<ObiektSavedMessage>(OnObiektChanged, MvxReference.Strong);
        }
        private async Task GoBack()
        {
            await _navigationService.Close(this);
        }

        private void OnObiektChanged(ObiektSavedMessage msg)
        {
            if (msg.Obiekt.ObiektId != Obiekt.ObiektId) return;
            Obiekt = null;
            Obiekt = msg.Obiekt;
        }

        private void OnToast(ToastMessage msg)
        {
            Toasts.Add(msg);
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            foreach (var message in Toasts)
            {
                _userDialogs?.Toast(message.Message, message.Duration);
            }
            Toasts.Clear();
        }

        private async Task EdytujObiekt()
        {
            await _navigationService.Navigate<ObiektFormViewModel, int?>(Obiekt.ObiektId);
        }

        private bool CanShowImage()
        {
            return !string.IsNullOrEmpty(Obiekt?.ZdjecieLokal);
        }

        private bool CanDelete()
        {
            return Obiekt?.UserId != null;
        }

        private bool CanEdytuj()
        {
            return Obiekt?.UserId != null;
        }

        public async Task ShowMap()
        {
            await _navigationService.Navigate<MapViewModel, Obiekt>(Obiekt);
        }

        public override void Prepare(int obiektId)
        {
            _obiektId = obiektId;
        }

        public override async Task Initialize()
        {
            Obiekt = await _repository.GetOneAsync(_obiektId);
        }
        private async Task DeleteObiekt()
        {
            if (await Mvx.IoCProvider.Resolve<IUserDialogs>().ConfirmAsync("Czy na pewno chcesz usunąć?",
                $"Potwierdź usunięcie {Obiekt.Symbol}", "Tak", "Nie"))
            {
                await _repository.DeleteInstantlyAsync(Obiekt);
                _messenger.Publish(new ToastMessage(this, "Pomyślnie usunięto obiekt!", TimeSpan.FromSeconds(3)));
                _messenger.Publish(new ObiektDeletedMessage(this, Obiekt));
                await _navigationService.Close(this);
            }
        }

        public Obiekt Obiekt { get => _obiekt; set => SetProperty( ref _obiekt, value); }
        public IMvxAsyncCommand ShowMapCommand { get; set; }
        public IMvxAsyncCommand DeleteObiektCommand { get; set; }
        public IMvxAsyncCommand EdytujObiektCommand { get; set; }
        public IMvxCommand ShowImageCommand { get; set; }
        public IMvxAsyncCommand BackArrowCommand { get; set; }
        public IMvxInteraction<string> PopupImageInteraction => _popupImageInteraction;
        public List<ToastMessage> Toasts { get; } = new List<ToastMessage>();
    }
}