using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
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

        public static void Clear(this Bitmap src)
        {
            int[] pixels = new int[src.Width * src.Height];
            Array.Fill(pixels, UniColor.Transparent);
            BitmapData bitmapData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.WriteOnly, src.PixelFormat);
            Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
            src.UnlockBits(bitmapData);
        }

        public static bool ContainsPixel(this Bitmap src, int x, int y)
        {
            return x >= 0 && y >= 0 && x < src.Width && y < src.Height;
        }

        public static void CopyToClipboard(this Bitmap src)
        {
            IDataObject clipboardObject = new DataObject();
            MemoryStream memoryStream = new MemoryStream();
            src.Save(memoryStream, ImageFormat.Png);
            clipboardObject.SetData("PNG", memoryStream, false);
            Clipboard.Clear();
            Clipboard.SetDataObject(clipboardObject);
        }

        public static Bitmap BitmapFromSource(BitmapSource source)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(source));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        public static Bitmap? CreateFromClipboard()
        {
            if (Clipboard.ContainsData("PNG"))
            {
                var obj = Clipboard.GetData("PNG");

                if (obj is MemoryStream memoryStream)
                {
                    return (Bitmap?)Bitmap.FromStream(memoryStream);
                }
            }

            if (Clipboard.ContainsImage())
            {
                return BitmapFromSource(Clipboard.GetImage());
            }

            return null;
        }
    }
}
