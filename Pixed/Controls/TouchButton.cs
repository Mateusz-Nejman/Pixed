using Avalonia.Controls;

namespace Pixed.Controls;
internal class TouchButton : Button
{
    public TouchButton()
    {
        this.Holding += TouchButton_Holding;
        this.PointerExited += TouchButton_PointerExited;
    }

    private void TouchButton_PointerExited(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        if (sender is Control control)
        {
            if (ToolTip.GetTip(control) != null)
            {
                ToolTip.SetIsOpen(control, false);
            }
        }
    }

    private void TouchButton_Holding(object? sender, Avalonia.Input.HoldingRoutedEventArgs e)
    {
        if (sender is Control control)
        {
            if (ToolTip.GetTip(control) != null)
            {
                ToolTip.SetIsOpen(control, true);
            }
        }
    }
}
