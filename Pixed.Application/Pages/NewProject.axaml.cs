using Avalonia;
using Avalonia.Interactivity;
using Pixed.Application.Models;
using Pixed.Core.Models;

namespace Pixed.Application.Pages;

internal partial class NewProject : Modal
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

    public static readonly StyledProperty<int> WidthValueProperty = AvaloniaProperty.Register<NewProject, int>("WidthValue");
    public static readonly StyledProperty<int> HeightValueProperty = AvaloniaProperty.Register<NewProject, int>("HeightValue");
    public NewProject()
    {
        InitializeComponent();
        var applicationData = Provider.Get<ApplicationData>();

        if (applicationData != null)
        {
            WidthValue = applicationData.UserSettings.UserWidth;
            HeightValue = applicationData.UserSettings.UserHeight;
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(new NewProjectResult(WidthValue, HeightValue));
    }
}
