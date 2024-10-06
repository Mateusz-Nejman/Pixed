using Avalonia.Media;
using Pixed.Controls;
using Pixed.Models;
using Pixed.Tools;
using Pixed.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using Frame = Pixed.Models.Frame;

namespace Pixed.ViewModels;

internal class PaintCanvasViewModel : PixedViewModel, IDisposable
{
    private readonly ApplicationData _applicationData;
    private readonly ToolSelector _toolSelector;
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
    private bool _shiftPressed;
    private bool _controlPressed;
    private bool _altPressed;
    private bool _shiftChecked;
    private bool _controlChecked;
    private bool _altChecked;
    private bool _shiftEnabled;
    private bool _controlEnabled;
    private bool _altEnabled;
    private string _projectSizeText;
    private string _mouseCoordinatesText;
    private int _toolSize = 1;
    private Avalonia.Vector _scrollViewerOffset;
    private bool _isPinchEnabled = false;

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
    private readonly IDisposable _toolChanged;
    private readonly IDisposable _keyState;

    public ActionCommand<Point> LeftMouseDown { get; }
    public ActionCommand<Point> LeftMouseUp { get; }
    public ActionCommand<Point> MouseMove { get; }
    public ActionCommand<Point> RightMouseDown { get; }
    public ActionCommand<Point> RightMouseUp { get; }
    public ActionCommand<Point> MiddleMouseDown { get; }
    public ActionCommand<Point> MiddleMouseUp { get; }
    public ActionCommand<double> MouseWheel { get; }
    public ActionCommand MouseLeave { get; }
    public ActionCommand ZoomInCommand { get; }
    public ActionCommand ZoomOutCommand { get; }

    public int ToolSize
    {
        get => _toolSize;
        set
        {
            _toolSize = value;
            _applicationData.ToolSize = value;
            OnPropertyChanged();
        }
    }

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

    public bool ShiftChecked
    {
        get => _shiftChecked;
        set
        {
            _shiftChecked = value;
            OnPropertyChanged();
            _shiftPressed = value;
        }
    }

    public bool ControlChecked
    {
        get => _controlChecked;
        set
        {
            _controlChecked = value;
            OnPropertyChanged();
            _controlPressed = value;
        }
    }

    public bool AltChecked
    {
        get => _altChecked;
        set
        {
            _altChecked = value;
            OnPropertyChanged();
            _altPressed = value;
        }
    }

    public bool ShiftEnabled
    {
        get => _shiftEnabled;
        set
        {
            _shiftEnabled = value;
            OnPropertyChanged();
        }
    }

    public bool ControlEnabled
    {
        get => _controlEnabled;
        set
        {
            _controlEnabled = value;
            OnPropertyChanged();
        }
    }

    public bool AltEnabled
    {
        get => _altEnabled;
        set
        {
            _altEnabled = value;
            OnPropertyChanged();
        }
    }

    public string ProjectSizeText
    {
        get => _projectSizeText;
        set
        {
            _projectSizeText = value;
            OnPropertyChanged();
        }
    }

    public string MouseCoordinatesText
    {
        get => _mouseCoordinatesText;
        set
        {
            _mouseCoordinatesText = value;
            OnPropertyChanged();
        }
    }

    public Avalonia.Vector ScrollViewerOffset
    {
        get => _scrollViewerOffset;
        set
        {
            _scrollViewerOffset = value;
            OnPropertyChanged();
        }
    }

    public bool IsPinchEnabled
    {
        get => _isPinchEnabled;
        set
        {
            _isPinchEnabled = value;
            OnPropertyChanged();
        }
    }

    public PaintCanvasViewModel(ApplicationData applicationData, ToolSelector toolSelector, ToolMoveCanvas toolMoveCanvas, ToolZoom toolZoom)
    {
        _applicationData = applicationData;
        _toolSelector = toolSelector;
        _frame = new Frame(32, 32);
        LeftMouseDown = new ActionCommand<Point>(LeftMouseDownAction);
        LeftMouseUp = new ActionCommand<Point>(LeftMouseUpAction);
        RightMouseDown = new ActionCommand<Point>(RightMouseDownAction);
        RightMouseUp = new ActionCommand<Point>(RightMouseUpAction);
        MouseMove = new ActionCommand<Point>(MouseMoveAction);
        MouseWheel = new ActionCommand<double>(MouseWheelAction);
        MouseLeave = new ActionCommand(MouseLeaveAction);
        ZoomInCommand = new ActionCommand(ZoomInAction);
        ZoomOutCommand = new ActionCommand(ZoomOutAction);

        _projectModified = Subjects.ProjectModified.Subscribe(p =>
        {
            RecalculateFactor(_lastWindowSize);
        });

        _projectChanged = Subjects.ProjectChanged.Subscribe(p =>
        {
            RecalculateFactor(_lastWindowSize);
            GridBrush = GetGridBrush();
            ProjectSizeText = "[" + p.Width + "x" + p.Height + "]";

        });

        _frameChanged = Subjects.FrameChanged.Subscribe(f =>
        {
            _frame = f;
            _frame.RefreshLayerRenderSources();
            ReloadFrameRender();
            Overlay.Clear();
            AvaloniaOverlayBitmap = Overlay.ToAvaloniaBitmap();
        });

        _frameModified = Subjects.FrameModified.Subscribe(f =>
        {
            f.RefreshLayerRenderSources();
            f.RefreshRenderSource();
            ReloadFrameRender();
        });

        _layerAdded = Subjects.LayerAdded.Subscribe(ReloadFrameRender);

        _layerRemoved = Subjects.LayerRemoved.Subscribe(ReloadFrameRender);

        _layerChanged = Subjects.LayerChanged.Subscribe(ReloadFrameRender);

        _layerModified = Subjects.LayerModified.Subscribe(ReloadFrameRender);

        _mouseWheel = Subjects.MouseWheel.Subscribe(d =>
        {
            int distX = (int)(GridWidth / _applicationData.CurrentFrame.Width);
            int distY = (int)(GridHeight / _applicationData.CurrentFrame.Height);

            if (distX == 0 || distY == 0)
            {
                return;
            }

            GridBrush = GetGridBrush();
        });

        _gridChanged = Subjects.GridChanged.Subscribe(enabled =>
        {
            GridBrush = GetGridBrush();
        });

        _toolChanged = Subjects.ToolChanged.Subscribe(tool =>
        {
            if (tool.Previous != null)
            {
                tool.Previous.Reset();
                ResetOverlay();
            }
            ShiftEnabled = tool.Current.ShiftHandle;
            ControlEnabled = tool.Current.ControlHandle;
            AltEnabled = tool.Current.AltHandle;

            if (!ShiftEnabled)
            {
                _shiftChecked = false;
                OnPropertyChanged(nameof(ShiftChecked));
            }

            if (!ControlEnabled)
            {
                _controlChecked = false;
                OnPropertyChanged(nameof(ControlChecked));
            }
        });

        _keyState = Subjects.KeyState.Subscribe(state =>
        {
            if (!ShiftChecked)
            {
                _shiftPressed = state.IsShift;
            }

            if (!ControlChecked)
            {
                _controlPressed = state.IsCtrl;
            }

            if (!AltChecked)
            {
                _altChecked = state.IsAlt;
            }
        });

        toolMoveCanvas.MoveAction = offset =>
        {
            _scrollViewerOffset = offset;
            OnPropertyChanged(nameof(ScrollViewerOffset));
        };

        toolMoveCanvas.GetOffset = () => ScrollViewerOffset;

        toolZoom.ZoomAction = Zoom;
        toolZoom.SetEnabled = enabled => IsPinchEnabled = enabled;
        toolZoom.GetZoom = () => _imageFactor;
    }
    public void RecalculateFactor(Point windowSize)
    {
        var factor = Math.Min(windowSize.X, windowSize.Y - 32) / Math.Min(_frame.Width, _frame.Height);
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
        AvaloniaOverlayBitmap = Overlay.ToAvaloniaBitmap();
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
                _gridChanged?.Dispose();
                _toolChanged?.Dispose();
                _keyState?.Dispose();
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
        if (_toolSelector.ToolSelected is ToolMoveCanvas toolMoveCanvas)
        {
            toolMoveCanvas.ApplyTool(point.X, point.Y, _frame, ref _overlayBitmap, _shiftPressed, _controlPressed, _altPressed);
        }
        else if (_toolSelector.ToolSelected is ToolZoom toolZoom)
        {
            toolZoom.ApplyTool(point.X, point.Y, _frame, ref _overlayBitmap, _shiftPressed, _controlPressed, _altPressed);
        }
        else
        {
            _toolSelector.ToolSelected?.ApplyTool(imageX, imageY, _frame, ref _overlayBitmap, _shiftPressed, _controlPressed, _altPressed);
        }
        ReloadOverlay();
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
        _toolSelector.ToolSelected?.ReleaseTool(imageX, imageY, _frame, ref _overlayBitmap, _shiftPressed, _controlPressed, _altPressed);

        if (_toolSelector.ToolSelected != null && _toolSelector.ToolSelected.AddToHistory)
        {
            _applicationData.CurrentModel.AddHistory();
        }
        ReloadOverlay();
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
        if (_toolSelector.ToolSelected is ToolMoveCanvas toolMoveCanvas)
        {
            toolMoveCanvas.ApplyTool(point.X, point.Y, _frame, ref _overlayBitmap, _shiftPressed, _controlPressed, _altPressed);
        }
        else if (_toolSelector.ToolSelected is ToolZoom toolZoom)
        {
            toolZoom.ApplyTool(point.X, point.Y, _frame, ref _overlayBitmap, _shiftPressed, _controlPressed, _altPressed);
        }
        else
        {
            _toolSelector.ToolSelected?.ApplyTool(imageX, imageY, _frame, ref _overlayBitmap, _shiftPressed, _controlPressed, _altPressed);
        }
        ReloadOverlay();
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
        _toolSelector.ToolSelected?.ReleaseTool(imageX, imageY, _frame, ref _overlayBitmap, _shiftPressed, _controlPressed, _altPressed);

        if (_toolSelector.ToolSelected != null && _toolSelector.ToolSelected.AddToHistory)
        {
            _applicationData.CurrentModel.AddHistory();
        }
        ReloadOverlay();
        Subjects.LayerModified.OnNext(_frame.CurrentLayer);
        Subjects.FrameModified.OnNext(_frame);
    }

    private void MouseMoveAction(Point point)
    {
        int imageX = (int)(point.X / _imageFactor);
        int imageY = (int)(point.Y / _imageFactor);

        MouseCoordinatesText = "[" + imageX + "x" + imageY + "]";

        if (!_frame.ContainsPixel(imageX, imageY))
        {
            return;
        }

        if (_leftPressed || _rightPressed)
        {
            if (_toolSelector.ToolSelected is ToolMoveCanvas toolMoveCanvas)
            {
                toolMoveCanvas.MoveTool(point.X, point.Y, _frame, ref _overlayBitmap, _shiftPressed, _controlPressed, _altPressed);
            }
            else if (_toolSelector.ToolSelected is ToolZoom toolZoom)
            {
                toolZoom.MoveTool(point.X, point.Y, _frame, ref _overlayBitmap, _shiftPressed, _controlPressed, _altPressed);
            }
            else
            {
                _toolSelector.ToolSelected?.MoveTool(imageX, imageY, _frame, ref _overlayBitmap, _shiftPressed, _controlPressed, _altPressed);
            }
        }
        else
        {
            _toolSelector.ToolSelected?.UpdateHighlightedPixel(imageX, imageY, _frame, ref _overlayBitmap);
        }

        ReloadOverlay();
    }

    private void ZoomInAction()
    {
        MouseWheelAction(1.0);
    }

    private void ZoomOutAction()
    {
        MouseWheelAction(-1.0);
    }

    private void MouseWheelAction(double delta)
    {
        double multiplier = delta;
        var step = multiplier * Math.Max(0.1, Math.Abs(_imageFactor) / 15);
        var factor = Math.Max(0.1, _imageFactor + step);
        Zoom(factor);
    }

    private void Zoom(double factor)
    {
        _imageFactor = Math.Clamp(factor, 1, 300);
        GridWidth = _frame.Width * _imageFactor;
        GridHeight = _frame.Height * _imageFactor;
        Subjects.MouseWheel.OnNext(factor);
    }

    private void MouseLeaveAction()
    {
        if (_rightPressed || _leftPressed)
        {
            _toolSelector.ToolSelected?.ReleaseTool(0, 0, _frame, ref _overlayBitmap, _shiftPressed, _controlPressed, _altPressed);
        }
        _rightPressed = false;
        _leftPressed = false;
    }

    private void ReloadOverlay()
    {
        AvaloniaOverlayBitmap = Overlay.ToAvaloniaBitmap();
    }

    private DrawingBrush? GetGridBrush()
    {
        if (!_applicationData.UserSettings.GridEnabled)
        {
            return null;
        }
        double distX = (GridWidth / (double)_applicationData.CurrentFrame.Width) * _applicationData.UserSettings.GridWidth;
        double distY = (GridHeight / (double)_applicationData.CurrentFrame.Height) * _applicationData.UserSettings.GridHeight;

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

        Avalonia.Media.Pen pen = new(new SolidColorBrush(_applicationData.UserSettings.GridColor, 0.5));
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

    private void ReloadFrameRender()
    {
        List<Pixel>? pixels = null;

        if (_toolSelector.ToolSelected is ToolPen pen)
        {
            pixels = pen.GetPixels();
        }
        AvaloniaImageBitmap = _frame.Render(pixels).ToAvaloniaBitmap();
    }

    private void ReloadFrameRender(Layer _)
    {
        ReloadFrameRender();
    }
}
