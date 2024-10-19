using Avalonia.Controls;
using Pixed.Common.Models;
using Pixed.Common.Tools.Transform;

namespace Pixed.Application.Windows;

internal partial class TransformAlignWindow : Window
{
    private readonly ApplicationData _applicationData;
    public TransformAlignWindow(ApplicationData applicationData)
    {
        InitializeComponent();
        _applicationData = applicationData;
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AbstractTransformTool transform = new Center(_applicationData);
        transform.ApplyTransformation(applyToAllFrames.IsChecked == true, applyToAllLayers.IsChecked == true, false);
        Close(true);
    }
}