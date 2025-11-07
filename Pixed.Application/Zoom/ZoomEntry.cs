using Avalonia;

namespace Pixed.Application.Zoom;
internal readonly struct ZoomEntry(double zoom, double offsetX, double offsetY, Matrix matrix)
{
    public double Zoom { get; } = zoom;
    public double OffsetX { get; } = offsetX;
    public double OffsetY { get; } = offsetY;
    public Matrix Matrix { get; } = matrix;
}