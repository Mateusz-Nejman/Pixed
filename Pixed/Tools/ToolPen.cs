using Pixed.Models;
using Pixed.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pixed.Tools;

internal class ToolPen : BaseTool
{
    protected int _prevX = -1;
    protected int _prevY = -1;
    protected List<Pixel> _pixels = [];
    public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
    {
        _prevX = x;
        _prevY = y;

        var color = GetToolColor();
        DrawOnOverlay(color, x, y, frame, ref overlay);
    }

    public override void MoveTool(int x, int y, Frame frame, ref Bitmap overlay)
    {
        if ((Math.Abs(x - _prevX) > 1) || (Math.Abs(y - _prevY) > 1))
        {
            var interpolatedPixels = MathUtils.GetBresenhamLine(x, y, _prevX, _prevY);

            foreach (var pixel in interpolatedPixels)
            {
                ApplyTool(pixel.X, pixel.Y, frame, ref overlay);
            }
        }
        else
        {
            ApplyTool(x, y, frame, ref overlay);
        }

        _prevX = x;
        _prevY = y;
    }

    public override void ReleaseTool(int x, int y, Frame frame, ref Bitmap overlay)
    {
        SetPixelsToFrame(frame);
        _pixels.Clear();
        _prevX = -1;
        _prevY = -1;
    }

    protected void DrawOnOverlay(UniColor color, int x, int y, Frame frame, ref Bitmap overlay)
    {
        overlay.SetPixel(x, y, color);

        if (color == UniColor.Transparent)
        {
            frame.SetPixel(x, y, color);
        }

        this._pixels.Add(new Pixel(x, y, color));
    }

    private void SetPixelsToFrame(Frame frame)
    {
        foreach (var pixel in this._pixels)
        {
            frame.SetPixel(pixel.X, pixel.Y, pixel.Color);
        }

        Subjects.FrameModified.OnNext(frame);
    }
}
