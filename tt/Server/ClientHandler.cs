using System.Net.Sockets;
using tt.Data;
using tt.Shared;

namespace tt.Server
{
    public class ClientHandler
    {

        private readonly TcpClient _tcpClient;
        private readonly TestingDbContext _dbContext;
        private NetworkStream _stream;
        private readonly RequestProcessor _processor;


        public ClientHandler(TcpClient tcpClient, TestingDbContext dbContext)
        {
            _tcpClient = tcpClient;
            _dbContext = dbContext;
            _processor = new RequestProcessor(_dbContext);
        }

        public async Task HandleAsync()
        {
            try
            {
                _stream = _tcpClient.GetStream();

                while (_tcpClient.Connected)
                {
                    NetworkMessage message = await NetworkMessage.ReadFromStreamAsync(_stream);

                    if (message == null)
                        break;

                    Console.WriteLine($"Received [{message.Type}] from {_tcpClient.Client.RemoteEndPoint}");

                    if (message.Type == MessageType.Disconnect)
                        break;

                    if (message.Type == MessageType.Ping)
                    {
                        NetworkMessage pingResponse = new NetworkMessage(MessageType.Ping, null);
                        await pingResponse.WriteToStreamAsync(_stream);
                        continue;
                    }

                    NetworkMessage response = await _processor.ProcessAsync(message);
                    await response.WriteToStreamAsync(_stream);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Client connection lost (IOException): {ex.Message}");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Client connection lost (SocketException): {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error handling client: {ex.Message}");
            }
            finally
            {
                Disconnect();
            }
        }

        private void Disconnect()
        {
            try
            {
                Console.WriteLine($"Client disconnected: {_tcpClient.Client.RemoteEndPoint}");
            }
            catch
            {
                Console.WriteLine("Client disconnected (endpoint unavailable).");
            }

            _stream?.Close();
            _tcpClient?.Close();
        }
    }
}