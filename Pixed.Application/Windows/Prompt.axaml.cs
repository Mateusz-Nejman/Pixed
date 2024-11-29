using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;


namespace Pixed.Application.Windows;

internal abstract partial class Prompt : PixedWindow
{
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string DefaultValue
    {
        get => GetValue(DefaultValueProperty);
        set { SetValue(DefaultValueProperty, value); }
    }

    public string Value { get; private set; } = string.Empty;

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<Prompt, string>("Text", string.Empty);
    public static readonly StyledProperty<string> DefaultValueProperty = AvaloniaProperty.Register<Prompt, string>("DefaultValue", string.Empty);
    public Prompt()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Value = textBox.Text;
        Close(true);
    }
}
