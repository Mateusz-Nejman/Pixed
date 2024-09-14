namespace Pixed.Models;

internal class Pixel(int x, int y)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public int Color { get; set; } = UniColor.Transparent;

    public Pixel(int x, int y, int color) : this(x, y)
    {
        Color = color;
    }
}
