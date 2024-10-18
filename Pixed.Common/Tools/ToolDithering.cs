using Pixed.Input;
using Pixed.Models;
using Pixed.Services.Keyboard;
using Pixed.Utils;
using SkiaSharp;

namespace Pixed.Tools;
internal class ToolDithering(ApplicationData applicationData) : ToolPen(applicationData)
{
    public override string ImagePath => "avares://Pixed.Common/Resources/Icons/tools/tool-dithering.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Dithering");
    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(x, y, frame, ref overlay, keyState);
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
