using System.Threading.Tasks;
using System.Timers;
using Core.Extensions;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.Utility.ViewModel;
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
        private readonly Timer _timer;
        private bool _isConnected;

        public ObiektyViewModel(IObiektRepository repository, IMvxReachability reachability)
        {
            _repository = repository;
            _reachability = reachability;
            _timer = new Timer() {Interval = 10000, AutoReset = true, Enabled = true};
            _timer.Elapsed += Elapsed;
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            IsConnected = _reachability.IsHostReachable(WebService.ApiBaseUrl);
        }


        public override async Task Initialize()
        {
            IsConnected = _reachability.IsHostReachable(WebService.ApiBaseUrl);
            // Obiekty.AddRange(await _repository.GetAllAsync());
            await foreach (var obiekt in _repository.GetAsStream())
            {
                Obiekty.Add(obiekt);
            }
        }


        public async Task Refresh()
        {
            IsBusy = true;
            Obiekty.Clear();
            await Initialize();
            IsBusy = false;
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }
        public IMvxAsyncCommand RefreshCommand => new MvxAsyncCommand(Refresh);
        
        public MvxObservableCollection<Obiekt> Obiekty { get; } = new MvxObservableCollection<Obiekt>();
    }
}