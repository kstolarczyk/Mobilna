using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Core.Interfaces;
using Core.Models;
using Core.Repositories;
using Core.Utility.ViewModel;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Core.ViewModels
{
    public class ObiektFormViewModel : BaseViewModel<int?>
    {
        private readonly IObiektRepository _repository;
        private readonly IGrupaObiektowRepository _grupaRepository;
        private readonly IMvxNavigationService _navigationService;
        private GrupaObiektow _selectedGrupa;
        private readonly IPickImageService _pickImageService;
        private Obiekt _obiekt;
        private int? _obiektId;
        private byte[] _imageBytes;


        public ObiektFormViewModel(IObiektRepository repository, IGrupaObiektowRepository grupaObiektowRepository, IMvxNavigationService navigationService, IPickImageService pickImageService)
        {
            _repository = repository;
            _grupaRepository = grupaObiektowRepository;
            _navigationService = navigationService;
            _pickImageService = pickImageService;
            SaveCommand = new MvxAsyncCommand(Save, () => CanSave);
            PickImageCommand = new MvxAsyncCommand(PickImage);
        }

        private async Task PickImage()
        {
            _imageBytes = await _pickImageService.GetImageStreamAsync();
        }


        private async Task Save()
        {
            Obiekt.GrupaObiektow = SelectedGrupa;
            Obiekt.Parametry = Parametry.ToList();
            try
            {
                Obiekt.ZdjecieLokal = await SaveImageLocally();
                await (IsNew ? _repository.InsertInstantlyAsync(Obiekt) : _repository.UpdateInstantlyAsync(Obiekt));
                await _navigationService.Close(this);
                Mvx.IoCProvider.Resolve<IUserDialogs>().Toast("Obiekt pomyślnie zapisany!", TimeSpan.FromSeconds(3));
            }
            catch(Exception e)
            {
                Mvx.IoCProvider.Resolve<IUserDialogs>().Toast(e.Message, TimeSpan.FromSeconds(3));
            }
        }

        private async Task<string> SaveImageLocally()
        {
            if (_imageBytes == null) return Obiekt.ZdjecieLokal;
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var algorithm = HashAlgorithm.Create();
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
            GrupyObiektow.AddRange(await _grupaRepository.GetAllAsync());
            Obiekt = await _repository.GetOrCreateAsync(_obiektId);
            Obiekt.ErrorsChanged += ErrorsChanged;
            SelectedGrupa = Obiekt.GrupaObiektow;
            Parametry.AddRange(Obiekt.Parametry);
            Parametry.AsParallel().ForAll(p => p.ErrorsChanged += ErrorsChanged);
            Obiekt.ValidateEntity();
        }

        public override void Prepare(int? obiektId)
        {
            _obiektId = obiektId;
            RaisePropertyChanged(() => IsNew);
        }

  
        private void LoadParametry()
        {
            Obiekt.GrupaObiektow = SelectedGrupa;
            if (!IsNew) return;
            Parametry.AsParallel().ForAll(p => p.ErrorsChanged -= ErrorsChanged);
            Parametry.ReplaceRange(SelectedGrupa.TypyParametrow.Select(t => new Parametr()
            {
                TypParametrow = t
            }), 0, Parametry.Count);
            Parametry.AsParallel().ForAll(p => p.ErrorsChanged += ErrorsChanged);
            Parametry.AsParallel().ForAll(p => p.ValidateEntity());
        }

        public bool IsNew => _obiektId == null;
        public Obiekt Obiekt { get => _obiekt; set => SetProperty(ref _obiekt, value); }
        public GrupaObiektow SelectedGrupa { get => _selectedGrupa; set => SetProperty(ref _selectedGrupa, value, LoadParametry); }
        public bool CanSave => Obiekt != null && !Obiekt.HasErrors && Parametry.All(p => !p.HasErrors);
        public IMvxAsyncCommand SaveCommand { get; set; }
        public MvxObservableCollection<Parametr> Parametry { get; } = new MvxObservableCollection<Parametr>();
        public MvxObservableCollection<GrupaObiektow> GrupyObiektow { get; } = new MvxObservableCollection<GrupaObiektow>();
        public IMvxAsyncCommand PickImageCommand { get; set; }
    }
}