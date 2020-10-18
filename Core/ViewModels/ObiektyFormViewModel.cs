using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
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
    public class ObiektyFormViewModel : BaseViewModel 
    {
        private readonly IGrupaObiektowRepository _grupaRepository;
        private bool _isBusy;
        private bool _wait = true;
        private GrupaObiektow _selectedGrupa;
        private String _selectedAkceptowalnaWartosc;
        private  int TAKE_PICTURE = 1;
        private Uri imageUri;
        private TypParametrow _typ;


        public ObiektyFormViewModel(IObiektRepository repository, IGrupaObiektowRepository grupaObiektowRepository)
        {
            _grupaRepository = grupaObiektowRepository;
        }

        public override async Task Initialize()
        {

             foreach (var _grupa in await _grupaRepository.GetAllAsync())
             {
               GrupaObiektow.Add(_grupa);
             }    
        }
        public async Task SetParametr(GrupaObiektow grupa)
        {
                Parametry.Clear();
             
                foreach (var _param in grupa.TypyParametrow)
                {
                    Parametr p = new Parametr();
                    p.TypParametrow = _param;
                    Parametry.Add(p);
                }
        }



        public async Task Refresh()
        {
            IsBusy = true;
            GrupaObiektow.Clear();
            await Initialize();
            IsBusy = false;
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }





        public GrupaObiektow GrupaObiektowSelectedItem
        {
            get { return _selectedGrupa; }
            set
            {
                _selectedGrupa = value;
                SetParametr(value);
                RaisePropertyChanged(() => GrupaObiektowSelectedItem);
            }
        }






        public IMvxAsyncCommand RefreshCommand => new MvxAsyncCommand(Refresh);
        public MvxObservableCollection<Parametr> Parametry { get; set; } = new MvxObservableCollection<Parametr>();
        public MvxObservableCollection<GrupaObiektow> GrupaObiektow { get; } = new MvxObservableCollection<GrupaObiektow>();

    }
}