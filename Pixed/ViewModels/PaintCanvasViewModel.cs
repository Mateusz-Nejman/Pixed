using Pixed.Utils;
using System.Drawing;
using Frame = Pixed.Models.Frame;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace Pixed.ViewModels
{
    internal class PaintCanvasViewModel : PropertyChangedBase
    {
        private readonly double _gridWidth = 128;
        private readonly double _gridHeight = 128;
        private Point _imageOffset;
        private double _imageFactor;
        private Image? _image;
        private Image? _overlayImage;
        private Bitmap _overlayBitmap;
        private bool _leftPressed;
        private bool _rightPressed;
        private readonly IDisposable _refreshDisposable;
        private Frame _frame;
        private Grid _grid;
        private Point _lastWindowSize;

        public ActionCommand<Point> LeftMouseDown { get; set; }
        public ActionCommand<Point> LeftMouseUp { get; set; }
        public ActionCommand<Point> MouseMove { get; set; }
        public ActionCommand<Point> RightMouseDown { get; set; }
        public ActionCommand<Point> RightMouseUp { get; set; }
        public ActionCommand<Point> MiddleMouseDown { get; set; }
        public ActionCommand<Point> MiddleMouseUp { get; set; }
        public ActionCommand<int> MouseWheel { get; set; }
        public ActionCommand MouseLeave { get; set; }
        public double Zoom { get; set; } = 1.0;
        public Frame CurrentFrame
        {
            get => _frame;
            set
            {
                _frame = value;
                OnPropertyChanged();
                Subjects.RefreshCanvas.OnNext(true);
            }
        }

        public Bitmap Overlay
        {
            get => _overlayBitmap;
            set
            {
                _overlayBitmap = value;
                OnPropertyChanged();
                RefreshOverlay();
            }
        }

        public PaintCanvasViewModel()
        {
            _refreshDisposable = Subjects.RefreshCanvas.Subscribe(_ =>
            {
                if (_image != null)
                {
                    _image.Source = _frame.RenderTransparent().ToAvaloniaBitmap();
                    _frame.RefreshRenderSource();
                }
            });

            _frame = new Frame(32, 32);
            Subjects.RefreshCanvas.OnNext(true);
            LeftMouseDown = new ActionCommand<Point>(LeftMouseDownAction);
            LeftMouseUp = new ActionCommand<Point>(LeftMouseUpAction);
            RightMouseDown = new ActionCommand<Point>(RightMouseDownAction);
            RightMouseUp = new ActionCommand<Point>(RightMouseUpAction);
            MouseMove = new ActionCommand<Point>(MouseMoveAction);
            MouseWheel = new ActionCommand<int>(MouseWheelAction);
            MouseLeave = new ActionCommand(MouseLeaveAction);

            Subjects.FrameChanged.Subscribe(f =>
            {
                CurrentFrame = Global.CurrentModel.Frames[f];
                RecalculateFactor(_lastWindowSize);
            });
        }
        public void RecalculateFactor(Point windowSize)
        {
            var factor = Math.Min(windowSize.X, windowSize.Y) / Math.Min(_frame.Width, _frame.Height);
            _imageFactor = factor;
            _grid.Width = _frame.Width * factor;
            _grid.Height = _frame.Height * factor;
            _lastWindowSize = windowSize;
        }
        public void Initialize(Image image, Grid grid, Image overlay)
        {
            _image = image;
            _grid = grid;
            _overlayImage = overlay;
        }

        public void ResetOverlay()
        {
            Overlay?.Clear();
            RefreshOverlay();
        }

        private void LeftMouseDownAction(Point point)
        {
            int imageX = (int)(point.X / _imageFactor);
            int imageY = (int)(point.Y / _imageFactor);

            if (!_frame.ContainsPixel(imageX, imageY))
            {
                return;
            }

            _leftPressed = true;
            Global.ToolSelected?.ApplyTool(imageX, imageY, _frame, ref _overlayBitmap);
            RefreshOverlay();
        }

        private void LeftMouseUpAction(Point point)
        {
            int imageX = (int)(point.X / _imageFactor);
            int imageY = (int)(point.Y / _imageFactor);

            if (!_frame.ContainsPixel(imageX, imageY))
            {
                return;
            }

            _leftPressed = false;
            Global.ToolSelected?.ReleaseTool(imageX, imageY, _frame, ref _overlayBitmap);
            RefreshOverlay();
        }

        private void RightMouseDownAction(Point point)
        {
            int imageX = (int)(point.X / _imageFactor);
            int imageY = (int)(point.Y / _imageFactor);

            if (!_frame.ContainsPixel(imageX, imageY))
            {
                return;
            }

            _rightPressed = true;
            Global.ToolSelected?.ApplyTool(imageX, imageY, _frame, ref _overlayBitmap);
            RefreshOverlay();
        }

        private void RightMouseUpAction(Point point)
        {
            int imageX = (int)(point.X / _imageFactor);
            int imageY = (int)(point.Y / _imageFactor);

            if (!_frame.ContainsPixel(imageX, imageY))
            {
                return;
            }

            _rightPressed = false;
            Global.ToolSelected?.ReleaseTool(imageX, imageY, _frame, ref _overlayBitmap);
            RefreshOverlay();
        }

        private void MouseMoveAction(Point point)
        {
            int imageX = (int)(point.X / _imageFactor);
            int imageY = (int)(point.Y / _imageFactor);
            if (_leftPressed || _rightPressed)
            {
                Global.ToolSelected?.MoveTool(imageX, imageY, _frame, ref _overlayBitmap);
                RefreshOverlay();
                Subjects.RefreshCanvas.OnNext(true);
            }
            else
            {
                Global.ToolSelected?.UpdateHighlightedPixel(imageX, imageY, _frame, ref _overlayBitmap);
                RefreshOverlay();
            }
        }

        private void MouseWheelAction(int delta)
        {
            double multiplier = delta / 120;
            var step = multiplier * Math.Max(0.1, Math.Abs(_imageFactor) / 15);
            _imageFactor = Math.Max(0.1, _imageFactor + step);
            _grid.Width = _frame.Width * _imageFactor;
            _grid.Height = _frame.Height * _imageFactor;
        }

        private void MouseLeaveAction()
        {
            if (_rightPressed || _leftPressed)
            {
                Global.ToolSelected?.ReleaseTool(0, 0, _frame, ref _overlayBitmap);
            }
            _rightPressed = false;
            _leftPressed = false;
        }

        private void RefreshOverlay()
        {
            if (_overlayImage != null && _overlayBitmap != null)
            {
                _overlayImage.Source = _overlayBitmap.ToAvaloniaBitmap();
            }
        }
    }
}
