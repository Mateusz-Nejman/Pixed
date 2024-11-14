using Pixed.Core;
using Pixed.Core.Models;

namespace Pixed.Common.Tools;

public class ToolEraser(ApplicationData applicationData) : ToolPen(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-eraser.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Eraser");
    public override UniColor GetToolColor()
    {
        return UniColor.Transparent;
    }
}
