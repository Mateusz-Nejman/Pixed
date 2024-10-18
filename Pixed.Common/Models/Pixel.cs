namespace Pixed.Models;

internal class Pixel(int x, int y)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public uint Color { get; set; } = UniColor.Transparent;

    public Pixel(int x, int y, uint color) : this(x, y)
    {
        Color = color;
    }
}
