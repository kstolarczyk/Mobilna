using System;
using System.Globalization;
using Android.App;
using Android.Graphics.Drawables;
using Core.Helpers;
using Core.Utility.Enum;
using MvvmCross.Converters;

namespace Mobilna.Converters
{
    public class CanSynchronizeDrawableConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SynchronizationStatus status)) return null;
            switch (status)
            {
                case SynchronizationStatus.Unavailable:
                    return Application.Context.GetDrawable(Resource.Drawable.ic_sync_disabled_red_400_36dp);
                case SynchronizationStatus.NotStarted:
                    return Application.Context.GetDrawable(Resource.Drawable.ic_sync_green_400_36dp);
                case SynchronizationStatus.InProgress:
                    var animation = (AnimatedVectorDrawable) Application.Context.GetDrawable(Resource.Drawable.anim_sync_green_36dp);
                    animation?.Start();
                    return animation;
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}