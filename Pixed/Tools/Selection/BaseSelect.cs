using Pixed.Selection;
using Pixed.Utils;
using System.Drawing;
using System.Windows.Input;
using Frame = Pixed.Models.Frame;

namespace Pixed.Tools.Selection
{
    internal class BaseSelect : BaseTool
    {
        public enum SelectionMode
        {
            None,
            Select,
            MoveSelection
        }

        protected int _startX = 0;
        protected int _startY = 0;

        protected int _lastX = 0;
        protected int _lastY = 0;


        protected BaseSelection? _selection = null;
        protected SelectionMode _mode = SelectionMode.None;
        protected bool _hasSelection = false;
        protected bool _isMovingContent = false;

        public BaseSelect() : base()
        {
        }

        public void SelectAll(Action<Bitmap> overlayAction)
        {
            Global.ToolSelector.SelectTool("tool_rectangle_select");
            _hasSelection = true;
            _mode = SelectionMode.Select;
            _selection = new RectangularSelection(0, 0, Global.CurrentFrame.Width - 1, Global.CurrentFrame.Height - 1);
            overlayAction?.Invoke(CreateOverlayFromCurrentFrame());
            Subjects.SelectionCreated.OnNext(_selection);
        }

        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            base.ApplyTool(x, y, frame, ref overlay);
            _startX = x;
            _startY = y;
            _lastX = x;
            _lastY = y;

            if (!IsInSelection(x, y))
            {
                _mode = SelectionMode.Select;
                OnSelectStart(x, y, frame, ref overlay);
            }
            else
            {
                _mode = SelectionMode.MoveSelection;

                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    _isMovingContent = true;
                    Subjects.ClipboardCut.OnNext(_selection);
                    DrawSelectionOnOverlay(ref overlay);
                }

                OnSelectionMoveStart(x, y, frame, ref overlay);
            }
        }

        public override void MoveTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            if (_mode == SelectionMode.Select)
            {
                OnSelect(x, y, frame, ref overlay);
            }
            else if (_mode == SelectionMode.MoveSelection)
            {
                OnSelectionMove(x, y, frame, ref overlay);
            }
        }

        public override void ReleaseTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            if (_mode == SelectionMode.Select)
            {
                OnSelectEnd(x, y, frame, ref overlay);
            }
            else if (_mode == SelectionMode.MoveSelection)
            {
                OnSelectionMoveEnd(x, y, frame, ref overlay);
            }
        }

        public override void UpdateHighlightedPixel(int x, int y, Frame frame, ref Bitmap overlay)
        {
            if (!_hasSelection)
            {
                base.UpdateHighlightedPixel(x, y, frame, ref overlay);
            }
        }
        public virtual void OnSelectStart(int x, int y, Frame frame, ref Bitmap overlay) { }
        public virtual void OnSelect(int x, int y, Frame frame, ref Bitmap overlay) { }
        public virtual void OnSelectEnd(int x, int y, Frame frame, ref Bitmap overlay) { }
        public virtual void OnSelectionMoveStart(int x, int y, Frame frame, ref Bitmap overlay) { }
        public virtual void OnSelectionMove(int x, int y, Frame frame, ref Bitmap overlay)
        {
            var deltaX = x - _lastX;
            var deltaY = y - _lastY;

            var diffX = x - _startX;
            var diffY = y - _startY;

            _selection?.Move(deltaX, deltaY);

            overlay = new Bitmap(overlay.Width, overlay.Height);
            DrawSelectionOnOverlay(ref overlay);

            _lastX = x;
            _lastY = y;
        }
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

                if (!bitmap.ContainsPixel(pixel.X, pixel.Y))
                {
                    continue;
                }

                var hasColor = Constants.TRANSPARENT_COLOR.ToArgb() != pixel.Color;
                var color = hasColor ? Color.FromArgb(128, Color.FromArgb(pixel.Color)) : Color.FromArgb(128, Color.CornflowerBlue);

                bitmap.SetPixel(pixel.X, pixel.Y, color);
            }
        }

        private Bitmap CreateOverlayFromCurrentFrame()
        {
            Bitmap newOverlay = new Bitmap(Global.CurrentFrame.Width, Global.CurrentFrame.Height);
            DrawSelectionOnOverlay(ref newOverlay);
            return newOverlay;
        }
    }
}
