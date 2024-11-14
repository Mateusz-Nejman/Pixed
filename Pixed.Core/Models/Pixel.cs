namespace Pixed.Core.Models;

public struct Pixel(Point position)
{
    public Point Position { get; set; } = position;
    public uint Color { get; set; } = UniColor.Transparent;

    public Pixel(Point position, uint color) : this(position)
    {
        Color = color;
    }
}
