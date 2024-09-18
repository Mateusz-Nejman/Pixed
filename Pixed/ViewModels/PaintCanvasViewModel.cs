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
    private bool _leftPressed;
    private bool _rightPressed;
    private Frame _frame;
    private Point _lastWindowSize;
    private bool _disposedValue;

    private IDisposable _projectModified;
    private IDisposable _projectChanged;
    private IDisposable _frameChanged;
    private IDisposable _frameModified;
    private IDisposable _layerRemoved;
    private IDisposable _layerAdded;
    private IDisposable _layerChanged;

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
    }
    public void RecalculateFactor(Point windowSize)
    {
        var factor = Math.Min(windowSize.X, windowSize.Y) / Math.Min(_frame.Width, _frame.Height);
        _imageFactor = factor;
        GridWidth = _frame.Width * factor;
        GridHeight = _frame.Height * factor;
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
                _layerRemoved.Dispose();
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
        GridWidth = _frame.Width * _imageFactor;
        GridHeight = _frame.Height * _imageFactor;
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
}
