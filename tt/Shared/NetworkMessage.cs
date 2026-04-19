using System.Text;
using System.Text.Json;

namespace tt.Shared
{
    /// <summary>
    /// Represents a single message travelling over the TCP connection in either direction.
    ///
    /// Wire format (length-prefixed framing):
    ///   [4 bytes] int32  — total byte length of the JSON body that follows
    ///   [N bytes] UTF-8  — JSON-serialised NetworkMessage
    ///
    /// Both the client and the server use the same Read/Write helpers so the
    /// framing logic only needs to be implemented once.
    /// </summary>
    public class NetworkMessage
    {
        // ── Fields ────────────────────────────────────────────────────────

        /// <summary>Identifies what this message does (Login, GetTests, etc.).</summary>
        public MessageType Type { get; set; }

        /// <summary>
        /// The request or response body, serialised as a JSON string.
        /// The sender JSON-serialises its data object into this field;
        /// the receiver deserialises it back into the correct type.
        /// </summary>
        public string Payload { get; set; }

        /// <summary>True when the server processed the request successfully.</summary>
        public bool Success { get; set; }

        /// <summary>Human-readable reason populated by the server when Success is false.</summary>
        public string ErrorMessage { get; set; }

        // ── Constructors ──────────────────────────────────────────────────

        public NetworkMessage()
        {
            // TODO: Set Payload to an empty string so it is never null.
            // TODO: Set Success to true as the default (most messages succeed).
            // TODO: Set ErrorMessage to an empty string so it is never null.
        }

        public NetworkMessage(MessageType type, object payloadObject)
        {
            // TODO: Set the Type property to the type parameter.
            // TODO: Serialise payloadObject to JSON using JsonSerializer.Serialize and store the result in Payload.
            // TODO: Set Success to true.
            // TODO: Set ErrorMessage to an empty string.
        }

        // ── Static helpers ────────────────────────────────────────────────

        /// <summary>
        /// Creates an error response message with Success = false.
        /// Used by the server to signal that a request could not be completed.
        /// </summary>
        public static NetworkMessage CreateError(MessageType type, string errorMessage)
        {
            // TODO: Create and return a new NetworkMessage where:
            //   - Type         = the type parameter
            //   - Success      = false
            //   - ErrorMessage = the errorMessage parameter
            //   - Payload      = empty string
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserialises the Payload JSON string into the requested type T.
        /// Called by the receiver after identifying the MessageType.
        /// </summary>
        public T GetPayload<T>()
        {
            // TODO: If Payload is null or whitespace, return the default value for T.
            // TODO: Deserialise Payload using JsonSerializer.Deserialize<T> and return the result.
            throw new NotImplementedException();
        }

        // ── Transport: writing ────────────────────────────────────────────

        /// <summary>
        /// Serialises this message to bytes and writes it to the stream
        /// using the 4-byte length-prefix framing.
        /// </summary>
        public async Task WriteToStreamAsync(Stream stream)
        {
            // TODO: Serialise this entire NetworkMessage object to a JSON string using JsonSerializer.Serialize.
            // TODO: Encode the JSON string to a byte array using Encoding.UTF8.GetBytes.
            // TODO: Convert the byte array length to a 4-byte big-endian byte array using BitConverter.GetBytes.
            // TODO: If BitConverter.IsLittleEndian is true, reverse the length byte array so it is big-endian.
            // TODO: Write the 4 length bytes to the stream using stream.WriteAsync.
            // TODO: Write the JSON body bytes to the stream using stream.WriteAsync.
            // TODO: Call stream.FlushAsync() to ensure all bytes are sent immediately.
            throw new NotImplementedException();
        }

        // ── Transport: reading ────────────────────────────────────────────

        /// <summary>
        /// Reads exactly one message from the stream using the 4-byte length-prefix framing.
        /// Returns null if the connection was closed cleanly (0 bytes read for the header).
        /// </summary>
        public static async Task<NetworkMessage> ReadFromStreamAsync(Stream stream)
        {
            // TODO: Create a 4-byte buffer for the length prefix.
            // TODO: Call ReadExactAsync(stream, lengthBuffer) to read exactly 4 bytes.
            // TODO: If ReadExactAsync returned false, the connection is closed — return null.
            // TODO: If BitConverter.IsLittleEndian is true, reverse the length buffer so it is big-endian.
            // TODO: Convert the 4 bytes to an int32 using BitConverter.ToInt32 — this is the JSON body length.
            // TODO: Create a byte array of that length for the JSON body.
            // TODO: Call ReadExactAsync(stream, bodyBuffer) to read all body bytes.
            // TODO: If ReadExactAsync returned false, return null.
            // TODO: Decode the body bytes to a string using Encoding.UTF8.GetString.
            // TODO: Deserialise the string into a NetworkMessage using JsonSerializer.Deserialize<NetworkMessage>.
            // TODO: Return the resulting NetworkMessage.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fills the buffer completely from the stream, handling partial reads.
        /// Returns false if the stream closes before the buffer is filled.
        /// </summary>
        private static async Task<bool> ReadExactAsync(Stream stream, byte[] buffer)
        {
            // TODO: Create an integer variable called totalRead and set it to 0.
            // TODO: Loop while totalRead is less than buffer.Length:
            //   - Call stream.ReadAsync(buffer, totalRead, buffer.Length - totalRead) using await.
            //   - Store the number of bytes read in a variable called bytesRead.
            //   - If bytesRead is 0, the stream has closed — return false.
            //   - Add bytesRead to totalRead.
            // TODO: After the loop, return true (the buffer was fully read).
            throw new NotImplementedException();
        }
    }
}
