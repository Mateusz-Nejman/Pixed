using Pixed.Models;
using Pixed.Utils;
using SkiaSharp;

namespace Pixed.Tools;
internal class ToolNoiseFill(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        base.ApplyTool(x, y, frame, ref overlay, shiftPressed, controlPressed, altPressed);
        PaintUtils.PaintNoiseSimiliarConnected(frame.CurrentLayer, x, y, _applicationData.PrimaryColor, _applicationData.SecondaryColor);
        Subjects.LayerModified.OnNext(frame.CurrentLayer);
        Subjects.FrameModified.OnNext(frame);
    }
}
