using System;
using System.Globalization;
using System.IO;
using Android.Content.Res;
using Android.Graphics;
using MvvmCross.Converters;
using Path = System.IO.Path;
using Uri = Android.Net.Uri;

namespace Mobilna.Converters
{
    public class ImagePathBitmapConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string path) || targetType != typeof(Bitmap)) return null;
            if (!File.Exists(path))
                return BitmapFactory.DecodeStream(Android.App.Application.Context.Assets?.Open("placeholder.jpg"));
            var stream = File.OpenRead(path);
            var bmp = BitmapFactory.DecodeStream(stream);
            return bmp;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}