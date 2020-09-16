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
    }
}