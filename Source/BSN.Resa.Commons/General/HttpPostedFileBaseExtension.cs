using System.Collections.Generic;
using System.Drawing;
using System.Web;
using BSN.Resa.Locale;

namespace BSN.Resa.Commons
{
    public static class HttpPostedFileBaseExtension
    {
        public static readonly List<string> IMAGE_FORMATS = new List<string>() { "image/jpg", "image/jpeg" };

        public class FileToImageModel
        {
            public bool IsValid { get; set; }
            public Image Image { get; set; }
            public string Message { get; set; }
        }

        public static bool IsExist(this HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength <= 0)
                return false;

            return true;
        }

        public static FileToImageModel ToImageWithValidation(this HttpPostedFileBase file, int maxImageSize, int maximumWidth, int minimumWidth)
        {
            if (!file.IsExist() || !IMAGE_FORMATS.Contains(file.ContentType))
            {
                return new FileToImageModel { IsValid = false, Image = null, Message = Resources.FILE_FORMAT_IS_NOT_VALID };
            }

            if (file.ContentLength > maxImageSize)
            {
                return new FileToImageModel { IsValid = false, Image = null, Message = Resources.IMAGE_FILE_SIZE_IS_NOT_VALID };
            }

            var image = file.ToImage();
            var imageIsNotValid = image.Width != image.Height || image.Width > maximumWidth || image.Width < minimumWidth;
            if (imageIsNotValid)
            {
                return new FileToImageModel { IsValid = false, Image = null, Message = Resources.IMAGE_DIMENSIONS_IS_NOT_VALID };
            }

            return new FileToImageModel { IsValid = true, Image = image, Message = Resources.FILE_CONVERTION_IS_SUCCESS };

        }
        public static Image ToImage(this HttpPostedFileBase file)
        {
            return Image.FromStream(file.InputStream);
        }
    }

}