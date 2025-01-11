using Pixed.Common.Tools.Transform;
using Pixed.Core.Models;

namespace Pixed.Application.Pages;

internal partial class TransformAlign : Modal
{
    private readonly ApplicationData _applicationData;
    public TransformAlign()
    {
        InitializeComponent();
        _applicationData = Provider.Get<ApplicationData>();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AbstractTransformTool transform = new Center(_applicationData);
        transform.ApplyTransformation(applyToAllFrames.IsChecked == true, applyToAllLayers.IsChecked == true, false);
        Close(true);
    }
}