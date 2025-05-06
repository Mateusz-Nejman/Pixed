using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using Pixed.Core.Utils;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Common.Tools;
public class ToolLighten(ApplicationData applicationData) : ToolPenBase(applicationData)
{
    private const string PROP_DARKEN = "Darken";
    private const string PROP_APPLY_ONCE = "Apply once per pixel";

    private readonly List<Point> _modified = [];

    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_add_subtract_circle_48_regular.svg";
    public override string Name => "Lighten tool";
    public override string Id => "tool_lighten";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Lighten", "Ctrl", "Darken", "Shift", "Apply once per pixel");
    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState, BaseSelection? selection)
    {
        ApplyToolBase(point, frame, ref overlay, keyState, selection);
        var controlPressed = keyState.IsCtrl || GetProperty(PROP_DARKEN);
        var shiftPressed = keyState.IsShift || GetProperty(PROP_APPLY_ONCE);

        if (!frame.ContainsPixel(point))
        {
            return;
        }

        _prev = point;

        var modifiedColor = GetModifierColor(point, frame, ref overlay, shiftPressed, controlPressed);

        _modified.AddRange(PaintUtils.GetToolPoints(point, _applicationData.ToolSize));
        DrawOnOverlay(modifiedColor, point, frame, ref overlay, selection);
        Subjects.OverlayModified.OnNext(overlay);
    }

    public override void ReleaseTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState, BaseSelection? selection)
    {
        base.ReleaseTool(point, frame, ref overlay, keyState, selection);
        _modified.Clear();
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            new ToolProperty(PROP_DARKEN),
            new ToolProperty(PROP_APPLY_ONCE),
        ];
    }

    private int GetModifierColor(Point point, Frame frame, ref SKBitmap overlay, bool oncePerPixel, bool isDarken)
    {
        UniColor overlayColor = overlay.GetPixel(point.X, point.Y);
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

        return color;
    }
}
