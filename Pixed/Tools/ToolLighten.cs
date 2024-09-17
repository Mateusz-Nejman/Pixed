using Pixed.Input;
using Pixed.Models;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolLighten : ToolPen
    {
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            if (!frame.ContainsPixel(x, y))
            {
                return;
            }

            _prevX = x;
            _prevY = y;

            var modifiedColor = GetModifierColor(x, y, frame, ref overlay);
            DrawOnOverlay(modifiedColor, x, y, frame, ref overlay);
        }

        private int GetModifierColor(int x, int y, Frame frame, ref Bitmap overlay)
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

            bool oncePerPixel = Keyboard.Modifiers.HasFlag(Avalonia.Input.KeyModifiers.Shift);
            if (oncePerPixel && isPixelModified)
            {
                return pixelColor;
            }

            var step = oncePerPixel ? 6 : 3;
            var isDarken = Keyboard.Modifiers.HasFlag(Avalonia.Input.KeyModifiers.Control);

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
