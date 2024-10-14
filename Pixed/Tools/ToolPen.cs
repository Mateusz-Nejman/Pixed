using Pixed.Algos;
using Pixed.Models;
using Pixed.Services.Keyboard;
using Pixed.Utils;
using SkiaSharp;
using System.Collections.Generic;
using System.Drawing;

namespace Pixed.Tools;

internal class ToolPen(ApplicationData applicationData) : BaseTool(applicationData)
{
    protected int _prevX = -1;
    protected int _prevY = -1;
    private readonly List<Pixel> _pixels = [];
    private readonly List<Point> _modifiedPoints = [];
    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(x, y, frame, ref overlay, keyState);
        _prevX = x;
        _prevY = y;

        DrawOnOverlay(ToolColor, x, y, frame, ref overlay);
        Subjects.OverlayModified.OnNext(overlay);

        if (_pixels.Count > 0)
        {
            Subjects.CurrentLayerRenderModified.OnNext(_pixels);
        }
    }

    public override void MoveTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        if (_prevX != x || _prevY != y)
        {
            var interpolatedPixels = BresenhamLine.Get(x, y, _prevX, _prevY);

            foreach (var pixel in interpolatedPixels)
            {
                ApplyTool(pixel.X, pixel.Y, frame, ref overlay, keyState);
            }
        }
        else
        {
            ApplyTool(x, y, frame, ref overlay, keyState);
        }

        _prevX = x;
        _prevY = y;
    }

    public override void ReleaseTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        SetPixels(frame, _pixels);
        _pixels.Clear();
        _modifiedPoints.Clear();
        _prevX = -1;
        _prevY = -1;

        overlay.Clear();
        Subjects.OverlayModified.OnNext(overlay);
        ReleaseToolBase(x, y, frame, ref overlay, keyState);
    }

    public List<Pixel> GetPixels()
    {
        return _pixels;
    }

    protected bool IsPixelModified(Point point)
    {
        return _modifiedPoints.Contains(point);
    }

    protected void AddPixel(int x, int y, uint color)
    {
        AddPixel(new Point(x, y), color);
    }

    protected void AddPixel(Point point, uint color)
    {
        if (IsPixelModified(point))
        {
            return;
        }

        _pixels.Add(new Pixel(point.X, point.Y, color));
        _modifiedPoints.Add(point);
    }

    protected void DrawOnOverlay(UniColor color, int x, int y, Frame frame, ref SKBitmap overlay)
    {
        var toolSize = _applicationData.ToolSize;
        overlay.SetPixel(x, y, color, toolSize);

        if (color == UniColor.Transparent)
        {
            frame.SetPixel(x, y, color, toolSize);
        }

        var toolPoints = PaintUtils.GetToolPoints(x, y, toolSize);

        foreach (var toolPoint in toolPoints)
        {
            if (frame.ContainsPixel(toolPoint.X, toolPoint.Y))
            {
                AddPixel(toolPoint, color);
            }
        }
    }
}
