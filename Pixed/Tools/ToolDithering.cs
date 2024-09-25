using Pixed.Input;
using Pixed.Models;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolDithering(ApplicationData applicationData) : ToolPen(applicationData)
    {
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
        {
            _prevX = x;
            _prevY = y;

            bool usePrimary = (x + y) % 2 != 0;

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                usePrimary = !usePrimary;
            }

            var color = usePrimary ? _applicationData.PrimaryColor : _applicationData.SecondaryColor;

            DrawOnOverlay(color, x, y, frame, ref overlay);
        }
    }
}
