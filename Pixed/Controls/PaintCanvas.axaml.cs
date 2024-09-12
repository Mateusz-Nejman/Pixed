using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Pixed.Input;
using Pixed.ViewModels;
using System;
using System.Drawing;

namespace Pixed.Controls
{
    /// <summary>
    /// Interaction logic for PaintCanvas.xaml
    /// </summary>
    public partial class PaintCanvas : UserControl
    {
        private readonly PaintCanvasViewModel _viewModel;
        private readonly int _scrollBarSize = 18;

        internal PaintCanvasViewModel ViewModel => _viewModel;
        public PaintCanvas()
        {
            InitializeComponent();
            _viewModel = (PaintCanvasViewModel)DataContext;
            _viewModel.Initialize(image, imageGrid, overlay);
            SizeChanged += PaintCanvas_SizeChanged;
            Loaded += PaintCanvas_Loaded;
        }

        private void PaintCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.RecalculateFactor(new Point((int)(Width - _scrollBarSize), (int)(Height - _scrollBarSize)));
        }

        private void PaintCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _viewModel.RecalculateFactor(new Point((int)(e.NewSize.Width - _scrollBarSize), (int)(e.NewSize.Height - _scrollBarSize)));
        }

        private void Frame_MouseDown(object sender, PointerPressedEventArgs e)
        {
            Panel frame = (Panel)sender;
            var pos = e.GetPosition(frame);
            var currentPoint = e.GetCurrentPoint(sender as Control);

            if (currentPoint.Properties.IsLeftButtonPressed)
            {
                _viewModel.LeftMouseDown?.Execute(pos);
            }
            else if (currentPoint.Properties.IsRightButtonPressed)
            {
                _viewModel.RightMouseDown?.Execute(pos);
            }
            else if (currentPoint.Properties.IsMiddleButtonPressed)
            {
                _viewModel.MiddleMouseDown?.Execute(pos);
            }
        }

        private void Frame_MouseMove(object sender, PointerEventArgs e)
        {
            Panel frame = (Panel)sender;
            var pos = e.GetPosition(frame);

            _viewModel.MouseMove?.Execute(pos);
        }

        private void Frame_MouseUp(object sender, PointerReleasedEventArgs e)
        {
            Panel frame = (Panel)sender;
            var pos = e.GetPosition(frame);
            MouseMapper mapper = new MouseMapper(e, sender as Control);

            if (mapper.ChangedButton == MouseButton.Left && mapper.ButtonState == MouseButtonState.Released)
            {
                _viewModel.LeftMouseUp?.Execute(pos);
            }
            else if (mapper.ChangedButton == MouseButton.Right && mapper.ButtonState == MouseButtonState.Released)
            {
                _viewModel.RightMouseUp?.Execute(pos);
            }
            else if (mapper.ChangedButton == MouseButton.Middle && mapper.ButtonState == MouseButtonState.Released)
            {
                _viewModel.MiddleMouseUp?.Execute(pos);
            }
        }

        private void Frame_MouseWheel(object sender, PointerWheelEventArgs e)
        {
            _viewModel?.MouseWheel?.Execute(e.Delta);
            e.Handled = true;
        }

        private void Frame_MouseLeave(object sender, PointerEventArgs e)
        {
            _viewModel.MouseLeave?.Execute();
        }
    }
}
