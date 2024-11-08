using Pixed.Common.Models;
using Pixed.Common.Selection;
using Pixed.Common.Utils;
using SkiaSharp;

namespace Pixed.Common.Tools.Selection;

internal class RectangleSelect(ApplicationData applicationData, ToolSelector toolSelector) : AbstractDragSelect(applicationData, toolSelector)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-rectangle-select.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Rectangle selection", "Ctrl+C", "Copy the selected area", "Ctrl+V", "Paste the copied area");
    public override void OnDragSelectStart(Point point, Frame frame, ref SKBitmap overlay)
    {
        //TODO drag_start x, y
    }
    public override void OnDragSelect(Point point, Frame frame, ref SKBitmap overlay)
    {
        overlay.Clear();
        _selection = new RectangularSelection(_start, point, frame);
        Subjects.SelectionCreating.OnNext(_selection);
    }

    public override void OnDragSelectEnd(Point point, Frame frame, ref SKBitmap overlay)
    {
        OnSelect(point, frame, ref overlay);
        //TODO drag_end
    }
}
