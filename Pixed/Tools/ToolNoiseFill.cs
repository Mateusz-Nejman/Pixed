using Pixed.Models;
using Pixed.Utils;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolNoiseFill : BaseTool
    {
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
        {
            PaintUtils.PaintNoiseSimiliarConnected(frame.CurrentLayer, x, y);
        }
    }
}
