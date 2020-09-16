using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Utility.Repository
{
    public class BaseRepository
    {
        static BaseRepository()
        {
            lock (App.Mutex)
            {
                App.DbInitialization ??= App.InitializeDatabase();   
            }
            
            App.DbInitialization.Wait();
        }
    }
}