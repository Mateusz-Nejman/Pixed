using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Pixed.Models;


namespace Pixed.Windows;

internal partial class NewProjectWindow : Window
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

    public static readonly StyledProperty<int> WidthValueProperty = AvaloniaProperty.Register<NewProjectWindow, int>("WidthValue");
    public static readonly StyledProperty<int> HeightValueProperty = AvaloniaProperty.Register<NewProjectWindow, int>("HeightValue");
    public NewProjectWindow(ApplicationData applicationData)
    {
        InitializeComponent();
        WidthValue = applicationData.UserSettings.UserWidth;
        HeightValue = applicationData.UserSettings.UserHeight;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(true);
    }
}
