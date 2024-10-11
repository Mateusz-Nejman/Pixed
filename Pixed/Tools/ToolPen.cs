using Pixed.Models;
using Pixed.Utils;
using System.Collections.Generic;
using System.Drawing;

namespace Pixed.Tools;

internal class ToolPen(ApplicationData applicationData) : BaseTool(applicationData)
{
    protected int _prevX = -1;
    protected int _prevY = -1;
    private readonly List<Pixel> _pixels = [];
    private readonly List<Point> _modifiedPoints = [];
    public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        _prevX = x;
        _prevY = y;

        var color = GetToolColor();
        DrawOnOverlay(color, x, y, frame, ref overlay);
    }

    public override void MoveTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        if (_prevX != x || _prevY != y)
        {
            var interpolatedPixels = MathUtils.GetBresenhamLine(x, y, _prevX, _prevY);

            foreach (var pixel in interpolatedPixels)
            {
                ApplyTool(pixel.X, pixel.Y, frame, ref overlay, shiftPressed, controlPressed, altPressed);
            }
        }
        else
        {
            ApplyTool(x, y, frame, ref overlay, shiftPressed, controlPressed, altPressed);
        }

        _prevX = x;
        _prevY = y;
    }

    public override void ReleaseTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        SetPixels(frame, _pixels);
        _pixels.Clear();
        _modifiedPoints.Clear();
        _prevX = -1;
        _prevY = -1;
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

    protected void DrawOnOverlay(UniColor color, int x, int y, Frame frame, ref Bitmap overlay)
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

        Subjects.LayerModified.OnNext(frame.CurrentLayer);
    }
}
