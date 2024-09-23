using Pixed.Models;
using Pixed.Utils;
using System.Drawing;

namespace Pixed.Tools;
internal class ToolBucket : BaseTool
{
    public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
    {
        int color = GetToolColor();
        PaintUtils.PaintSimiliarConnected(frame.CurrentLayer, x, y, color);
    }
}
