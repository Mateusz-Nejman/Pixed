using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using Pixed.Core.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pixed.Common.Tools;
public abstract class ShapeTool(ApplicationData applicationData) : BaseTool(applicationData)
{
    protected string PROP_SHIFT = "Keep 1 to 1 ratio";
    protected Point _start = new(-1);
    protected List<Point> _shapePoints = [];

    public override void ApplyTool(Point point, Frame frame, KeyState keyState, BaseSelection? selection)
    {
        ApplyToolBase(point, frame, keyState, selection);
        _start = point;
    }

    public override void MoveTool(Point point, Frame frame, KeyState keyState, BaseSelection? selection)
    {
        MoveToolBase(point, frame, keyState, selection);

        _shapePoints = GetShapePoints(point, keyState.IsShift || GetProperty(PROP_SHIFT));
    }

    public override void ReleaseTool(Point point, Frame frame, KeyState keyState, BaseSelection? selection)
    {
        var color = ToolColor;

        var canvas = frame.GetCanvas();
        canvas.DrawPoints(SKPointMode.Points, [.. _shapePoints.Select(p => p.ToSKPoint())], new SKPaint() { Color = color });
        canvas.Dispose();
        Subjects.FrameModified.OnNext(frame);

        ReleaseToolBase(point, frame, keyState, selection);
    }

    public override void OnOverlay(SKCanvas canvas)
    {
        base.OnOverlay(canvas);
        canvas.DrawPoints(SKPointMode.Points, [.. _shapePoints.Select(p => p.ToSKPoint())], new SKPaint() { Color = ToolColor, StrokeWidth = 1 });
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            new ToolProperty(PROP_SHIFT)
            ];
    }

    protected abstract List<Point> GetShapePoints(Point point, bool shiftPressed);
}
