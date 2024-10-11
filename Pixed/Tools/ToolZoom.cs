using Pixed.Models;
using SkiaSharp;
using System;

namespace Pixed.Tools;
internal class ToolZoom(ApplicationData applicationData) : BaseTool(applicationData)
{
    private double _startZoom;
    private double _zoom;
    private IDisposable _disposed;
    public override bool AddToHistory { get; protected set; } = false;
    public Action<double> ZoomAction { get; set; }
    public Action<bool> SetEnabled { get; set; }
    public Func<double> GetZoom { get; set; }

    public override bool SingleHighlightedPixel { get; protected set; } = true;

    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        SetEnabled?.Invoke(true);
        _startZoom = GetZoom();
        SKBitmap bitmap = overlay;
        _disposed = Subjects.ZoomChanged.Subscribe(zoom =>
        {
            _zoom = zoom;

            MoveTool(x, y, frame, ref bitmap, shiftPressed, controlPressed, altPressed);
        });
    }

    public override void MoveTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        ZoomAction?.Invoke(_startZoom * _zoom);
    }

    public override void ReleaseTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        SetEnabled?.Invoke(false);
        _disposed?.Dispose();
    }
}
