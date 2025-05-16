using SkiaSharp;

namespace Pixed.Core.Models;
public class BitmapHandle
{
    private readonly SKBitmap _bitmap;
    private readonly unsafe uint* _ptr;

    public BitmapHandle(SKBitmap bitmap)
    {
        _bitmap = bitmap;
        
        unsafe
        {
            _ptr = (uint*)_bitmap.GetPixels().ToPointer();
        }
    }

    public unsafe uint* GetPixels()
    {
        return _ptr;
    }

    public unsafe void SetPixel(Point point, UniColor color)
    {
        SetPixel(point, color.ToUInt());
    }

    public unsafe void SetPixel(Point point, uint color)
    {
        uint* ptr = _ptr + _bitmap.Width * point.Y + point.X;
        *ptr = color;
    }

    public void SetPixels(List<Pixel> pixels)
    {
        foreach (Pixel pixel in pixels)
        {
            SetPixel(pixel.Position, pixel.Color);
        }
    }

    public void SetPixels(List<Point> points, UniColor color)
    {
        foreach(var point in points)
        {
            SetPixel(point, color);
        }
    }
}