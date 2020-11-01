using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Timers;
using Acr.UserDialogs;
using Core.Extensions;
using Core.Helpers;
using Core.Interactions;
using Core.Messages;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.Utility.Enum;
using Core.Utility.ViewModel;
using Microsoft.EntityFrameworkCore.Internal;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.Plugin.Network.Reachability;
using MvvmCross.ViewModels;

namespace Core.ViewModels
{
    public class ObiektyViewModel : BaseViewModel<int>
    {
        private readonly IObiektRepository _repository;
        private bool _isBusy;
        private readonly MvxInteraction<ContextMenuInteraction<Obiekt>> _contextMenuInteraction =
            new MvxInteraction<ContextMenuInteraction<Obiekt>>();
        private readonly IMvxNavigationService _navigation;
        private readonly IUserDialogs _userDialogs;
        private readonly IMvxMessenger _messenger;
        private int _grupaId;
        private readonly MvxSubscriptionToken _statusChangedTag;
        private readonly MvxSubscriptionToken _obiektSavedTag;
        private readonly MvxSubscriptionToken _obiektDeletedTag;

        public ObiektyViewModel(IObiektRepository repository,
            IMvxNavigationService navigationService, IUserDialogs userDialogs, IMvxMessenger messenger)
        {
            _repository = repository;
            _navigation = navigationService;
            _userDialogs = userDialogs;
            _messenger = messenger;
            ContextMenuCommand = new MvxAsyncCommand<Obiekt>(async o => _contextMenuInteraction.Raise(
                new ContextMenuInteraction<Obiekt>()
                {
                    CurrentObiekt = await _repository.GetOneAsync(o.ObiektId),
                    ContextMenuCallback = ContextMenuHandle
                }));
            DetailsCommand = new MvxAsyncCommand<Obiekt>(Details);
            RefreshCommand = new MvxAsyncCommand(Refresh);
            _statusChangedTag = _messenger.Subscribe<SyncStatusChangedMessage>(SyncStatusChanged);
            _obiektSavedTag = _messenger.Subscribe<ObiektSavedMessage>(OnObiektChanged, MvxReference.Strong);
            _obiektDeletedTag = _messenger.Subscribe<ObiektDeletedMessage>(OnObiektDeleted, MvxReference.Strong);
        }

        private async void SyncStatusChanged(SyncStatusChangedMessage msg)
        {
            if (msg.IsSynchronizing) return;
            await Reload();
        }

        protected override void InitFromBundle(IMvxBundle parameters)
        {
            _grupaId = parameters.Read<ViewModelParameter<int>>().Value;
        }


        private void OnObiektDeleted(ObiektDeletedMessage msg)
        {
            if (msg.Obiekt.GrupaObiektowId != _grupaId) return;
            var index = Obiekty.IndexOf(o => o.ObiektId == msg.Obiekt.ObiektId);
            if (index < 0) return;
            Obiekty.RemoveAt(index);
        }

        private void OnObiektChanged(ObiektSavedMessage msg)
        {
            if (msg.Obiekt.GrupaObiektowId != _grupaId) return;
            var index = Obiekty.IndexOf(o => o.ObiektId == msg.Obiekt.ObiektId);
            if (index >= 0)
            {
                Obiekty[index] = msg.Obiekt;
            }
            else
            {
                Obiekty.Add(msg.Obiekt);
            }
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(viewFinishing);
            _messenger.Unsubscribe<SyncStatusChangedMessage>(_statusChangedTag);
            _messenger.Unsubscribe<ObiektDeletedMessage>(_obiektDeletedTag);
            _messenger.Unsubscribe<ObiektSavedMessage>(_obiektSavedTag);
        }

        private async void ContextMenuHandle(Obiekt obiekt, ContextMenuOption option)
        {
            switch (option)
            {
                case ContextMenuOption.Details:
                    await Details(obiekt);
                    break;
                case ContextMenuOption.Delete:
                    if (await _userDialogs.ConfirmAsync("Czy na pewno chcesz usunąć?",
                        $"Potwierdź usunięcie {obiekt.Symbol}", "Tak", "Nie"))
                    {
                        DeleteObiekt(obiekt);
                    }

                    break;
                case ContextMenuOption.Edit:
                    await _navigation.Navigate<ObiektFormViewModel, int?>(obiekt.ObiektId);
                    break;
                case ContextMenuOption.None:
                    return;
            }
        }

        public async Task Details(Obiekt obiekt)
        {
            await _navigation.Navigate<ObiektDetailsViewModel, int>(obiekt.ObiektId);
        }

        private async void DeleteObiekt(Obiekt obiekt)
        {
            await _repository.DeleteInstantlyAsync(obiekt);
            Obiekty.Remove(obiekt);
            _userDialogs.Toast("Pomyślnie usunięto obiekt!", TimeSpan.FromSeconds(3));
        }

        public override async Task Initialize()
        {
            await Reload();
        }


        public async Task Refresh()
        {
            IsBusy = true;
            await Reload();
            IsBusy = false;
        }

        public async Task Reload()
        {
            if (ObiektSynchronizer.IsSynchronizing || GrupaSynchronizer.IsSynchronizing) return;
            Obiekty.Clear();
            await foreach (var obiekt in _repository.GetAsStream(_grupaId))
            {
                Obiekty.Add(obiekt);
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public MvxAsyncCommand<Obiekt> ContextMenuCommand { get; set; }
        public IMvxAsyncCommand RefreshCommand { get; set; }
        public MvxObservableCollection<Obiekt> Obiekty { get; } = new MvxObservableCollection<Obiekt>();
        public IMvxInteraction<ContextMenuInteraction<Obiekt>> ContextMenuInteraction => _contextMenuInteraction;
        public IMvxAsyncCommand<Obiekt> DetailsCommand { get; set; }
    }
}