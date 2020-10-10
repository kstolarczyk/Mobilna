using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Acr.UserDialogs;
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
            Mvx.IoCProvider.RegisterType(() => UserDialogs.Instance);
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
                // await context.Database.EnsureDeletedAsync(); // delete database
                await context.Database.MigrateAsync().ConfigureAwait(false);
                // await DodajObiektTestowy(context).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await context.Database.EnsureDeletedAsync().ConfigureAwait(false);
                await context.Database.MigrateAsync().ConfigureAwait(false);
            }


            return true;
        }

        private static async Task DodajObiektTestowy(MyDbContext context)
        {
            var grupa = await context.GrupyObiektow.Include(g => g.TypyParametrow).FirstOrDefaultAsync().ConfigureAwait(false);
            if (grupa == null) return;
            
            var obiekt = new Obiekt()
            {
                Nazwa = "TestowyObiekt",
                Symbol = "TEST",
                Latitude = 15.0m,
                Longitude = 15.0m,
                Status = 1,
                OstatniaAktualizacja = DateTime.Now
            };

            var parametry = grupa.TypyParametrow.Select(t => new Parametr()
            {
                TypParametrowId = t.TypParametrowId,
                Wartosc = "5"
            });
            obiekt.GrupaObiektow = grupa;
            obiekt.Parametry.AddRange(parametry);
            context.Add(obiekt);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}