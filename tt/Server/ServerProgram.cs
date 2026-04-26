using System.Net;
using System.Net.Sockets;
using tt.Server;

namespace tt
{
    internal static class ServerProgram
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Testing system Server Starting...");
            const int port = 9000;
            try
            {
                TcpServer tcpServer = new TcpServer(port);
                Console.WriteLine($"Server listening on port {port}");
                await tcpServer.StartAsync();

            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadKey();
            }
            Console.WriteLine("Server stopped.");
        }
    }
}
