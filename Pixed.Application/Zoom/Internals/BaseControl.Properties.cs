using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media.Transformation;
using System;

namespace Pixed.Application.Zoom.Internals;

internal partial class BaseControl
{
    public static readonly StyledProperty<double> ZoomSpeedProperty =
        AvaloniaProperty.Register<BaseControl, double>(nameof(ZoomSpeed), 1.2, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> PowerFactorProperty =
        AvaloniaProperty.Register<BaseControl, double>(nameof(PowerFactor), 1, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> TransitionThresholdProperty =
        AvaloniaProperty.Register<BaseControl, double>(nameof(TransitionThreshold), 0.5, false, BindingMode.TwoWay);

    public static readonly StyledProperty<double> MinZoomProperty =
        AvaloniaProperty.Register<BaseControl, double>(nameof(MinZoom), 0.9, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MaxZoomProperty =
        AvaloniaProperty.Register<BaseControl, double>(nameof(MaxZoom), 9000, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MinOffsetXProperty =
        AvaloniaProperty.Register<BaseControl, double>(nameof(MinOffsetX), double.NegativeInfinity, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MaxOffsetXProperty =
        AvaloniaProperty.Register<BaseControl, double>(nameof(MaxOffsetX), double.PositiveInfinity, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MinOffsetYProperty =
        AvaloniaProperty.Register<BaseControl, double>(nameof(MinOffsetY), double.NegativeInfinity, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MaxOffsetYProperty =
        AvaloniaProperty.Register<BaseControl, double>(nameof(MaxOffsetY), double.PositiveInfinity, false, BindingMode.TwoWay);
    public static readonly StyledProperty<bool> GestureZoomEnabledProperty =
        AvaloniaProperty.Register<BaseControl, bool>(nameof(GestureZoomEnabled), false, false, BindingMode.TwoWay, null, (obj, value) =>
        {
            if (obj is BaseControl control)
            {
               control.ZoomControl.GesturesEnabled = value;
            }
            return value;
        });

    static BaseControl()
    {
        AffectsArrange<BaseControl>(
            ZoomSpeedProperty,
            MinZoomProperty,
            MaxZoomProperty,
            MinOffsetXProperty,
            MaxOffsetXProperty,
            MinOffsetYProperty,
            MaxOffsetYProperty);
    }

    private Control? _element;
    private Point _pan;
    private Point _previous;
    private Matrix _matrix;
    private TransformOperations.Builder _transformBuilder;
    private bool _isPanning;
    private volatile bool _updating = false;
    private double _zoom = 1.0;
    private double _offsetX = 0.0;
    private double _offsetY = 0.0;
    private bool _captured = false;
    private readonly PanGestureRecognizer _panGestureRecognizer;
    private bool _disposedValue;
    private readonly IDisposable _childChanged;
    private Matrix? _gestureMatrix;
    private readonly ZoomControl _parent;

    public double ZoomSpeed
    {
        get => GetValue(ZoomSpeedProperty);
        set => SetValue(ZoomSpeedProperty, value);
    }
    public double PowerFactor
    {
        get => GetValue(PowerFactorProperty);
        set => SetValue(PowerFactorProperty, value);
    }
    public double TransitionThreshold
    {
        get => GetValue(TransitionThresholdProperty);
        set => SetValue(TransitionThresholdProperty, value);
    }
    public ZoomControl ZoomControl => _parent;
    public Matrix Matrix => _matrix;
    public Matrix? GestureMatrix => _gestureMatrix;
    public double Zoom => _zoom;
    public Point Offset => new(_offsetX, _offsetY);
    public bool GestureStarted => _gestureMatrix != null;

    public double MinZoom
    {
        get => GetValue(MinZoomProperty);
        set => SetValue(MinZoomProperty, value);
    }
    public double MaxZoom
    {
        get => GetValue(MaxZoomProperty);
        set => SetValue(MaxZoomProperty, value);
    }
    public double MinOffsetX
    {
        get => GetValue(MinOffsetXProperty);
        set => SetValue(MinOffsetXProperty, value);
    }
    public double MaxOffsetX
    {
        get => GetValue(MaxOffsetXProperty);
        set => SetValue(MaxOffsetXProperty, value);
    }
    public double MinOffsetY
    {
        get => GetValue(MinOffsetYProperty);
        set => SetValue(MinOffsetYProperty, value);
    }
    public double MaxOffsetY
    {
        get => GetValue(MaxOffsetYProperty);
        set => SetValue(MaxOffsetYProperty, value);
    }

    public bool GestureZoomEnabled
    {
        get => GetValue(GestureZoomEnabledProperty);
        set
        {
            SetValue(GestureZoomEnabledProperty, value);
        }
    }
}