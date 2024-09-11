using Pixed.Models;
using Pixed.Selection;
using Pixed.Utils;
using System.Drawing;

namespace Pixed.Tools.Selection
{
    internal class ShapeSelect : BaseSelect
    {
        public override void OnSelectStart(int x, int y, Frame frame, ref Bitmap overlay)
        {
            _hasSelection = true;
            var pixels = PaintUtils.GetSimiliarConnectedPixels(frame, x, y);
            _selection = new ShapeSelection(pixels);
            Subjects.SelectionCreated.OnNext(_selection);
            DrawSelectionOnOverlay(ref overlay);
        }
    }
}
