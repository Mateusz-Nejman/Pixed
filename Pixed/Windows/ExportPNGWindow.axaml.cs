using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Pixed.Windows;

public partial class ExportPNGWindow : Window
{
    public int ColumnsCount
    {
        get => GetValue(ColumnsCountProperty);
        set => SetValue(ColumnsCountProperty, value);
    }

    public static readonly StyledProperty<int> ColumnsCountProperty = AvaloniaProperty.Register<ExportPNGWindow, int>("ColumnsCount", Global.CurrentModel.Frames.Count);
    public ExportPNGWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(true);
    }
}