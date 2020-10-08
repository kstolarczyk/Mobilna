using System.Threading.Tasks;
using Core.Models;
using Core.Repositories;
using Core.Utility.ViewModel;

namespace Core.ViewModels
{
    public class ObiektDetailsViewModel : BaseViewModel<int>
    {
        private readonly IObiektRepository _repository;
        private Obiekt _obiekt;
        private int _obiektId;

        public ObiektDetailsViewModel(IObiektRepository repository)
        {
            _repository = repository;
        }
        public override void Prepare(int obiektId)
        {
            _obiektId = obiektId;
        }

        public override async Task Initialize()
        {
            Obiekt = await _repository.GetOneAsync(_obiektId);
        }

        public Obiekt Obiekt { get => _obiekt; set => SetProperty(ref _obiekt, value); }
    }
}