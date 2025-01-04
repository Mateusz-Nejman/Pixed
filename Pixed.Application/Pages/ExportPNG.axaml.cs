using Avalonia;
using Avalonia.Interactivity;
using Pixed.Core.Models;

namespace Pixed.Application.Pages;

internal partial class ExportPNG : Modal
{
    public int ColumnsCount
    {
        get => GetValue(ColumnsCountProperty);
        set => SetValue(ColumnsCountProperty, value);
    }

    public static readonly StyledProperty<int> ColumnsCountProperty = AvaloniaProperty.Register<ExportPNG, int>("ColumnsCount", 1);
    public ExportPNG()
    {
        InitializeComponent();
        var applicationData = Provider.Get<ApplicationData>();
        ColumnsCount = applicationData.CurrentModel.Frames.Count;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(ColumnsCount);
    }
}