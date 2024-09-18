using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;


namespace Pixed.Windows;

public partial class NewProjectWindow : Window
{
    public int WidthValue
    {
        get => GetValue(WidthValueProperty);
        set => SetValue(WidthValueProperty, value);
    }

    public int HeightValue
    {
        get => GetValue(HeightValueProperty);
        set => SetValue(HeightValueProperty, value);
    }

    public static readonly StyledProperty<int> WidthValueProperty = AvaloniaProperty.Register<Prompt, int>("WidthValue", Global.UserSettings.UserWidth);
    public static readonly StyledProperty<int> HeightValueProperty = AvaloniaProperty.Register<Prompt, int>("HeightValue", Global.UserSettings.UserHeight);
    public NewProjectWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(true);
    }
}
