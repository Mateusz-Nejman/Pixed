using Avalonia;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;
using System.Runtime.InteropServices;
using Point = Pixed.Core.Models.Point;

namespace Pixed.Core.Utils;
public static class SkiaUtils
{
    private static readonly object _lock = new();

    public static void DrawPixels(this SKCanvas canvas, List<Pixel> pixels)
    {
        var colors = pixels.Select(p => p.Color).Distinct();

        foreach(var color in colors)
        {
            SKPaint paint = new() { Color = color, Style = SKPaintStyle.Fill };
            var points = pixels.Where(p => p.Color == color).Select(p => new SKPoint(p.Position.X + 0.5f, p.Position.Y + 0.5f)).ToArray();
            canvas.DrawPoints(SKPointMode.Points, points, paint);
        }
    }
    public static void DrawBitmapLock(this SKCanvas canvas, SKBitmap bitmap, Rect rect)
    {
        DrawBitmapLock(canvas, bitmap, SKRect.Create((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height));
    }

    public static void DrawBitmapLock(this SKCanvas canvas, SKBitmap bitmap, SKRect rect)
    {
        lock (_lock)
        {
            if (!IsNull(bitmap))
            {
                canvas.DrawBitmap(bitmap, rect);
            }
        }
    }

    public static SKBitmap FromArray(IList<uint> array, Point size)
    {
        var bitmap = new SKBitmap(size.X, size.Y);
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
        if (toolSize <= 1 && bitmap.ContainsPixel(point))
        {
            bitmap.SetPixel(point.X, point.Y, (UniColor)color);
            return;
        }

        SKCanvas canvas = new(bitmap);

        if(toolSize == 1)
        {
            canvas.DrawPoint(point.X, point.Y, (UniColor)color);
        }
        else
        {
            var points = PaintUtils.GetToolPoints(point, toolSize);
            var minX = points.Min(p => p.X);
            var minY = points.Min(p => p.Y);
            var maxX = points.Max(p => p.X);
            var maxY = points.Max(p => p.Y);

            SKRect rect = new(minX, minY, maxX + 1, maxY + 1);
            SKPaint paint = new()
            {
                Color = color,
                Style = SKPaintStyle.Fill
            };
            canvas.DrawRect(rect, paint);
        }

        canvas.Dispose();
    }

    public static void DrawLine(this SKCanvas canvas, SKPoint p0, SKPoint p1, UniColor color)
    {
        var paint = new SKPaint
        {
            Color = color,
            Style = SKPaintStyle.Stroke
        };

        canvas.DrawLine(p0, p1, paint);
    }

    public static void DrawPatternLine(this SKCanvas canvas, SKPoint p0, SKPoint p1, float[] pattern, UniColor color, float lineWidth = 1f)
    {
        SKPath path = new();
        path.MoveTo(p0.X, p0.Y);
        path.LineTo(p1.X, p1.Y);

        var effect = SKPathEffect.CreateDash(pattern, 0);

        var paint = new SKPaint
        {
            Color = color,
            PathEffect = effect,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = lineWidth
        };

        canvas.DrawPath(path, paint);
    }

    public static bool IsNull(SKBitmap? bitmap)
    {
        return bitmap == null || bitmap.Handle == IntPtr.Zero || bitmap.IsNull;
    }
}
