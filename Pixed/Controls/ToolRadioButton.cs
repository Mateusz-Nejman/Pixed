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
    }
}
