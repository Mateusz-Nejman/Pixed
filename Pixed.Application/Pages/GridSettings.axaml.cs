using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Media;
using Pixed.Application.Models;
using Pixed.Core.Models;

namespace Pixed.Application.Pages;

internal partial class GridSettings : Modal
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

    public static readonly StyledProperty<int> WidthValueProperty = AvaloniaProperty.Register<GridSettings, int>("WidthValue");
    public static readonly StyledProperty<int> HeightValueProperty = AvaloniaProperty.Register<GridSettings, int>("HeightValue");
    public static readonly StyledProperty<Color> GridColorProperty = AvaloniaProperty.Register<GridSettings, Color>("GridColor");
    public GridSettings()
    {
        InitializeComponent();
        var applicationData = Provider.Get<ApplicationData>();

        if (applicationData == null) return;
        GridColor = applicationData.UserSettings.GridColor;
        WidthValue = applicationData.UserSettings.GridWidth;
        HeightValue = applicationData.UserSettings.GridHeight;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(new GridSettingsResult(WidthValue, HeightValue, GridColor));
    }
}