using Pixed.Models;
using System.Drawing;
using System.Windows.Input;

namespace Pixed.Tools
{
    internal abstract class BaseTool
    {
        protected int highlightedX = 0;
        protected int highlightedY = 0;

        public virtual UniColor GetToolColor()
        {
            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                return Global.SecondaryColor;
            }

            return Global.PrimaryColor;
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
            overlay ??= new Bitmap(frame.Width, frame.Height);

            if (highlightedX != x || highlightedY != y)
            {
                //overlay.Clear();
                overlay?.Dispose();
                overlay = new Bitmap(frame.Width, frame.Height);
            }

            int pixel = frame.GetPixel(x, y);

            if (x >= 0 && y >= 0 && x < overlay.Width && y < overlay.Height)
            {
                overlay.SetPixel(x, y, GetHighlightColor(pixel));
            }
        }

        private UniColor GetHighlightColor(int pixel)
        {
            UniColor.Hsl hsl = ((UniColor)pixel).ToHsl();

            if (hsl.L > 0.5f)
            {
                return UniColor.WithAlpha(50, UniColor.Black);
            }

            return UniColor.WithAlpha(50, UniColor.White);
        }
    }
}
