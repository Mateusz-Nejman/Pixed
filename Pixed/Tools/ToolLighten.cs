using Pixed.Input;
using Pixed.Models;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolLighten(ApplicationData applicationData) : ToolPen(applicationData)
    {
        public override bool ShiftHandle { get; protected set; } = true;
        public override bool ControlHandle { get; protected set; } = true;
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
        {
            if (!frame.ContainsPixel(x, y))
            {
                return;
            }

            _prevX = x;
            _prevY = y;

            var modifiedColor = GetModifierColor(x, y, frame, ref overlay, shiftPressed, controlPressed);
            DrawOnOverlay(modifiedColor, x, y, frame, ref overlay);
        }

        private int GetModifierColor(int x, int y, Frame frame, ref Bitmap overlay, bool oncePerPixel, bool isDarken)
        {
            UniColor overlayColor = overlay.GetPixel(x, y);
            UniColor frameColor = frame.GetPixel(x, y);

            bool isPixelModified = _pixels.Exists(p => p.X == x && p.Y == y);
            var pixelColor = isPixelModified ? overlayColor : frameColor;

            bool isTransparent = pixelColor == UniColor.Transparent;

            if (isTransparent)
            {
                return UniColor.Transparent;
            }

            if (oncePerPixel && isPixelModified)
            {
                return pixelColor;
            }

            var step = oncePerPixel ? 6 : 3;

            UniColor color;
            if (isDarken)
            {
                color = pixelColor.Darken(step);
            }
            else
            {
                color = pixelColor.Lighten(step);
            }

            return color;
        }
    }
}
