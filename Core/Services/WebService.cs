using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using MvvmCross.Plugin.Network.Rest;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Core.Services
{
    public interface IRemoteLoginService
    {
        Task<User> LoginAsync(string login, string password);
    }
    public class WebService : IRemoteLoginService
    {
        private readonly RestClient _client;
        private readonly object _credentials;
        // public const string ApiBaseUrl = "https://77.55.217.165/EwidencjaObiektow/index.php/Api";
        public const string ApiBaseUrl = "http://192.168.1.118/EwidencjaObiektow/index.php/Api";

        public WebService(MyDbContext context)
        {
            _client = new RestClient(ApiBaseUrl);
            _client.UseNewtonsoftJson(new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
            var user = context.Users.AsNoTracking().FirstOrDefault();
            _credentials = new {base64_login = Convert.ToBase64String(user?.Username.Select(Convert.ToByte).ToArray() ?? Array.Empty<byte>()), base64_password = user?.EncodedPassword};
        }

        public async Task<List<Obiekt>> GetObiektyAsync(DateTime? lastUpdate, IEnumerable<int> grupyIds)
        {
            var tasks = grupyIds.Select(id => new RestRequest($"Obiekt/Lista/{id}").AddJsonBody(new
                {
                    credentials = _credentials, lastUpdate
                }))
                .Select(r => _client.PostAsync<ApiResponse<Obiekt>>(r)).ToList();
            try
            {
                await Task.WhenAll(tasks);
                return tasks.Where(t => !t.IsFaulted).SelectMany(t => t.Result.Data).ToList();
            }
            catch (Exception e)
            {
                return new List<Obiekt>();
            }
            
        }

        public async Task<List<GrupaObiektow>> GetGrupyAsync(DateTime? lastUpdate)
        {
            var request = new RestRequest("GrupaObiektow").AddJsonBody(new
            {
                credentials = _credentials, lastUpdate
            });
            try
            {
                var response = await _client.PostAsync<ApiResponse<GrupaObiektow>>(request);
                return response.Data;
            }
            catch (Exception e)
            {
                return new List<GrupaObiektow>();
            }
        }

        public async Task<List<TypParametrow>> GetTypyAsync(DateTime? lastUpdate)
        {
            var request = new RestRequest("TypParametru").AddJsonBody(new
            {
                credentials = _credentials, lastUpdate
            });
            try
            {
                var response = await _client.PostAsync<ApiResponse<TypParametrow>>(request);
                return response.Data;
            }
            catch (Exception e)
            {
                return new List<TypParametrow>();
            }
        }
        public async Task<User> LoginAsync(string login, string password)
        {
            var credentials = new { credentials = new { base64_login = Convert.ToBase64String(Encoding.UTF8.GetBytes(login)), base64_password = Convert.ToBase64String(Encoding.UTF8.GetBytes(password)) } };
            var request = new RestRequest($"User").AddJsonBody(credentials);
            var response = await _client.ExecutePostAsync(request);
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("Wrong credentials!");
            }
            
            var user = JsonConvert.DeserializeObject<ApiResponse<User>>(response.Content).Data.FirstOrDefault();
            if (user != null) user.EncodedPassword = credentials.credentials.base64_password;
            return user;
        }
    }

    public class ApiResponse<T>
    {
        public List<string> Errors { get; set; } = new List<string>();
        public List<T> Data { get; set; } = new List<T>();
    }
}