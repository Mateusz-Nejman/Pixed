using Pixed.Models;
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
        public override void OnDragSelect(int x, int y, Frame frame, ref Bitmap overlay)
        {
            overlay = new Bitmap(frame.Width, frame.Height);
            //TODO set selection
            DrawSelectionOnOverlay(ref overlay);
        }

        public override void OnDragSelectEnd(int x, int y, Frame frame, ref Bitmap overlay)
        {
            OnSelect(x, y, frame, ref overlay);
        }
    }
}
