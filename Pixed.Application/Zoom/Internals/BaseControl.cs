using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Media.Transformation;
using Avalonia.VisualTree;
using Pixed.Application.Controls.PaintCanvas;
using System;
using System.Linq;
using System.Reactive;

namespace Pixed.Application.Zoom.Internals;

[PseudoClasses(":isPanning")]
internal partial class BaseControl : Decorator, IDisposable
{
    public BaseControl(ZoomControl parent)
    {
        _parent = parent;
        _isPanning = false;
        _matrix = Matrix.Identity;
        _captured = false;

        Focusable = true;

        AttachedToVisualTree += PanAndZoom_AttachedToVisualTree;
        DetachedFromVisualTree += PanAndZoom_DetachedFromVisualTree;

        _childChanged = this.GetObservable(ChildProperty).Subscribe(new AnonymousObserver<Control?>(ChildChanged));
        _panGestureRecognizer = new PanGestureRecognizer(this);
        ClipToBounds = true;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _childChanged?.Dispose();
                AttachedToVisualTree -= PanAndZoom_AttachedToVisualTree;
                DetachedFromVisualTree -= PanAndZoom_DetachedFromVisualTree;
            }
            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void ConfigureOffsetBounds(int frameWidth, int frameHeight)
    {
        double xBound = (frameWidth * _zoom);
        double yBound = (frameHeight * _zoom);
        MinOffsetX = -xBound;
        MinOffsetY = -yBound;
        MaxOffsetX = xBound;
        MaxOffsetY = yBound;
        Constrain();
    }

    public void ZoomTo(double ratio, Point zoomPoint, Matrix matrix, bool skipTransitions = false)
    {
        if (_updating)
        {
            return;
        }
        _updating = true;

        if ((Zoom >= MaxZoom && ratio > 1) || (Zoom <= MinZoom && ratio < 1))
        {
            _updating = false;
            return;
        }

        _matrix = MatrixHelper.ScaleAtPrepend(matrix, ratio, zoomPoint);
        Invalidate(skipTransitions);

        _updating = false;
    }
    public void ZoomDeltaTo(double delta, Point zoomPoint, Matrix matrix, bool skipTransitions = false)
    {
        int sign = Math.Sign(delta);

        if (sign == 0)
        {
            sign = 1;
        }
        double realDelta = sign * Math.Abs(delta);
        ZoomTo(Math.Pow(ZoomSpeed, realDelta), zoomPoint, matrix, skipTransitions || Math.Abs(realDelta) <= TransitionThreshold);
    }

    public void SetMatrix(Matrix matrix, bool skipTransitions = false)
    {
        if (_updating)
        {
            return;
        }
        _updating = true;
        _matrix = matrix;
        Invalidate(skipTransitions);

        _updating = false;
    }

    public void GestureMatrixEndUpdate()
    {
        if (_gestureMatrix == null)
        {
            _captured = false;
            _isPanning = false;
            _gestureMatrix = _matrix;
            _updating = false;
        }
    }

    public void GestureMatrixEndHandling(PinchEndedEventArgs e)
    {
        _gestureMatrix = null;
        _updating = false;
        e.Handled = true;
    }

    private void ResetMatrix()
    {
        ResetMatrix(false);
    }
    private void ResetMatrix(bool skipTransitions)
    {
        SetMatrix(Matrix.Identity, skipTransitions);
    }

    private void ZoomDeltaTo(double delta, Point zoomPoint, bool skipTransitions = false)
    {
        ZoomDeltaTo(delta, zoomPoint, _matrix, skipTransitions);
    }
    private void BeginPanTo(Point point)
    {
        _pan = new Point();
        _previous = point;
    }
    private void ContinuePanTo(Point point, bool skipTransitions = false)
    {
        if (_updating)
        {
            return;
        }
        _updating = true;
        var dx = point.X - _previous.X;
        var dy = point.Y - _previous.Y;
        var delta = new Point(dx, dy);
        _previous = point;
        _pan = new Point(_pan.X + delta.X, _pan.Y + delta.Y);
        _matrix = MatrixHelper.TranslatePrepend(_matrix, _pan);
        Invalidate(skipTransitions);

        _updating = false;
    }

    private void Element_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == BoundsProperty)
        {
            InvalidateScrollable();
        }
    }
    private void PanAndZoom_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        ChildChanged(Child);
    }

    private void PanAndZoom_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        DetachElement();
    }

    private void Border_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_element == null || _captured)
        {
            return;
        }
        var point = e.GetPosition(_element);

        if (e.Delta.Y < 0 && Zoom >= MaxZoom)
        {
            e.Delta.Y.ToString();
        }
        ZoomDeltaTo(e.Delta.Y, point);
    }

    private void ChildChanged(Control? element)
    {
        if (element != null && element != _element && _element != null)
        {
            DetachElement();
        }

        if (element != null && element != _element)
        {
            AttachElement(element);

            var children = element.GetVisualChildren();

            foreach (var child in children)
            {
                if (child is OverlayControl control)
                {
                    control.AttachToZoomControl(_parent);
                }
            }
        }
    }

    private void AttachElement(Control? element)
    {
        if (element == null)
        {
            return;
        }
        _element = element;
        PointerWheelChanged += Border_PointerWheelChanged;
        AddGestureRecognizers();
    }

    private void DetachElement()
    {
        if (_element == null)
        {
            return;
        }
        PointerWheelChanged -= Border_PointerWheelChanged;
        RemoveGestureRecognizers();
        _element.RenderTransform = null;
        _element = null;
    }
    private void RaiseZoomChanged()
    {
        ZoomControl.ZoomChanged.OnNext(new ZoomEntry(_zoom, _offsetX, _offsetY));
    }

    private void Constrain()
    {
        var zoom = ClampValue(_matrix.M11, MinZoom, MaxZoom);
        var offsetX = ClampValue(_matrix.M31, MinOffsetX, MaxOffsetX);
        var offsetY = ClampValue(_matrix.M32, MinOffsetY, MaxOffsetY);
        _matrix = new Matrix(zoom, 0.0, 0.0, zoom, offsetX, offsetY);
    }
    private void Invalidate(bool skipTransitions = false)
    {
        if (_element == null)
        {
            return;
        }

        Constrain();

        _zoom = _matrix.M11;
        _offsetX = _matrix.M31;
        _offsetY = _matrix.M32;

        InvalidateScrollable();
        InvalidateElement(skipTransitions);
        RaiseZoomChanged();
    }
    private void InvalidateElement(bool skipTransitions)
    {
        if (_element == null)
        {
            return;
        }

        Transitions? backupTransitions = null;

        if (skipTransitions)
        {
            if (_element is Animatable anim)
            {
                backupTransitions = anim.Transitions;
                anim.Transitions = null;
            }
        }

        _element.RenderTransformOrigin = new RelativePoint(new Point(0, 0), RelativeUnit.Relative);
        _transformBuilder = new TransformOperations.Builder(1);
        _transformBuilder.AppendMatrix(_matrix);
        _element.RenderTransform = _transformBuilder.Build();

        if (skipTransitions && backupTransitions != null)
        {
            if (_element is Animatable anim)
            {
                anim.Transitions = backupTransitions;
            }
        }
        _element.InvalidateVisual();
    }

    private void AddGestureRecognizers()
    {
        if (!GestureRecognizers.Contains(_panGestureRecognizer))
        {
            GestureRecognizers.Add(_panGestureRecognizer);
        }

        _panGestureRecognizer.IsEnabled = true;
    }

    private void RemoveGestureRecognizers()
    {
        _panGestureRecognizer.IsEnabled = false;
    }
    private static double ClampValue(double value, double minimum, double maximum)
    {
        if (minimum > maximum)
            throw new ArgumentException($"Parameter {nameof(minimum)} is greater than {nameof(maximum)}.");

        if (maximum < minimum)
            throw new ArgumentException($"Parameter {nameof(maximum)} is lower than {nameof(minimum)}.");

        return Math.Min(Math.Max(value, minimum), maximum);
    }

    private void SetPseudoClass(string name, bool flag) => PseudoClasses.Set(name, flag);
}