using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Core.Utility.Model
{
    public class BindableBase : INotifyPropertyChanged
    {
        protected readonly Dictionary<string, object> Properties;

        public BindableBase()
        {
            Properties = GetModelProperties();
        }

        protected Dictionary<string, object> GetModelProperties()
        {
            return GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(p => p.Name, p => (object)null);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual T GetValue<T>([CallerMemberName] string propertyName = "")
        {
            return (T)Properties[propertyName];
        }

        protected virtual void SetValue<T>(T value, [CallerMemberName] string propertyName = "")
        {
            Properties[propertyName] = value;
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}