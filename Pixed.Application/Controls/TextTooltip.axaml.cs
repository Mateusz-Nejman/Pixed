using Avalonia;

namespace Pixed.Application.Controls;

internal partial class TextTooltip : EmptyExtendedControl
{
    public string Text
    {
        get { return GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<TextTooltip, string>(nameof(Text), "Text");
    public TextTooltip() : base()
    {
        InitializeComponent();
    }
}
