using Avalonia;
using Avalonia.Interactivity;
using Pixed.Application.Models;
using Pixed.Core.Models;

namespace Pixed.Application.Windows;

internal partial class NewProjectWindow : PixedWindow
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
    public NewProjectWindow()
    {
        InitializeComponent();
        var applicationData = Provider.Get<ApplicationData>();
        WidthValue = applicationData.UserSettings.UserWidth;
        HeightValue = applicationData.UserSettings.UserHeight;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(new NewProjectResult(WidthValue, HeightValue));
    }
}
