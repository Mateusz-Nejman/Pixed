using Pixed.Models;
using Pixed.Selection;
using Pixed.Utils;
using SkiaSharp;
using System;
using System.Linq;
using Frame = Pixed.Models.Frame;

namespace Pixed.Tools.Selection;

internal class BaseSelect(ApplicationData applicationData, ToolSelector toolSelector) : BaseTool(applicationData)
{
    public enum SelectionMode
    {
        None,
        Select
    }

    private readonly ToolSelector _toolSelector = toolSelector;
    protected int _startX = 0;
    protected int _startY = 0;

    protected int _lastX = 0;
    protected int _lastY = 0;


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
        _selection = new RectangularSelection(0, 0, _applicationData.CurrentFrame.Width - 1, _applicationData.CurrentFrame.Height - 1, _applicationData.CurrentFrame);
        overlayAction?.Invoke(CreateOverlayFromCurrentFrame());
        Subjects.SelectionCreated.OnNext(_selection);
    }

    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        _startX = x;
        _startY = y;
        _lastX = x;
        _lastY = y;

        if (!IsInSelection(x, y))
        {
            _mode = SelectionMode.Select;
            OnSelectStart(x, y, frame, ref overlay);
        }
        else
        {
            OnSelectionMoveStart(x, y, frame, ref overlay);
        }
    }

    public override void MoveTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        if (_mode == SelectionMode.Select)
        {
            OnSelect(x, y, frame, ref overlay);
        }
    }

    public override void ReleaseTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        if (_mode == SelectionMode.Select)
        {
            OnSelectEnd(x, y, frame, ref overlay);
        }
    }

    public override void UpdateHighlightedPixel(int x, int y, Frame frame, ref SKBitmap overlay)
    {
        if (!_hasSelection)
        {
            base.UpdateHighlightedPixel(x, y, frame, ref overlay);
        }
    }

    public virtual void OnSelectStart(int x, int y, Frame frame, ref SKBitmap overlay) { }
    public virtual void OnSelect(int x, int y, Frame frame, ref SKBitmap overlay) { }
    public virtual void OnSelectEnd(int x, int y, Frame frame, ref SKBitmap overlay) { }
    public virtual void OnSelectionMoveStart(int x, int y, Frame frame, ref SKBitmap overlay) { }
    public virtual void OnSelectionMove(int x, int y, Frame frame, ref SKBitmap overlay)
    {
        var deltaX = x - _lastX;
        var deltaY = y - _lastY;
        _selection?.Move(deltaX, deltaY);

        overlay.Clear();
        DrawSelectionOnOverlay(ref overlay);

        _lastX = x;
        _lastY = y;
    }
    public virtual void OnSelectionMoveEnd(int x, int y, Frame frame, ref SKBitmap overlay)
    {
        OnSelectionMove(x, y, frame, ref overlay);
    }

    public override void Reset()
    {
        _selection?.Pixels.Clear();
    }

    protected bool IsInSelection(int x, int y)
    {
        return _selection != null && _selection.Pixels.Any(p => p.X == x && p.Y == y);
    }

    protected void DrawSelectionOnOverlay(ref SKBitmap bitmap)
    {
        var pixels = _selection.Pixels;

        for (int i = 0; i < pixels.Count; i++)
        {
            var pixel = pixels[i];
            var hasColor = pixel.Color != UniColor.Transparent;

            if (!bitmap.ContainsPixel(pixel.X, pixel.Y))
            {
                continue;
            }

            var color = UniColor.WithAlpha(128, UniColor.GetFromResources("Accent"));

            if (hasColor)
            {
                color = pixel.Color;
                color = color.Lighten(10);
                color.A = 128;
            }

            bitmap.SetPixel(pixel.X, pixel.Y, color);
        }
    }

    private SKBitmap CreateOverlayFromCurrentFrame()
    {
        SKBitmap newOverlay = new(_applicationData.CurrentFrame.Width, _applicationData.CurrentFrame.Height, true);
        DrawSelectionOnOverlay(ref newOverlay);
        return newOverlay;
    }
}
