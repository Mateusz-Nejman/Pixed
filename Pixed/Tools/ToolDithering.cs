using Pixed.Input;
using Pixed.Models;
using Pixed.Utils;
using SkiaSharp;

namespace Pixed.Tools;
internal class ToolDithering(ApplicationData applicationData) : ToolPen(applicationData)
{
    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        _prevX = x;
        _prevY = y;
        var toolPoints = PaintUtils.GetToolPoints(x, y, _applicationData.ToolSize);

        foreach (var toolPoint in toolPoints)
        {
            if (!frame.ContainsPixel(toolPoint.X, toolPoint.Y))
            {
                continue;
            }

            bool usePrimary = (toolPoint.X + toolPoint.Y) % 2 != 0;

            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                usePrimary = !usePrimary;
            }

            var color = usePrimary ? _applicationData.PrimaryColor : _applicationData.SecondaryColor;
            AddPixel(toolPoint, color);
        }
    }
}
