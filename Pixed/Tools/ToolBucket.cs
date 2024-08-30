using Pixed.Models;
using Pixed.Utils;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolBucket : BaseTool
    {
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            int color = GetToolColor();

            var history = PaintUtils.PaintSimiliarConnected(frame.Layers[frame.SelectedLayer], x, y, color);
            history.FrameId = frame.Id;
            Global.Models[0].AddHistory(history.ToEntry());
            Subjects.RefreshCanvas.OnNext(true);
        }
    }
}
