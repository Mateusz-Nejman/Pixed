using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Utils;
using SkiaSharp;

namespace Pixed.Common.Tools;
public class ToolBucket(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-paint-bucket.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Fill color");
    public override bool SingleHighlightedPixel { get; protected set; } = true;
    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(x, y, frame, ref overlay, keyState);
        uint color = ToolColor;
        PaintUtils.PaintSimiliarConnected(frame.CurrentLayer, x, y, color);
        Subjects.FrameModified.OnNext(frame);
    }
}
