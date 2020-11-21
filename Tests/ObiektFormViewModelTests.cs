using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Core.Interfaces;
using Core.Models;
using Core.Repositories;
using Core.ViewModels;
using Moq;
using MvvmCross.Base;
using MvvmCross.Navigation;
using MvvmCross.Navigation.EventArguments;
using MvvmCross.Plugin.Messenger;
using MvvmCross.Tests;
using MvvmCross.ViewModels;
using MvvmCross.Views;
using NUnit.Framework;

namespace Tests
{
    public class ObiektFormViewModelTests : MvxIoCSupportingTest
    {
        private Mock<IUserDialogs> _userDialogsMock;
        private Mock<ILocationService> _locationServiceMock;
        private Mock<IPictureService> _pictureServiceMock;
        private Mock<IMvxNavigationService> _navigationServiceMock;
        private Mock<IObiektRepository> _mockRepo;


        [Test]
        public async Task ShouldProperlyInitializeData()
        {
            var obiekt = MockObiekty.FirstOrDefault(o => o.ObiektId == 1);
            ViewModel.Prepare(obiekt?.ObiektId);
            await ViewModel.Initialize();
            Assert.NotNull(ViewModel.Obiekt);
            Assert.NotNull(ViewModel.SelectedGrupa);
            Assert.True(ViewModel.GrupyObiektow.Count > 0);
            Assert.True(ViewModel.Parametry.Count == (obiekt?.Parametry.Count ?? 0));
            Assert.AreEqual(obiekt, ViewModel.Obiekt);
        }

        [Test]
        public async Task ShouldSelectGrupyLoadParameters()
        {
            var grupa = MockGrupyObiektow.First();
            await ViewModel.Initialize();
            Assert.True(ViewModel.Parametry.Count == 0);
            ViewModel.SelectedGrupa = ViewModel.GrupyObiektow.First(g => g.GrupaObiektowId == grupa.GrupaObiektowId);
            Assert.AreEqual(grupa, ViewModel.SelectedGrupa);
            Assert.True(ViewModel.Parametry.Count == grupa.TypyParametrow.Count);
        }

        [Test]
        public async Task ShouldShowPickImageDialog()
        {
            await ViewModel.PickImageCommand.ExecuteAsync();
            _userDialogsMock.Verify(u => u.ActionSheet(It.IsAny<ActionSheetConfig>()), Times.Once);
        }

        
        [Test]
        public async Task ShouldTakePictureFromService()
        {
            await ViewModel.TakePhoto();

            _pictureServiceMock.Verify(p => p.TakePictureAsync(), Times.Once);
            Assert.True(ViewModel.ImageBytes.Length == 4);
            Assert.True(ViewModel.ImageBytes[0] == 10);
        }

        [Test]
        public async Task ShouldGetLocationFromService()
        {
            await ViewModel.Initialize();
            ViewModel.GetCurrentLocation();

            _locationServiceMock.Verify(l => l.GetLocation(), Times.Once);
            Assert.True(ViewModel.Obiekt.Latitude == 24.0m);
            Assert.True(ViewModel.Obiekt.Longitude == 24.0m);
        }

        [Test]
        public async Task ShouldNavigateToSetLocationMapViewModel()
        {
            await ViewModel.Initialize();
            ViewModel.SetPointOnMap();
            _navigationServiceMock.Verify(n =>
                n.Navigate<SetLocationMapViewModel, Obiekt, Obiekt>(It.IsAny<Obiekt>(),
                    It.IsAny<IMvxBundle>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ShouldSaveObiektWithImage()
        {
            ViewModel.Prepare(1);
            await ViewModel.Initialize();
            await ViewModel.TakePhoto();
            await ViewModel.SaveCommand.ExecuteAsync();

            Assert.NotNull(ViewModel.Obiekt.ZdjecieLokal);
            Assert.True(File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ViewModel.Obiekt.ZdjecieLokal)));

            Assert.False(ViewModel.Obiekt.HasErrors);
            _mockRepo.Verify(m => m.UpdateInstantlyAsync(It.IsAny<Obiekt>()), Times.Once);
        }

        [Test]
        public async Task ShouldNotSaveObiektWithErrors()
        {
            await ViewModel.Initialize();
            await ViewModel.SaveCommand.ExecuteAsync();

            Assert.True(ViewModel.Obiekt.HasErrors);
            _mockRepo.Verify(m => m.InsertInstantlyAsync(It.IsAny<Obiekt>()), Times.Never);
        }

        protected MockDispatcher MockDispatcher {
            get;
            private set;
        }

        protected ObiektFormViewModel ViewModel { get; set; }

        [SetUp]
        public new void Setup()
        {
            base.Setup();
            SetupRepository();

            _userDialogsMock = new Mock<IUserDialogs>(); 
            _locationServiceMock = new Mock<ILocationService>();
            _pictureServiceMock = new Mock<IPictureService>();
            _locationServiceMock.Setup(l => l.GetLocation()).ReturnsAsync((24.0, 24.0));
            _pictureServiceMock.Setup(p => p.ChoosePictureAsync()).ReturnsAsync(new byte[] {1, 2, 3, 4});
            _pictureServiceMock.Setup(p => p.TakePictureAsync()).ReturnsAsync(new byte[] {10, 20, 30, 40});

            Ioc.RegisterSingleton(_userDialogsMock.Object);
            Ioc.RegisterSingleton(_locationServiceMock.Object);
            Ioc.RegisterSingleton(_pictureServiceMock.Object);
            Ioc.RegisterSingleton(new Mock<IMvxMessenger>().Object);
            Ioc.RegisterSingleton(new Mock<IMvxMainThreadAsyncDispatcher>().Object);
            ViewModel = Ioc.IoCConstruct<ObiektFormViewModel>();
        }

        private void SetupRepository()
        {
            _mockRepo = new Mock<IObiektRepository>();
            _mockRepo.Setup(o => o.GetOrCreateAsync(It.IsAny<int?>())).Returns<int?>(i =>
            {
                if (i == null) return Task.FromResult(new Obiekt());
                var obiekt = MockObiekty.FirstOrDefault(o => o.ObiektId == i);
                if (obiekt == null) return null;
                obiekt.GrupaObiektow =
                    MockGrupyObiektow.FirstOrDefault(g => g.GrupaObiektowId == obiekt.GrupaObiektowId);
                return Task.FromResult(obiekt);
            });
            _mockRepo.Setup(o => o.GetGrupyAsync()).Returns(() =>
            {
                var typy = MockTypyParametrow;
                var grupy = MockGrupyObiektow;
                grupy.ForEach(g => g.TypyParametrow = typy.Where(t => g.GrupaObiektowTypParametrow.Any(gt => gt.TypParametrowId == t.TypParametrowId)).ToList());
                return Task.FromResult(grupy);
            });
            Ioc.RegisterSingleton(_mockRepo.Object);
        }

        protected override void AdditionalSetup()
        {
            _navigationServiceMock = new Mock<IMvxNavigationService>();
            _navigationServiceMock.Setup(n => n.Navigate(It.IsAny<Type>(), It.IsAny<It.IsAnyType>(),
                It.IsAny<IMvxBundle>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
            _navigationServiceMock.Setup(n => n.Navigate(It.IsAny<Type>(),
                It.IsAny<IMvxBundle>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
            MockDispatcher = new MockDispatcher();
            Ioc.RegisterSingleton<IMvxViewDispatcher>(MockDispatcher);
            Ioc.RegisterSingleton<IMvxMainThreadDispatcher>(MockDispatcher);
            Ioc.RegisterSingleton(_navigationServiceMock.Object);
        }

        #region MockData

        public static List<GrupaObiektow> MockGrupyObiektow = new List<GrupaObiektow>()
        {
            new GrupaObiektow()
            {
                GrupaObiektowId = 1, Nazwa = "Testowe #1", Symbol = "Test#1",GrupaObiektowTypParametrow = new List<GrupaObiektowTypParametrow>()
                {
                    new GrupaObiektowTypParametrow() { GrupaObiektowId = 1, TypParametrowId = 1},
                    new GrupaObiektowTypParametrow() { GrupaObiektowId = 1, TypParametrowId = 2},
                }
            },
            new GrupaObiektow()
            {
                GrupaObiektowId = 2, Nazwa = "Testowe #2", Symbol = "Test#2",GrupaObiektowTypParametrow = new List<GrupaObiektowTypParametrow>()
                {
                    new GrupaObiektowTypParametrow() { GrupaObiektowId = 2, TypParametrowId = 2},
                    new GrupaObiektowTypParametrow() { GrupaObiektowId = 2, TypParametrowId = 3},
                }
            },
        };

        public static List<TypParametrow> MockTypyParametrow = new List<TypParametrow>()
        {
            new TypParametrow() { Nazwa = "Szerko��", Symbol = "Width", JednostkaMiary = "metr", TypDanych = "FLOAT"},
            new TypParametrow() { Nazwa = "Data", Symbol = "Date", TypDanych = "DATE"},
            new TypParametrow() { Nazwa = "Typ", Symbol = "Type", TypDanych = "ENUM", AkceptowalneWartosci = new []
            {
                "Testowy typ 1",
                "Testowy typ 2"
            }},
        };

        public static List<Obiekt> MockObiekty = new List<Obiekt>()
        {
            new Obiekt() { ObiektId = 1, GrupaObiektowId = 1, Nazwa = "Testowy #1", Symbol = "#1", Parametry = new List<Parametr>()
            {
                new Parametr() {ObiektId = 1, ParametrId = 1, TypParametrowId = 1, Wartosc = "15"},
                new Parametr() {ObiektId = 1, ParametrId = 2, TypParametrowId = 2, Wartosc = "2020-01-01"},
            }},
            new Obiekt() { ObiektId = 2, GrupaObiektowId = 2, Nazwa = "Testowy #2", Symbol = "#2", Parametry = new List<Parametr>()
            {
                new Parametr() {ObiektId = 2, ParametrId = 3, TypParametrowId = 3, Wartosc = "Testowy typ 1"},
                new Parametr() {ObiektId = 2, ParametrId = 4, TypParametrowId = 2, Wartosc = "2020-01-01"},
            }},
        };


        #endregion
    }
}