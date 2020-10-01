using System;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
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
            _client = new SftpClient(Host, Username, Password);
        }

        public bool DownloadFile(string path, Stream destination)
        {
            if (!_client.IsConnected)
            {
                _client.Connect();
            }
            var remotePath = Path.Combine(RemoteDir, path);
            try
            {
                if (!_client.Exists(remotePath)) return false;
                _client.DownloadFile(remotePath, destination);
            }
            catch (Exception e)
            {
                return false;
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