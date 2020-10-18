using Core.Models;
using Core.Utility.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public interface IGrupaObiektowRepository
    {
        Task<GrupaObiektow> GetOneAsync(int id);
        GrupaObiektow GetOne(int id);
        IAsyncEnumerable<GrupaObiektow> GetAsStream();
        Task<List<GrupaObiektow>> GetAllAsync();

    }
        class GrupaObiektowRepository : BaseRepository, IGrupaObiektowRepository
    {
        private readonly MyDbContext _context;

        public GrupaObiektowRepository(MyDbContext context)
        {
            _context = context;
        }
        public async Task<List<GrupaObiektow>> GetAllAsync()
        {
             return await _context.GrupyObiektow.Include(t => t.TypyParametrow).AsNoTracking().ToListAsync().ConfigureAwait(false);
        }
        public async Task<GrupaObiektow> GetOneAsync(int id)
        {
            return await _context.GrupyObiektow.FindAsync(id).ConfigureAwait(false);
        }
        public IAsyncEnumerable<GrupaObiektow> GetAsStream()
        {
            return _context.GrupyObiektow.Include(t => t.TypyParametrow).AsNoTracking().AsAsyncEnumerable();
        }

        public GrupaObiektow GetOne(int id)
        {
            return _context.GrupyObiektow.Find(id);
        }
    }
}
