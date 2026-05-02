using System.Drawing.Imaging;

namespace tt.Utils
{
    public static class ImageHelper
    {
        public static Image ByteArrayToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                return Image.FromStream(ms);
            }
        }

        public static byte[] ImageToByteArray(Image image)
        {
            if (image == null)
                return null;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

        public static string GetMimeType(Image image)
        {
            return image.RawFormat.Guid switch
            {
                var g when g == ImageFormat.Jpeg.Guid => "image/jpeg",
                var g when g == ImageFormat.Png.Guid  => "image/png",
                var g when g == ImageFormat.Gif.Guid  => "image/gif",
                var g when g == ImageFormat.Bmp.Guid  => "image/bmp",
                _ => "application/octet-stream"
            };
        }
    }
}
