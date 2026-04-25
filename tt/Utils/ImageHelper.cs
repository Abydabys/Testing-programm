using System.Drawing.Imaging;

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
            if (imageData == null || imageData.Length == 0)
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                return Image.FromStream(ms);
            }
            throw new NotImplementedException();
        }

        public static byte[] ImageToByteArray(Image image)
        {
            // TODO: If image is null, return null.
            // TODO: Create a new MemoryStream inside a using block.
            // TODO: Call image.Save(ms, image.RawFormat) to write the image data into the stream.
            // TODO: Return ms.ToArray() to get the byte array.
            if (image == null)
            {
                return null;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
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
            return image.RawFormat.Guid switch
            {
                var g when g == ImageFormat.Jpeg.Guid => "image/jpeg",
                var g when g == ImageFormat.Png.Guid => "image/png",
                var g when g == ImageFormat.Gif.Guid => "image/gif",
                var g when g == ImageFormat.Bmp.Guid => "image/bmp",
                _ => "application/octet-stream"
            };
            throw new NotImplementedException();
        }
    }
}
