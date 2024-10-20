using Pixed.Common;
using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Tools;
using Pixed.Common.Utils;
using SkiaSharp;

namespace Pixed.SimpleExtension;
public class SimpleTool(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override string ImagePath => "avares://Pixed.SimpleExtension/Resources/tux.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Simple Tool");

    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        base.ApplyTool(x, y, frame, ref overlay, keyState);
        PaintUtils.PaintSimiliarConnected(frame.CurrentLayer, x, y, UniColor.CornflowerBlue);
    }
}