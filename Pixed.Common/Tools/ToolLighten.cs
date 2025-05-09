using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Pixed.Common.Tools;
public class ToolLighten(ApplicationData applicationData) : ToolPenBase(applicationData)
{
    private const string PROP_DARKEN = "Darken";
    [Obsolete("Will be removed layet and used by default")]
    private const string PROP_APPLY_ONCE = "Apply once per pixel";

    private readonly List<Point> _modified = [];

    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_add_subtract_circle_48_regular.svg";
    public override string Name => "Lighten tool";
    public override string Id => "tool_lighten";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Lighten", "Ctrl", "Darken", "Shift", "Apply once per pixel");
    public override void ApplyTool(Point point, Frame frame, KeyState keyState, BaseSelection? selection)
    {
        ApplyToolBase(point, frame, keyState, selection);
        var controlPressed = keyState.IsCtrl || GetProperty(PROP_DARKEN);
        var shiftPressed = keyState.IsShift || GetProperty(PROP_APPLY_ONCE);

        if (!frame.ContainsPixel(point))
        {
            return;
        }

        _prev = point;
        bool canModify = !shiftPressed || !_modified.Contains(point);

        if(canModify)
        {
            _canvas ??= frame.GetCanvas();
            var modifiedColor = GetModifierColor(point, frame, shiftPressed, controlPressed);
            _modified.AddRange(DrawOnCanvas(modifiedColor, point, _canvas, selection));
        }
    }

    public override void ReleaseTool(Point point, Frame frame, KeyState keyState, BaseSelection? selection)
    {
        base.ReleaseTool(point, frame, keyState, selection);
        _modified.Clear();
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            new ToolProperty(PROP_DARKEN),
            new ToolProperty(PROP_APPLY_ONCE),
        ];
    }

    private UniColor GetModifierColor(Point point, Frame frame, bool oncePerPixel, bool isDarken)
    {
        return UniColor.White; //TODO
        /*UniColor overlayColor = overlay.GetPixel(point.X, point.Y);
        UniColor frameColor = frame.GetPixel(point);

        bool isPixelModified = _modified.Contains(point);
        var pixelColor = isPixelModified ? overlayColor : frameColor;

        bool isTransparent = pixelColor == UniColor.Transparent;

        if (isTransparent)
        {
            return UniColor.Transparent;
        }

        if (oncePerPixel && isPixelModified)
        {
            return pixelColor;
        }

        var step = oncePerPixel ? 6 : 3;

        UniColor color;
        if (isDarken)
        {
            color = pixelColor.Darken(step);
        }
        else
        {
            color = pixelColor.Lighten(step);
        }

        return color;*/
    }
}
