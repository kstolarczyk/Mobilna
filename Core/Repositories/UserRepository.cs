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
        public Task<User> CheckUserAsync(string login);
        public void Insert(User user);
        public void Delete(User user);

    }
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly MyDbContext _context;

        public UserRepository(MyDbContext context)
        {
            _context = context;
        }


        public async Task<User> CheckUserAsync(string login)
        {
            return await _context.Users.FindAsync(login).ConfigureAwait(false);
        }

        public void Insert(User user)
        {
            _context.Add(user);
        }
        public void Delete(User user)
        {
            _context.Remove(user);
        }
    }
}
