using Pixed.Common.Services.Keyboard;
using Pixed.Core;
using Pixed.Core.Algorithms;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using Pixed.Core.Utils;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Common.Tools;

public abstract class ToolPenBase(ApplicationData applicationData) : BaseTool(applicationData)
{
    protected Point _prev = new(-1);
    private readonly List<Pixel> _pixels = [];
    private readonly List<Point> _modifiedPoints = [];

    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_pen_48_regular.svg";
    public override string Name => "Pen tool";
    public override string Id => "tool_pen";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Pen tool");
    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState, BaseSelection? selection)
    {
        ApplyToolBase(point, frame, ref overlay, keyState, selection);
        _prev = point;

        DrawOnOverlay(ToolColor, point, frame, ref overlay, selection);
        Subjects.OverlayModified.OnNext(overlay);

        if (_pixels.Count > 0)
        {
            Subjects.CurrentLayerRenderModified.OnNext(_pixels);
        }
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
        SetPixels(frame, _pixels);
        _pixels.Clear();
        _modifiedPoints.Clear();
        _prev = new Point(-1);

        overlay.Clear();
        Subjects.OverlayModified.OnNext(overlay);
        ReleaseToolBase(point, frame, ref overlay, keyState, selection);
    }

    public List<Pixel> GetPixels()
    {
        return _pixels;
    }

    protected bool IsPixelModified(Point point)
    {
        return _modifiedPoints.Contains(point);
    }

    protected void AddPixel(Point point, uint color, BaseSelection? selection)
    {
        if (IsPixelModified(point) || (selection != null && !selection.InSelection(point)))
        {
            return;
        }

        _pixels.Add(new Pixel(point, color));
        _modifiedPoints.Add(point);
    }

    protected void DrawOnOverlay(UniColor color, Point point, Frame frame, ref SKBitmap overlay, BaseSelection? selection)
    {
        if (selection != null && !selection.InSelection(point))
        {
            return;
        }
        var toolSize = _applicationData.ToolSize;
        overlay.SetPixel(point, color, toolSize);

        if (color == UniColor.Transparent)
        {
            frame.SetPixel(point, color, toolSize);
        }

        var toolPoints = PaintUtils.GetToolPoints(point, toolSize);

        foreach (var toolPoint in toolPoints)
        {
            if (frame.ContainsPixel(toolPoint))
            {
                AddPixel(toolPoint, color, selection);
            }
        }
    }
}
