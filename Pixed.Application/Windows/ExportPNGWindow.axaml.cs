using Avalonia;
using Avalonia.Interactivity;
using Pixed.Core.Models;

namespace Pixed.Application.Windows;

internal partial class ExportPNGWindow : PixedWindow
{
    public int ColumnsCount
    {
        get => GetValue(ColumnsCountProperty);
        set => SetValue(ColumnsCountProperty, value);
    }

    public static readonly StyledProperty<int> ColumnsCountProperty = AvaloniaProperty.Register<ExportPNGWindow, int>("ColumnsCount", 1);
    public ExportPNGWindow()
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