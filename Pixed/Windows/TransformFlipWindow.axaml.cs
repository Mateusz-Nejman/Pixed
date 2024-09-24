using Avalonia.Controls;
using Pixed.Tools.Transform;

namespace Pixed.Windows;

public partial class TransformFlipWindow : Window
{
    public TransformFlipWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AbstractTransformTool transform = new Flip();
        transform.ApplyTransformation(applyToAllFrames.IsChecked == true, applyToAllLayers.IsChecked == true, flipVertically.IsChecked == true);
        Close(true);
    }
}