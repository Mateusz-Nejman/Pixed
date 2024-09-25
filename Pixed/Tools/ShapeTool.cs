using Pixed.Input;
using Pixed.Models;
using Pixed.Utils;
using System;
using System.Drawing;

namespace Pixed.Tools
{
    internal abstract class ShapeTool(ApplicationData applicationData) : BaseTool(applicationData)
    {
        protected int _startX = -1;
        protected int _startY = -1;

        public override bool ShiftHandle { get; protected set; } = true;

        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
        {
            _startX = x;
            _startY = y;

            overlay.SetPixel(x, y, GetToolColor());
        }

        public override void MoveTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
        {
            overlay.Clear();
            var color = GetToolColor();

            if (color == UniColor.Transparent)
            {
                color = new UniColor(50, 160, 215, 240);
            }

            Draw(x, y, color, shiftPressed, ref overlay);
        }

        public override void ReleaseTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
        {
            var color = GetToolColor();

            Draw(x, y, color, shiftPressed, frame);

            overlay.Clear();
        }

        protected void Draw(int x, int y, int color, bool shiftPressed, ref Bitmap overlay)
        {
            Bitmap bitmap = overlay;
            Draw(x, y, color, shiftPressed, (x, y, color) =>
            {
                bitmap.SetPixel(x, y, (UniColor)color);
            });

            overlay = bitmap;
        }

        protected void Draw(int x, int y, int color, bool shiftPressed, Frame frame)
        {
            Draw(x, y, color, shiftPressed, (x1, y1, _) =>
            {
                frame.SetPixel(x1, y1, color);
            });

            Subjects.FrameModified.OnNext(frame);
        }
        protected abstract void Draw(int x, int y, int color, bool shiftPressed, Action<int, int, int> setPixelAction);
    }
}
