using System;

namespace Pixed.Application.Zoom;

public class ZoomChangedEventArgs(double zoomX, double zoomY, double offsetX, double offsetY) : EventArgs
{
    public double ZoomX { get; } = zoomX;
    public double ZoomY { get; } = zoomY;
    public double OffsetX { get; } = offsetX;
    public double OffsetY { get; } = offsetY;
}