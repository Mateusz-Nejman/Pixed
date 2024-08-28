using Pixed.Models;
using System.Drawing;

namespace Pixed.Tools.Selection
{
    internal abstract class AbstractDragSelect : BaseSelect
    {
        public override void OnSelect(int x, int y, Frame frame, ref Bitmap overlay)
        {
            if (!_hasSelection && (x != _startX || y != _startY))
            {
                _hasSelection = true;
                OnDragSelectStart(x, y, frame, ref overlay);
            }

            if (_hasSelection)
            {
                OnDragSelect(x, y, frame, ref overlay);
            }
        }

        public override void OnSelectEnd(int x, int y, Frame frame, ref Bitmap overlay)
        {
            if (_hasSelection)
            {
                OnDragSelectEnd(x, y, frame, ref overlay);
            }
        }

        public override void OnSelectStart(int x, int y, Frame frame, ref Bitmap overlay)
        {
            if (_hasSelection)
            {
                _hasSelection = false;
                //CommitSelection(ref overlay);
            }
            else
            {
                _hasSelection = true;
                OnDragSelectStart(x, y, frame, ref overlay);
                overlay.SetPixel(x, y, Color.FromArgb(128, 130, 163, 170));
            }
        }

        public virtual void OnDragSelectStart(int x, int y, Frame frame, ref Bitmap overlay) { }
        public virtual void OnDragSelect(int x, int y, Frame frame, ref Bitmap overlay) { }
        public virtual void OnDragSelectEnd(int x, int y, Frame frame, ref Bitmap overlay) { }
    }
}
