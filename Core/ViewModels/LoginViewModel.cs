using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.Utility.ViewModel;
using MvvmCross.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserRepository _repository;
        private readonly WebService _webService;
        private readonly MyDbContext _context;
        private string _login;
        private string _password;
        public ICommand Submit { private set; get; }


        public LoginViewModel(IUserRepository repository)
        {
            _repository = repository;
            _context = new MyDbContext();


        }
        public MvxCommand SubmitCommand
        {
            get
            {
                return new MvxCommand(() => Login());
            }
        }

        public string LoginEdit
        {
            get { return _login; }
            set
            {
                _login = value;
                RaisePropertyChanged(() => LoginEdit);
            }
        }
        public string PasswordEdit
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged(() => PasswordEdit);
            }
        }


        public void Login()
        {
            if (_repository.CheckUserAsync(_login) != null) // Jeśeli user jest w bazie to:
            {
                // załadowanie widoku ObiektyView
            }
            var task = _webService.GetUserAsync(_login, _password);
            task.Wait();
           // User user = task.Result;
          //  _repository.Insert(user);
        }
    }
}

