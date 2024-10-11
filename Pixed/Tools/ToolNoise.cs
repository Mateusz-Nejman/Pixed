using Pixed.Models;
using Pixed.Utils;
using SkiaSharp;

namespace Pixed.Tools;
internal class ToolNoise(ApplicationData applicationData) : ToolPen(applicationData)
{
    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        if (!frame.ContainsPixel(x, y))
        {
            return;
        }

        _prevX = x;
        _prevY = y;

        DrawOnOverlay(PaintUtils.GetNoiseColor(_applicationData.PrimaryColor, _applicationData.SecondaryColor), x, y, frame, ref overlay);
    }
}
