using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Services
{
    public interface IObiektyDataService
    {
        Task<List<Obiekt>> GetObiektyAsync();
    }
    public class ObiektyDataService : IObiektyDataService
    {
        public ObiektyDataService()
        {
            
        }
        public Task<List<Obiekt>> GetObiektyAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}