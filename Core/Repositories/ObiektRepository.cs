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
        IAsyncEnumerable<Obiekt> GetAsStream(int grupaId);
        Task<List<Obiekt>> GetAllAsync(int grupaId);
        Task<List<GrupaObiektow>> GetGrupyAsync();
        Task<Obiekt> GetOneAsync(int obiektId);
        Task<Obiekt> GetOrCreateAsync(int? obiektId); 
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

        public async Task<List<GrupaObiektow>> GetGrupyAsync()
        {
            return await _context.GrupyObiektow.AsNoTracking().ToListAsync().ConfigureAwait(false);
        }
        public async Task<List<Obiekt>> GetAllAsync(int grupaId)
        {
            return await _context.Obiekty
                .Where(o => o.Status != 3 && o.GrupaObiektowId == grupaId).AsNoTracking().ToListAsync().ConfigureAwait(false);
        }

        public IAsyncEnumerable<Obiekt> GetAsStream(int grupaId)
        {
            return _context.Obiekty
                .Where(o => o.Status != 3 && o.GrupaObiektowId == grupaId).AsNoTracking().AsAsyncEnumerable();
        }

        public async Task<Obiekt> GetOneAsync(int obiektId)
        {
            return await _context.Obiekty.Include(o => o.User)
                .Include(o => o.GrupaObiektow)
                .Include(o => o.Parametry)
                .ThenInclude(p => p.TypParametrow).FirstOrDefaultAsync(o => o.ObiektId == obiektId).ConfigureAwait(false);
        }

        public async Task<Obiekt> GetOrCreateAsync(int? obiektId)
        {
            return obiektId == null ? new Obiekt() : await GetOneAsync((int) obiektId).ConfigureAwait(false);
        }

        public void Insert(Obiekt obiekt)
        {
            obiekt.Status = 1;
            obiekt.User = _context.Users.FirstOrDefault();
            _context.Update(obiekt);
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