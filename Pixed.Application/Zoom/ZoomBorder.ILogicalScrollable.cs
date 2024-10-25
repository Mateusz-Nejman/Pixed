using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using System;

namespace Pixed.Application.Zoom;
internal partial class ZoomBorder : ILogicalScrollable
{
    private Size _extent;
    private Size _viewport;
    private Vector _offset;
    private bool _canHorizontallyScroll;
    private bool _canVerticallyScroll;
    private EventHandler? _scrollInvalidated;

    public static void CalculateScrollable(Rect source, Size borderSize, Matrix matrix, out Size extent, out Size viewport, out Vector offset)
    {
        var bounds = new Rect(0, 0, source.Width, source.Height);

        viewport = borderSize;

        var transformed = bounds.TransformToAABB(matrix);

        var width = transformed.Size.Width;
        var height = transformed.Size.Height;

        if (width < viewport.Width)
        {
            width = viewport.Width;

            if (transformed.Position.X < 0.0)
            {
                width += Math.Abs(transformed.Position.X);
            }
            else
            {
                var widthTranslated = transformed.Size.Width + transformed.Position.X;
                if (widthTranslated > width)
                {
                    width += widthTranslated - width;
                }
            }
        }
        else if (!(width > viewport.Width))
        {
            width += Math.Abs(transformed.Position.X);
        }

        if (height < viewport.Height)
        {
            height = viewport.Height;

            if (transformed.Position.Y < 0.0)
            {
                height += Math.Abs(transformed.Position.Y);
            }
            else
            {
                var heightTranslated = transformed.Size.Height + transformed.Position.Y;
                if (heightTranslated > height)
                {
                    height += heightTranslated - height;
                }
            }
        }
        else if (!(height > viewport.Height))
        {
            height += Math.Abs(transformed.Position.Y);
        }

        extent = new Size(width, height);

        var ox = transformed.Position.X;
        var oy = transformed.Position.Y;

        var offsetX = ox < 0 ? Math.Abs(ox) : 0;
        var offsetY = oy < 0 ? Math.Abs(oy) : 0;

        offset = new Vector(offsetX, offsetY);
    }
    Size IScrollable.Extent => _extent;
    Vector IScrollable.Offset
    {
        get => _offset;
        set
        {
            if (_updating)
            {
                return;
            }
            _updating = true;

            var (x, y) = _offset;
            var dx = x - value.X;
            var dy = y - value.Y;

            _offset = value;
            _matrix = MatrixHelper.ScaleAndTranslate(_zoom, _matrix.M31 + dx, _matrix.M32 + dy);
            Invalidate(!this.IsPointerOver);

            _updating = false;
        }
    }
    Size IScrollable.Viewport => _viewport;

    bool ILogicalScrollable.CanHorizontallyScroll
    {
        get => _canHorizontallyScroll;
        set
        {
            _canHorizontallyScroll = value;
            InvalidateMeasure();
        }
    }

    bool ILogicalScrollable.CanVerticallyScroll
    {
        get => _canVerticallyScroll;
        set
        {
            _canVerticallyScroll = value;
            InvalidateMeasure();
        }
    }

    bool ILogicalScrollable.IsLogicalScrollEnabled => true;

    event EventHandler? ILogicalScrollable.ScrollInvalidated
    {
        add => _scrollInvalidated += value;
        remove => _scrollInvalidated -= value;
    }

    Size ILogicalScrollable.ScrollSize => new(1, 1);

    Size ILogicalScrollable.PageScrollSize => new(10, 10);

    bool ILogicalScrollable.BringIntoView(Control target, Rect targetRect)
    {
        return false;
    }

    Control? ILogicalScrollable.GetControlInDirection(NavigationDirection direction, Control? from)
    {
        return null;
    }

    void ILogicalScrollable.RaiseScrollInvalidated(EventArgs e)
    {
        _scrollInvalidated?.Invoke(this, e);
    }

    private void InvalidateScrollable()
    {
        if (this is not ILogicalScrollable scrollable)
        {
            return;
        }

        if (_element == null)
        {
            return;
        }

        CalculateScrollable(_element.Bounds, Bounds.Size, _matrix, out var extent, out var viewport, out var offset);
        _extent = extent;
        _offset = offset;
        _viewport = viewport;

        scrollable.RaiseScrollInvalidated(EventArgs.Empty);
    }
}
