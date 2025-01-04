using Pixed.Common.Tools.Transform;
using Pixed.Core.Models;

namespace Pixed.Application.Pages;

internal partial class TransformRotate : Modal
{
    private readonly ApplicationData _applicationData;
    public TransformRotate()
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