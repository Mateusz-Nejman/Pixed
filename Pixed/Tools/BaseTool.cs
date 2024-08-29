using Pixed.Models;
using Pixed.Utils;
using System.Drawing;
using System.Windows.Input;

namespace Pixed.Tools
{
    internal abstract class BaseTool
    {
        protected int highlightedX = 0;
        protected int highlightedY = 0;

        public virtual System.Drawing.Color GetToolColor()
        {
            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                return System.Drawing.Color.CornflowerBlue; //TODO colorpicker
            }

            return System.Drawing.Color.Black;
        }

        public virtual void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
        {

        }

        public virtual void MoveTool(int x, int y, Frame frame, ref Bitmap overlay)
        {

        }

        public virtual void ReleaseTool(int x, int y, Frame frame, ref Bitmap overlay)
        {

        }

        public virtual void UpdateHighlightedPixel(int x, int y, Frame frame, ref Bitmap overlay)
        {
            if (highlightedX != x || highlightedY != y || overlay == null)
            {
                overlay?.Dispose();
                overlay = new Bitmap(frame.Width, frame.Height);
            }

            int pixel = frame.GetPixel(x, y);

            if (x >= 0 && y >= 0 && x < overlay.Width && y < overlay.Height)
            {
                overlay.SetPixel(x, y, GetHighlightColor(pixel));
            }
        }

        private System.Drawing.Color GetHighlightColor(int pixel)
        {
            var hsl = ColorUtils.ToHsl(pixel);

            if (hsl.L > 0.5f)
            {
                return System.Drawing.Color.FromArgb(50, System.Drawing.Color.Black);
            }

            return System.Drawing.Color.FromArgb(50, System.Drawing.Color.White);
        }
    }
}
