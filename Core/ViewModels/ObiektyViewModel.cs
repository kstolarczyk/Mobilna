using System.Threading.Tasks;
using Core.Extensions;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.Utility.ViewModel;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Core.ViewModels
{
    public class ObiektyViewModel : BaseViewModel
    {
        private readonly IObiektRepository _repository;
        private bool _isBusy;

        public ObiektyViewModel(IObiektRepository repository)
        {
            _repository = repository;
        }

        public override async Task Initialize()
        {
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
        public IMvxAsyncCommand RefreshCommand => new MvxAsyncCommand(Refresh);

        public MvxObservableCollection<Obiekt> Obiekty { get; } = new MvxObservableCollection<Obiekt>();
    }
}