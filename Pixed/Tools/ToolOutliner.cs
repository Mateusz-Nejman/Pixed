using Pixed.Models;
using Pixed.Utils;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolOutliner(ApplicationData applicationData) : BaseTool(applicationData)
    {
        public override bool ControlHandle { get; protected set; } = true;
        public override bool SingleHighlightedPixel { get; protected set; } = true;
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
        {
            var color = GetToolColor();
            PaintUtils.OutlineSimiliarConnectedPixels(frame.CurrentLayer, x, y, color, controlPressed);
        }
    }
}
