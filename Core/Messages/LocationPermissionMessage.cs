using MvvmCross.Plugin.Location;
using MvvmCross.Plugin.Messenger;

namespace Core.Messages
{
    public class LocationPermissionMessage : MvxMessage
    {
        public MvxLocationPermission Status { get; }

        public LocationPermissionMessage(object sender, MvxLocationPermission status) : base(sender)
        {
            Status = status;
        }
    }
}