using Avalonia.Controls;
using Pixed.Common.Tools.Transform;
using Pixed.Core.Models;

namespace Pixed.Application.Windows;

internal partial class TransformFlipWindow : PixedWindow
{
    private readonly ApplicationData _applicationData;
    public TransformFlipWindow()
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