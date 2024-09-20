using Avalonia.Controls;
using Avalonia.Interactivity;
using Pixed.Models;
using Pixed.Utils;
using Pixed.ViewModels;

namespace Pixed.Windows;

internal partial class ResizeProjectWindow : Window
{
    public readonly struct ResizeResult(int width, int height, bool resizeCanvasContent, ResizeUtils.Origin anchor)
    {
        public int Width { get; } = width;
        public int Height { get; } = height;
        public bool ResizeCanvasContent { get; } = resizeCanvasContent;
        public ResizeUtils.Origin Anchor { get; } = anchor;
    }
    private readonly ResizeProjectWindowViewModel _viewModel;

    public ResizeResult Result { get; private set; }
    public ResizeProjectWindow(PixedModel model)
    {
        InitializeComponent();
        _viewModel = DataContext as ResizeProjectWindowViewModel;
        _viewModel.Width = model.Width;
        _viewModel.Height = model.Height;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Result = new ResizeResult(_viewModel.Width, _viewModel.Height, _viewModel.ResizeCanvasContent, _viewModel.AnchorEnum);
        Close(true);
    }
}