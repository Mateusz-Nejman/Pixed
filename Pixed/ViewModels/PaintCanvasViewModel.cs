using Pixed.Models;
using Pixed.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;
using Frame = Pixed.Models.Frame;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace Pixed.ViewModels
{
    internal class PaintCanvasViewModel : PropertyChangedBase
    {
        private double _gridWidth = 128;
        private double _gridHeight = 128;
        private Point _imageOffset;
        private double _imageFactor;
        private Image? _image;
        private bool _leftPressed;
        private bool _rightPressed;
        private IDisposable _refreshDisposable;
        private Frame _frame;
        private Grid _grid;
        private ScrollViewer _scrollViewer;

        public double GridWidth
        {
            get => _gridWidth;
            set
            {
                _gridWidth = value;
                OnPropertyChanged();
            }
        }

        public double GridHeight
        {
            get => _gridHeight;
            set
            {
                _gridHeight = value;
                OnPropertyChanged();
            }
        }

        public ActionCommand<Point> LeftMouseDown { get; set; }
        public ActionCommand<Point> LeftMouseUp { get; set; }
        public ActionCommand<Point> MouseMove { get; set; }
        public ActionCommand<Point> RightMouseDown { get; set; }
        public ActionCommand<Point> RightMouseUp { get; set; }
        public ActionCommand<Point> MiddleMouseDown { get; set; }
        public ActionCommand<Point> MiddleMouseUp { get; set; }
        public ActionCommand<int> MouseWheel { get; set; }
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

        public PaintCanvasViewModel()
        {
            _refreshDisposable = Subjects.RefreshCanvas.Subscribe(_ =>
            {
                if(_image != null)
                {
                    _image.Source = _frame.Render().ToBitmapImage();
                }
            });

            _frame = new Frame(32, 32);
            Subjects.RefreshCanvas.OnNext(true);
            LeftMouseDown = new ActionCommand<Point>(LeftMouseDownAction);
            LeftMouseUp = new ActionCommand<Point>(LeftMouseUpAction);
            MouseMove = new ActionCommand<Point>(MouseMoveAction);
            MouseWheel = new ActionCommand<int>(MouseWheelAction);
        }
        public void RecalculateFactor(Point windowSize)
        {
            var factor = Math.Min(windowSize.X, windowSize.Y) / Math.Min(_frame.Width, _frame.Height);
            _imageFactor = factor;
            _grid.Width = _frame.Width * factor;
            _grid.Height = _frame.Height * factor;
        }
        public void Initialize(Image image, Grid grid, ScrollViewer scrollViewer)
        {
            _image = image;
            _grid = grid;
            _scrollViewer = scrollViewer;
        }

        private void LeftMouseDownAction(Point point)
        {
            _leftPressed = true;
            MouseMoveAction(point);
        }

        private void LeftMouseUpAction(Point point)
        {
            _leftPressed = false;
        }

        private void MouseMoveAction(Point point)
        {
            if(_leftPressed)
            {
                int imageX = (int)(point.X / _imageFactor);
                int imageY = (int)(point.Y / _imageFactor);
                _frame.SetPixel(0, imageX, imageY, Color.Black.ToArgb());
                Subjects.RefreshCanvas.OnNext(true);
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
    }
}
