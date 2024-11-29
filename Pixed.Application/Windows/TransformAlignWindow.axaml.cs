using Avalonia.Controls;
using Pixed.Common.Tools.Transform;
using Pixed.Core.Models;

namespace Pixed.Application.Windows;

internal partial class TransformAlignWindow : PixedWindow
{
    private readonly ApplicationData _applicationData;
    public TransformAlignWindow()
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