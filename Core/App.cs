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
        public override void Initialize()
        {
            CreatableTypes().InNamespace("Core.Repositories").AsInterfaces().RegisterAsDynamic();
            CreatableTypes().InNamespace("Core.Services").AsInterfaces().RegisterAsLazySingleton();
            Mvx.IoCProvider.RegisterType<MyDbContext>();
            RegisterAppStart<ObiektyViewModel>();
            // InitializeFirstData();
        }

        public static async Task InitializeDatabase()
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
        }

        private async void InitializeFirstData()
        {
            try
            {
                await using var context = Mvx.IoCProvider.Resolve<MyDbContext>();
                await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
                if (!context.TypyParametrow.Any()) await InsertTypyParametrow(context);
                if (!context.GrupyObiektow.Any()) await InsertGrupyObiektow(context);
                if (!context.Users.Any()) await InsertUsers(context);
                // if (!context.Obiekty.Any()) await InsertObiekty(context);
                await context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                // ignored
            }

        }

        private async Task InsertUsers(MyDbContext context)
        {
            await context.Users.AddRangeAsync(InMemoryData.Users);
        }

        private async Task InsertObiekty(MyDbContext context)
        {
            await context.Obiekty.AddRangeAsync(InMemoryData.Obiekty);
        }

        private async Task InsertGrupyObiektow(MyDbContext context)
        {
            await context.GrupyObiektow.AddRangeAsync(InMemoryData.Grupy);
        }

        private async Task InsertTypyParametrow(MyDbContext context)
        {
            await context.TypyParametrow.AddRangeAsync(InMemoryData.TypyParametrow);
        }
    }
}