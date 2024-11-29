namespace Pixed.Application.Models;
internal readonly struct NewProjectResult(int width, int height)
{
    public int Width { get; } = width;
    public int Height { get; } = height;
}