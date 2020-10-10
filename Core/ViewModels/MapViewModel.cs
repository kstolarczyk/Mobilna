using Core.Models;
using Core.Utility.ViewModel;

namespace Core.ViewModels
{
    public class MapViewModel : BaseViewModel
    {
        private Obiekt _obiekt;
        public MapViewModel(Obiekt obiekt)
        {
            _obiekt = obiekt;
        }

        public Obiekt Obiekt
        {
            get => _obiekt;
            set => SetProperty(ref _obiekt, value);
        }
    }
}