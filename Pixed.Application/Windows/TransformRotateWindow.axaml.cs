using Avalonia.Controls;
using Pixed.Common.Tools.Transform;
using Pixed.Core.Models;

namespace Pixed.Application.Windows;

internal partial class TransformRotateWindow : PixedWindow
{
    private readonly ApplicationData _applicationData;
    public TransformRotateWindow()
    {
        InitializeComponent();
        _applicationData = Provider.Get<ApplicationData>();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AbstractTransformTool transform = new Rotate(_applicationData);
        transform.ApplyTransformation(applyToAllFrames.IsChecked == true, applyToAllLayers.IsChecked == true, counterClockwiseRotation.IsChecked == true);
        Close(true);
    }
}