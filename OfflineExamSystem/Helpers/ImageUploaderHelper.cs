using System.Drawing;

namespace OfflineExamSystem.Helpers
{
    public static class ImageUploaderHelper
    {
        #region Public Methods
        public static Size NewImageSize(Size imageSize, Size newSize)
        {
            Size finalSize;
            double tempval;
            if (imageSize.Height > newSize.Height || imageSize.Width > newSize.Width)
            {
                if (imageSize.Height > imageSize.Width)
                    tempval = newSize.Height / (imageSize.Height * 1.0);
                else
                    tempval = newSize.Width / (imageSize.Width * 1.0);

                finalSize = new Size((int)(tempval * imageSize.Width), (int)(tempval * imageSize.Height));
            }
            else
                finalSize = imageSize; // image is already small size

            return finalSize;
        }
        public static void SaveImageToFolder(Image img, string extension, Size newSize, string pathToSave)
        {
            using (Image newImg = ImageHelper.ResizeImage(img, newSize.Width, newSize.Height))
            {
                newImg.Save(System.Web.Hosting.HostingEnvironment.MapPath(pathToSave), img.RawFormat);
            }
        }
        public static void SaveThumbToFolder(Image img, string extension, Size newSize, string pathToSave)
        {
            Size imgSize = NewImageSize(img.Size, newSize);// Get new resolution
            using (Image newImg = ImageHelper.ResizeImage(img, imgSize.Width, imgSize.Height))
            {
                newImg.Save(System.Web.Hosting.HostingEnvironment.MapPath(pathToSave), img.RawFormat);
            }
        }
        #endregion Public Methods
    }
}