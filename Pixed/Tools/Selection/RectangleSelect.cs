using Pixed.Models;
using Pixed.Selection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Tools.Selection
{
    internal class RectangleSelect : AbstractDragSelect
    {
        public override void OnDragSelectStart(int x, int y, Frame frame, ref Bitmap overlay)
        {
            //TODO drag_start x, y
        }
        public override void OnDragSelect(int x, int y, Frame frame, ref Bitmap overlay)
        {
            overlay = new Bitmap(frame.Width, frame.Height);
            _selection = new RectangularSelection(_startX, _startY, x, y);
            Subjects.SelectionCreated.OnNext(_selection);
            DrawSelectionOnOverlay(ref overlay);
        }

        public override void OnDragSelectEnd(int x, int y, Frame frame, ref Bitmap overlay)
        {
            OnSelect(x, y, frame, ref overlay);
            //TODO drag_end
        }
    }
}
