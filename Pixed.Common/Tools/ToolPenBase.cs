using Pixed.Common.Services.Keyboard;
using Pixed.Core;
using Pixed.Core.Algorithms;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using Pixed.Core.Utils;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace Pixed.Common.Tools;

public abstract class ToolPenBase(ApplicationData applicationData) : BaseTool(applicationData)
{
    protected Point _prev = new(-1);
    protected SKCanvas? _canvas = null;
    private List<SKPoint> _points = [];

    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_pen_48_regular.svg";
    public override string Name => "Pen tool";
    public override string Id => "tool_pen";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Pen tool");
    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState, BaseSelection? selection)
    {
        ApplyToolBase(point, frame, ref overlay, keyState, selection);

        _canvas ??= frame.GetCanvas();
        _prev = point;
        _points.Add(point.ToSKPoint());

        DrawOnCanvas(ToolColor, point, _canvas, selection);
    }

    public override void MoveTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState, BaseSelection? selection)
    {
        if (_prev != point)
        {
            var interpolatedPixels = BresenhamLine.Get(point, _prev);

            foreach (var pixel in interpolatedPixels)
            {
                ApplyTool(pixel, frame, ref overlay, keyState, selection);
            }
        }
        else
        {
            ApplyTool(point, frame, ref overlay, keyState, selection);
        }

        _prev = point;
    }

    public override void ReleaseTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState, BaseSelection? selection)
    {
        _canvas?.Dispose();
        _canvas = null;
        frame.ResetID();
        _points.Clear();

        Subjects.FrameModified.OnNext(frame);
        _prev = new Point(-1);

        overlay.Clear();
        Subjects.OverlayModified.OnNext(overlay);
        ReleaseToolBase(point, frame, ref overlay, keyState, selection);
    }

    public override void OnOverlay(SKCanvas canvas, double zoom)
    {
        canvas.DrawPoints(SKPointMode.Points, [.. _points], new SKPaint() { Color = ToolColor, StrokeWidth = (float)(1d / zoom) });
    }

    protected List<Point> DrawOnCanvas(UniColor color, Point point, SKCanvas canvas, BaseSelection? selection)
    {
        if(selection != null && !selection.InSelection(point))
        {
            return [];
        }

        if (_applicationData.ToolSize == 1)
        {
            canvas.DrawPoint(new SKPoint(point.X + 0.5f, point.Y + 0.5f), color);
            return [point];
        }

        var toolPoints = PaintUtils.GetToolPoints(point, _applicationData.ToolSize);
        var minX = toolPoints.Min(p => p.X);
        var minY = toolPoints.Min(p => p.Y);
        var maxX = toolPoints.Max(p => p.X) + 1;
        var maxY = toolPoints.Max(p => p.Y) + 1;

        SKPaint paint = new() { Color = color, Style = SKPaintStyle.Fill };
        canvas.DrawRect(new SKRect(minX, minY, maxX, maxY), paint);
        paint.Dispose();
        return toolPoints;
    }
}
