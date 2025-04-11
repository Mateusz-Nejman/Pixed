using Pixed.Core.Models;
using Pixed.Core.Selection;
using Pixed.Core.Utils;

namespace Pixed.Common.Tools.Selection;

public class ToolSelectShape(ApplicationData applicationData) : ToolSelectBase(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_wand_48_regular.svg";
    public override string Name => "Shape selection";
    public override string Id => "tool_shape_select";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Shape selection", "Ctrl+C", "Copy the selected area", "Ctrl+V", "Paste the copied area");

    public override void OnSelectionEnd(Point startPoint, Point currentPoint, Point previousPoint, Frame frame)
    {
        var pixels = PaintUtils.GetSimiliarConnectedPixels(frame, currentPoint);
        _selection = new ShapeSelection(pixels);
        Subjects.SelectionCreating.OnNext(_selection);
        Subjects.SelectionCreated.OnNext(_selection);
    }
}
