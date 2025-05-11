using SkiaSharp;

namespace Pixed.Core.Models;

public struct SKPixel(SKPoint position)
{
    public SKPoint Position { get; set; } = position;
    public uint Color { get; set; } = UniColor.Transparent;

    public SKPixel(SKPoint position, uint color) : this(position)
    {
        Color = color;
    }

    public readonly Pixel ToPixel()
    {
        return new Pixel(new Point((int)Position.X, (int)Position.Y), Color);
    }
}
