using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using SkiaSharp;
using System;

namespace Pixed.Common.Tools;
public class ToolZoom(ApplicationData applicationData) : BaseTool(applicationData)
{
    private double _startZoom;
    private double _zoom;
    private IDisposable _disposed;

    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-zoom.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Zoom");
    public override bool AddToHistory { get; protected set; } = false;
    public override bool GridMovement { get; protected set; } = false;
    public Action<double> ZoomAction { get; set; }
    public Action<bool> SetEnabled { get; set; }
    public Func<double> GetZoom { get; set; }

    public override bool SingleHighlightedPixel { get; protected set; } = true;

    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(x, y, frame, ref overlay, keyState);
        SetEnabled?.Invoke(true);
        _startZoom = GetZoom();
        SKBitmap bitmap = overlay;
        _disposed = Subjects.ZoomChanged.Subscribe(zoom =>
        {
            _zoom = zoom;

            MoveTool(x, y, frame, ref bitmap, keyState);
        });
    }

    public override void MoveTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ZoomAction?.Invoke(_startZoom * _zoom);
    }

    public override void ReleaseTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        SetEnabled?.Invoke(false);
        _disposed?.Dispose();
        ReleaseToolBase(x, y, frame, ref overlay, keyState);
    }
}
