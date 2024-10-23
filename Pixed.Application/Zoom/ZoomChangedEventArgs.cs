using System;

namespace Pixed.Application.Zoom;

public class ZoomChangedEventArgs(double zoom, double offsetX, double offsetY) : EventArgs
{
    public double Zoom { get; } = zoom;
    public double OffsetX { get; } = offsetX;
    public double OffsetY { get; } = offsetY;
}