using System;
using MvvmCross.Plugin.Messenger;

namespace Core.Messages
{
    public class ToastMessage : MvxMessage
    {
        public string Message { get; }
        public TimeSpan Duration { get; }

        public ToastMessage(object sender, string message, TimeSpan duration) : base(sender)
        {
            Message = message;
            Duration = duration;
        }
    }
}