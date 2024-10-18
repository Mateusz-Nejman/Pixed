using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Pixed.Models;

namespace Pixed.Windows;

internal partial class ExportPNGWindow : Window
{
    public int ColumnsCount
    {
        get => GetValue(ColumnsCountProperty);
        set => SetValue(ColumnsCountProperty, value);
    }

    public static readonly StyledProperty<int> ColumnsCountProperty = AvaloniaProperty.Register<ExportPNGWindow, int>("ColumnsCount", 1);
    public ExportPNGWindow(ApplicationData applicationData)
    {
        InitializeComponent();
        ColumnsCount = applicationData.CurrentModel.Frames.Count;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(true);
    }
}