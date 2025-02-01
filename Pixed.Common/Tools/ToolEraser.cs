using Pixed.Core;
using Pixed.Core.Models;

namespace Pixed.Common.Tools;

public class ToolEraser(ApplicationData applicationData) : ToolPenBase(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_eraser_24_regular.svg";
    public override string Name => "Eraser tool";
    public override string Id => "tool_eraser";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Eraser");
    public override UniColor GetToolColor()
    {
        return UniColor.Transparent;
    }
}
