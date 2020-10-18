using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface INotificationService
    {
        Task ShowNotificationAsync(string message);
        void ShowNotification(string message);
    }
}