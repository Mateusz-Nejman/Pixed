using Avalonia.Input.GestureRecognizers;
using Avalonia.Input;

namespace Pixed.Controls.Gestures;
internal class HandledScrollGestureRecognizer : ScrollGestureRecognizer
{
    protected override void PointerMoved(PointerEventArgs e)
    {
        if(e.Handled)
        {
            return;
        }
        base.PointerMoved(e);
    }

    protected override void PointerPressed(PointerPressedEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }
        base.PointerPressed(e);
    }

    protected override void PointerReleased(PointerReleasedEventArgs e)
    {
        if(e.Handled)
        {
            return;
        }
        base.PointerReleased(e);
    }
}