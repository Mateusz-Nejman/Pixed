using Pixed.Input;
using Pixed.Models;
using Pixed.Utils;
using System;
using System.Drawing;

namespace Pixed.Tools
{
    internal abstract class ShapeTool : BaseTool
    {
        protected int _startX = -1;
        protected int _startY = -1;

        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            _startX = x;
            _startY = y;

            overlay.SetPixel(x, y, GetToolColor());
        }

        public override void MoveTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            overlay.Clear();
            bool isShift = Keyboard.Modifiers.HasFlag(Avalonia.Input.KeyModifiers.Shift);
            var color = GetToolColor();

            if (color == UniColor.Transparent)
            {
                color = new UniColor(50, 160, 215, 240);
            }

            Draw(x, y, color, isShift, ref overlay);
        }

        public override void ReleaseTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            bool isShift = Keyboard.Modifiers.HasFlag(Avalonia.Input.KeyModifiers.Shift);
            var color = GetToolColor();

            Draw(x, y, color, isShift, frame);

            overlay.Clear();
        }

        protected void Draw(int x, int y, int color, bool isShift, ref Bitmap overlay)
        {
            Bitmap bitmap = overlay;
            Draw(x, y, color, isShift, (x, y, color) =>
            {
                bitmap.SetPixel(x, y, (UniColor)color);
            });

            overlay = bitmap;
        }

        protected void Draw(int x, int y, int color, bool isShift, Frame frame)
        {
            Draw(x, y, color, isShift, (x1, y1, _) =>
            {
                frame.SetPixel(x1, y1, color);
            });

            Subjects.FrameModified.OnNext(frame);
        }
        protected abstract void Draw(int x, int y, int color, bool isShift, Action<int, int, int> setPixelAction);
    }
}
