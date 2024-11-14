using Pixed.Common.Services.Keyboard;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using SkiaSharp;
using System.Linq;
using Frame = Pixed.Core.Models.Frame;

namespace Pixed.Common.Tools.Selection;

public class ToolSelectBase(ApplicationData applicationData) : BaseTool(applicationData)
{
    public override string ImagePath => string.Empty;
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

    public override bool ShiftHandle { get; protected set; } = true;
    public override bool AddToHistory { get; protected set; } = false;
    public override bool SingleHighlightedPixel { get; protected set; }

    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(point, frame, ref overlay, keyState);
        _start = point;
        _prev = point;

        OnSelectionBegin(_start, point, _prev, frame);
        _stage = SelectionStage.Selecting;
    }

    public override void MoveTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        if (_stage == SelectionStage.Selecting)
        {
            OnSelection(_start, point, _prev, frame);
            _prev = point;
        }
    }

    public override void ReleaseTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        if (_stage == SelectionStage.Selecting)
        {
            OnSelectionEnd(_start, point, _prev, frame);
        }

        ReleaseToolBase(point, frame, ref overlay, keyState);
    }

    public virtual void OnSelectionBegin(Point startPoint, Point currentPoint, Point previousPoint, Frame frame) { }
    public virtual void OnSelection(Point startPoint, Point currentPoint, Point previousPoint, Frame frame) { }
    public virtual void OnSelectionEnd(Point startPoint, Point currentPoint, Point previousPoint, Frame frame) { }

    public override void Reset()
    {
        _selection?.Pixels.Clear();
        _selection = null;
        Subjects.SelectionDismissed.OnNext(null);
    }

    protected bool IsInSelection(Point point)
    {
        return _selection != null && _selection.Pixels.Any(p => p.Position == point);
    }
}
