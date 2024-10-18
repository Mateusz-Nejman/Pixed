using Pixed.Models;

namespace Pixed.Tools;

internal class ToolEraser(ApplicationData applicationData) : ToolPen(applicationData)
{
    public override string ImagePath => "avares://Pixed.Common/Resources/Icons/tools/tool-eraser.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Eraser");
    public override UniColor GetToolColor()
    {
        return UniColor.Transparent;
    }
}
