using Avalonia.Interactivity;
using Pixed.Application.Models;
using Pixed.Application.ViewModels;
using Pixed.Core.Models;

namespace Pixed.Application.Pages;

internal partial class ResizeProject : Modal
{
    private readonly ResizeProjectViewModel? _viewModel;
    public ResizeProject()
    {
        InitializeComponent();
        var applicationData = Provider.Get<ApplicationData>();
        _viewModel = DataContext as ResizeProjectViewModel;
        if (_viewModel != null && applicationData != null)
        {
            _viewModel.Width = applicationData.CurrentModel.Width;
            _viewModel.Height = applicationData.CurrentModel.Height;
            _viewModel.MaintainAspectRatio = applicationData.UserSettings.MaintainAspectRatio;
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel == null)
        {
            return;
        }
        
        Close(new ResizeResult(_viewModel.Width, _viewModel.Height, _viewModel.ResizeCanvasContent, _viewModel.AnchorEnum, _viewModel.MaintainAspectRatio));
    }
}