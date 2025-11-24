using Pixed.Common.Services;
using Pixed.Common.Tools.Transform;
using Pixed.Core.Models;

namespace Pixed.Application.Pages;

internal partial class TransformFlip : Modal
{
    private readonly ApplicationData? _applicationData;
    private readonly IHistoryService? _historyService;
    public TransformFlip()
    {
        InitializeComponent();
        _applicationData = Provider.Get<ApplicationData>();
        _historyService = Provider.Get<IHistoryService>();
    }

    private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_applicationData == null || _historyService == null)
        {
            return;
        }
        
        AbstractTransformTool transform = new Flip(_applicationData, _historyService);
        await transform.ApplyTransformation(applyToAllFrames.IsChecked == true, applyToAllLayers.IsChecked == true, flipVertically.IsChecked == true);
        await Close(true);
    }
}