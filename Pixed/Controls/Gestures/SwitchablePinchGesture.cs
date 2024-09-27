using Avalonia;
using Avalonia.Input;

namespace Pixed.Controls.Gestures;
internal class SwitchablePinchGesture : PinchGestureRecognizer
{
    public bool IsEnabled
    {
        get { return GetValue(IsEnabledProperty); }
        set { SetValue(IsEnabledProperty, value); }
    }

    public static readonly StyledProperty<bool> IsEnabledProperty = AvaloniaProperty.Register<SwitchablePinchGesture, bool>("IsEnabled", false);

    protected override void PointerCaptureLost(IPointer pointer)
    {
        if (!IsEnabled) return;
        base.PointerCaptureLost(pointer);
    }

    protected override void PointerMoved(PointerEventArgs e)
    {
        if (!IsEnabled) return;
        base.PointerMoved(e);
    }
    protected override void PointerPressed(PointerPressedEventArgs e)
    {
        if (!IsEnabled) return;
        base.PointerPressed(e);
    }

    protected override void PointerReleased(PointerReleasedEventArgs e)
    {
        if (!IsEnabled) return;
        base.PointerReleased(e);
    }
}
