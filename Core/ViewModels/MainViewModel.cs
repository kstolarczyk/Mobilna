using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Acr.UserDialogs;
using Core.Helpers;
using Core.Interactions;
using Core.Messages;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.Utility.Enum;
using Core.Utility.ViewModel;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.Plugin.Network.Reachability;
using MvvmCross.ViewModels;

namespace Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private IObiektRepository _repository;
        private IMvxReachability _reachability;
        private bool _isConnected;
        private IMvxNavigationService _navigation;
        private IUserDialogs _userDialogs;
        private readonly IMvxMessenger _messenger;
        private MvxSubscriptionToken _toastTag;
        private MvxSubscriptionToken _syncStatusTag;
        private readonly MvxInteraction<IEnumerable<GrupaObiektow>> _grupySyncDoneInteraction = new MvxInteraction<IEnumerable<GrupaObiektow>>();

        public MainViewModel(IObiektRepository repository, IMvxReachability reachability,
            IMvxNavigationService navigationService, IUserDialogs userDialogs, IMvxMessenger messenger)
        {
            _repository = repository;
            _reachability = reachability;
            _navigation = navigationService;
            _userDialogs = userDialogs;
            _messenger = messenger;
            var timer = new Timer() {Interval = 5000, AutoReset = true, Enabled = true};
            timer.Elapsed += Elapsed;
            SynchronizeCommand = new MvxAsyncCommand(Synchronize, () => CanSynchronize);
            NowyObiektCommand = new MvxAsyncCommand(NowyObiekt, () => !IsSynchronizing);
            GrupaSynchronizer.SynchronizingChanged += NotifySyncChanged;
            GrupaSynchronizer.SynchronizingChanged += GrupyReload;
            _syncStatusTag = _messenger.Subscribe<SyncStatusChangedMessage>(SyncStatusChanged, MvxReference.Strong);
            _toastTag = _messenger.Subscribe<ToastMessage>(OnToast, MvxReference.Strong);
        }

        private void SyncStatusChanged(SyncStatusChangedMessage msg)
        {
            NotifySyncChanged();
        }

        private async void GrupyReload()
        {
            if (GrupaSynchronizer.IsSynchronizing) return;
            await Reload();
        }

        private async Task Reload()
        {
            GrupyObiektow.Clear();
            await Initialize();
            _grupySyncDoneInteraction.Raise(GrupyObiektow);
        }

        private void OnToast(ToastMessage msg)
        {
            Messages.Add(msg);
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            foreach (var msg in Messages)
            {
                _userDialogs.Toast(msg.Message, msg.Duration);
            }
            Messages.Clear();
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(viewFinishing);
            _messenger.Unsubscribe<SyncStatusChangedMessage>(_syncStatusTag);
            _messenger.Unsubscribe<ToastMessage>(_toastTag);
        }

        private async Task NowyObiekt()
        {
            await _navigation.Navigate<ObiektFormViewModel, int?>(null);
        }

        public async void NotifySyncChanged()
        {
            SynchronizeCommand.RaiseCanExecuteChanged();
            NowyObiektCommand.RaiseCanExecuteChanged();
            await RaisePropertyChanged(() => SyncStatus);
            await RaisePropertyChanged(() => IsSynchronizing);
        }

        public override async Task Initialize()
        {
            GrupyObiektow.AddRange(await _repository.GetGrupyAsync());
        }

        public async Task Synchronize()
        {
            try
            {
                await ObiektSynchronizer.SynchronizeObiekty();
            }
            catch (Exception e)
            {
                _userDialogs.Toast(e.Message, TimeSpan.FromSeconds(3));
            }
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            IsConnected = _reachability.IsHostReachable(WebService.ApiBaseUrl);
        }

        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value, NotifySyncChanged);
        }

        public SynchronizationStatus SyncStatus =>
            !IsConnected
                ? SynchronizationStatus.Unavailable
                : IsSynchronizing
                    ? SynchronizationStatus.InProgress
                    : SynchronizationStatus.NotStarted;

        public bool CanSynchronize => IsConnected && !ObiektSynchronizer.IsSynchronizing;
        public IMvxAsyncCommand SynchronizeCommand { get; set; }
        public MvxObservableCollection<GrupaObiektow> GrupyObiektow { get; } = new MvxObservableCollection<GrupaObiektow>();
        public IMvxAsyncCommand NowyObiektCommand { get; set; }
        public bool IsSynchronizing => ObiektSynchronizer.IsSynchronizing || GrupaSynchronizer.IsSynchronizing;
        public List<ToastMessage> Messages { get; set; } = new List<ToastMessage>();
        public IMvxInteraction<IEnumerable<GrupaObiektow>> GrupySyncDoneInteraction => _grupySyncDoneInteraction;
    }
}