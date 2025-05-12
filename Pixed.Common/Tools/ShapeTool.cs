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

    public override void ToolBegin(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        ToolBeginBase();
        _start = point;
    }

    public override void ToolMove(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        _shapePoints = GetShapePoints(point, keyState.IsShift || GetProperty(PROP_SHIFT));
    }

    public override void ToolEnd(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        var color = ToolColor;

        var canvas = model.CurrentFrame.GetCanvas();
        canvas.DrawPoints(SKPointMode.Points, [.. _shapePoints.Select(p => p.ToSKPoint())], new SKPaint() { Color = color });
        canvas.Dispose();
        model.ResetRecursive();
        _shapePoints.Clear();
        Subjects.FrameModified.OnNext(model.CurrentFrame);

        ToolEndBase();
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
