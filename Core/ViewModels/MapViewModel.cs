using Core.Models;
using Core.Utility.ViewModel;

namespace Core.ViewModels
{
    public class MapViewModel : BaseViewModel<Obiekt>
    {
        public MapViewModel()
        {
        }

        public override void Prepare(Obiekt parameter)
        {
            Obiekt = parameter;
        }

        private Obiekt _obiekt;
        public Obiekt Obiekt
        {
            get => _obiekt;
            set => SetProperty(ref _obiekt, value);
        }
    }
}