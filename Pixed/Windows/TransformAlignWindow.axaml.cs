using Avalonia.Controls;
using Pixed.Tools.Transform;

namespace Pixed.Windows;

public partial class TransformAlignWindow : Window
{
    public TransformAlignWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AbstractTransformTool transform = new Center();
        transform.ApplyTransformation(applyToAllFrames.IsChecked == true, applyToAllLayers.IsChecked == true, false);
        Close(true);
    }
}