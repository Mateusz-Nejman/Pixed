using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using Pixed.Core.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Pixed.Common.Tools;
public abstract class ShapeTool(ApplicationData applicationData) : BaseTool(applicationData)
{
    protected string PROP_SHIFT = "Keep 1 to 1 ratio";
    protected Point _start = new(-1);

    public override void ApplyTool(Point point, Frame frame, KeyState keyState, BaseSelection? selection)
    {
        ApplyToolBase(point, frame, keyState, selection);
        _start = point;

        //TODO overlay.SetPixel(point, ToolColor, _applicationData.ToolSize);
        //TODO Subjects.OverlayModified.OnNext(overlay);
    }

    public override void MoveTool(Point point, Frame frame, KeyState keyState, BaseSelection? selection)
    {
        MoveToolBase(point, frame, keyState, selection);
        //TODO overlay.Clear();
        var color = ToolColor;

        if (color == UniColor.Transparent)
        {
            color = UniColor.WithAlpha(128, UniColor.GetFromResources("Accent"));
        }

        Draw(point, color, keyState.IsShift || GetProperty(PROP_SHIFT), _applicationData.ToolSize, selection);
    }

    public override void ReleaseTool(Point point, Frame frame, KeyState keyState, BaseSelection? selection)
    {
        var color = ToolColor;

        Draw(point, color, keyState.IsShift || GetProperty(PROP_SHIFT), _applicationData.ToolSize, frame, selection);

        //TODO overlay.Clear();
        ReleaseToolBase(point, frame, keyState, selection);
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            new ToolProperty(PROP_SHIFT)
            ];
    }

    protected void Draw(Point point, uint color, bool shiftPressed, int toolSize, BaseSelection? selection)
    {
        /*TODO SKBitmap bitmap = overlay;
        Draw(point, color, shiftPressed, (p, color) =>
        {
            if(selection != null && !selection.InSelection(p))
            {
                return;
            }

            bitmap.SetPixel(p, color, toolSize);
        });

        overlay = bitmap;*/
        //TODO Subjects.OverlayModified.OnNext(overlay);
    }

    protected void Draw(Point point, uint color, bool shiftPressed, int toolSize, Frame frame, BaseSelection? selection)
    {
        Draw(point, color, shiftPressed, (p, _) =>
        {
            if (selection != null && !selection.InSelection(p))
            {
                return;
            }

            frame.SetPixel(p, color, toolSize);
        });
        Subjects.FrameModified.OnNext(frame);
    }
    protected abstract void Draw(Point point, uint color, bool shiftPressed, Action<Point, uint> setPixelAction);
}
