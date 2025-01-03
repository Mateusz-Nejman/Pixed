using Pixed.Core.Models;
using Pixed.Core.Selection;
using System.Reactive.Subjects;

namespace Pixed.Common.Tools.Selection;

public class ToolSelectRectangle(ApplicationData applicationData) : ToolSelectBase(applicationData)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-rectangle-select.png";
    public override string Name => "Rectangle selection";
    public override string Id => "tool_rectangle_select";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Rectangle selection", "Ctrl+C", "Copy the selected area", "Ctrl+V", "Paste the copied area");
    public override void OnSelectionBegin(Point startPoint, Point currentPoint, Point previousPoint, Frame frame)
    {
        CreateAndSubject(startPoint, currentPoint, frame, Subjects.SelectionStarted);
    }

    public override void OnSelection(Point startPoint, Point currentPoint, Point previousPoint, Frame frame)
    {
        CreateAndSubject(startPoint, currentPoint, frame, Subjects.SelectionCreating);
    }

    public override void OnSelectionEnd(Point startPoint, Point currentPoint, Point previousPoint, Frame frame)
    {
        CreateAndSubject(startPoint, currentPoint, frame, Subjects.SelectionCreated);
    }

    public static RectangularSelection Create(Point from, Point to, Frame frame)
    {
        return new RectangularSelection(from, to, frame);
    }

    private void CreateAndSubject(Point startPoint, Point currentPoint, Frame frame, Subject<BaseSelection> subject)
    {
        _selection = Create(startPoint, currentPoint, frame);
        subject.OnNext(_selection);
    }
}
