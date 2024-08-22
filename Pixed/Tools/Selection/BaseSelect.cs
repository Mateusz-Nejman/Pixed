using Pixed.Models;
using Pixed.Selection;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Pixed.Tools.Selection
{
    internal class BaseSelect : BaseTool
    {
        protected int _startX = 0;
        protected int _startY = 0;

        protected int _lastX = 0;
        protected int _lastY = 0;


        protected BaseSelection? _selection = null;
        protected bool _hasSelection = false;

        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            base.ApplyTool(x, y, frame, ref overlay);
            _startX = x;
            _startY = y;
            _lastX = x;
            _lastY = y;


        }

        public virtual void OnSelectStart(int x, int y, Frame frame, ref Bitmap overlay) { }
        public virtual void OnSelect(int x, int y, Frame frame, ref Bitmap overlay) { }
        public virtual void OnSelectEnd(int x, int y, Frame frame, ref Bitmap overlay) { }
        public virtual void OnSelectionMoveStart(int x, int y, Frame frame, ref Bitmap overlay) { }
        public virtual void OnSelectionMove(int x, int y, Frame frame, ref Bitmap overlay) { }
        public virtual void OnSelectionMoveEnd(int x, int y, Frame frame, ref Bitmap overlay)
        {
            OnSelectionMove(x, y, frame, ref overlay);
        }


        protected bool IsInSelection(int x, int y)
        {
            return _selection != null && _selection.Pixels.Any(p => p.X == x && p.Y == y);
        }

        protected void DrawSelectionOnOverlay(ref Bitmap bitmap)
        {
            var pixels = _selection.Pixels;

            for (int i = 0; i < pixels.Count; i++)
            {
                var pixel = pixels[i];
                var hasColor = Constants.TRANSPARENT_COLOR.ToArgb() != pixel.Color;
                var color = hasColor ? Color.FromArgb(128, Color.FromArgb(pixel.Color)) : Constants.TRANSPARENT_COLOR;

                bitmap.SetPixel(pixel.X, pixel.Y, color);
            }
        }
    }
}
