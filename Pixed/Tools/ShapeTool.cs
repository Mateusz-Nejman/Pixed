using Pixed.Models;
using Pixed.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pixed.Tools;
internal abstract class ShapeTool(ApplicationData applicationData) : BaseTool(applicationData)
{
    protected string PROP_SHIFT = "Keep 1 to 1 ratio";
    protected int _startX = -1;
    protected int _startY = -1;

    public override bool ShiftHandle { get; protected set; } = true;

    public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        _startX = x;
        _startY = y;

        overlay.SetPixel(x, y, GetToolColor(), _applicationData.ToolSize);
    }

    public override void MoveTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        overlay.Clear();
        var color = GetToolColor();

        if (color == UniColor.Transparent)
        {
            color = UniColor.WithAlpha(128, UniColor.GetFromResources("Accent"));
        }

        Draw(x, y, color, shiftPressed || GetProperty(PROP_SHIFT), _applicationData.ToolSize, ref overlay);
    }

    public override void ReleaseTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        var color = GetToolColor();

        Draw(x, y, color, shiftPressed || GetProperty(PROP_SHIFT), _applicationData.ToolSize, frame);

        overlay.Clear();
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            new ToolProperty(PROP_SHIFT)
            ];
    }

    protected void Draw(int x, int y, int color, bool shiftPressed, int toolSize, ref Bitmap overlay)
    {
        Bitmap bitmap = overlay;
        Draw(x, y, color, shiftPressed, (x, y, color) =>
        {
            bitmap.SetPixel(x, y, color, toolSize);
        });

        overlay = bitmap;
    }

    protected void Draw(int x, int y, int color, bool shiftPressed, int toolSize, Frame frame)
    {
        Draw(x, y, color, shiftPressed, (x1, y1, _) =>
        {
            frame.SetPixel(x1, y1, color, toolSize);
        });
        Subjects.FrameModified.OnNext(frame);
    }
    protected abstract void Draw(int x, int y, int color, bool shiftPressed, Action<int, int, int> setPixelAction);
}
