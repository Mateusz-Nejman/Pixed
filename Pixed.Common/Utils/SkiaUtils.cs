using Pixed.Common.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Point = Pixed.Common.Models.Point;

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
    public static SKBitmap FromArray(IList<uint> array, Point size)
    {
        var bitmap = new SKBitmap(size.X, size.Y, true);
        var gcHandle = GCHandle.Alloc(array.ToArray(), GCHandleType.Pinned);
        var info = new SKImageInfo(size.X, size.Y, SKImageInfo.PlatformColorType, SKAlphaType.Unpremul);
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

    public static bool ContainsPixel(this SKBitmap src, Point point)
    {
        return point.X >= 0 && point.Y >= 0 && point.X < src.Width && point.Y < src.Height;
    }

    public static void SetPixel(this SKBitmap bitmap, Point point, uint color, int toolSize)
    {
        if (toolSize <= 1)
        {
            bitmap.SetPixel(point.X, point.Y, (UniColor)color);
            return;
        }
        for (int y = 0; y < toolSize; y++)
        {
            for (int x = 0; x < toolSize; x++)
            {
                Point toolPoint = new(point.X - (int)Math.Floor(toolSize / 2.0d) + x, point.Y - (int)Math.Floor(toolSize / 2.0d) + y);

                if (!bitmap.ContainsPixel(toolPoint))
                {
                    continue;
                }

                bitmap.SetPixel(toolPoint.X, toolPoint.Y, (UniColor)color);
            }
        }
    }
}
