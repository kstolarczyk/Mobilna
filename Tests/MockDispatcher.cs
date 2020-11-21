using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvvmCross.Base;
using MvvmCross.ViewModels;
using MvvmCross.Views;

namespace Tests
{
    public class MockDispatcher : MvxMainThreadDispatcher, IMvxViewDispatcher
    {
        public readonly List<MvxViewModelRequest> Requests = new List<MvxViewModelRequest>();
        public readonly List<MvxPresentationHint> Hints = new List<MvxPresentationHint>();

        public override bool RequestMainThreadAction(Action action, bool mask = true)
        {
            action();
            return true;
        }

        public Task<bool> ShowViewModel(MvxViewModelRequest request)
        {
            Requests.Add(request);
            return Task.FromResult(true);
        }


        public Task<bool> ChangePresentation(MvxPresentationHint hint)
        {
            Hints.Add(hint);
            return Task.FromResult(true);
        }


        public Task ExecuteOnMainThreadAsync(Action action, bool maskExceptions = true)
        {
            action();
            return Task.CompletedTask;
        }

        public async Task ExecuteOnMainThreadAsync(Func<Task> action, bool maskExceptions = true)
        {
            await action();
        }

        public override bool IsOnMainThread { get; }
    }
}