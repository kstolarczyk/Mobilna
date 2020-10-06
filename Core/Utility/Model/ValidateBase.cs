using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

namespace Core.Utility.Model
{
    public class ValidateBase : BindableBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errorsByProperty = new Dictionary<string, List<string>>();
        public ValidateBase()
        {
        }

        public void ValidateEntity()
        {
            foreach (var property in Properties)
            {
                ValidateProperty(property.Key, property.Value);
            }
        }
        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName) => GetErrors(propertyName);
        public IEnumerable<string> GetErrors(string propertyName)
        {
            return _errorsByProperty[propertyName] ?? Enumerable.Empty<string>();
        }

        public string GetSingleError(string propertyName)
        {
            return _errorsByProperty[propertyName]?.FirstOrDefault();
        }

        protected override void SetValue<T>(T value, [CallerMemberName] string propertyName = "")
        {
            base.SetValue(value, propertyName);
            ValidateProperty(propertyName, value);
        }

        private void ValidateProperty<T>(string propertyName, T value)
        {
            ClearErrors(propertyName);
            var validationContext = new ValidationContext(this, null, null) { MemberName = propertyName};
            var result = new List<ValidationResult>();
            if (!Validator.TryValidateProperty(value, validationContext, result))
            {
                AddErrors(propertyName, result);
            }

            var customValidationErrors = ValidateCustom(propertyName).ToList();
            if (!customValidationErrors.Any()) return;
            foreach (var error in customValidationErrors)
            {
                AddError(propertyName, error);
            }
            OnErrorsChanged(propertyName);
        }

        protected virtual IEnumerable<string> ValidateCustom(string propertyName)
        {
            return Enumerable.Empty<string>();
        }

        private void ClearErrors(string propertyName)
        {
            if(!_errorsByProperty.Remove(propertyName)) return;
            OnErrorsChanged(propertyName);
        }

        private void AddErrors(string propertyName, IEnumerable<ValidationResult> errors)
        {
            foreach (var validationResult in errors)
            {
                AddError(propertyName, validationResult.ErrorMessage, false);
            }
            OnErrorsChanged(propertyName);
        }

        public void AddError(string propertyName, string errorMessage, bool notify = true)
        {
            if (!_errorsByProperty.ContainsKey(propertyName))
            {
                _errorsByProperty.Add(propertyName, new List<string>());
            }
            _errorsByProperty[propertyName].Add(errorMessage);
            if(notify) OnErrorsChanged(propertyName);
        }

        private void OnErrorsChanged([CallerMemberName] string propertyName = "")
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public bool HasErrors => _errorsByProperty.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    }
}