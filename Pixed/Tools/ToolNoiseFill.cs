using Pixed.Models;
using Pixed.Utils;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolNoiseFill(ApplicationData applicationData) : BaseTool(applicationData)
    {
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
        {
            PaintUtils.PaintNoiseSimiliarConnected(frame.CurrentLayer, x, y, _applicationData.PrimaryColor, _applicationData.SecondaryColor);
            Subjects.LayerModified.OnNext(frame.CurrentLayer);
        }
    }
}
