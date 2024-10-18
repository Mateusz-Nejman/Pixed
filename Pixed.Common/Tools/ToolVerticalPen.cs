using Pixed.Models;
using Pixed.Services.Keyboard;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Tools;
internal class ToolVerticalPen(ApplicationData applicationData) : ToolPen(applicationData)
{
    private const string PROP_HORIZONTAL = "Use horizontal axis";
    private const string PROP_BOTH_AXIS = "Use horizontal and vertical axis";

    public override string ImagePath => "avares://Pixed.Common/Resources/Icons/tools/tool-vertical-mirror-pen.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Vertical mirror pen", "Ctrl", "Use horizontal axis", "Shift", "Use horizontal and vertical axis");
    public override bool ShiftHandle { get; protected set; } = true;
    public override bool ControlHandle { get; protected set; } = true;
    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(x, y, frame, ref overlay, keyState);
        var shiftPressed = keyState.IsShift || GetProperty(PROP_BOTH_AXIS);
        var controlPressed = keyState.IsCtrl || GetProperty(PROP_HORIZONTAL);

        var color = ToolColor;
        DrawOnOverlay(color, x, y, frame, ref overlay);

        int symX = GetSymmetricX(x, frame);
        int symY = GetSymmetricY(y, frame);

        if (!controlPressed)
        {
            DrawOnOverlay(color, symX, y, frame, ref overlay);
        }

        if (shiftPressed || controlPressed)
        {
            DrawOnOverlay(color, x, symY, frame, ref overlay);
        }

        if (shiftPressed)
        {
            DrawOnOverlay(color, symX, symY, frame, ref overlay);
        }

        Subjects.OverlayModified.OnNext(overlay);

        _prevX = x;
        _prevY = y;
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            new ToolProperty(PROP_HORIZONTAL),
            new ToolProperty(PROP_BOTH_AXIS)
            ];
    }

    private static int GetSymmetricX(int x, Frame frame)
    {
        return frame.Width - 1 - x;
    }

    private static int GetSymmetricY(int y, Frame frame)
    {
        return frame.Height - 1 - y;
    }
}
