using Pixed.Common.Services.Keyboard;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;

namespace Pixed.Common.Tools;
public class ToolBucket(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-paint-bucket.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Fill color");
    public override bool SingleHighlightedPixel { get; protected set; } = true;
    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(point, frame, ref overlay, keyState);
        uint color = ToolColor;
        PaintUtils.PaintSimiliarConnected(frame.CurrentLayer, point, color);
        Subjects.FrameModified.OnNext(frame);
    }
}
