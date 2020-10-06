using System.ComponentModel.DataAnnotations;
using Core.Utility.Model;

namespace Core.Wrappers
{
    public class LoginWrapper : ValidateBase
    {
        public LoginWrapper()
        {
            ValidateEntity();
            ErrorsChanged += (s, e) => OnPropertyChanged($"{e.PropertyName}Error");
        }
        [Required]
        public string Login { get => GetValue<string>(); set => SetValue(value); }
        [Required]
        public string Password { get => GetValue<string>(); set => SetValue(value); }

        public string LoginError => GetSingleError(nameof(Login));
        public string PasswordError => GetSingleError(nameof(Password));

    }
}