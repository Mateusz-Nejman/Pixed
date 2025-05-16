using SkiaSharp;

namespace Pixed.Core.Models;

public struct Pixel(Point position)
{
    public Point Position { get; set; } = position;
    public uint Color { get; set; } = UniColor.Transparent;

    public Pixel(Point position, uint color) : this(position)
    {
        Color = color;
    }

    public readonly SKPixel ToSKPixel()
    {
        return new SKPixel(new SKPoint(Position.X, Position.Y), Color);
    }
}
