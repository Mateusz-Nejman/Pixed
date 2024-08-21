using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace Pixed.Utils
{
    internal static class BitmapUtils
    {
        public static BitmapImage ToBitmapImage(this Bitmap src)
        {
            MemoryStream ms = new();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage image = new();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        public static Bitmap OpacityImage(this Bitmap src, float opacity)
        {
            Bitmap bmp = new(src.Width, src.Height);
            Graphics graphics = Graphics.FromImage(bmp);
            ColorMatrix colormatrix = new()
            {
                Matrix33 = opacity
            };
            ImageAttributes imgAttribute = new();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(src, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, src.Width, src.Height, GraphicsUnit.Pixel, imgAttribute);
            graphics.Dispose();
            return bmp;
        }
    }
}
