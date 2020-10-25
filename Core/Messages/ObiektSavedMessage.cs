using Core.Models;
using MvvmCross.Plugin.Messenger;

namespace Core.Messages
{
    public class ObiektSavedMessage : MvxMessage
    {
        public Obiekt Obiekt { get; }

        public ObiektSavedMessage(object sender, Obiekt obiekt) : base(sender)
        {
            Obiekt = obiekt;
        }
    }
}