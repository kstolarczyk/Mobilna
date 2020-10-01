using System;
using System.Globalization;
using MvvmCross.Converters;

namespace Mobilna.Converters
{
    public class IsConnectedConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isConnected && targetType == typeof(string))
            {
                return isConnected ? "Online" : "Offline";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}