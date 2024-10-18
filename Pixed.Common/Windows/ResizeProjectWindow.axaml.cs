using Avalonia.Controls;
using Avalonia.Interactivity;
using Pixed.Models;
using Pixed.Utils;
using Pixed.ViewModels;

namespace Pixed.Windows;

internal partial class ResizeProjectWindow : Window
{
    public readonly struct ResizeResult(int width, int height, bool resizeCanvasContent, ResizeUtils.Origin anchor, bool maintainAspectRatio)
    {
        public int Width { get; } = width;
        public int Height { get; } = height;
        public bool ResizeCanvasContent { get; } = resizeCanvasContent;
        public ResizeUtils.Origin Anchor { get; } = anchor;
        public bool MaintainAspectRatio { get; } = maintainAspectRatio;
    }
    private readonly ResizeProjectWindowViewModel _viewModel;
    private readonly ApplicationData _applicationData;

    public ResizeResult Result { get; private set; }
    public ResizeProjectWindow(ApplicationData applicationData)
    {
        InitializeComponent();
        _applicationData = applicationData;
        _viewModel = DataContext as ResizeProjectWindowViewModel;
        _viewModel.Width = applicationData.CurrentModel.Width;
        _viewModel.Height = applicationData.CurrentModel.Height;
        _viewModel.MaintainAspectRatio = applicationData.UserSettings.MaintainAspectRatio;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Result = new ResizeResult(_viewModel.Width, _viewModel.Height, _viewModel.ResizeCanvasContent, _viewModel.AnchorEnum, _viewModel.MaintainAspectRatio);
        Close(true);
    }
}