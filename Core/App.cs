using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Models;
using Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MvvmCross;
using MvvmCross.Core;
using MvvmCross.IoC;
using MvvmCross.ViewModels;

namespace Core
{
    public class App : MvxApplication
    {
        public static Task DbInitialization { get; set; }
        public static object Mutex = new object();
        public static bool LoggedIn { get; set; }
        public override void Initialize()
        {
            CreatableTypes().InNamespace("Core.Repositories").AsInterfaces().RegisterAsDynamic();
            CreatableTypes().InNamespace("Core.Services").AsInterfaces().RegisterAsLazySingleton();
            Mvx.IoCProvider.RegisterType<MyDbContext>();
            var context = Mvx.IoCProvider.Resolve<MyDbContext>();
            lock (Mutex)
            {
                DbInitialization ??= InitializeDatabase();
            }
            DbInitialization.Wait();
            if (context.Users.Any())
            {
                LoggedIn = true;
                RegisterAppStart<ObiektyViewModel>();
            }
            else
            {
                RegisterAppStart<LoginViewModel>();
            }
        }

        public static async Task<bool> InitializeDatabase()
        {
            await using var context = new MyDbContext();
            try
            {
                await context.Database.MigrateAsync();
            }
            catch (Exception e)
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
            }
            return true;
        }
    }
}