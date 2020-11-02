﻿using Core.Models;
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
using Acr.UserDialogs;
using Core.Exceptions;
using Core.Wrappers;
using MvvmCross;
using MvvmCross.Navigation;

namespace Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserRepository _repository;
        private readonly IRemoteLoginService _loginService;
        private readonly IMvxNavigationService _navigationService;
        private readonly IUserDialogs _userDialogs;

        public LoginViewModel(IUserRepository repository, IRemoteLoginService loginService,
            IMvxNavigationService navigationService, IUserDialogs userDialogs)
        {
            _repository = repository;
            _loginService = loginService;
            _navigationService = navigationService;
            _userDialogs = userDialogs;
            LoginModel = new LoginWrapper();
            SubmitCommand = new MvxAsyncCommand(Login, () => !LoginModel.HasErrors);
            LoginModel.ErrorsChanged += (s, e) => SubmitCommand.RaiseCanExecuteChanged();
        }

        public async Task Login()
        {
            if(App.LoggedIn) await _navigationService.Navigate<MainViewModel>();
            try
            {
                _userDialogs.ShowLoading("Logowanie...");
                var user = await _loginService.LoginAsync(LoginModel.Login, LoginModel.Password);
                _repository.Insert(user);
                await _repository.SaveAsync();
                App.LoggedIn = true;
                await _navigationService.Navigate<MainViewModel>();
            }
            catch(ApiLoginException e)
            {
                LoginModel.AddError(e.ApiError == ApiLoginError.Password ? nameof(LoginModel.Password) : nameof(LoginModel.Login), e.Message);
            }
            _userDialogs.HideLoading();
        }

        
        public LoginWrapper LoginModel { get; set; }
        public IMvxAsyncCommand SubmitCommand { get; set; }

    }
}

