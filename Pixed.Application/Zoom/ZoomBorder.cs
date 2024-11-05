using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Reactive;
using System;
using System.Linq;

namespace Pixed.Application.Zoom;

[PseudoClasses(":isPanning")]
internal partial class ZoomBorder : Border, IDisposable
{
    public ZoomBorder()
    {
        _isPanning = false;
        _matrix = Matrix.Identity;
        _captured = false;

        Focusable = true;
        Background = Brushes.Transparent;

        AttachedToVisualTree += PanAndZoom_AttachedToVisualTree;
        DetachedFromVisualTree += PanAndZoom_DetachedFromVisualTree;

        _childChanged = this.GetObservable(ChildProperty).Subscribe(new AnonymousObserver<Control?>(ChildChanged));
        _zoomGestureRecognizer = new();
        _panGestureRecognizer = new PanGestureRecognizer(this);
        this.AddHandler(Gestures.PinchEvent, PinchHandler);
        this.AddHandler(Gestures.PinchEndedEvent, PinchEndedHandler);
        KeyDown += ZoomBorder_KeyDown;
    }

    private void ZoomBorder_KeyDown(object? sender, KeyEventArgs e)
    {
#if DEBUG
        if (e.Key == Key.K)
        {
            ResetMatrix();
        }
#endif
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
    protected virtual void OnZoomChanged(ZoomChangedEventArgs e)
    {
        ZoomChanged?.Invoke(this, e);
    }

    private void ResetMatrix()
    {
        ResetMatrix(false);
    }
    private void ResetMatrix(bool skipTransitions)
    {
        SetMatrix(Matrix.Identity, skipTransitions);
    }

    private void SetMatrix(Matrix matrix, bool skipTransitions = false)
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
    private void PinchHandler(object? sender, PinchEventArgs e)
    {
        if (_gestureMatrix == null)
        {
            _captured = false;
            _isPanning = false;
            _gestureMatrix = _matrix;
            _updating = false;
        }

        bool negative = e.Scale < 1;
        ZoomDeltaTo(negative ? -Math.Abs(1 - e.Scale) : e.Scale, e.ScaleOrigin.X, e.ScaleOrigin.Y, _gestureMatrix.Value);
        e.Handled = true;
    }
    private void PinchEndedHandler(object? sender, PinchEndedEventArgs e)
    {
        _gestureMatrix = null;
        _updating = false;
        e.Handled = true;
    }

    private void ZoomTo(double ratio, double x, double y, Matrix matrix, bool skipTransitions = false)
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

        _matrix = MatrixHelper.ScaleAtPrepend(matrix, ratio, x, y);
        Invalidate(skipTransitions);

        _updating = false;
    }
    private void ZoomDeltaTo(double delta, double x, double y, bool skipTransitions = false)
    {
        ZoomDeltaTo(delta, x, y, _matrix, skipTransitions);
    }
    private void ZoomDeltaTo(double delta, double x, double y, Matrix matrix, bool skipTransitions = false)
    {
        double realDelta = Math.Sign(delta) * Math.Pow(Math.Abs(delta), PowerFactor);
        ZoomTo(Math.Pow(ZoomSpeed, realDelta), x, y, matrix, skipTransitions || Math.Abs(realDelta) <= TransitionThreshold);
    }
    private void BeginPanTo(double x, double y)
    {
        _pan = new Point();
        _previous = new Point(x, y);
    }
    private void ContinuePanTo(double x, double y, bool skipTransitions = false)
    {
        if (_updating)
        {
            return;
        }
        _updating = true;
        var dx = x - _previous.X;
        var dy = y - _previous.Y;
        var delta = new Point(dx, dy);
        _previous = new Point(x, y);
        _pan = new Point(_pan.X + delta.X, _pan.Y + delta.Y);
        _matrix = MatrixHelper.TranslatePrepend(_matrix, _pan.X, _pan.Y);
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
        ZoomDeltaTo(e.Delta.Y, point.X, point.Y);
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
        }
    }

    private void AttachElement(Control? element)
    {
        if (element == null)
        {
            return;
        }
        _element = element;
        _zoomGestureRecognizer.UpdateVisual(_element);
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
        var args = new ZoomChangedEventArgs(_zoom, _offsetX, _offsetY);
        OnZoomChanged(args);
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
        InvalidateProperties();
        InvalidateScrollable();
        InvalidateElement(skipTransitions);
        RaiseZoomChanged();
    }
    private void InvalidateProperties()
    {
        SetAndRaise(ZoomProperty, ref _zoom, _matrix.M11);
        SetAndRaise(OffsetXProperty, ref _offsetX, _matrix.M31);
        SetAndRaise(OffsetYProperty, ref _offsetY, _matrix.M32);
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
        if (!GestureRecognizers.Contains(_zoomGestureRecognizer))
        {
            GestureRecognizers.Add(_zoomGestureRecognizer);
        }

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
