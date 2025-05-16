using Pixed.Common.Services;
using Pixed.Common.Tools.Transform;
using Pixed.Core.Models;

namespace Pixed.Application.Pages;

internal partial class TransformRotate : Modal
{
    private readonly ApplicationData _applicationData;
    private readonly IHistoryService _historyService;
    public TransformRotate()
    {
        InitializeComponent();
        _applicationData = Provider.Get<ApplicationData>();
        _historyService = Provider.Get<IHistoryService>();
    }

    private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AbstractTransformTool transform = new Rotate(_applicationData, _historyService);
        await transform.ApplyTransformation(applyToAllFrames.IsChecked == true, applyToAllLayers.IsChecked == true, counterClockwiseRotation.IsChecked == true);
        await Close(true);
    }
}