using Pixed.Common.Services.Keyboard;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;

namespace Pixed.Common.Tools;
public class ToolNoiseFill(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-noise-fill.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Noise fill");
    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(point, frame, ref overlay, keyState);
        PaintUtils.PaintNoiseSimiliarConnected(frame.CurrentLayer, point, _applicationData.PrimaryColor, _applicationData.SecondaryColor);
        Subjects.FrameModified.OnNext(frame);
    }
}
