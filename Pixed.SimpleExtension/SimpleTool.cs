using Pixed.Common.Services;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Tools;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using Pixed.Core.Utils;

namespace Pixed.SimpleExtension;
public class SimpleTool(ApplicationData applicationData, IHistoryService historyService) : BaseTool(applicationData)
{
    private readonly IHistoryService _historyService = historyService;
    public override string ImagePath => "avares://Pixed.SimpleExtension/Resources/tux.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Simple Tool");

    public override string Name => "Simple Tool";

    public override string Id => "tool_simple";

    public override void ToolBegin(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        base.ToolBegin(point, model, keyState, selection);
        PaintUtils.PaintSimiliarConnected(model.CurrentFrame.CurrentLayer, point, UniColor.CornflowerBlue, selection);
        model.ResetRecursive();
        _historyService.AddToHistory(model);
    }
}