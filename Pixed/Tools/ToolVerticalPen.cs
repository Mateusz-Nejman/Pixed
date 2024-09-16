using Pixed.Input;
using Pixed.Models;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolVerticalPen : ToolPen
    {
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            var color = GetToolColor();
            DrawOnOverlay(color, x, y, frame, ref overlay);

            int symX = GetSymmetricX(x, frame);
            int symY = GetSymmetricY(y, frame);

            bool ctrlClicked = Keyboard.Modifiers.HasFlag(Avalonia.Input.KeyModifiers.Control);
            bool shiftClicked = Keyboard.Modifiers.HasFlag(Avalonia.Input.KeyModifiers.Shift);

            if (!ctrlClicked)
            {
                DrawOnOverlay(color, symX, y, frame, ref overlay);
            }

            if (shiftClicked || ctrlClicked)
            {
                DrawOnOverlay(color, x, symY, frame, ref overlay);
            }

            if (shiftClicked)
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
