using Avalonia.Input;
using Pixed.Common.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Pixed.Common.Utils;
public static class SkiaUtils
{
    public static void Export(this SKBitmap value, Stream stream)
    {
        var colors = value.ToByteArray();

        Bitmap bitmap = new(value.Width, value.Height);
        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, value.Width, value.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
        Marshal.Copy(colors, 0, bitmapData.Scan0, colors.Length);
        bitmap.UnlockBits(bitmapData);
        bitmap.Save(stream, ImageFormat.Png);
        bitmap.Dispose();
    }
    public static SKBitmap FromArray(IList<uint> array, int width, int height)
    {
        var bitmap = new SKBitmap(width, height, true);
        var gcHandle = GCHandle.Alloc(array.ToArray(), GCHandleType.Pinned);
        var info = new SKImageInfo(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Unpremul);
        bitmap.InstallPixels(info, gcHandle.AddrOfPinnedObject(), info.RowBytes, delegate { gcHandle.Free(); }, null);
        return bitmap;
    }

    public static uint[] ToArray(this SKBitmap bitmap)
    {
        return bitmap.ToByteArray().ToUInt();
    }

    public static byte[] ToByteArray(this SKBitmap bitmap)
    {
        unsafe
        {
            var ptr = bitmap.GetPixels(out nint length);
            byte[] bytes = new byte[length];
            Marshal.Copy(ptr, bytes, 0, bytes.Length);
            return bytes;
        }
    }

    public static void Clear(this SKBitmap src)
    {
        uint[] pixels = new uint[src.Width * src.Height];
        Array.Fill(pixels, UniColor.Transparent);
        src.Fill(pixels);
    }

    public static void Fill(this SKBitmap src, IList<uint> array)
    {
        var gcHandle = GCHandle.Alloc(array.ToArray(), GCHandleType.Pinned);
        var info = new SKImageInfo(src.Width, src.Height, SKImageInfo.PlatformColorType, SKAlphaType.Unpremul);
        src.InstallPixels(info, gcHandle.AddrOfPinnedObject(), info.RowBytes, delegate { gcHandle.Free(); }, null);
    }

    public static bool ContainsPixel(this SKBitmap src, int x, int y)
    {
        return x >= 0 && y >= 0 && x < src.Width && y < src.Height;
    }

    public static void SetPixel(this SKBitmap bitmap, int x, int y, uint color, int toolSize)
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
                Point point = new(x - (int)Math.Floor(toolSize / 2.0d) + x1, y - (int)Math.Floor(toolSize / 2.0d) + y1);

                if (!bitmap.ContainsPixel(point.X, point.Y))
                {
                    continue;
                }

                bitmap.SetPixel(point.X, point.Y, (UniColor)color);
            }
        }
    }

    public static async Task CopyToClipboard(this SKBitmap src)
    {
        DataObject clipboardObject = new();
        MemoryStream memoryStream = new();
        src.Encode(memoryStream, SKEncodedImageFormat.Png, 100);
        clipboardObject.Set("PNG", memoryStream.ToArray());
        memoryStream.Dispose();
        await Clipboard.ClearAsync();
        await Clipboard.SetDataObjectAsync(clipboardObject);
    }

    public static async Task<SKBitmap?> CreateFromClipboard()
    {
        var formats = await Clipboard.GetFormatsAsync();

        if (formats.Contains("PNG"))
        {
            var data = await Clipboard.GetDataAsync("PNG");

            if (data is byte[] array)
            {
                return SKBitmap.Decode(array);
            }
        }

        return null;
    }
}
