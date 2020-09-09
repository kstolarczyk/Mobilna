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
        private readonly Dictionary<string, PropertyInfo> _properties;
        public BindableBase()
        {
            _properties = GetModelProperties();
        }

        protected Dictionary<string, PropertyInfo> GetModelProperties()
        {
            return GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(p => p.Name, p => p);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual T GetValue<T>([CallerMemberName] string propertyName = "")
        {
            return (T)_properties[propertyName].GetValue(this);
        }

        protected virtual void SetValue<T>(T value, [CallerMemberName] string propertyName = "")
        {
            _properties[propertyName].SetValue(this, value);
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}