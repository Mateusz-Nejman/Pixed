using Pixed.Models;
using Pixed.Utils;
using SkiaSharp;

namespace Pixed.Tools;
internal class ToolBucket(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override bool SingleHighlightedPixel { get; protected set; } = true;
    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        uint color = GetToolColor();
        PaintUtils.PaintSimiliarConnected(frame.CurrentLayer, x, y, color);
        Subjects.LayerModified.OnNext(frame.CurrentLayer);
        Subjects.FrameModified.OnNext(frame);
    }
}
