using Avalonia.Controls;
using Avalonia.Media;
using Pixed.Utils;
using System;
using System.Drawing;
using Frame = Pixed.Models.Frame;

namespace Pixed.ViewModels;

internal class PaintCanvasViewModel : PropertyChangedBase, IDisposable
{
    private double _gridWidth;
    private double _gridHeight;
    private double _imageFactor;
    private Bitmap _overlayBitmap;
    private Avalonia.Media.Imaging.Bitmap _avaloniaImageBitmap;
    private Avalonia.Media.Imaging.Bitmap _avaloniaOverlayBitmap;
    private DrawingBrush? _gridBrush;
    private bool _leftPressed;
    private bool _rightPressed;
    private Frame _frame;
    private Point _lastWindowSize;
    private bool _disposedValue;

    private readonly IDisposable _projectModified;
    private readonly IDisposable _projectChanged;
    private readonly IDisposable _frameChanged;
    private readonly IDisposable _frameModified;
    private readonly IDisposable _layerRemoved;
    private readonly IDisposable _layerAdded;
    private readonly IDisposable _layerChanged;
    private readonly IDisposable _layerModified;
    private readonly IDisposable _mouseWheel;
    private readonly IDisposable _gridChanged;

    public ActionCommand<Point> LeftMouseDown { get; set; }
    public ActionCommand<Point> LeftMouseUp { get; set; }
    public ActionCommand<Point> MouseMove { get; set; }
    public ActionCommand<Point> RightMouseDown { get; set; }
    public ActionCommand<Point> RightMouseUp { get; set; }
    public ActionCommand<Point> MiddleMouseDown { get; set; }
    public ActionCommand<Point> MiddleMouseUp { get; set; }
    public ActionCommand<double> MouseWheel { get; set; }
    public ActionCommand MouseLeave { get; set; }
    public double Zoom { get; set; } = 1.0;


    public Bitmap Overlay
    {
        get => _overlayBitmap;
        set
        {
            _overlayBitmap = value;
            OnPropertyChanged();
            AvaloniaOverlayBitmap = _overlayBitmap.ToAvaloniaBitmap();
        }
    }

    public Avalonia.Media.Imaging.Bitmap AvaloniaImageBitmap
    {
        get => _avaloniaImageBitmap;
        set
        {
            _avaloniaImageBitmap = value;
            OnPropertyChanged();
        }
    }

    public Avalonia.Media.Imaging.Bitmap AvaloniaOverlayBitmap
    {
        get => _avaloniaOverlayBitmap;
        set
        {
            _avaloniaOverlayBitmap = value;
            OnPropertyChanged();
        }
    }

    public DrawingBrush? GridBrush
    {
        get => _gridBrush;
        set
        {
            _gridBrush = value;
            OnPropertyChanged();
        }
    }

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

    public PaintCanvasViewModel()
    {
        _frame = new Frame(32, 32);
        LeftMouseDown = new ActionCommand<Point>(LeftMouseDownAction);
        LeftMouseUp = new ActionCommand<Point>(LeftMouseUpAction);
        RightMouseDown = new ActionCommand<Point>(RightMouseDownAction);
        RightMouseUp = new ActionCommand<Point>(RightMouseUpAction);
        MouseMove = new ActionCommand<Point>(MouseMoveAction);
        MouseWheel = new ActionCommand<double>(MouseWheelAction);
        MouseLeave = new ActionCommand(MouseLeaveAction);

        _projectModified = Subjects.ProjectModified.Subscribe(p =>
        {
            RecalculateFactor(_lastWindowSize);
        });

        _projectChanged = Subjects.ProjectChanged.Subscribe(p =>
        {
            RecalculateFactor(_lastWindowSize);
            GridBrush = GetGridBrush();
            //AvaloniaGridBitmap = GetGrid().ToAvaloniaBitmap();
        });

        _frameChanged = Subjects.FrameChanged.Subscribe(f =>
        {
            _frame = f;
            _frame.RefreshLayerRenderSources();
            AvaloniaImageBitmap = _frame.RenderTransparent().ToAvaloniaBitmap();
        });

        _frameModified = Subjects.FrameModified.Subscribe(f =>
        {
            f.RefreshLayerRenderSources();
            f.RefreshRenderSource();
        });

        _layerAdded = Subjects.LayerAdded.Subscribe(l =>
        {
            AvaloniaImageBitmap = _frame.RenderTransparent().ToAvaloniaBitmap();
        });

        _layerRemoved = Subjects.LayerRemoved.Subscribe(l =>
        {
            AvaloniaImageBitmap = _frame.RenderTransparent().ToAvaloniaBitmap();
        });

        _layerChanged = Subjects.LayerChanged.Subscribe(l =>
        {
            AvaloniaImageBitmap = _frame.RenderTransparent().ToAvaloniaBitmap();
        });

        _layerModified = Subjects.LayerModified.Subscribe(l =>
        {
            AvaloniaImageBitmap = _frame.RenderTransparent().ToAvaloniaBitmap();
        });

        _mouseWheel = Subjects.MouseWheel.Subscribe(d =>
        {
            int distX = (int)(GridWidth / Global.CurrentFrame.Width);
            int distY = (int)(GridHeight / Global.CurrentFrame.Height);

            if(distX == 0 || distY == 0)
            {
                return;
            }

            GridBrush = GetGridBrush();
        });

        _gridChanged = Subjects.GridChanged.Subscribe(enabled =>
        {
            GridBrush = GetGridBrush();
        });
    }
    public void RecalculateFactor(Point windowSize)
    {
        var factor = Math.Min(windowSize.X, windowSize.Y) / Math.Min(_frame.Width, _frame.Height);
        _imageFactor = Math.Clamp(factor, 1, 300);
        GridWidth = _frame.Width * factor;
        GridHeight = _frame.Height * factor;
        GridBrush = GetGridBrush();
        _lastWindowSize = windowSize;
        ResetOverlay();
    }

    public void ResetOverlay()
    {
        Overlay?.Dispose();
        Overlay = new Bitmap(_frame.Width, _frame.Height);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _projectModified?.Dispose();
                _projectChanged?.Dispose();
                _frameChanged?.Dispose();
                _frameModified?.Dispose();
                _layerAdded?.Dispose();
                _layerChanged?.Dispose();
                _layerRemoved?.Dispose();
                _layerModified?.Dispose();
                _mouseWheel?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
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
        AvaloniaImageBitmap = _frame.RenderTransparent().ToAvaloniaBitmap();
        Subjects.LayerModified.OnNext(_frame.CurrentLayer);
        Subjects.FrameModified.OnNext(_frame);
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
        AvaloniaImageBitmap = _frame.RenderTransparent().ToAvaloniaBitmap();
        Subjects.LayerModified.OnNext(_frame.CurrentLayer);
        Subjects.FrameModified.OnNext(_frame);
    }

    private void MouseMoveAction(Point point)
    {
        int imageX = (int)(point.X / _imageFactor);
        int imageY = (int)(point.Y / _imageFactor);

        if (!_frame.ContainsPixel(imageX, imageY))
        {
            return;
        }

        if (_leftPressed || _rightPressed)
        {
            Global.ToolSelected?.MoveTool(imageX, imageY, _frame, ref _overlayBitmap);
            RefreshOverlay();
        }
        else
        {
            Global.ToolSelected?.UpdateHighlightedPixel(imageX, imageY, _frame, ref _overlayBitmap);
            RefreshOverlay();
        }
    }

    private void MouseWheelAction(double delta)
    {
        double multiplier = delta;
        var step = multiplier * Math.Max(0.1, Math.Abs(_imageFactor) / 15);
        _imageFactor = Math.Max(0.1, _imageFactor + step);
        _imageFactor = Math.Clamp(_imageFactor, 1, 300);
        GridWidth = _frame.Width * _imageFactor;
        GridHeight = _frame.Height * _imageFactor;
        Subjects.MouseWheel.OnNext(delta);
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
        AvaloniaOverlayBitmap = Overlay.ToAvaloniaBitmap();
    }

    private DrawingBrush? GetGridBrush()
    {
        if(!Global.UserSettings.GridEnabled)
        {
            return null;
        }
        double distX = (GridWidth / (double)Global.CurrentFrame.Width) * Global.UserSettings.GridWidth;
        double distY = (GridHeight / (double)Global.CurrentFrame.Height) * Global.UserSettings.GridHeight;

        LineGeometry horizontalLine = new()
        {
            StartPoint = new Avalonia.Point(0, distY),
            EndPoint = new Avalonia.Point(distX, distY)
        };

        LineGeometry verticalLine = new()
        {
            StartPoint = new Avalonia.Point(distX, 0),
            EndPoint = new Avalonia.Point(distX, distY)
        };

        GeometryGroup group = new();
        group.Children.Add(horizontalLine);
        group.Children.Add(verticalLine);

        Avalonia.Media.Pen pen = new(new SolidColorBrush(Global.UserSettings.GridColor, 0.5));
        GeometryDrawing geometry = new()
        {
            Pen = pen,
            Geometry = group
        };

        DrawingBrush brush = new()
        {
            TileMode = TileMode.Tile,
            DestinationRect = new Avalonia.RelativeRect(0, 0, distX, distY, Avalonia.RelativeUnit.Absolute),
            Drawing = geometry
        };
        return brush;
    }
}
