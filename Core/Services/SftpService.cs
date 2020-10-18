using System;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
using Acr.UserDialogs;
using MvvmCross;
using Renci.SshNet;

namespace Core.Services
{
    public class SftpService : IDisposable
    {
        private readonly SftpClient _client;
        private const string Host = "77.55.217.165";
        private const int Port = 22;
        private const string Username = "sftpuser";
        private const string Password = "EOinz2020";
        private const string RemoteDir = "ZdjeciaObiektow";

        public SftpService()
        {
            _client = new SftpClient(Host, Port, Username, Password);
        }

        public async Task<bool> DownloadFileAsync(string path, Stream destination)
        {
            if (!_client.IsConnected)
            {
                _client.Connect();
            }
            var remotePath = Path.Combine(RemoteDir, path);
            try
            {
                if (!_client.Exists(remotePath)) return false;
                await Task.Factory
                    .FromAsync(_client.BeginDownloadFile(remotePath, destination), _client.EndDownloadFile)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Mvx.IoCProvider.Resolve<IUserDialogs>().Toast(e.Message, TimeSpan.FromSeconds(3));
            }

            return true;
        }

        public async Task<bool> SendFileAsync(string localPath, string fileName)
        {
            if (!_client.IsConnected)
            {
                _client.Connect();
            }

            try
            {
                if (!File.Exists(localPath)) return false;
                using var inputStream = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                await Task.Factory.FromAsync(_client.BeginUploadFile(inputStream, Path.Combine(RemoteDir, fileName)), _client.EndUploadFile).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Mvx.IoCProvider.Resolve<IUserDialogs>().Toast(e.Message, TimeSpan.FromSeconds(3));
            }

            return true;
        }

        public void Dispose()
        {
            _client.Disconnect();
            _client?.Dispose();
        }
    }
}