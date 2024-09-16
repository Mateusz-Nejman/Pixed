using Avalonia.Controls;
using Avalonia.Input;
using Pixed.Input;
using Pixed.Utils;
using Pixed.ViewModels;
using System.Drawing;

namespace Pixed.Controls;

public partial class PaintCanvas : UserControl
{
    private readonly PaintCanvasViewModel _viewModel;
    private readonly int _scrollBarSize = 18;

    internal PaintCanvasViewModel ViewModel => _viewModel;
    public PaintCanvas()
    {
        InitializeComponent();
        _viewModel = DataContext as PaintCanvasViewModel;
        _viewModel?.Initialize(image, imageGrid, overlay);
        SizeChanged += PaintCanvas_SizeChanged;
    }

    private void PaintCanvas_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        _viewModel.RecalculateFactor(new Point((int)(e.NewSize.Width - _scrollBarSize), (int)(e.NewSize.Height - _scrollBarSize)));
    }

    private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border)
        {
            var pos = e.GetPosition(border);
            MouseMapper mapper = new(e, border);

            if (mapper.ChangedButton == MouseButton.Left && mapper.ButtonState == MouseButtonState.Pressed)
            {
                _viewModel.LeftMouseDown?.Execute(pos.ToSystemPoint());
            }
            else if (mapper.ChangedButton == MouseButton.Right && mapper.ButtonState == MouseButtonState.Pressed)
            {
                _viewModel.RightMouseDown?.Execute(pos.ToSystemPoint());
            }
            else if (mapper.ChangedButton == MouseButton.Middle && mapper.ButtonState == MouseButtonState.Pressed)
            {
                _viewModel.MiddleMouseDown?.Execute(pos.ToSystemPoint());
            }
        }
    }

    private void Border_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Border border)
        {
            var pos = e.GetPosition(border);
            MouseMapper mapper = new(e, border);

            if (mapper.ChangedButton == MouseButton.Left && mapper.ButtonState == MouseButtonState.Released)
            {
                _viewModel.LeftMouseUp?.Execute(pos.ToSystemPoint());
            }
            else if (mapper.ChangedButton == MouseButton.Right && mapper.ButtonState == MouseButtonState.Released)
            {
                _viewModel.RightMouseUp?.Execute(pos.ToSystemPoint());
            }
            else if (mapper.ChangedButton == MouseButton.Middle && mapper.ButtonState == MouseButtonState.Released)
            {
                _viewModel.MiddleMouseUp?.Execute(pos.ToSystemPoint());
            }
        }
    }

    private void Border_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        _viewModel?.MouseWheel?.Execute(e.Delta.Y);
        e.Handled = true;
    }

    private void Border_PointerExited(object? sender, PointerEventArgs e)
    {
        _viewModel?.MouseLeave?.Execute();
    }

    private void Border_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is Border border)
        {
            var pos = e.GetPosition(border);

            _viewModel?.MouseMove?.Execute(pos.ToSystemPoint());
        }
    }
}