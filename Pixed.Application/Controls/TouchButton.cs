using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Reactive.Linq;

namespace Pixed.Application.Controls;
internal class TouchButton : Button
{
    private bool _handled = false;
    public TouchButton()
    {
        this.Holding += TouchButton_Holding;
        Tapped += TouchButton_Tapped;
    }

    private void TouchButton_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (_handled)
        {
            _handled = false;
            return;
        }

        base.OnClick();
    }

    protected override void OnClick()
    {

    }

    private void TouchButton_Holding(object? sender, Avalonia.Input.HoldingRoutedEventArgs e)
    {
        if (sender is Control control)
        {
            if (ToolTip.GetTip(control) != null)
            {
                _handled = true;
                ToolTip.SetIsOpen(control, _handled);
                Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(x =>
                {
                    _handled = false;
                    Dispatcher.UIThread.Invoke(() => ToolTip.SetIsOpen(control, false));
                });
            }
        }
    }
}
