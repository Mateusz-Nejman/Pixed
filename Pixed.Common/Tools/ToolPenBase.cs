using Pixed.Common.Services.Keyboard;
using Pixed.Core;
using Pixed.Core.Algorithms;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using Pixed.Core.Utils;
using SkiaSharp;
using System.Collections.Concurrent;

namespace Pixed.Common.Tools;

public abstract class ToolPenBase(ApplicationData applicationData) : BaseTool(applicationData)
{
    protected Point _prev = new(-1);
    protected BitmapHandle? _handle = null;
    private readonly ConcurrentBag<SKPoint> _points = [];

    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_pen_48_regular.svg";
    public override string Name => "Pen tool";
    public override string Id => "tool_pen";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Pen tool");
    public override void ToolBegin(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        ToolBeginBase();

        _handle ??= model.CurrentFrame.GetHandle();
        _prev = point;
        _points.Add(point.ToSKPoint());

        DrawOnBitmapHandle(ToolColor, point, _handle, selection);
    }

    public override void ToolMove(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
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
        model.ResetRecursive();
        _points.Clear();
        _handle = null;

        Subjects.FrameModified.OnNext(model.CurrentFrame);
        _prev = new Point(-1);

        ToolEndBase();
    }

    public override void OnOverlay(SKCanvas canvas)
    {
#pragma warning disable IDE0305 // Simplify collection initialization
        canvas.DrawPoints(SKPointMode.Points, _points.ToArray(), new SKPaint() { Color = ToolColor, StrokeWidth = 1 });
#pragma warning restore IDE0305 // Simplify collection initialization
    }

    protected void DrawOnBitmapHandle(UniColor color, Point point, BitmapHandle handle, BaseSelection? selection)
    {
        if (selection != null && !selection.InSelection(point))
        {
            return;
        }

        if (_applicationData.ToolSize == 1)
        {
            handle.SetPixel(point, color);;
            return;
        }

        var toolPoints = PaintUtils.GetToolPoints(point, _applicationData.ToolSize);
        handle.SetPixels(toolPoints, color);
    }
}
