using System;
using System.Linq;
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

        public static int IndexOf<T>(this MvxObservableCollection<T> collection, Func<T, bool> predicate)
        {
            var find = collection.AsParallel().Select((item, i) => (i, predicate(item))).FirstOrDefault(p => p.Item2);
            return find != default ? find.i : -1;
        }
    }
}