namespace tt
{
    internal static class ServerProgram
    {
        static async Task Main(strig[] args)
        {
            Console.WriteLine("Testing system Server Starting...");
            const int port = 55555;
            try
            {
                TcpServer tcpServer = new TcpServer(IPAddress.Any, Server, port);
                Console.WriteLine($"Server listening on port {port}");
                await tcpServer.StartAsync();

            }

            catch(Exception ex)
            {
                Console.WriteLine($"Errors: {ex.Message}");
                Console.ReadKey();
                Console.WriteLine(Stopped);
            }
        }
    }
}
