using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Pixed.Core.Utils;
using System;

namespace Pixed.Application.Zoom.Internals;

internal class ExternalControl : Border
{
    private readonly BaseControl.PinchGestureRecognizer _pinchGestureRecognizer;
    private readonly ZoomControl _zoomControl;

    public bool GestureEnabled
    {
        get => _pinchGestureRecognizer.IsEnabled;
        set
        {
            _pinchGestureRecognizer.IsEnabled = value;
        }
    }

    public ExternalControl(ZoomControl zoomControl)
    {
        _pinchGestureRecognizer = new BaseControl.PinchGestureRecognizer(zoomControl);
        _zoomControl = zoomControl;
        GestureRecognizers.Add(_pinchGestureRecognizer);
        this.AddHandler(Gestures.PinchEvent, PinchHandler);
        this.AddHandler(Gestures.PinchEndedEvent, PinchEndedHandler);
    }

    private void PinchHandler(object? sender, PinchEventArgs e)
    {
        _zoomControl.BaseControl.GestureMatrixEndUpdate();
        if (_zoomControl.InternalGestureMatrix.HasValue && !e.Scale.EqualsPrecision(1, 0.00001))
        {
            double scale = 1;

            if (e.Scale < 1)
            {
                scale = -Math.Abs(1 - e.Scale);
            }
            else if (e.Scale > 1)
            {
                scale = e.Scale;
            }
            _zoomControl.SetMatrix(new Matrix(scale, 0, 0, scale, e.ScaleOrigin.X, e.ScaleOrigin.Y));
            e.Handled = true;
        }
    }

    private void PinchEndedHandler(object? sender, PinchEndedEventArgs e)
    {
        _zoomControl.BaseControl.GestureMatrixEndHandling(e);
    }
}