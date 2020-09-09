using Core.Services;
using Core.Utility.ViewModel;

namespace Core.ViewModels
{
    public class ObiektyViewModel : BaseViewModel
    {
        private readonly IObiektyDataService _dataService;

        public ObiektyViewModel(IObiektyDataService dataService)
        {
            _dataService = dataService;
        }

    }
}