using Pixed.Common.Services.Keyboard;
using Pixed.Core;
using Pixed.Core.Algorithms;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using Pixed.Core.Utils;
using SkiaSharp;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Pixed.Common.Tools;

public abstract class ToolPenBase(ApplicationData applicationData) : BaseTool(applicationData)
{
    protected Point _prev = new(-1);
    protected SKCanvas? _canvas = null;
    private readonly ConcurrentBag<SKPoint> _points = [];

    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_pen_48_regular.svg";
    public override string Name => "Pen tool";
    public override string Id => "tool_pen";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Pen tool");
    public override void ToolBegin(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        ToolBeginBase(point, model, keyState, selection);

        _canvas ??= model.CurrentFrame.GetCanvas();
        _prev = point;
        _points.Add(point.ToSKPoint());

        DrawOnCanvas(ToolColor, point, _canvas, selection);
    }

    public override void ToolMove(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        ToolMoveBase(point, model, keyState, selection);
        if (_prev != point)
        {
            var interpolatedPixels = BresenhamLine.Get(point, _prev);

            foreach (var pixel in interpolatedPixels)
            {
                ToolBegin(pixel, model, keyState, selection);
            }
        }
        else
        {
            ToolBegin(point, model, keyState, selection);
        }

        _prev = point;
    }

    public override void ToolEnd(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        _canvas?.Dispose();
        _canvas = null;
        var frame = model.CurrentFrame;
        ResetCurrentID(model);
        _points.Clear();

        Subjects.FrameModified.OnNext(frame);
        _prev = new Point(-1);

        ToolEndBase(point, model, keyState, selection);
    }

    public override void OnOverlay(SKCanvas canvas)
    {
        canvas.DrawPoints(SKPointMode.Points, [.. _points], new SKPaint() { Color = ToolColor, StrokeWidth = 1 });
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
