using Pixed.Input;
using Pixed.Models;
using Pixed.Services.History;
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

            var historyEntry = Draw(x, y, color, isShift, frame);
            Global.CurrentModel.AddHistory(historyEntry);

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

        protected HistoryEntry Draw(int x, int y, int color, bool isShift, Frame frame)
        {
            DynamicHistoryEntry entry = new()
            {
                FrameId = frame.Id,
                LayerId = frame.CurrentLayer.Id
            };
            Draw(x, y, color, isShift, (x1, y1, _) =>
            {
                int oldColor = frame.GetPixel(x1, y1);
                entry.Add(x1, y1, oldColor, color);
                frame.SetPixel(x1, y1, color);
            });

            Subjects.FrameModified.OnNext(frame);

            return entry.ToEntry();
        }
        protected abstract void Draw(int x, int y, int color, bool isShift, Action<int, int, int> setPixelAction);
    }
}
