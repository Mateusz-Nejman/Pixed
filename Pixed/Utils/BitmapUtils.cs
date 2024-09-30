using Avalonia.Input;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Pixed.Utils;

internal static class BitmapUtils
{
    public static Avalonia.Media.Imaging.Bitmap ToAvaloniaBitmap(this Bitmap src)
    {
        Bitmap oldBitmap = src.Clone(new Rectangle(0, 0, src.Width, src.Height), src.PixelFormat);
        var data = oldBitmap.LockBits(new Rectangle(0, 0, oldBitmap.Width, oldBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        Avalonia.Media.Imaging.Bitmap bitmap = new(Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Unpremul, data.Scan0, new Avalonia.PixelSize(data.Width, data.Height), new Avalonia.Vector(96, 96), data.Stride);
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
        DataObject clipboardObject = new();
        MemoryStream memoryStream = new();
        src.Save(memoryStream, ImageFormat.Png);
        clipboardObject.Set("PNG", memoryStream.ToArray());
        await Clipboard.ClearAsync();
        await Clipboard.SetDataObjectAsync(clipboardObject);
    }

    public static async Task<Bitmap?> CreateFromClipboard()
    {
        var formats = await Clipboard.GetFormatsAsync();

        if (formats.Contains("PNG"))
        {
            var data = await Clipboard.GetDataAsync("PNG");

            if (data is byte[] array)
            {
                return (Bitmap?)Image.FromStream(new MemoryStream(array));
            }
        }

        return null;
    }

    public static void SetPixel(this Bitmap bitmap, int x, int y, int color, int toolSize)
    {
        if (toolSize <= 1)
        {
            bitmap.SetPixel(x, y, (UniColor)color);
            return;
        }
        for (int y1 = 0; y1 < toolSize; y1++)
        {
            for (int x1 = 0; x1 < toolSize; x1++)
            {
                Point point = new(x - (int)Math.Floor((double)toolSize / 2.0d) + x1, y - (int)Math.Floor((double)toolSize / 2.0d) + y1);

                if (!bitmap.ContainsPixel(point.X, point.Y))
                {
                    continue;
                }

                bitmap.SetPixel(point.X, point.Y, (UniColor)color);
            }
        }
    }
}
