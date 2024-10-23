using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media.Transformation;

namespace Pixed.Application.Zoom;

internal partial class ZoomBorder
{
    public static readonly StyledProperty<double> ZoomSpeedProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(ZoomSpeed), 1.2, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> PowerFactorProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(PowerFactor), 1, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> TransitionThresholdProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(TransitionThreshold), 0.5, false, BindingMode.TwoWay);

    public static readonly DirectProperty<ZoomBorder, double> ZoomXProperty =
        AvaloniaProperty.RegisterDirect<ZoomBorder, double>(nameof(ZoomX), o => o.ZoomX, null, 1.0);
    public static readonly DirectProperty<ZoomBorder, double> ZoomYProperty =
        AvaloniaProperty.RegisterDirect<ZoomBorder, double>(nameof(ZoomY), o => o.ZoomY, null, 1.0);
    public static readonly DirectProperty<ZoomBorder, double> OffsetXProperty =
        AvaloniaProperty.RegisterDirect<ZoomBorder, double>(nameof(OffsetX), o => o.OffsetX, null, 0.0);
    public static readonly DirectProperty<ZoomBorder, double> OffsetYProperty =
        AvaloniaProperty.RegisterDirect<ZoomBorder, double>(nameof(OffsetY), o => o.OffsetY, null, 0.0);

    public static readonly StyledProperty<double> MinZoomXProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MinZoomX), double.NegativeInfinity, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MaxZoomXProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MaxZoomX), double.PositiveInfinity, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MinZoomYProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MinZoomY), double.NegativeInfinity, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MaxZoomYProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MaxZoomY), double.PositiveInfinity, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MinOffsetXProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MinOffsetX), double.NegativeInfinity, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MaxOffsetXProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MaxOffsetX), double.PositiveInfinity, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MinOffsetYProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MinOffsetY), double.NegativeInfinity, false, BindingMode.TwoWay);
    public static readonly StyledProperty<double> MaxOffsetYProperty =
        AvaloniaProperty.Register<ZoomBorder, double>(nameof(MaxOffsetY), double.PositiveInfinity, false, BindingMode.TwoWay);
    public static readonly StyledProperty<bool> EnableGestureZoomProperty =
        AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnableGestureZoom), true, false, BindingMode.TwoWay);
    public static readonly StyledProperty<bool> EnableGestureRotationProperty =
        AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnableGestureRotation), true, false, BindingMode.TwoWay);
    public static readonly StyledProperty<bool> EnableGestureTranslationProperty =
        AvaloniaProperty.Register<ZoomBorder, bool>(nameof(EnableGestureTranslation), true, false, BindingMode.TwoWay);

    static ZoomBorder()
    {
        AffectsArrange<ZoomBorder>(
            ZoomSpeedProperty,
            MinZoomXProperty,
            MaxZoomXProperty,
            MinZoomYProperty,
            MaxZoomYProperty,
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
    private double _zoomX = 1.0;
    private double _zoomY = 1.0;
    private double _offsetX = 0.0;
    private double _offsetY = 0.0;
    private bool _captured = false;

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
    public double ZoomX => _zoomX;
    public double ZoomY => _zoomY;
    public double OffsetX => _offsetX;
    public double OffsetY => _offsetY;

    public double MinZoomX
    {
        get => GetValue(MinZoomXProperty);
        set => SetValue(MinZoomXProperty, value);
    }
    public double MaxZoomX
    {
        get => GetValue(MaxZoomXProperty);
        set => SetValue(MaxZoomXProperty, value);
    }
    public double MinZoomY
    {
        get => GetValue(MinZoomYProperty);
        set => SetValue(MinZoomYProperty, value);
    }
    public double MaxZoomY
    {
        get => GetValue(MaxZoomYProperty);
        set => SetValue(MaxZoomYProperty, value);
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

    public bool EnableGestureZoom
    {
        get => GetValue(EnableGestureZoomProperty);
        set => SetValue(EnableGestureZoomProperty, value);
    }
    public bool EnableGestureRotation
    {
        get => GetValue(EnableGestureRotationProperty);
        set => SetValue(EnableGestureRotationProperty, value);
    }
    public bool EnableGestureTranslation
    {
        get => GetValue(EnableGestureTranslationProperty);
        set => SetValue(EnableGestureTranslationProperty, value);
    }
}
