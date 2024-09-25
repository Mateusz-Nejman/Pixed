using Pixed.Models;
using Pixed.Utils;
using System.Drawing;

namespace Pixed.Tools;
internal class ToolBucket(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        int color = GetToolColor();
        PaintUtils.PaintSimiliarConnected(frame.CurrentLayer, x, y, color);
    }
}
