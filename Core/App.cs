using System.Net.Http;
using Core.ViewModels;
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

            RegisterAppStart<ObiektyViewModel>();
        }
    }
}