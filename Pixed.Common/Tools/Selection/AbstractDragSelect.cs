using Pixed.Common.Models;
using SkiaSharp;

namespace Pixed.Common.Tools.Selection;

internal abstract class AbstractDragSelect(ApplicationData applicationData, ToolSelector toolSelector) : BaseSelect(applicationData, toolSelector)
{
    public override void OnSelect(Point point, Frame frame, ref SKBitmap overlay)
    {
        if (!_hasSelection && point != _start)
        {
            _hasSelection = true;
            OnDragSelectStart(point, frame, ref overlay);
        }

        if (_hasSelection)
        {
            OnDragSelect(point, frame, ref overlay);
        }
    }

    public override void OnSelectEnd(Point point, Frame frame, ref SKBitmap overlay)
    {
        if (_hasSelection)
        {
            OnDragSelectEnd(point, frame, ref overlay);
        }

        Subjects.SelectionCreated.OnNext(_selection);
    }

    public override void OnSelectStart(Point point, Frame frame, ref SKBitmap overlay)
    {
        if (_hasSelection)
        {
            _hasSelection = false;
        }
        else
        {
            _hasSelection = true;
            OnDragSelectStart(point, frame, ref overlay);
            overlay.SetPixel(point.X, point.Y, UniColor.WithAlpha(128, UniColor.CornflowerBlue));
        }
    }

    public virtual void OnDragSelectStart(Point point, Frame frame, ref SKBitmap overlay) { }
    public virtual void OnDragSelect(Point point, Frame frame, ref SKBitmap overlay) { }
    public virtual void OnDragSelectEnd(Point point, Frame frame, ref SKBitmap overlay) { }
}
