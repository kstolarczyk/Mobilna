using System.Threading.Tasks;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.Utility.ViewModel;
using MvvmCross.ViewModels;

namespace Core.ViewModels
{
    public class ObiektyViewModel : BaseViewModel
    {
        private readonly IObiektRepository _repository;

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


        public MvxObservableCollection<Obiekt> Obiekty { get; } = new MvxObservableCollection<Obiekt>();
    }
}