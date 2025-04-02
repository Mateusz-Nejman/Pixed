using Pixed.Core.Models;
using System;

namespace Pixed.Common.Tools;
public class ToolMoveCanvas(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_arrow_move_24_regular.svg";
    public override string Name => "Move canvas";
    public override string Id => "tool_move_canvas";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Move canvas");
    public override bool AddToHistory { get; protected set; } = false;
    public Action<bool>? SetGestureEnabledAction { get; set; }

    public override bool SingleHighlightedPixel { get; protected set; } = true;

    public override void Initialize()
    {
        SetGestureEnabledAction?.Invoke(true);
    }

    public override void Reset()
    {
        SetGestureEnabledAction?.Invoke(false);
    }
}
