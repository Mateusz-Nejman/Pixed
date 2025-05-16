using Avalonia;
using Avalonia.Input;
using Pixed.Application.Utils;
using Pixed.Core;
using SkiaSharp;
using System;

namespace Pixed.Application.Controls.PaintCanvas;
internal class CursorOverlay : OverlayControl
{
    private double _zoom;
    private bool _visible = false;
    private Point _point = new Point();
    public override double Zoom
    {
        get => _zoom;
        set
        {
            _zoom = value;
        }
    }

    public CursorOverlay()
    {
        PointerEntered += Entered;
        PointerMoved += Moved;
        PointerExited += Exited;
        HitTestEnabled = true;
    }

    public override void Render(SKCanvas canvas)
    {
        if (_visible)
        {
            var rounded = new Point(Math.Ceiling(_point.X) - 1, Math.Ceiling(_point.Y) - 1);
            canvas.DrawRect(SKRect.Create(rounded.X.ToFloat(), rounded.Y.ToFloat(), 1f, 1f), new SKPaint() { Color = UniColor.White, StrokeWidth = 1f / Zoom.ToFloat(), Style = SKPaintStyle.Stroke });
        }
    }

    private void Exited(object? sender, PointerEventArgs e)
    {
        _visible = false;
    }

    private void Moved(object? sender, PointerEventArgs e)
    {
        var point = e.GetCurrentPoint(this as Visual);

        _point = point.Position;
    }

    private void Entered(object? sender, PointerEventArgs e)
    {
        _visible = true;
    }
}