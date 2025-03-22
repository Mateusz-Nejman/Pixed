using Avalonia.Controls;
using Avalonia.Input;
using Pixed.Core.Utils;
using System;

namespace Pixed.Application.Zoom.Internals;

internal class ExternalControl : Border
{
    private readonly BaseControl.PinchGestureRecognizer _pinchGestureRecognizer;
    private readonly ZoomControl _zoomControl;
    private bool _enabled = true;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
        }
    }

    public ExternalControl(ZoomControl zoomControl)
    {
        _pinchGestureRecognizer = new BaseControl.PinchGestureRecognizer();
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
            var origin = _zoomControl.GestureToContentMatrix.Transform(e.ScaleOrigin);
            _zoomControl.ZoomDeltaTo(scale, origin.X, origin.Y, _zoomControl.InternalGestureMatrix.Value);
            e.Handled = true;
        }
    }

    private void PinchEndedHandler(object? sender, PinchEndedEventArgs e)
    {
        _zoomControl.BaseControl.GestureMatrixEndHandling(e);
    }
}