using Pixed.Common.Models;
using Pixed.Common.Selection;
using Pixed.Common.Utils;
using SkiaSharp;

namespace Pixed.Common.Tools.Selection;

internal class ShapeSelect(ApplicationData applicationData, ToolSelector toolSelector) : AbstractDragSelect(applicationData, toolSelector)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-shape-select.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Shape selection", "Ctrl+C", "Copy the selected area", "Ctrl+V", "Paste the copied area");
    public override void OnSelectStart(Point point, Frame frame, ref SKBitmap overlay)
    {
        _hasSelection = true;
        var pixels = PaintUtils.GetSimiliarConnectedPixels(frame, point);
        _selection = new ShapeSelection(pixels);
        Subjects.SelectionCreating.OnNext(_selection);
    }
}
