using System.IO;
using MvvmCross.Plugin.Messenger;

namespace Core.Messages
{
    public class PictureMessage : MvxMessage
    {
        public byte[] Bytes { get; }

        public PictureMessage(object sender, byte[] bytes) : base(sender)
        {
            Bytes = bytes;
        }
    }
}