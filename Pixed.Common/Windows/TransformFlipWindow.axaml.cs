using Avalonia.Controls;
using Pixed.Models;
using Pixed.Tools.Transform;

namespace Pixed.Windows;

internal partial class TransformFlipWindow : Window
{
    private readonly ApplicationData _applicationData;
    public TransformFlipWindow(ApplicationData applicationData)
    {
        InitializeComponent();
        _applicationData = applicationData;
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AbstractTransformTool transform = new Flip(_applicationData);
        transform.ApplyTransformation(applyToAllFrames.IsChecked == true, applyToAllLayers.IsChecked == true, flipVertically.IsChecked == true);
        Close(true);
    }
}