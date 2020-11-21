using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Core.Interfaces;
using Core.Messages;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.Utility.ViewModel;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Location;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;

namespace Core.ViewModels
{
    public class ObiektFormViewModel : BaseViewModel<int?>
    {
        private readonly IObiektRepository _repository;
        private readonly IMvxNavigationService _navigationService;
        private readonly IUserDialogs _userDialogs;
        private GrupaObiektow _selectedGrupa;
        private readonly ILocationService _locationService;
        private readonly IPictureService _pictureService;
        private readonly IMvxMessenger _messenger;
        private Obiekt _obiekt;
        private int? _obiektId;
        private byte[] _imageBytes;


        public ObiektFormViewModel(IObiektRepository repository,
            IMvxNavigationService navigationService, IUserDialogs userDialogs,
            ILocationService locationService, IPictureService pictureService,
            IMvxMessenger messenger)
        {
            _repository = repository;
            _navigationService = navigationService;
            _userDialogs = userDialogs;
            _locationService = locationService;
            _pictureService = pictureService;
            _messenger = messenger;
            SaveCommand = new MvxAsyncCommand(Save, () => CanSave);
            PickImageCommand = new MvxAsyncCommand(ChooseImage);
            GetCoordsCommand = new MvxAsyncCommand(GetCoords);
            BackArrowCommand = new MvxAsyncCommand(GoBack);
        }

        private async Task GoBack()
        {
            await _navigationService.Close(this);
        }

        private Task ChooseImage()
        {
            _userDialogs.ActionSheet(new ActionSheetConfig()
            {
                Options = new List<ActionSheetOption>()
                {
                    new ActionSheetOption("Zrób zdjęcie", async () => await TakePhoto()),
                    new ActionSheetOption("Wybierz z galerii", async () => await PickImage())
                }
            });
            return Task.CompletedTask;
        }

        private Task GetCoords()
        {
            _userDialogs.ActionSheet(new ActionSheetConfig()
            {
                Options = new List<ActionSheetOption>()
                {
                    new ActionSheetOption("Użyj twojej lokalizacji", GetCurrentLocation),
                    new ActionSheetOption("Ustaw punkt na mapie", SetPointOnMap)
                }
            });
            return Task.CompletedTask;
        }

        public async Task TakePhoto()
        {
            ImageBytes = await _pictureService.TakePictureAsync();
        }

        public async void SetPointOnMap()
        {
            await _navigationService.Navigate<SetLocationMapViewModel, Obiekt, Obiekt>(Obiekt);
            await Obiekt.RaiseAllPropertiesChanged();
        }

        public async void GetCurrentLocation()
        {
            IsGettingCoords = true;
            try
            {
                var (latitude, longitude) = await _locationService.GetLocation();
                Obiekt.Latitude = (decimal) latitude;
                Obiekt.Longitude = (decimal) longitude;
            }
            catch (Exception e)
            {
                _userDialogs.Toast(e.Message, TimeSpan.FromSeconds(3));
            }
            IsGettingCoords = false;
        }


        public async Task PickImage()
        {
            ImageBytes = await _pictureService.ChoosePictureAsync();
        }


        private async Task Save()
        {
            _userDialogs.ShowLoading("Zapisywanie...");
            Obiekt.Parametry = Parametry.ToList();
            try
            {
                Obiekt.ZdjecieLokal = await SaveImageLocally();
                await (IsNew ? _repository.InsertInstantlyAsync(Obiekt) : _repository.UpdateInstantlyAsync(Obiekt));
                _messenger.Publish(new ToastMessage(this, "Obiekt pomyślnie zapisany!", TimeSpan.FromSeconds(3)));
                _messenger.Publish(new ObiektSavedMessage(this, Obiekt));
                await _navigationService.Close(this);
            }
            catch(Exception e)
            {
                _userDialogs.Toast(e.Message, TimeSpan.FromSeconds(3));
            }
            _userDialogs.HideLoading();

        }

        private async Task<string> SaveImageLocally()
        {
            if (_imageBytes == null) return Obiekt.ZdjecieLokal;
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var algorithm = HashAlgorithm.Create("sha256");
            var hashBytes = algorithm.ComputeHash(_imageBytes);
            var localName = hashBytes.Aggregate(new StringBuilder(), (builder, hb) => builder.Append(hb.ToString("X"))).ToString();
            var fileName = $"{localName}.jpg";
            var outPath = Path.Combine(localFolder, fileName);
            using var stream = new FileStream(outPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            await stream.WriteAsync(_imageBytes, 0, _imageBytes.Length).ConfigureAwait(false);
            if(!string.IsNullOrEmpty(Obiekt.ZdjecieLokal) && File.Exists(Obiekt.ZdjecieLokal))
            {
                File.Delete(Obiekt.ZdjecieLokal);
            }
            return fileName;
        }

        public void ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            SaveCommand.RaiseCanExecuteChanged();
        }
        public override async Task Initialize()
        {
            GrupyObiektow.AddRange(await _repository.GetGrupyAsync());
            Obiekt = await _repository.GetOrCreateAsync(_obiektId);
            ImageBytes = await ReadImageAsync(Obiekt.ZdjecieLokal);
            Obiekt.ErrorsChanged += ErrorsChanged;
            SelectedGrupa = Obiekt.GrupaObiektow;
            Parametry.AddRange(Obiekt.Parametry);
            Parametry.AsParallel().ForAll(p => p.ErrorsChanged += ErrorsChanged);
            Obiekt.ValidateEntity();
        }

        private async Task<byte[]> ReadImageAsync(string zdjecie)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), zdjecie);
            if (string.IsNullOrEmpty(zdjecie) || !File.Exists(path))
            {
                return default;
            }

            using var stream = File.OpenRead(path);
            var bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes, 0, (int) stream.Length);
            return bytes;
        }

        public override void Prepare(int? obiektId)
        {
            _obiektId = obiektId;
            RaisePropertyChanged(() => IsNew);
        }

  
        private void LoadParametry()
        {
            if (!IsNew) return;
            Obiekt.GrupaObiektow = SelectedGrupa;
            Parametry.AsParallel().ForAll(p => p.ErrorsChanged -= ErrorsChanged);
            Parametry.ReplaceRange(SelectedGrupa.TypyParametrow.Select(t => new Parametr()
            {
                TypParametrow = t
            }), 0, Parametry.Count);
            Parametry.AsParallel().ForAll(p => p.ErrorsChanged += ErrorsChanged);
            Parametry.AsParallel().ForAll(p => p.ValidateEntity());
        }

        private bool _isGettingCoords;
        public byte[] ImageBytes { get => _imageBytes; set => SetProperty(ref _imageBytes, value); }
        public bool IsGettingCoords { get => _isGettingCoords; set => SetProperty(ref _isGettingCoords, value); }
        public bool IsNew => _obiektId == null;
        public Obiekt Obiekt { get => _obiekt; set => SetProperty(ref _obiekt, value); }
        public GrupaObiektow SelectedGrupa { get => _selectedGrupa; set => SetProperty(ref _selectedGrupa, value, LoadParametry); }
        public bool CanSave => Obiekt != null && !Obiekt.HasErrors && Parametry.All(p => !p.HasErrors);
        public IMvxAsyncCommand SaveCommand { get; set; }
        public MvxObservableCollection<Parametr> Parametry { get; } = new MvxObservableCollection<Parametr>();
        public MvxObservableCollection<GrupaObiektow> GrupyObiektow { get; } = new MvxObservableCollection<GrupaObiektow>();
        public IMvxAsyncCommand PickImageCommand { get; set; }
        public IMvxAsyncCommand GetCoordsCommand { get; set; }
        public IMvxAsyncCommand BackArrowCommand { get; set; }
    }
}