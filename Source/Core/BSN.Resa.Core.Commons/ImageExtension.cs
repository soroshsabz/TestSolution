using System.Drawing;
using System.Drawing.Drawing2D;

namespace BSN.Resa.Core.Commons
{
    public static class ImageExtension
    {
        public enum ImageSize
        {
            UnModified = 0,
            Origin = 1,
            Profile = 2,
            ProfileThumbnail = 3
        }

        public static Image Resize(this Image image, uint width, uint height)
        {
            var resized = new Bitmap((int)width, (int)height);
            using (var graphics = Graphics.FromImage(resized))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(image, 0, 0, width, height);
            }
            return resized;
        }

        public static Image FromByteArray(this byte[] byteArrayIn)
        {
            using (var memoryStream = new System.IO.MemoryStream(byteArrayIn))
            {
                var returnImage = Image.FromStream(memoryStream);
                return returnImage;
            }
        }

        public static byte[] ToByteArray(this Image imageIn)
        {
            using (var memoryStream = new System.IO.MemoryStream())
            {
                imageIn.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return memoryStream.ToArray();
            }
        }

        public static Image Resize(this Image image, ImageSize size)
        {
            switch (size)
            {
                case ImageSize.Profile:
                    return image.Resize(150, 150);
                case ImageSize.ProfileThumbnail:
                    return image.Resize(50, 50);
                case ImageSize.Origin:
                    return image.Resize(500, 500);
                case ImageSize.UnModified:
                default:
                    return image;
            }
        }
    }
}