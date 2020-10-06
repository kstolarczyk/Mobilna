﻿using System;
using System.Globalization;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using MvvmCross.Converters;

namespace Mobilna.Converters
{
    public class IsConnectedDrawableConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool isConnected)
            {
                return isConnected
                    ? Application.Context.GetDrawable(Resource.Drawable.ic_wifi_tethering_green_400_36dp)
                    : Application.Context.GetDrawable(Resource.Drawable.ic_portable_wifi_off_red_400_36dp);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}