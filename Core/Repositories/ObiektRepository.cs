using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Utility.Repository;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories
{
    public interface IObiektRepository
    {
        IAsyncEnumerable<Obiekt> GetAsStream();
        Task<List<Obiekt>> GetAllAsync();
        Task<Obiekt> GetOneAsync(int obiektId);
        void Insert(Obiekt obiekt);
        void Update(Obiekt obiekt);
        void Delete(Obiekt obiekt);
        Task InsertInstantlyAsync(Obiekt obiekt);
        Task DeleteInstantlyAsync(Obiekt obiekt);
        Task UpdateInstantlyAsync(Obiekt obiekt);
        Task SaveChangesAsync();
    }

    public class ObiektRepository : BaseRepository, IObiektRepository
    {
        private readonly MyDbContext _context;

        public ObiektRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Obiekt>> GetAllAsync()
        {
            return await _context.Obiekty
                .Where(o => o.Status != 3).AsNoTracking().ToListAsync().ConfigureAwait(false);
        }

        public IAsyncEnumerable<Obiekt> GetAsStream()
        {
            return _context.Obiekty
                .Where(o => o.Status != 3).AsNoTracking().AsAsyncEnumerable();
        }

        public async Task<Obiekt> GetOneAsync(int obiektId)
        {
            return await _context.Obiekty.Include(o => o.User).FirstOrDefaultAsync(o => o.ObiektId == obiektId).ConfigureAwait(false);
        }

        public void Insert(Obiekt obiekt)
        {
            obiekt.Status = 1;
            _context.Add(obiekt);
        }

        public void Update(Obiekt obiekt)
        {
            obiekt.Status = 2;
            _context.Update(obiekt);
        }

        public void Delete(Obiekt obiekt)
        {
            obiekt.Status = 3;
            _context.Update(obiekt);
        }

        public async Task InsertInstantlyAsync(Obiekt obiekt)
        {
            Insert(obiekt);
            await SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteInstantlyAsync(Obiekt obiekt)
        {
            Delete(obiekt);
            await SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateInstantlyAsync(Obiekt obiekt)
        {
            Update(obiekt);
            await SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}