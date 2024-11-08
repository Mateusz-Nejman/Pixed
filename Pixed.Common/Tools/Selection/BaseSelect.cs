using Pixed.Common.Models;
using Pixed.Common.Selection;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Utils;
using SkiaSharp;
using System;
using System.Linq;
using Frame = Pixed.Common.Models.Frame;

namespace Pixed.Common.Tools.Selection;

internal class BaseSelect(ApplicationData applicationData, ToolSelector toolSelector) : BaseTool(applicationData)
{
    public override string ImagePath => string.Empty;
    public enum SelectionMode
    {
        None,
        Select
    }

    private readonly ToolSelector _toolSelector = toolSelector;
    protected Point _start = new();
    protected Point _last = new();


    protected BaseSelection? _selection = null;
    protected SelectionMode _mode = SelectionMode.None;
    protected bool _hasSelection = false;

    public override bool ShiftHandle { get; protected set; } = true;
    public override bool AddToHistory { get; protected set; } = false;
    public override bool SingleHighlightedPixel { get; protected set; }

    public void SelectAll(Action<SKBitmap> overlayAction)
    {
        _toolSelector.SelectTool("tool_rectangle_select");
        _hasSelection = true;
        _mode = SelectionMode.Select;
        _selection = new RectangularSelection(new Point(), new Point(_applicationData.CurrentFrame.Width - 1, _applicationData.CurrentFrame.Height - 1), _applicationData.CurrentFrame);
        Subjects.SelectionCreating.OnNext(_selection);
    }

    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(point, frame, ref overlay, keyState);
        _start = point;
        _last = point;

        if (!IsInSelection(point))
        {
            _mode = SelectionMode.Select;
            OnSelectStart(point, frame, ref overlay);
        }
        else
        {
            OnSelectionMoveStart(point, frame, ref overlay);
        }

        Subjects.SelectionStarted.OnNext(_selection);
    }

    public override void MoveTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        if (_mode == SelectionMode.Select)
        {
            OnSelect(point, frame, ref overlay);
        }
    }

    public override void ReleaseTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        if (_mode == SelectionMode.Select)
        {
            OnSelectEnd(point, frame, ref overlay);
        }

        ReleaseToolBase(point, frame, ref overlay, keyState);
    }

    public override void UpdateHighlightedPixel(Point point, Frame frame, ref SKBitmap overlay)
    {
        if (!_hasSelection)
        {
            base.UpdateHighlightedPixel(point, frame, ref overlay);
        }
    }

    public virtual void OnSelectStart(Point point, Frame frame, ref SKBitmap overlay) { }
    public virtual void OnSelect(Point point, Frame frame, ref SKBitmap overlay) { }
    public virtual void OnSelectEnd(Point point, Frame frame, ref SKBitmap overlay) { }
    public virtual void OnSelectionMoveStart(Point point, Frame frame, ref SKBitmap overlay) { }
    public virtual void OnSelectionMove(Point point, Frame frame, ref SKBitmap overlay)
    {
        _selection?.Move(point - _last);

        overlay.Clear();
        _last = point;
    }
    public virtual void OnSelectionMoveEnd(Point point, Frame frame, ref SKBitmap overlay)
    {
        OnSelectionMove(point, frame, ref overlay);
    }

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
