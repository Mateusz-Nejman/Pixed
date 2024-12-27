namespace Pixed.Application.Models;

internal readonly struct OpenSvgResult(int width, int height)
{
    public int Width { get; } = width;
    public int Height { get; } = height;
}