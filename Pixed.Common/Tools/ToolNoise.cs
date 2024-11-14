using Pixed.Common.Services.Keyboard;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;

namespace Pixed.Common.Tools;
public class ToolNoise(ApplicationData applicationData) : ToolPen(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-noise.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Noise");
    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(point, frame, ref overlay, keyState);
        if (!frame.ContainsPixel(point))
        {
            return;
        }

        _prev = point;

        DrawOnOverlay(PaintUtils.GetNoiseColor(_applicationData.PrimaryColor, _applicationData.SecondaryColor), point, frame, ref overlay);
        Subjects.OverlayModified.OnNext(overlay);
    }
}
