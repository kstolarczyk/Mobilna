using System.Threading.Tasks;
using Acr.UserDialogs;
using Android.Widget;
using Core.Interfaces;

namespace Mobilna.Services
{
    public class ToastNotificationService : INotificationService
    {
        public Task ShowNotificationAsync(string message)
        {
            ShowNotification(message);
            return Task.CompletedTask;
        }

        public void ShowNotification(string message)
        {
            var toast = Toast.MakeText(null, message, ToastLength.Long);
            toast?.Show();
        }
    }
}