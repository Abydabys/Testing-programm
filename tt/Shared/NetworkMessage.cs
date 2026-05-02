using System.Text;
using System.Text.Json;

namespace tt.Shared
{
    public class NetworkMessage
    {
        public MessageType Type { get; set; }
        public string Payload { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public NetworkMessage()
        {
            Payload = string.Empty;
            Success = true;
            ErrorMessage = string.Empty;
        }

        public NetworkMessage(MessageType type, object payloadObject)
        {
            Type = type;
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };
            Payload = JsonSerializer.Serialize(payloadObject, options);
            Success = true;
            ErrorMessage = string.Empty;
        }

        public static NetworkMessage CreateError(MessageType type, string errorMessage)
        {
            return new NetworkMessage
            {
                Type = type,
                Success = false,
                ErrorMessage = errorMessage,
                Payload = string.Empty
            };
            throw new NotImplementedException();
        }
        public T GetPayload<T>()
        {
            if (string.IsNullOrWhiteSpace(Payload))
                return default;
            return JsonSerializer.Deserialize<T>(Payload);
        }
        
        public async Task WriteToStreamAsync(Stream stream)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };
            string json = JsonSerializer.Serialize(this, options);
            byte[] bodyBytes = Encoding.UTF8.GetBytes(json);
            byte[] lengthBytes = BitConverter.GetBytes(bodyBytes.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(lengthBytes);
            await stream.WriteAsync(lengthBytes, 0, 4);
            await stream.WriteAsync(bodyBytes, 0, bodyBytes.Length);
            await stream.FlushAsync();
        }
        public static async Task<NetworkMessage> ReadFromStreamAsync(Stream stream)
        {
            byte[] lengthBuffer = new byte[4];
            bool headerOk = await ReadExactAsync(stream, lengthBuffer);
            if (!headerOk)
                return null;
            if (BitConverter.IsLittleEndian)
                Array.Reverse(lengthBuffer);
            int bodyLength = BitConverter.ToInt32(lengthBuffer, 0);
            byte[] bodyBuffer = new byte[bodyLength];
            bool bodyOk = await ReadExactAsync(stream, bodyBuffer);
            if (!bodyOk)
                return null;
            string json = Encoding.UTF8.GetString(bodyBuffer);
            return JsonSerializer.Deserialize<NetworkMessage>(json);
        }
        private static async Task<bool> ReadExactAsync(Stream stream, byte[] buffer)
        {
            int totalRead = 0;
            while(totalRead < buffer.Length)
            {
                int bytesRead = await stream.ReadAsync(
                    buffer,
                    totalRead,
                    buffer.Length - totalRead
                );
                if(bytesRead == 0)
                {
                    return false;
                }
                totalRead += bytesRead;

            }
            return true;
        }
    }
}
