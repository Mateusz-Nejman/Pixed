using Pixed.Common.Services.Keyboard;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using SkiaSharp;
using System.Linq;
using Frame = Pixed.Core.Models.Frame;

namespace Pixed.Common.Tools.Selection;

public abstract class ToolSelectBase(ApplicationData applicationData) : BaseTool(applicationData)
{
    public enum SelectionStage
    {
        None,
        Selecting,
        Selected
    }

    private Point _start = new();
    private Point _prev = new();

    protected BaseSelection? _selection = null;
    protected SelectionStage _stage = SelectionStage.None;
    protected bool _hasSelection = false;
    public override bool AddToHistory { get; protected set; } = false;
    public override bool SingleHighlightedPixel { get; protected set; }

    public override void ToolBegin(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        ToolBeginBase(point, model, keyState, selection);
        _start = point;
        _prev = point;

        OnSelectionBegin(_start, point, _prev, model.CurrentFrame);
        _stage = SelectionStage.Selecting;
    }

    public override void ToolMove(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        if (_stage == SelectionStage.Selecting)
        {
            OnSelection(_start, point, _prev, model.CurrentFrame);
            _prev = point;
        }
    }

    public override void ToolEnd(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        if (_stage == SelectionStage.Selecting)
        {
            OnSelectionEnd(_start, point, _prev, model.CurrentFrame);
        }

        ToolEndBase(point, model, keyState, selection);
    }

    public virtual void OnSelectionBegin(Point startPoint, Point currentPoint, Point previousPoint, Frame frame) { }
    public virtual void OnSelection(Point startPoint, Point currentPoint, Point previousPoint, Frame frame) { }
    public virtual void OnSelectionEnd(Point startPoint, Point currentPoint, Point previousPoint, Frame frame) { }

    protected bool IsInSelection(Point point)
    {
        return _selection != null && _selection.Pixels.Any(p => p.Position == point);
    }
}
