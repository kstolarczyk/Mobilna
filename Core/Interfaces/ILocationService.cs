using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ILocationService
    {
        Task<(double,double)> GetLocation();
    }
}