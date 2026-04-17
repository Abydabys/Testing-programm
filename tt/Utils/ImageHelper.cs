namespace tt.Utils
{
    /// <summary>
    /// Helper class for working with images
    /// </summary>
    public static class ImageHelper
    {
        public static Image ByteArrayToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;

            using (var ms = new MemoryStream(imageData))
            {
                return Image.FromStream(ms);
            }
        }

        public static byte[] ImageToByteArray(Image image)
        {
            if (image == null)
                return null;

            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

        public static string GetMimeType(Image image)
        {
            return image.RawFormat.Guid switch
            {
                var guid when guid == System.Drawing.Imaging.ImageFormat.Jpeg.Guid => "image/jpeg",
                var guid when guid == System.Drawing.Imaging.ImageFormat.Png.Guid => "image/png",
                var guid when guid == System.Drawing.Imaging.ImageFormat.Gif.Guid => "image/gif",
                var guid when guid == System.Drawing.Imaging.ImageFormat.Bmp.Guid => "image/bmp",
                _ => "application/octet-stream"
            };
        }
    }
}
