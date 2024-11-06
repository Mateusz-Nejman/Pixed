using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Pixed.Common.Tools;
public abstract class ShapeTool(ApplicationData applicationData) : BaseTool(applicationData)
{
    protected string PROP_SHIFT = "Keep 1 to 1 ratio";
    protected Point _start = new(-1);

    public override bool ShiftHandle { get; protected set; } = true;

    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(point, frame, ref overlay, keyState);
        _start = point;

        overlay.SetPixel(point, ToolColor, _applicationData.ToolSize);
        Subjects.OverlayModified.OnNext(overlay);
    }

    public override void MoveTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        overlay.Clear();
        var color = ToolColor;

        if (color == UniColor.Transparent)
        {
            color = UniColor.WithAlpha(128, UniColor.GetFromResources("Accent"));
        }

        Draw(point, color, keyState.IsShift || GetProperty(PROP_SHIFT), _applicationData.ToolSize, ref overlay);
    }

    public override void ReleaseTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        var color = ToolColor;

        Draw(point, color, keyState.IsShift || GetProperty(PROP_SHIFT), _applicationData.ToolSize, frame);

        overlay.Clear();
        ReleaseToolBase(point, frame, ref overlay, keyState);
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            new ToolProperty(PROP_SHIFT)
            ];
    }

    protected void Draw(Point point, uint color, bool shiftPressed, int toolSize, ref SKBitmap overlay)
    {
        SKBitmap bitmap = overlay;
        Draw(point, color, shiftPressed, (p, color) =>
        {
            bitmap.SetPixel(p, color, toolSize);
        });

        overlay = bitmap;
        Subjects.OverlayModified.OnNext(overlay);
    }

    protected void Draw(Point point, uint color, bool shiftPressed, int toolSize, Frame frame)
    {
        Draw(point, color, shiftPressed, (p1, _) =>
        {
            frame.SetPixel(p1, color, toolSize);
        });
        Subjects.FrameModified.OnNext(frame);
    }
    protected abstract void Draw(Point point, uint color, bool shiftPressed, Action<Point, uint> setPixelAction);
}
