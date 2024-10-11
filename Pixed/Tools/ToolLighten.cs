using Pixed.Models;
using SkiaSharp;
using System.Collections.Generic;
using System.Drawing;

namespace Pixed.Tools;
internal class ToolLighten(ApplicationData applicationData) : ToolPen(applicationData)
{
    private const string PROP_DARKEN = "Darken";
    private const string PROP_APPLY_ONCE = "Apply once per pixel";
    public override bool ShiftHandle { get; protected set; } = true;
    public override bool ControlHandle { get; protected set; } = true;
    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        controlPressed = controlPressed || GetProperty(PROP_DARKEN);
        shiftPressed = shiftPressed || GetProperty(PROP_APPLY_ONCE);

        if (!frame.ContainsPixel(x, y))
        {
            return;
        }

        _prevX = x;
        _prevY = y;

        var modifiedColor = GetModifierColor(x, y, frame, ref overlay, shiftPressed, controlPressed);
        DrawOnOverlay(modifiedColor, x, y, frame, ref overlay);
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            new ToolProperty(PROP_DARKEN),
            new ToolProperty(PROP_APPLY_ONCE),
        ];
    }

    private int GetModifierColor(int x, int y, Frame frame, ref SKBitmap overlay, bool oncePerPixel, bool isDarken)
    {
        UniColor overlayColor = overlay.GetPixel(x, y);
        UniColor frameColor = frame.GetPixel(x, y);

        bool isPixelModified = IsPixelModified(new Point(x, y));
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
