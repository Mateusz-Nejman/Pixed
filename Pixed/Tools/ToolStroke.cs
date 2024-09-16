using Pixed.Input;
using Pixed.Models;
using Pixed.Services.History;
using Pixed.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Tools
{
    internal class ToolStroke : BaseTool
    {
        private int _startX = -1;
        private int _startY = -1;

        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            _startX = x;
            _startY = y;

            overlay.SetPixel(x, y, GetToolColor());
        }

        public override void MoveTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            overlay.Clear();
            bool isStraight = Keyboard.Modifiers.HasFlag(Avalonia.Input.KeyModifiers.Shift);
            var color = GetToolColor();

            if(color == UniColor.Transparent)
            {
                color = new UniColor(50, 160, 215, 240);
            }

            DrawLine(x, y, color, isStraight, overlay);
        }

        public override void ReleaseTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            bool isStraight = Keyboard.Modifiers.HasFlag(Avalonia.Input.KeyModifiers.Shift);
            var color = GetToolColor();

            var historyEntry = DrawLine(x, y, color, isStraight, frame);
            Global.CurrentModel.AddHistory(historyEntry);

            overlay.Clear();
            Subjects.RefreshCanvas.OnNext(null);
        }

        private void DrawLine(int x, int y, int color, bool isStraight, Bitmap overlay)
        {
            DrawLine(x, y, color, isStraight, (x, y, color) =>
            {
                overlay.SetPixel(x, y, (UniColor)color);
            });
        }

        private HistoryEntry DrawLine(int x, int y, int color, bool isStraight, Frame frame)
        {
            DynamicHistoryEntry entry = new DynamicHistoryEntry();
            entry.FrameId = frame.Id;
            entry.LayerId = frame.Layers[frame.SelectedLayer].Id;
            DrawLine(x, y, color, isStraight, (x1, y1, _) =>
            {
                int oldColor = frame.GetPixel(x1, y1);
                entry.Add(x1, y1, oldColor, color);
                frame.SetPixel(x1, y1, color);
            });

            return entry.ToEntry();
        }

        private void DrawLine(int x, int y, int color, bool isStraight, Action<int, int, int> setPixelAction)
        {
            List<Point> linePixels;

            if (isStraight)
            {
                linePixels = MathUtils.GetUniformLinePixels(_startX, _startY, x, y);
            }
            else
            {
                linePixels = MathUtils.GetBresenhamLine(x, y, _startX, _startY);
            }

            setPixelAction.Invoke(linePixels[0].X, linePixels[0].Y, color);
            setPixelAction.Invoke(linePixels[^1].X, linePixels[^1].Y, color);

            foreach (var point in linePixels)
            {
                setPixelAction.Invoke(point.X, point.Y, color);
            }
        }
    }
}
