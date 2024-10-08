using Pixed.Models;
using System.Collections.Generic;
using System.Drawing;

namespace Pixed.Tools;
internal class ToolVerticalPen(ApplicationData applicationData) : ToolPen(applicationData)
{
    private const string TOOL_HORIZONTAL = "Use horizontal axis";
    private const string TOOL_BOTH = "Use horizontal and vertical axis";
    public override bool ShiftHandle { get; protected set; } = true;
    public override bool ControlHandle { get; protected set; } = true;
    public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        bool horizontalProp = GetProperty(TOOL_HORIZONTAL);
        bool bothProp = GetProperty(TOOL_BOTH);

        controlPressed = controlPressed || horizontalProp;
        shiftPressed = shiftPressed || bothProp;

        var color = GetToolColor();
        DrawOnOverlay(color, x, y, frame, ref overlay);

        int symX = GetSymmetricX(x, frame);
        int symY = GetSymmetricY(y, frame);

        if (!controlPressed && !horizontalProp)
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

        _prevX = x;
        _prevY = y;
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            new ToolProperty(TOOL_HORIZONTAL),
            new ToolProperty(TOOL_BOTH)
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
