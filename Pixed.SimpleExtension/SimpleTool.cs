using Pixed.Common.Services.Keyboard;
using Pixed.Common.Tools;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using Pixed.Core.Utils;
using SkiaSharp;

namespace Pixed.SimpleExtension;
public class SimpleTool(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override string ImagePath => "avares://Pixed.SimpleExtension/Resources/tux.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Simple Tool");

    public override string Name => "Simple Tool";

    public override string Id => "tool_simple";

    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState, BaseSelection? selection)
    {
        base.ApplyTool(point, frame, ref overlay, keyState, selection);
        PaintUtils.PaintSimiliarConnected(frame.CurrentLayer, point, UniColor.CornflowerBlue, selection);
    }
}