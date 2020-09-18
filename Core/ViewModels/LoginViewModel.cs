using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.Utility.ViewModel;
using MvvmCross.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Navigation;

namespace Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserRepository _repository;
        private readonly IRemoteLoginService _loginService;
        private readonly IMvxNavigationService _navigationService;
        private string _login;
        private string _password;

        public LoginViewModel(IUserRepository repository, IRemoteLoginService loginService, IMvxNavigationService navigationService)
        {
            _repository = repository;
            _loginService = loginService;
            _navigationService = navigationService;
        }
        public IMvxAsyncCommand SubmitCommand => new MvxAsyncCommand(Login);

        public async Task Login()
        {
            try
            {
                var user = await _loginService.LoginAsync(LoginEdit, PasswordEdit);
                _repository.Insert(user);
                await _repository.SaveAsync();
                App.LoggedIn = true;
                await _navigationService.Navigate<ObiektyViewModel>();
            }
            catch (Exception e)
            {
                LoginEdit = "chujnia";
            }

        }

        public string LoginEdit
        {
            get => _login;
            set
            {
                _login = value;
                RaisePropertyChanged(() => LoginEdit);
            }
        }
        public string PasswordEdit
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChanged(() => PasswordEdit);
            }
        }
    }
}

