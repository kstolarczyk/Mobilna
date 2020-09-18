using Core.Models;
using Core.Utility.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public interface IUserRepository
    {
        public void Insert(User user);
        public void Delete(User user);
        public Task SaveAsync();

    }
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly MyDbContext _context;

        public UserRepository(MyDbContext context)
        {
            _context = context;
        }

        public void Insert(User user)
        {
            _context.Add(user);
        }
        public void Delete(User user)
        {
            _context.Remove(user);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
