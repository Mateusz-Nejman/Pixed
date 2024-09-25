using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Threading;
using System;
using System.Reactive.Linq;

namespace Pixed.Controls;

internal class ToolRadioButton : RadioButton
{
    private bool _handled = false;

    [Content]
    public IImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly StyledProperty<IImage?> SourceProperty =
        AvaloniaProperty.Register<ToolRadioButton, IImage?>(nameof(Source), coerce: (o, img) =>
        {
            if (o is ToolRadioButton radio)
            {
                Image image = new()
                {
                    Source = img
                };
                radio.Content = image;
            }

            return img;
        });
    public ToolRadioButton()
    {
        GroupName = "Tool";
        this.Holding += ToolRadioButton_Holding;
        Tapped += ToolRadioButton_Tapped;
    }

    private void ToolRadioButton_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
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

    private void ToolRadioButton_Holding(object? sender, Avalonia.Input.HoldingRoutedEventArgs e)
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
