using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Core.Exceptions;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using MvvmCross.Plugin.Network.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using AggregateException = System.AggregateException;

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
        public const string ApiBaseUrl = "https://77.55.217.165/EwidencjaObiektow/index.php/Api";
        // public const string ApiBaseUrl = "http://192.168.1.118/EwidencjaObiektow/index.php/Api";

        public WebService(MyDbContext context)
        {
            _client = new RestClient(ApiBaseUrl);
            _client.UseNewtonsoftJson(new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy()}
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
                .Select(r => _client.ExecutePostAsync(r)).ToList();
            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
                if(tasks.Any(t => t.IsFaulted)) throw new AggregateException(tasks.Where(t => t.IsFaulted).Select(t => t.Exception));
                return tasks.SelectMany(t => JsonConvert.DeserializeObject<ApiResponse<Obiekt>>(t.Result.Content).Data).ToList();
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
                var response = await _client.PostAsync<ApiResponse<GrupaObiektow>>(request).ConfigureAwait(false);
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
                var response = await _client.PostAsync<ApiResponse<TypParametrow>>(request).ConfigureAwait(false);
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
            var response = await _client.ExecutePostAsync(request).ConfigureAwait(false);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<User>>(response.Content);
            var user = apiResponse.Data.FirstOrDefault();
            if (user != null) user.EncodedPassword = credentials.credentials.base64_password;
            return response.StatusCode switch
            {
                HttpStatusCode.OK => user,
                HttpStatusCode.Unauthorized => throw new ApiLoginException(apiResponse.Errors.FirstOrDefault(), ApiLoginError.Password),
                HttpStatusCode.Forbidden => throw new ApiLoginException(apiResponse.Errors.FirstOrDefault(), ApiLoginError.Login),
                _ => throw new ApiLoginException(apiResponse.Errors.FirstOrDefault(), ApiLoginError.Undefined)
            };
        }

        public async IAsyncEnumerable<Obiekt> SendNewObiektyAsync(IAsyncEnumerable<Obiekt> obiekty)
        {
            await foreach (var obiekt in obiekty.ConfigureAwait(false))
            {
                obiekt.Zdjecie = obiekt.ZdjecieLokal;
                var request = new RestRequest("Obiekt/Dodaj").AddJsonBody(new { data = obiekt, credentials = _credentials});
                var resp = await _client.ExecutePostAsync(request).ConfigureAwait(false);
                var response = JsonConvert.DeserializeObject<ApiResponse<Obiekt>>(resp.Content);
                if(response.Errors.Any()) continue;
                obiekt.RemoteId = response.Data.FirstOrDefault()?.RemoteId;
                yield return obiekt;
            }
        }

        public async IAsyncEnumerable<Obiekt> SendUpdateObiektyAsync(IAsyncEnumerable<Obiekt> obiekty)
        {
            await foreach (var obiekt in obiekty.ConfigureAwait(false))
            {
                var tmp = obiekt.Zdjecie;
                obiekt.Zdjecie = obiekt.ZdjecieLokal;
                var request = new RestRequest($"Obiekt/Edytuj/{obiekt.RemoteId}").AddJsonBody(new { data = obiekt, credentials = _credentials});
                var response = await _client.PostAsync<ApiResponse<Obiekt>>(request).ConfigureAwait(false);
                obiekt.Zdjecie = tmp;
                if(response.Errors.Any()) continue;
                yield return obiekt;
            }
        }

        public async IAsyncEnumerable<Obiekt> SendDeleteObiektyAsync(IAsyncEnumerable<Obiekt> obiekty)
        {
            await foreach (var obiekt in obiekty.ConfigureAwait(false))
            {
                var request = new RestRequest($"Obiekt/Usun/{obiekt.RemoteId}").AddJsonBody(new { credentials = _credentials});
                var response = await _client.ExecutePostAsync(request).ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.OK) continue;
                yield return obiekt;
            }
        }

    }

    public class ApiResponse<T>
    {
        public List<string> Errors { get; set; } = new List<string>();
        public List<T> Data { get; set; } = new List<T>();
    }
}