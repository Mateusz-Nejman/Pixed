using Pixed.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Tools.Selection
{
    internal abstract class AbstractDragSelect : BaseSelect
    {
        public override void OnSelect(int x, int y, Frame frame, ref Bitmap overlay)
        {
            if(!HasSelection && (x != _startX || y != _startY))
            {
                OnDragSelectStart(x, y, frame, ref overlay);
            }

            if(HasSelection)
            {
                OnDragSelect(x, y, frame, ref overlay);
            }
        }

        public override void OnSelectEnd(int x, int y, Frame frame, ref Bitmap overlay)
        {
            if(HasSelection)
            {
                OnDragSelectEnd(x, y, frame, ref overlay);
            }
        }

        public override void OnSelectStart(int x, int y, Frame frame, ref Bitmap overlay)
        {
            if(HasSelection)
            {
                //TODO commit selection
            }
            else
            {
                OnDragSelectStart(x, y, frame, ref overlay);
                overlay.SetPixel(x, y, Color.FromArgb(128, 130, 163, 170));
            }
        }

        public virtual void OnDragSelectStart(int x, int y, Frame frame, ref Bitmap overlay) { }
        public virtual void OnDragSelect(int x, int y, Frame frame, ref Bitmap overlay) { }
        public virtual void OnDragSelectEnd(int x, int y, Frame frame, ref Bitmap overlay) { }
    }
}
