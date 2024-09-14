using Pixed.Models;
using Pixed.Selection;
using Pixed.Utils;
using System.Drawing;

namespace Pixed.Tools.Selection;

internal class RectangleSelect : AbstractDragSelect
{
    public override void OnDragSelectStart(int x, int y, Frame frame, ref Bitmap overlay)
    {
        //TODO drag_start x, y
    }
    public override void OnDragSelect(int x, int y, Frame frame, ref Bitmap overlay)
    {
        overlay.Clear();
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
