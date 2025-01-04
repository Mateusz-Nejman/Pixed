using Avalonia.Interactivity;
using Pixed.Application.Models;
using Pixed.Application.ViewModels;
using Pixed.Core.Models;

namespace Pixed.Application.Pages;

internal partial class ResizeProject : Modal
{
    private readonly ResizeProjectWindowViewModel _viewModel;
    public ResizeProject()
    {
        InitializeComponent();
        var applicationData = Provider.Get<ApplicationData>();
        _viewModel = DataContext as ResizeProjectWindowViewModel;
        _viewModel.Width = applicationData.CurrentModel.Width;
        _viewModel.Height = applicationData.CurrentModel.Height;
        _viewModel.MaintainAspectRatio = applicationData.UserSettings.MaintainAspectRatio;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(new ResizeResult(_viewModel.Width, _viewModel.Height, _viewModel.ResizeCanvasContent, _viewModel.AnchorEnum, _viewModel.MaintainAspectRatio));
    }
}