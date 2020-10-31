using MvvmCross.Plugin.Messenger;

namespace Core.Messages
{
    public class SyncStatusChangedMessage : MvxMessage
    {
        public bool IsSynchronizing { get; }

        public SyncStatusChangedMessage(object sender, bool isSynchronizing) : base(sender)
        {
            IsSynchronizing = isSynchronizing;
        }
    }
}