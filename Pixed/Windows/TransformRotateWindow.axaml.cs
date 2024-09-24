using Avalonia.Controls;
using Pixed.Tools.Transform;

namespace Pixed.Windows;

public partial class TransformRotateWindow : Window
{
    public TransformRotateWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AbstractTransformTool transform = new Rotate();
        transform.ApplyTransformation(applyToAllFrames.IsChecked == true, applyToAllLayers.IsChecked == true, counterClockwiseRotation.IsChecked == true);
        Close(true);
    }
}