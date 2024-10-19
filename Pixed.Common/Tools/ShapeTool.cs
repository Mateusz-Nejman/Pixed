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
    protected int _startX = -1;
    protected int _startY = -1;

    public override bool ShiftHandle { get; protected set; } = true;

    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(x, y, frame, ref overlay, keyState);
        _startX = x;
        _startY = y;

        overlay.SetPixel(x, y, ToolColor, _applicationData.ToolSize);
        Subjects.OverlayModified.OnNext(overlay);
    }

    public override void MoveTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        overlay.Clear();
        var color = ToolColor;

        if (color == UniColor.Transparent)
        {
            color = UniColor.WithAlpha(128, UniColor.GetFromResources("Accent"));
        }

        Draw(x, y, color, keyState.IsShift || GetProperty(PROP_SHIFT), _applicationData.ToolSize, ref overlay);
    }

    public override void ReleaseTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        var color = ToolColor;

        Draw(x, y, color, keyState.IsShift || GetProperty(PROP_SHIFT), _applicationData.ToolSize, frame);

        overlay.Clear();
        ReleaseToolBase(x, y, frame, ref overlay, keyState);
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            new ToolProperty(PROP_SHIFT)
            ];
    }

    protected void Draw(int x, int y, uint color, bool shiftPressed, int toolSize, ref SKBitmap overlay)
    {
        SKBitmap bitmap = overlay;
        Draw(x, y, color, shiftPressed, (x, y, color) =>
        {
            bitmap.SetPixel(x, y, color, toolSize);
        });

        overlay = bitmap;
        Subjects.OverlayModified.OnNext(overlay);
    }

    protected void Draw(int x, int y, uint color, bool shiftPressed, int toolSize, Frame frame)
    {
        Draw(x, y, color, shiftPressed, (x1, y1, _) =>
        {
            frame.SetPixel(x1, y1, color, toolSize);
        });
        Subjects.FrameModified.OnNext(frame);
    }
    protected abstract void Draw(int x, int y, uint color, bool shiftPressed, Action<int, int, uint> setPixelAction);
}
