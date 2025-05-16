using Pixed.Common.Input;
using Pixed.Common.Services.Keyboard;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using Pixed.Core.Utils;

namespace Pixed.Common.Tools;
public class ToolDithering(ApplicationData applicationData) : ToolPenBase(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_transparency_square_24_regular.svg";
    public override string Name => "Dithering tool";
    public override string Id => "tool_dithering";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Dithering");
    public override void ToolBegin(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        ToolBeginBase();
        var frame = model.CurrentFrame;
        _prev = point;
        var toolPoints = PaintUtils.GetToolPoints(point, _applicationData.ToolSize);

        foreach (var toolPoint in toolPoints)
        {
            if (!frame.ContainsPixel(toolPoint))
            {
                continue;
            }

            bool usePrimary = (toolPoint.X + toolPoint.Y) % 2 != 0;

            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                usePrimary = !usePrimary;
            }

            var color = usePrimary ? _applicationData.PrimaryColor : _applicationData.SecondaryColor;
            _handle ??= frame.GetHandle();
            DrawOnBitmapHandle(color, toolPoint, _handle, selection);
        }
    }
}
