using System.ComponentModel.DataAnnotations;
using Core.Utility.Model;

namespace Core.Wrappers
{
    public class LoginWrapper : ValidateBase
    {
        private string _login;
        private string _password;
        public LoginWrapper()
        {
            ValidateEntity();
            ErrorsChanged += (s, e) => RaisePropertyChanged($"{e.PropertyName}Error");
        }
        [Required]
        public string Login { get => _login; set => SetProperty(ref _login, value); }
        [Required]
        public string Password { get => _password; set => SetProperty(ref _password, value); }

        public string LoginError => GetSingleError(nameof(Login));
        public string PasswordError => GetSingleError(nameof(Password));

    }
}