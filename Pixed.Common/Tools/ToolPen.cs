using Pixed.Common.Services.Keyboard;
using Pixed.Core;
using Pixed.Core.Algorithms;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Common.Tools;

public class ToolPen(ApplicationData applicationData) : BaseTool(applicationData)
{
    protected Point _prev = new(-1);
    private readonly List<Pixel> _pixels = [];
    private readonly List<Point> _modifiedPoints = [];

    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-pen.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Simple pen");
    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(point, frame, ref overlay, keyState);
        _prev = point;

        DrawOnOverlay(ToolColor, point, frame, ref overlay);
        Subjects.OverlayModified.OnNext(overlay);

        if (_pixels.Count > 0)
        {
            Subjects.CurrentLayerRenderModified.OnNext(_pixels);
        }
    }

    public override void MoveTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        if (_prev != point)
        {
            var interpolatedPixels = BresenhamLine.Get(point, _prev);

            foreach (var pixel in interpolatedPixels)
            {
                ApplyTool(pixel, frame, ref overlay, keyState);
            }
        }
        else
        {
            ApplyTool(point, frame, ref overlay, keyState);
        }

        _prev = point;
    }

    public override void ReleaseTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        SetPixels(frame, _pixels);
        _pixels.Clear();
        _modifiedPoints.Clear();
        _prev = new Point(-1);

        overlay.Clear();
        Subjects.OverlayModified.OnNext(overlay);
        ReleaseToolBase(point, frame, ref overlay, keyState);
    }

    public List<Pixel> GetPixels()
    {
        return _pixels;
    }

    protected bool IsPixelModified(Point point)
    {
        return _modifiedPoints.Contains(point);
    }

    protected void AddPixel(Point point, uint color)
    {
        if (IsPixelModified(point))
        {
            return;
        }

        _pixels.Add(new Pixel(point, color));
        _modifiedPoints.Add(point);
    }

    protected void DrawOnOverlay(UniColor color, Point point, Frame frame, ref SKBitmap overlay)
    {
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
                AddPixel(toolPoint, color);
            }
        }
    }
}
