namespace Pixed.Application.Zoom;
internal readonly struct ZoomEntry(double zoom, double offsetX, double offsetY)
{
    public double Zoom { get; } = zoom;
    public double OffsetX { get; } = offsetX;
    public double OffsetY { get; } = offsetY;
}