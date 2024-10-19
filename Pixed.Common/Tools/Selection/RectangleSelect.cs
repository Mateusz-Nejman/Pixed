using Pixed.Common.Models;
using Pixed.Common.Selection;
using Pixed.Common.Utils;
using SkiaSharp;

namespace Pixed.Common.Tools.Selection;

internal class RectangleSelect(ApplicationData applicationData, ToolSelector toolSelector) : AbstractDragSelect(applicationData, toolSelector)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-rectangle-select.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Rectangle selection", "Ctrl+C", "Copy the selected area", "Ctrl+V", "Paste the copied area");
    public override void OnDragSelectStart(int x, int y, Frame frame, ref SKBitmap overlay)
    {
        //TODO drag_start x, y
    }
    public override void OnDragSelect(int x, int y, Frame frame, ref SKBitmap overlay)
    {
        overlay.Clear();
        _selection = new RectangularSelection(_startX, _startY, x, y, frame);
        Subjects.SelectionCreated.OnNext(_selection);
        DrawSelectionOnOverlay(ref overlay);
    }

    public override void OnDragSelectEnd(int x, int y, Frame frame, ref SKBitmap overlay)
    {
        OnSelect(x, y, frame, ref overlay);
        //TODO drag_end
    }
}
