using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Models;
using MvvmCross.Plugin.Network.Rest;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Core.Services
{
    public class WebService
    {
        private readonly RestClient _client;
        private readonly User _user;
        private readonly object _credentials;
        public const string ApiBaseUrl = "http://192.168.1.105/EwidencjaObiektow/index.php/Api";

        public WebService(MyDbContext context)
        {
            _client = new RestClient(ApiBaseUrl);
            _client.UseNewtonsoftJson();
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
            _user = context.Users.FirstOrDefault();
            _credentials = new { credentials = new {base64_login = Convert.ToBase64String(_user?.Username.Select(Convert.ToByte).ToArray() ?? Array.Empty<byte>()), base64_password = _user?.EncodedPassword} };
        }

        public async Task<List<Obiekt>> GetObiektyAsync(IEnumerable<int> grupyIds)
        {
            var tasks = grupyIds.Select(id => new RestRequest($"Obiekt/Lista/{id}").AddJsonBody(_credentials))
                .Select(r => _client.PostAsync<List<Obiekt>>(r)).ToList();
            await Task.WhenAll(tasks);
            var obiekty = tasks.Where(t => !t.IsFaulted).SelectMany(t => t.Result).ToList();
            return obiekty;
        }

    }
}