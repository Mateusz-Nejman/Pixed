using Pixed.Common.Models;
using Pixed.Common.Selection;
using Pixed.Common.Utils;

namespace Pixed.Common.Tools.Selection;

public class ToolSelectShape(ApplicationData applicationData) : ToolSelectBase(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-shape-select.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Shape selection", "Ctrl+C", "Copy the selected area", "Ctrl+V", "Paste the copied area");

    public override void OnSelectionEnd(Point startPoint, Point currentPoint, Point previousPoint, Frame frame)
    {
        var pixels = PaintUtils.GetSimiliarConnectedPixels(frame, currentPoint);
        _selection = new ShapeSelection(pixels);
        Subjects.SelectionCreated.OnNext(_selection);
    }
}
