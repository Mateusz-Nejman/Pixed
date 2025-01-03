using Avalonia;
using Pixed.Core.Utils;
using SkiaSharp;
using System.Runtime.InteropServices;
using Point = Pixed.Core.Models.Point;

namespace Pixed.Core.Utils;
public static class SkiaUtils
{
    private static readonly object _lock = new();

    public static void DrawBitmap(this SKCanvas canvas, SKBitmap bitmap, Rect rect)
    {
        lock (_lock)
        {
            if (!IsNull(bitmap))
            {
                SKImage image = SKImage.FromBitmap(bitmap);
                canvas.DrawImage(image, SKRect.Create((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height));
            }
        }
    }

    public static SKBitmap FromArray(IList<uint> array, Point size)
    {
        var bitmap = new SKBitmap(size.X, size.Y, true);
        var gcHandle = GCHandle.Alloc(array.ToArray(), GCHandleType.Pinned);
        var info = new SKImageInfo(size.X, size.Y, SKColorType.Bgra8888, SKAlphaType.Unpremul);
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
        var info = new SKImageInfo(src.Width, src.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
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

    public static void DrawPatternLine(this SKCanvas canvas, SKPoint p0, SKPoint p1, float[] pattern, UniColor color)
    {
        SKPath path = new();
        path.MoveTo(p0.X, p0.Y);
        path.LineTo(p1.X, p1.Y);

        var effect = SKPathEffect.CreateDash(pattern, 0);

        var paint = new SKPaint
        {
            Color = color,
            PathEffect = effect,
            Style = SKPaintStyle.Stroke
        };

        canvas.DrawPath(path, paint);
    }

    public static bool IsNull(SKBitmap? bitmap)
    {
        return bitmap == null || bitmap.Handle == IntPtr.Zero;
    }
}
