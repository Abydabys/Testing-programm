using tt.Server;

namespace tt
{
    /// <summary>
    /// Entry point for the server console application.
    /// This is a separate project from the WinForms client.
    /// It should be started first, before any clients connect.
    /// </summary>
    internal static class ServerProgram
    {
        static async Task Main(string[] args)
        {
            // TODO: Print a startup banner to the console, e.g. "Testing System Server Starting...".
            // TODO: Define the port number the server will listen on (e.g. 9000) as a constant or variable.
            // TODO: Wrap the entire startup in a try-catch block.

            // TODO: Inside the try block:
            //   - Create a new TcpServer instance, passing the port number.
            //   - Print a message confirming the server is listening, e.g. "Server listening on port 9000".
            //   - Call tcpServer.StartAsync() using await to begin accepting client connections.
            //   - This call should block until the server is stopped (it runs in an infinite accept loop).

            // TODO: Inside the catch block:
            //   - Print the exception message to the console.
            //   - Wait for the user to press a key before the console window closes.

            // TODO: After the try-catch, print "Server stopped." to the console.
        }
    }
}
