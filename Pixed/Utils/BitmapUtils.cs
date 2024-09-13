using Avalonia.Input;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Pixed.Utils
{
    internal static class BitmapUtils
    {
        public static Avalonia.Media.Imaging.Bitmap ToAvaloniaBitmap(this Bitmap src)
        {
            Bitmap oldBitmap = src.Clone(new Rectangle(0, 0, src.Width, src.Height), src.PixelFormat);
            var data = oldBitmap.LockBits(new Rectangle(0, 0, oldBitmap.Width, oldBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Avalonia.Media.Imaging.Bitmap bitmap = new Avalonia.Media.Imaging.Bitmap(Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Unpremul, data.Scan0, new Avalonia.PixelSize(data.Width, data.Height), new Avalonia.Vector(96, 96), data.Stride);
            oldBitmap.UnlockBits(data);
            oldBitmap.Dispose();
            return bitmap;
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

        public static async Task CopyToClipboard(this Bitmap src)
        {
            DataObject clipboardObject = new DataObject();
            MemoryStream memoryStream = new MemoryStream();
            src.Save(memoryStream, ImageFormat.Png);
            clipboardObject.Set("PNG", memoryStream);
            await Clipboard.ClearAsync();
            await Clipboard.SetDataObjectAsync(clipboardObject);
        }

        /*public static Bitmap BitmapFromSource(BitmapSource source)
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
        }*/

        public static Bitmap? CreateFromClipboard()
        {
            //TODO
            /*
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
            }*/

            return null;
        }
    }
}
