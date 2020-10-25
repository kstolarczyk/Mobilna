using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ITakePhotoService
    {
        Task<byte[]> TakePhoto();
    }
}