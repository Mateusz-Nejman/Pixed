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
        overlayAction?.Invoke(CreateOverlayFromCurrentFrame());
        Subjects.SelectionCreated.OnNext(_selection);
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
        DrawSelectionOnOverlay(ref overlay);

        _last = point;
    }
    public virtual void OnSelectionMoveEnd(Point point, Frame frame, ref SKBitmap overlay)
    {
        OnSelectionMove(point, frame, ref overlay);
    }

    public override void Reset()
    {
        _selection?.Pixels.Clear();
    }

    protected bool IsInSelection(Point point)
    {
        return _selection != null && _selection.Pixels.Any(p => p.Position == point);
    }

    protected void DrawSelectionOnOverlay(ref SKBitmap bitmap)
    {
        var pixels = _selection.Pixels;

        for (int i = 0; i < pixels.Count; i++)
        {
            var pixel = pixels[i];
            var hasColor = pixel.Color != UniColor.Transparent;

            if (!bitmap.ContainsPixel(pixel.Position))
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

            bitmap.SetPixel(pixel.Position.X, pixel.Position.Y, color);
        }

        Subjects.OverlayModified.OnNext(bitmap);
    }

    private SKBitmap CreateOverlayFromCurrentFrame()
    {
        SKBitmap newOverlay = new(_applicationData.CurrentFrame.Width, _applicationData.CurrentFrame.Height, true);
        DrawSelectionOnOverlay(ref newOverlay);
        return newOverlay;
    }
}
