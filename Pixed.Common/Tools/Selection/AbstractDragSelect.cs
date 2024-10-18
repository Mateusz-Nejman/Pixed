using Pixed.Models;
using SkiaSharp;

namespace Pixed.Tools.Selection;

internal abstract class AbstractDragSelect(ApplicationData applicationData, ToolSelector toolSelector) : BaseSelect(applicationData, toolSelector)
{
    public override void OnSelect(int x, int y, Frame frame, ref SKBitmap overlay)
    {
        if (!_hasSelection && (x != _startX || y != _startY))
        {
            _hasSelection = true;
            OnDragSelectStart(x, y, frame, ref overlay);
        }

        if (_hasSelection)
        {
            OnDragSelect(x, y, frame, ref overlay);
        }
    }

    public override void OnSelectEnd(int x, int y, Frame frame, ref SKBitmap overlay)
    {
        if (_hasSelection)
        {
            OnDragSelectEnd(x, y, frame, ref overlay);
        }
    }

    public override void OnSelectStart(int x, int y, Frame frame, ref SKBitmap overlay)
    {
        if (_hasSelection)
        {
            _hasSelection = false;
        }
        else
        {
            _hasSelection = true;
            OnDragSelectStart(x, y, frame, ref overlay);
            overlay.SetPixel(x, y, UniColor.WithAlpha(128, UniColor.CornflowerBlue));
        }
    }

    public virtual void OnDragSelectStart(int x, int y, Frame frame, ref SKBitmap overlay) { }
    public virtual void OnDragSelect(int x, int y, Frame frame, ref SKBitmap overlay) { }
    public virtual void OnDragSelectEnd(int x, int y, Frame frame, ref SKBitmap overlay) { }
}
