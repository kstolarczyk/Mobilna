using MvvmCross.Plugin.Messenger;

namespace Core.Messages
{
    public class LocationMessage : MvxMessage
    {
        public LocationMessage(object sender, double latitude, double longitude) : base(sender)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
        public double Latitude { get; }
        public double Longitude { get; }
    }
}