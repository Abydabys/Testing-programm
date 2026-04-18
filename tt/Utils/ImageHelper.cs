namespace tt.Utils
{
    /// <summary>
    /// Helper class for working with images
    /// </summary>
    public static class ImageHelper
    {
        public static Image ByteArrayToImage(byte[] imageData)
        {
            // TODO: If imageData is null or its Length is 0, return null.
            // TODO: Create a new MemoryStream using the imageData byte array inside a using block.
            // TODO: Call Image.FromStream(ms) to create an Image from the stream and return it.
            throw new NotImplementedException();
        }

        public static byte[] ImageToByteArray(Image image)
        {
            // TODO: If image is null, return null.
            // TODO: Create a new MemoryStream inside a using block.
            // TODO: Call image.Save(ms, image.RawFormat) to write the image data into the stream.
            // TODO: Return ms.ToArray() to get the byte array.
            throw new NotImplementedException();
        }

        public static string GetMimeType(Image image)
        {
            // TODO: Use a switch expression on image.RawFormat.Guid to return the correct MIME type string:
            //   - ImageFormat.Jpeg.Guid  → "image/jpeg"
            //   - ImageFormat.Png.Guid   → "image/png"
            //   - ImageFormat.Gif.Guid   → "image/gif"
            //   - ImageFormat.Bmp.Guid   → "image/bmp"
            //   - Any other format       → "application/octet-stream"
            throw new NotImplementedException();
        }
    }
}
