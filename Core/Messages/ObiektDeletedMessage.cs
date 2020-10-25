using Core.Models;
using MvvmCross.Plugin.Messenger;

namespace Core.Messages
{
    public class ObiektDeletedMessage : MvxMessage
    {
        public Obiekt Obiekt { get; }

        public ObiektDeletedMessage(object sender, Obiekt obiekt) : base(sender)
        {
            Obiekt = obiekt;
        }
    }
}