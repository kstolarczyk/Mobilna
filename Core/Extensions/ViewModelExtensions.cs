using System;
using System.Runtime.CompilerServices;
using MvvmCross.ViewModels;

namespace Core.Extensions
{
    public static class ViewModelExtensions
    {
        public static void SetValue<T>(this MvxViewModel viewModel, T value,
            [CallerMemberName] string propertyName = "")
        {
            var property = viewModel.GetType().GetProperty(propertyName);
            if(property == null) throw new Exception("Property not found!");
            property.SetValue(viewModel, value);
            viewModel.RaisePropertyChanged(propertyName);
        }

        public static T GetValue<T>(this MvxViewModel viewModel, [CallerMemberName] string propertyName = "")
        {
            var property = viewModel.GetType().GetProperty(propertyName);
            if(property == null) throw new Exception("Property not found!");
            return (T) property.GetValue(viewModel);
        }
    }
}