using Pixed.Models;
using Pixed.Utils;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolNoise : ToolPen
    {
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
        {
            if (!frame.ContainsPixel(x, y))
            {
                return;
            }

            _prevX = x;
            _prevY = y;

            DrawOnOverlay(PaintUtils.GetNoiseColor(), x, y, frame, ref overlay);
        }
    }
}
