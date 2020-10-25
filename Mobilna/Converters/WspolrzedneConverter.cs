using System;
using System.Globalization;
using MvvmCross.Converters;

namespace Mobilna.Converters
{
    public class WspolrzedneConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string coords && parameter is bool isGettingCoords)) return null;
            return isGettingCoords ? "Ustalanie lokalizacji..." : coords;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}