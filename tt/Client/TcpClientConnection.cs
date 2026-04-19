using System.Net.Sockets;
using tt.Shared;

namespace tt.Client.Network
{
    /// <summary>
    /// Manages the client-side TCP socket connection to the server.
    ///
    /// Responsibilities:
    ///   - Connect to / disconnect from the server.
    ///   - Send a NetworkMessage and wait for exactly one NetworkMessage response.
    ///   - Thread-safety: uses a SemaphoreSlim so only one request travels
    ///     over the socket at a time (request → response, strictly sequential).
    ///
    /// All UI-layer code talks to this class; it never touches a raw socket.
    /// </summary>
    public class TcpClientConnection : IDisposable
    {
        // ── Fields ────────────────────────────────────────────────────────

        private readonly string _serverAddress;
        private readonly int _serverPort;
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);
        private bool _isConnected;

        // ── Constructor ───────────────────────────────────────────────────

        public TcpClientConnection(string serverAddress, int serverPort)
        {
            // TODO: Store the serverAddress parameter in the _serverAddress field.
            // TODO: Store the serverPort parameter in the _serverPort field.
            // TODO: Set _isConnected to false.
        }

        // ── Connection management ─────────────────────────────────────────

        /// <summary>
        /// Opens the TCP connection to the server.
        /// Must be called once before any requests are sent.
        /// </summary>
        public async Task ConnectAsync()
        {
            // TODO: Wrap the method body in a try-catch.
            // TODO: In the try block:
            //   - Create a new TcpClient instance and store it in _tcpClient.
            //   - Call _tcpClient.ConnectAsync(_serverAddress, _serverPort) using await.
            //   - Retrieve the NetworkStream using _tcpClient.GetStream() and store it in _stream.
            //   - Set _isConnected to true.
            //   - Print "Connected to server at {_serverAddress}:{_serverPort}" to the console.
            // TODO: In the catch block:
            //   - Set _isConnected to false.
            //   - Rethrow the exception so the caller (NetworkServiceContainer) can handle it.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends a Disconnect message to the server and closes the socket cleanly.
        /// </summary>
        public async Task DisconnectAsync()
        {
            // TODO: Check if _isConnected is false; if so, return immediately (nothing to do).
            // TODO: Create a new NetworkMessage with MessageType.Disconnect and an empty payload.
            // TODO: Call SendAsync(disconnectMessage) using await to notify the server.
            // TODO: Set _isConnected to false.
            // TODO: Call _stream?.Close() to close the stream.
            // TODO: Call _tcpClient?.Close() to close the connection.
            // TODO: Print "Disconnected from server." to the console.
            throw new NotImplementedException();
        }

        // ── Request / Response ────────────────────────────────────────────

        /// <summary>
        /// Sends a request and returns the server's response.
        /// Acquires _sendLock so only one request is in flight at a time.
        /// </summary>
        public async Task<NetworkMessage> SendAsync(NetworkMessage request)
        {
            // TODO: If _isConnected is false, throw an InvalidOperationException with "Not connected to server."
            // TODO: Call _sendLock.WaitAsync() using await to acquire exclusive access to the stream.
            // TODO: Wrap the rest of the method in a try-finally that releases the lock in the finally block.
            // TODO: Inside the try block:
            //   - Call request.WriteToStreamAsync(_stream) using await to send the request.
            //   - Call NetworkMessage.ReadFromStreamAsync(_stream) using await to receive the response.
            //   - If the returned response is null, throw an IOException with "Server closed the connection."
            //   - Return the response.
            throw new NotImplementedException();
        }

        // ── Convenience helpers ───────────────────────────────────────────

        /// <summary>
        /// Sends a request with a single integer id as the payload.
        /// Used by the many service methods that only need to pass an Id.
        /// </summary>
        public async Task<NetworkMessage> SendIdRequestAsync(MessageType type, int id)
        {
            // TODO: Create a new NetworkMessage, passing type and an anonymous object new { Id = id } as the payload.
            // TODO: Call SendAsync with that message and return the result.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends a Ping to the server and returns true if a response is received.
        /// Used to check whether the connection is still alive.
        /// </summary>
        public async Task<bool> PingAsync()
        {
            // TODO: Wrap in a try-catch that returns false on any exception.
            // TODO: Create a new NetworkMessage with MessageType.Ping and an empty payload.
            // TODO: Call SendAsync using await and store the response.
            // TODO: Return true if the response is not null and response.Type == MessageType.Ping.
            throw new NotImplementedException();
        }

        // ── IDisposable ───────────────────────────────────────────────────

        public void Dispose()
        {
            // TODO: Call DisconnectAsync().GetAwaiter().GetResult() to disconnect synchronously.
            //       (Dispose cannot be async so we block here.)
            // TODO: Call _sendLock.Dispose() to release the semaphore.
            // TODO: Call _stream?.Dispose() and _tcpClient?.Dispose() if not already closed.
        }
    }
}
