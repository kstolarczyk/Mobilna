using System;
using System.Globalization;
using Android.Graphics;
using MvvmCross.Converters;

namespace Mobilna.Converters
{
    public class IsConnectedColorConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isConnected)
            {
                return isConnected ? Color.ParseColor("#FF2BAF2B") : Color.ParseColor("#FFE84E40");
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}