using Pixed.Models;
using Pixed.Utils;
using System.Drawing;

namespace Pixed.Tools;
internal class ToolBucket : BaseTool
{
    public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
    {
        int color = GetToolColor();

        var history = PaintUtils.PaintSimiliarConnected(frame.CurrentLayer, x, y, color);
        history.FrameId = frame.Id;
        Global.CurrentModel.AddHistory(history.ToEntry());
    }
}
