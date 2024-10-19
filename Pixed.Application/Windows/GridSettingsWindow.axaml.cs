using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Pixed.Common.Models;

namespace Pixed.Application.Windows;

internal partial class GridSettingsWindow : Window
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

    public Color GridColor
    {
        get => GetValue(GridColorProperty);
        set => SetValue(GridColorProperty, value);
    }

    public static readonly StyledProperty<int> WidthValueProperty = AvaloniaProperty.Register<GridSettingsWindow, int>("WidthValue");
    public static readonly StyledProperty<int> HeightValueProperty = AvaloniaProperty.Register<GridSettingsWindow, int>("HeightValue");
    public static readonly StyledProperty<Color> GridColorProperty = AvaloniaProperty.Register<GridSettingsWindow, Color>("GridColor");
    public GridSettingsWindow(ApplicationData applicationData)
    {
        InitializeComponent();
        GridColor = applicationData.UserSettings.GridColor;
        WidthValue = applicationData.UserSettings.GridWidth;
        HeightValue = applicationData.UserSettings.GridHeight;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(true);
    }
}