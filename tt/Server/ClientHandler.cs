using System.Net.Sockets;
using tt.Data;
using tt.Shared;

namespace tt.Server
{
    /// <summary>
    /// Manages the lifetime of a single connected TCP client.
    /// Runs a read-loop: receive a NetworkMessage → hand to RequestProcessor → send reply.
    ///
    /// One instance of this class is created per connected client.
    /// It runs on its own background Task (spawned by TcpServer).
    /// </summary>
    public class ClientHandler
    {
        // ── Fields ────────────────────────────────────────────────────────

        private readonly TcpClient _tcpClient;
        private readonly TestingDbContext _dbContext;
        private NetworkStream _stream;
        private readonly RequestProcessor _processor;

        // ── Constructor ───────────────────────────────────────────────────

        public ClientHandler(TcpClient tcpClient, TestingDbContext dbContext)
        {
            // TODO: Store the tcpClient parameter in the _tcpClient field.
            // TODO: Store the dbContext parameter in the _dbContext field.
            // TODO: Create a new RequestProcessor instance, passing _dbContext, and store it in _processor.
        }

        // ── Main loop ─────────────────────────────────────────────────────

        /// <summary>
        /// Entry point called by TcpServer on a background Task.
        /// Reads messages in a loop until the client disconnects or an error occurs.
        /// </summary>
        public async Task HandleAsync()
        {
            // TODO: Wrap the entire method body in a try-catch-finally block.

            // TODO: In the try block:
            //   - Get the NetworkStream from _tcpClient using _tcpClient.GetStream() and store it in _stream.
            //   - Enter a while loop that runs as long as _tcpClient.Connected is true.
            //     Inside the loop:
            //       1. Call NetworkMessage.ReadFromStreamAsync(_stream) using await to read one message.
            //       2. If the returned message is null, break out of the loop (client disconnected cleanly).
            //       3. Print "Received [{message.Type}] from {_tcpClient.Client.RemoteEndPoint}" to the console.
            //       4. If message.Type is MessageType.Disconnect, break out of the loop.
            //       5. If message.Type is MessageType.Ping, create a simple Ping response and send it (see step 6), then continue.
            //       6. Otherwise, call _processor.ProcessAsync(message) using await to get a response NetworkMessage.
            //       7. Call response.WriteToStreamAsync(_stream) using await to send the response back.

            // TODO: In the catch block:
            //   - Catch IOException and SocketException separately (both mean the connection was lost).
            //   - Print an appropriate disconnect message to the console for each.
            //   - Catch any other Exception and print its message.

            // TODO: In the finally block:
            //   - Call Disconnect() to clean up the client resources.
        }

        // ── Cleanup ───────────────────────────────────────────────────────

        private void Disconnect()
        {
            // TODO: Print "Client disconnected: {_tcpClient.Client.RemoteEndPoint}" to the console.
            //       Wrap the RemoteEndPoint access in a try-catch in case the socket is already closed.
            // TODO: Call _stream?.Close() to close the network stream if it is not null.
            // TODO: Call _tcpClient?.Close() to close the TCP connection if it is not null.
        }
    }
}
