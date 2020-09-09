using Core.Utility.Model;
using MvvmCross.ViewModels;

namespace Core.Utility.ViewModel
{
    public class BaseViewModel : MvxViewModel
    {
    }

    public class BaseViewModel<T> : MvxViewModel<T>
    {
        public override void Prepare(T parameter)
        {
            throw new System.NotImplementedException();
        }
    }

}