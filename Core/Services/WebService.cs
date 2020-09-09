using MvvmCross.Plugin.Network.Rest;

namespace Core.Services
{
    public class WebService
    {
        private readonly IMvxJsonRestClient _client;

        public WebService(IMvxJsonRestClient client)
        {
            _client = client;
        }
    }
}