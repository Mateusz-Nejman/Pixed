using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Metadata;
using Pixed.Application.Zoom.Internals;
using Pixed.Core;
using System;
using System.Reactive.Subjects;

namespace Pixed.Application.Zoom;
internal class ZoomControl : Decorator
{
    internal static Subject<ZoomEntry> ZoomChanged { get; set; } = new Subject<ZoomEntry>();

    public static readonly StyledProperty<Avalonia.Controls.Controls> ZoomContentProperty =
            AvaloniaProperty.Register<ZoomControl, Avalonia.Controls.Controls>(nameof(ZoomContent));

    public static readonly StyledProperty<bool> GesturesEnabledProperty =
        AvaloniaProperty.Register<ZoomControl, bool>(nameof(GesturesEnabled), false, false, BindingMode.TwoWay, null, (obj, value) =>
        {
            if (obj is ZoomControl zoomControl)
            {
                zoomControl._baseControl.GestureZoomEnabled = value;
            }
            return value;
        });

    public static readonly StyledProperty<double> ZoomWidthProperty =
        AvaloniaProperty.Register<ZoomControl, double>(nameof(ZoomWidth), 0, false, BindingMode.TwoWay, null, (obj, value) =>
        {
            if (obj is ZoomControl zoomControl)
            {
                zoomControl._childrenControl.Width = value;
            }
            return value;
        });

    public static readonly StyledProperty<double> ZoomHeightProperty =
        AvaloniaProperty.Register<ZoomControl, double>(nameof(ZoomHeight), 0, false, BindingMode.TwoWay, null, (obj, value) =>
        {
            if (obj is ZoomControl zoomControl)
            {
                zoomControl._childrenControl.Height = value;
            }
            return value;
        });

    private readonly ChildrenControl _childrenControl;
    private readonly BaseControl _baseControl;
    private readonly ExternalControl _externalControl;
    public Avalonia.Controls.Controls ZoomContent
    {
        get => _childrenControl.Children;
    }

    public double ZoomWidth
    {
        get => GetValue(ZoomWidthProperty);
        set => SetValue(ZoomWidthProperty, value);
    }

    public double ZoomHeight
    {
        get => GetValue(ZoomHeightProperty);
        set => SetValue(ZoomHeightProperty, value);
    }

    [Content]
    public Control Content => _externalControl;

    public bool GesturesEnabled
    {
        get => _baseControl.GestureZoomEnabled;
        set => _baseControl.GestureZoomEnabled = value;
    }

    public ActionCommand<Control, PointerPressedEventArgs>? PointerPressedCommand
    {
        get => _childrenControl.PointerPressedCommand;
        set
        {
            _childrenControl.PointerPressedCommand = value;
        }
    }
    public ActionCommand<Control, PointerReleasedEventArgs>? PointerReleasedCommand
    {
        get => _childrenControl.PointerReleasedCommand;
        set
        {
            _childrenControl.PointerReleasedCommand = value;
        }
    }
    public ActionCommand<Control, PointerWheelEventArgs>? PointerWheelCommand
    {
        get => _childrenControl.PointerWheelCommand;
        set
        {
            _childrenControl.PointerWheelCommand = value;
        }
    }
    public ActionCommand<Control, PointerEventArgs>? PointerExitedCommand
    {
        get => _childrenControl.PointerExitedCommand;
        set
        {
            _childrenControl.PointerExitedCommand = value;
        }
    }
    public ActionCommand<Control, PointerEventArgs>? PointerMovedCommand
    {
        get => _childrenControl.PointerMovedCommand;
        set
        {
            _childrenControl.PointerMovedCommand = value;
        }
    }

    public double Zoom => _baseControl.Zoom;
    public Point Offset => _baseControl.Offset;
    public Matrix ZoomMatrix => _baseControl.Matrix;
    public Matrix ContentMatrix => this.TransformToVisual(_childrenControl) ?? throw new Exception("Invalid matrix transformation");
    public Matrix GestureMatrix => this.TransformToVisual(_externalControl) ?? throw new Exception("Invalid gesture matrix transformation");
    public Matrix GestureToContentMatrix => GestureMatrix.Invert() * ContentMatrix;

    static ZoomControl()
    {
        AffectsMeasure<ZoomControl>(
                ZoomWidthProperty,
                ZoomHeightProperty);
    }

    public ZoomControl()
    {
        _externalControl = new ExternalControl();
        _baseControl = new BaseControl(this);
        _childrenControl = new ChildrenControl(this);

        _baseControl.Child = _childrenControl;
        _externalControl.Child = _baseControl;
        Child = _externalControl;
    }

    public void ConfigureOffsetBounds(int width, int height)
    {
        _baseControl.ConfigureOffsetBounds(width, height);
    }

    public void ZoomTo(double ratio, double x, double y, Matrix matrix, bool skipTransitions = false)
    {
        _baseControl.ZoomTo(ratio, x, y, matrix, skipTransitions);
    }
}
