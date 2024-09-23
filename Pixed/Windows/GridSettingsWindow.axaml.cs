using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Pixed.Windows;

public partial class GridSettingsWindow : Window
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

    public static readonly StyledProperty<int> WidthValueProperty = AvaloniaProperty.Register<GridSettingsWindow, int>("WidthValue", Global.UserSettings.GridWidth);
    public static readonly StyledProperty<int> HeightValueProperty = AvaloniaProperty.Register<GridSettingsWindow, int>("HeightValue", Global.UserSettings.GridHeight);
    public static readonly StyledProperty<Color> GridColorProperty = AvaloniaProperty.Register<GridSettingsWindow, Color>("GridColor", Global.UserSettings.GridColor);
    public GridSettingsWindow()
    {
        InitializeComponent();
        GridColor = Global.UserSettings.GridColor;
        WidthValue = Global.UserSettings.GridWidth;
        HeightValue = Global.UserSettings.GridHeight;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(true);
    }
}