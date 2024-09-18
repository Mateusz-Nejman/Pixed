using Pixed.Models;
using Pixed.Utils;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolNoiseFill : BaseTool
    {
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            var history = PaintUtils.PaintNoiseSimiliarConnected(frame.CurrentLayer, x, y);
            history.FrameId = frame.Id;
            Global.CurrentModel.AddHistory(history.ToEntry());
        }
    }
}
