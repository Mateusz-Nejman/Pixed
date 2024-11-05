using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media.Transformation;
using Pixed.Application.Controls.Gestures;
using System;

namespace Pixed.Application.Zoom;

internal partial class ZoomBorder
{
    public delegate void ZoomChangedEventHandler(object sender, ZoomChangedEventArgs e);

    public static readonly StyledProperty<double> ZoomSpeedProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(ZoomSpeed), 1.2, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> PowerFactorProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(PowerFactor), 1, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> TransitionThresholdProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(TransitionThreshold), 0.5, false, BindingMode.TwoWay);

    public static readonly DirectProperty<ZoomBorder, double> ZoomProperty =
        AvaloniaProperty.RegisterDirect<ZoomBorder, double>(nameof(Zoom), o => o.Zoom, null, 1.0);
    public static readonly DirectProperty<ZoomBorder, double> OffsetXProperty =
        AvaloniaProperty.RegisterDirect<ZoomBorder, double>(nameof(OffsetX), o => o.OffsetX, null, 0.0);
    public static readonly DirectProperty<ZoomBorder, double> OffsetYProperty =
        AvaloniaProperty.RegisterDirect<ZoomBorder, double>(nameof(OffsetY), o => o.OffsetY, null, 0.0);

    public static readonly StyledProperty<double> MinZoomProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MinZoom), 0.9, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MaxZoomProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MaxZoom), 9000, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MinOffsetXProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MinOffsetX), double.NegativeInfinity, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MaxOffsetXProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MaxOffsetX), double.PositiveInfinity, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MinOffsetYProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MinOffsetY), double.NegativeInfinity, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MaxOffsetYProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MaxOffsetY), double.PositiveInfinity, false, BindingMode.TwoWay);
    public static readonly StyledProperty<bool> GestureZoomEnabledProperty =
        AvaloniaProperty.Register<ZoomBorder, bool>(nameof(GestureZoomEnabled), false, false, BindingMode.TwoWay, null, (obj, value) =>
        {
            if (obj is ZoomBorder border)
            {
                border._zoomGestureRecognizer.IsEnabled = value;
            }
            return value;
        });

    static ZoomBorder()
    {
        AffectsArrange<ZoomBorder>(
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
    private readonly PinchGestureRecognizer _zoomGestureRecognizer;
    private readonly PanGestureRecognizer _panGestureRecognizer;
    private bool _disposedValue;
    private readonly IDisposable _childChanged;
    private Matrix? _gestureMatrix;

    public event ZoomChangedEventHandler? ZoomChanged;

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
    public Matrix Matrix => _matrix;
    public double Zoom => _zoom;
    public double OffsetX => _offsetX;
    public double OffsetY => _offsetY;
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
