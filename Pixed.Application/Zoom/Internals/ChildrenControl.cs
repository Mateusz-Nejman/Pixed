using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Pixed.Application.Controls.PaintCanvas;
using Pixed.Core;
using System.Collections.Specialized;

namespace Pixed.Application.Zoom.Internals;

internal class ChildrenControl : Canvas
{
    private readonly ZoomControl _parent;
    public ActionCommand<Control, PointerPressedEventArgs>? PointerPressedCommand { get; set; }
    public ActionCommand<Control, PointerReleasedEventArgs>? PointerReleasedCommand { get; set; }
    public ActionCommand<Control, PointerWheelEventArgs>? PointerWheelCommand { get; set; }
    public ActionCommand<Control, PointerEventArgs>? PointerExitedCommand { get; set; }
    public ActionCommand<Control, PointerEventArgs>? PointerMovedCommand { get; set; }
    public ChildrenControl(ZoomControl parent) : base()
    {
        _parent = parent;
        PointerPressed += OnPointerPressed;
        PointerReleased += OnPointerReleased;
        PointerWheelChanged += OnPointerWheelChanged;
        PointerExited += OnPointerExited;
        PointerMoved += OnPointerMoved;
        RenderOptions.SetBitmapInterpolationMode(this, Avalonia.Media.Imaging.BitmapInterpolationMode.None);
    }

    protected override void ChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var child in Children)
        {
            if (child is OverlayControl control)
            {
                control.AttachToZoomControl(_parent);
            }
        }

        base.ChildrenChanged(sender, e);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        PointerMovedCommand?.Execute(this, e);
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        PointerExitedCommand?.Execute(this, e);
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        PointerWheelCommand?.Execute(this, e);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        PointerReleasedCommand?.Execute(this, e);
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        PointerPressedCommand?.Execute(this, e);
    }
}