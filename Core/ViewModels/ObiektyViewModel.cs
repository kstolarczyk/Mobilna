using System.ComponentModel;
using System.Threading.Tasks;
using System.Timers;
using Acr.UserDialogs;
using Core.Extensions;
using Core.Helpers;
using Core.Interactions;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.Utility.Enum;
using Core.Utility.ViewModel;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Network.Reachability;
using MvvmCross.ViewModels;

namespace Core.ViewModels
{
    public class ObiektyViewModel : BaseViewModel
    {
        private readonly IObiektRepository _repository;
        private readonly IMvxReachability _reachability;
        private bool _isBusy;
        private bool _isConnected;
        private readonly MvxInteraction<ContextMenuInteraction<Obiekt>> _contextMenuInteraction = new MvxInteraction<ContextMenuInteraction<Obiekt>>();
        public ObiektyViewModel(IObiektRepository repository, IMvxReachability reachability)
        {
            _repository = repository;
            _reachability = reachability;
            var timer = new Timer() {Interval = 5000, AutoReset = true, Enabled = true};
            timer.Elapsed += Elapsed;
            ContextMenuCommand = new MvxAsyncCommand<Obiekt>(async o => _contextMenuInteraction.Raise(new ContextMenuInteraction<Obiekt>()
            {
                CurrentObiekt = await _repository.GetOneAsync(o.ObiektId),
                ContextMenuCallback = ContextMenuHandle
            }));
            SynchronizeCommand = new MvxAsyncCommand(Synchronize, () => CanSynchronize);
            RefreshCommand = new MvxAsyncCommand(Refresh);
            ObiektSynchronizer.SynchronizingChanged += NotifySyncChanged;
            ObiektSynchronizer.SynchronizingChanged += async () => await Reload();
        }

        private async void ContextMenuHandle(Obiekt obiekt, ContextMenuOption option)
        {
            switch (option)
            {
                case ContextMenuOption.Details:
                    break;
                case ContextMenuOption.Delete:
                    if (await Mvx.IoCProvider.Resolve<IUserDialogs>().ConfirmAsync("Czy na pewno chcesz usunąć?",
                        $"Potwierdź usunięcie {obiekt.Symbol}", "Tak", "Nie"))
                    {
                        DeleteObiekt(obiekt);
                    }
                    break;
                case ContextMenuOption.Edit:
                    break;
                case ContextMenuOption.None:
                    return;
            }
        }

        private async void DeleteObiekt(Obiekt obiekt)
        {
            await _repository.DeleteInstantlyAsync(obiekt);
            Obiekty.Remove(obiekt);
        }

        public MvxAsyncCommand<Obiekt> ContextMenuCommand { get; set; }

        public void NotifySyncChanged()
        {
            SynchronizeCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged(() => SyncStatus);
        }

        public async Task Synchronize()
        {
            await ObiektSynchronizer.SynchronizeObiekty();
            await Refresh();
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            IsConnected = _reachability.IsHostReachable(WebService.ApiBaseUrl);
        }


        public override async Task Initialize()
        {
            IsConnected = _reachability.IsHostReachable(WebService.ApiBaseUrl);
            await foreach (var obiekt in _repository.GetAsStream())
            {
                Obiekty.Add(obiekt);
            }
        }


        public async Task Refresh()
        {
            IsBusy = true;
            await Reload();
            IsBusy = false;
        }

        public async Task Reload()
        {
            if (ObiektSynchronizer.IsSynchronizing) return;
            Obiekty.Clear();
            await Initialize();
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value, NotifySyncChanged);
        }

        public SynchronizationStatus SyncStatus =>
            !IsConnected ? SynchronizationStatus.Unavailable :
            ObiektSynchronizer.IsSynchronizing ? SynchronizationStatus.InProgress :
            SynchronizationStatus.NotStarted;

        public IMvxAsyncCommand RefreshCommand { get; set; }
        public bool CanSynchronize => IsConnected && !ObiektSynchronizer.IsSynchronizing;
        public IMvxAsyncCommand SynchronizeCommand { get; set; }
        public MvxObservableCollection<Obiekt> Obiekty { get; } = new MvxObservableCollection<Obiekt>();
        public IMvxInteraction<ContextMenuInteraction<Obiekt>> ContextMenuInteraction => _contextMenuInteraction;
    }
}