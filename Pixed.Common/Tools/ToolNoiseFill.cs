using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Utils;
using SkiaSharp;

namespace Pixed.Common.Tools;
public class ToolNoiseFill(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-noise-fill.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Noise fill");
    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(x, y, frame, ref overlay, keyState);
        PaintUtils.PaintNoiseSimiliarConnected(frame.CurrentLayer, x, y, _applicationData.PrimaryColor, _applicationData.SecondaryColor);
        Subjects.LayerModified.OnNext(frame.CurrentLayer);
        Subjects.FrameModified.OnNext(frame);
    }
}
