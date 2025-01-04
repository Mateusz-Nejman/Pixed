using Pixed.Common.Tools.Transform;
using Pixed.Core.Models;

namespace Pixed.Application.Pages;

internal partial class TransformFlip : Modal
{
    private readonly ApplicationData _applicationData;
    public TransformFlip()
    {
        InitializeComponent();
        _applicationData = Provider.Get<ApplicationData>();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AbstractTransformTool transform = new Flip(_applicationData);
        transform.ApplyTransformation(applyToAllFrames.IsChecked == true, applyToAllLayers.IsChecked == true, flipVertically.IsChecked == true);
        Close(true);
    }
}