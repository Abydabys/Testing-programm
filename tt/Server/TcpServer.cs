using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using tt.Data;

namespace tt.Server
{
    public class TcpServer
    {
        private readonly int _port;
        private TcpListener _listener;
        private bool _isRunning;

        public TcpServer(int port)
        {
            _port = port;
            _isRunning = false;
        }

        public async Task StartAsync()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();

            _isRunning = true;
            Console.WriteLine($"Server started on port {_port}");

            while (_isRunning)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");
                    var dbContext = new TestingDbContext(); 

                    var handler = new ClientHandler(client, dbContext);

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await handler.HandleAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Client handler error: {ex.Message}");
                        }
                        finally
                        {
                            dbContext.Dispose();
                        }
                    });
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Accept error: {ex.Message}");
                }
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener?.Stop();
            Console.WriteLine("Server stopped.");
        }
    }
}