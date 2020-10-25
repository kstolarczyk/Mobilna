using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPictureService
    {
        void TakePicture();
        void ChoosePicture();
        Task<byte[]> TakePictureAsync();
        Task<byte[]> ChoosePictureAsync();
    }
}