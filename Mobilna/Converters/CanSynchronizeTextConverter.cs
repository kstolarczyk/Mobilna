using System;
using System.Globalization;
using Android.App;
using Core.Helpers;
using Core.Utility.Enum;
using MvvmCross.Converters;

namespace Mobilna.Converters
{
    public class CanSynchronizeTextConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SynchronizationStatus status)) return null;
            return status switch
            {
                SynchronizationStatus.Unavailable => "Niedostępne",
                SynchronizationStatus.NotStarted => "Synchronizuj",
                SynchronizationStatus.InProgress => "Synchronizuje...",
                _ => null
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}