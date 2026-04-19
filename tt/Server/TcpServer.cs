using System.Net;
using System.Net.Sockets;
using tt.Data;

namespace tt.Server
{
    /// <summary>
    /// Listens on a TCP port and accepts incoming client connections.
    /// For every accepted connection it spawns a dedicated ClientHandler
    /// on a background thread so multiple clients can be served concurrently.
    ///
    /// There is only ONE TcpServer instance for the whole process.
    /// There is ONE ClientHandler per connected client.
    /// All ClientHandlers share ONE TestingDbContext (created here).
    /// </summary>
    public class TcpServer
    {
        // ── Fields ────────────────────────────────────────────────────────

        private readonly int _port;
        private TcpListener _listener;
        private readonly TestingDbContext _dbContext;
        private bool _isRunning;

        // ── Constructor ───────────────────────────────────────────────────

        public TcpServer(int port)
        {
            // TODO: Store the port parameter in the _port field.
            // TODO: Create a new TestingDbContext instance and store it in _dbContext.
            //       This single context will be shared by all ClientHandlers.
            // TODO: Set _isRunning to false.
        }

        // ── Public API ────────────────────────────────────────────────────

        /// <summary>
        /// Binds to the port and enters an infinite loop accepting clients.
        /// Blocking — call from the main thread or await from an async Main.
        /// </summary>
        public async Task StartAsync()
        {
            // TODO: Create a new TcpListener bound to IPAddress.Any and _port, and store it in _listener.
            // TODO: Call _listener.Start() to begin listening for incoming connections.
            // TODO: Set _isRunning to true.
            // TODO: Print "Server started on port {_port}" to the console.

            // TODO: Enter a while loop that continues as long as _isRunning is true:
            //   - Call _listener.AcceptTcpClientAsync() using await to wait for a connection.
            //   - Store the returned TcpClient in a variable called client.
            //   - Print "Client connected: {client.Client.RemoteEndPoint}" to the console.
            //   - Create a new ClientHandler, passing client and _dbContext.
            //   - Call Task.Run(() => handler.HandleAsync()) to serve the client on a background thread.
            //   - Do NOT await the Task.Run result — the loop must keep running to accept more clients.

            // TODO: Wrap the accept loop body in a try-catch:
            //   - If an ObjectDisposedException is thrown, break out of the loop (server was stopped).
            //   - For any other exception, print the error and continue the loop.
        }

        /// <summary>
        /// Signals the server to stop accepting new connections and disposes the listener.
        /// </summary>
        public void Stop()
        {
            // TODO: Set _isRunning to false.
            // TODO: Call _listener.Stop() to close the listening socket.
            // TODO: Print "Server stopped." to the console.
        }
    }
}
