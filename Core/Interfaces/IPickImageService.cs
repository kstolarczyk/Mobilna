using System.IO;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPickImageService
    {
        Task<byte[]> GetImageStreamAsync();
    }
}