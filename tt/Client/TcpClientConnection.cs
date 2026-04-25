using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using tt.Shared;

namespace tt.Client.Network
{
    public class TcpClientConnection : IDisposable
    {
        private readonly string _serverAddress;
        private readonly int _serverPort;
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);
        private bool _isConnected;

        public TcpClientConnection(string serverAddress, int serverPort)
        {
            _serverAddress = serverAddress;
            _serverPort = serverPort;
            _isConnected = false;
        }

        public async Task ConnectAsync()
        {
            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(_serverAddress, _serverPort);
                _stream = _tcpClient.GetStream();
                _isConnected = true;

                Console.WriteLine($"Connected to server at {_serverAddress}:{_serverPort}");
            }
            catch
            {
                _isConnected = false;
                throw;
            }
        }

        public async Task DisconnectAsync()
        {
            if (!_isConnected)
                return;

            try
            {
                var disconnectMessage = new NetworkMessage(MessageType.Disconnect, new { });
                await SendAsync(disconnectMessage);
            }
            catch
            {
                // Ignore errors during disconnect notification
            }

            _isConnected = false;

            _stream?.Close();
            _tcpClient?.Close();

            Console.WriteLine("Disconnected from server.");
        }

        public async Task<NetworkMessage> SendAsync(NetworkMessage request)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to server.");

            await _sendLock.WaitAsync();

            try
            {
                await request.WriteToStreamAsync(_stream);

                var response = await NetworkMessage.ReadFromStreamAsync(_stream);

                if (response == null)
                    throw new IOException("Server closed the connection.");

                return response;
            }
            finally
            {
                _sendLock.Release();
            }
        }

        public async Task<NetworkMessage> SendIdRequestAsync(MessageType type, int id)
        {
            var message = new NetworkMessage(type, new { Id = id });
            return await SendAsync(message);
        }

        public async Task<bool> PingAsync()
        {
            try
            {
                var pingMessage = new NetworkMessage(MessageType.Ping, new { });
                var response = await SendAsync(pingMessage);

                return response != null && response.Type == MessageType.Ping;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            try
            {
                DisconnectAsync().GetAwaiter().GetResult();
            }
            catch
            {
                // Swallow exceptions during dispose
            }

            _sendLock.Dispose();
            _stream?.Dispose();
            _tcpClient?.Dispose();
        }
    }
}