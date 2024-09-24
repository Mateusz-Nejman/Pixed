using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;

namespace Pixed.Controls;

internal class ToolRadioButton : RadioButton
{
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
        this.PointerExited += ToolRadioButton_PointerExited;
    }

    private void ToolRadioButton_PointerExited(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        if (sender is Control control)
        {
            if (ToolTip.GetTip(control) != null)
            {
                ToolTip.SetIsOpen(control, false);
            }
        }
    }

    private void ToolRadioButton_Holding(object? sender, Avalonia.Input.HoldingRoutedEventArgs e)
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
