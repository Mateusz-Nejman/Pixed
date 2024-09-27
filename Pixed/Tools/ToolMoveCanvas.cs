using Pixed.Models;
using System;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolMoveCanvas(ApplicationData applicationData) : BaseTool(applicationData)
    {
        private double _startX;
        private double _startY;
        private Avalonia.Vector _offset;
        public override bool AddToHistory { get; protected set; } = false;
        public Action<Avalonia.Vector> MoveAction { get; set; }
        public Func<Avalonia.Vector> GetOffset { get; set; }

        public override bool SingleHighlightedPixel { get; protected set; } = true;

        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
        {
            _startX = x;
            _startY = y;
            _offset = GetOffset();
        }

        public override void MoveTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
        {
            var diffX = (x - _startX);
            var diffY = (y - _startY);

            var vector = new Avalonia.Vector(Math.Max(0, _offset.X - diffX), Math.Max(0, _offset.Y - diffY));
            MoveAction?.Invoke(vector);
        }
    }
}
