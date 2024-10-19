using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Utils;
using SkiaSharp;

namespace Pixed.Common.Tools;
public class ToolNoise(ApplicationData applicationData) : ToolPen(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-noise.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Noise");
    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(x, y, frame, ref overlay, keyState);
        if (!frame.ContainsPixel(x, y))
        {
            return;
        }

        _prevX = x;
        _prevY = y;

        DrawOnOverlay(PaintUtils.GetNoiseColor(_applicationData.PrimaryColor, _applicationData.SecondaryColor), x, y, frame, ref overlay);
        Subjects.OverlayModified.OnNext(overlay);
    }
}
