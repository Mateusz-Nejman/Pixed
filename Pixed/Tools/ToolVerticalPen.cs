using Pixed.Input;
using Pixed.Models;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolVerticalPen : ToolPen
    {
        public override bool ShiftHandle { get; protected set; } = true;
        public override bool ControlHandle { get; protected set; } = true;
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
        {
            var color = GetToolColor();
            DrawOnOverlay(color, x, y, frame, ref overlay);

            int symX = GetSymmetricX(x, frame);
            int symY = GetSymmetricY(y, frame);

            if (!controlPressed)
            {
                DrawOnOverlay(color, symX, y, frame, ref overlay);
            }

            if (shiftPressed || controlPressed)
            {
                DrawOnOverlay(color, x, symY, frame, ref overlay);
            }

            if (shiftPressed)
            {
                DrawOnOverlay(color, symX, symY, frame, ref overlay);
            }

            _prevX = x;
            _prevY = y;
        }

        private static int GetSymmetricX(int x, Frame frame)
        {
            return frame.Width - 1 - x;
        }

        private static int GetSymmetricY(int y, Frame frame)
        {
            return frame.Height - 1 - y;
        }
    }
}
