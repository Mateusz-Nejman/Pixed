using Avalonia.Controls;
using Pixed.ViewModels;

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

        private void PaintCanvas_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel.RecalculateFactor(new System.Windows.Point(ActualWidth - _scrollBarSize, ActualHeight - _scrollBarSize));
        }

        private void PaintCanvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            _viewModel.RecalculateFactor(new System.Windows.Point(e.NewSize.Width - _scrollBarSize, e.NewSize.Height - _scrollBarSize));
        }

        private void Frame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Frame frame = (Frame)sender;
            var pos = e.GetPosition(frame);

            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            {
                _viewModel.LeftMouseDown?.Execute(pos);
            }
            else if (e.ChangedButton == MouseButton.Right && e.ButtonState == MouseButtonState.Pressed)
            {
                _viewModel.RightMouseDown?.Execute(pos);
            }
            else if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
            {
                _viewModel.MiddleMouseDown?.Execute(pos);
            }
        }

        private void Frame_MouseMove(object sender, MouseEventArgs e)
        {
            Frame frame = (Frame)sender;
            var pos = e.GetPosition(frame);

            _viewModel.MouseMove?.Execute(pos);
        }

        private void Frame_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Frame frame = (Frame)sender;
            var pos = e.GetPosition(frame);

            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Released)
            {
                _viewModel.LeftMouseUp?.Execute(pos);
            }
            else if (e.ChangedButton == MouseButton.Right && e.ButtonState == MouseButtonState.Released)
            {
                _viewModel.RightMouseUp?.Execute(pos);
            }
            else if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Released)
            {
                _viewModel.MiddleMouseUp?.Execute(pos);
            }
        }

        private void Frame_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _viewModel?.MouseWheel?.Execute(e.Delta);
            e.Handled = true;
        }

        private void Frame_MouseLeave(object sender, MouseEventArgs e)
        {
            _viewModel.MouseLeave?.Execute();
        }
    }
}
