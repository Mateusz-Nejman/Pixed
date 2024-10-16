using Avalonia.Media;
using Pixed.Controls;
using Pixed.Input;
using Pixed.Models;
using Pixed.Services.Keyboard;
using Pixed.Tools;
using Pixed.Utils;
using SkiaSharp;
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
    private double _minimumImageFactor;
    private SKBitmap _overlayBitmap;
    private PixedImage _renderImage = new(null);
    private PixedImage _overlayImage = new(null);
    private DrawingBrush? _gridBrush;
    private bool _leftPressed;
    private bool _rightPressed;
    private Frame _frame;
    private Point _lastWindowSize;
    private bool _disposedValue;
    private KeyState _currentKeyState = new(Avalonia.Input.Key.None, false, false, false);
    private string _projectSizeText;
    private string _mouseCoordinatesText;
    private int _toolSize = 1;
    private Avalonia.Vector _scrollViewerOffset;
    private bool _isPinchEnabled = false;
    private int _prevX = -1;
    private int _prevY = -1;

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
    private readonly IDisposable _overlayChanged;
    private readonly IDisposable _currentLayerRenderModified;

    public ActionCommand<MouseEvent> LeftMouseDown { get; }
    public ActionCommand<MouseEvent> LeftMouseUp { get; }
    public ActionCommand<MouseEvent> MouseMove { get; }
    public ActionCommand<MouseEvent> RightMouseDown { get; }
    public ActionCommand<MouseEvent> RightMouseUp { get; }
    public ActionCommand<MouseEvent> MiddleMouseDown { get; }
    public ActionCommand<MouseEvent> MiddleMouseUp { get; }
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

    public SKBitmap Overlay
    {
        get => _overlayBitmap;
        set
        {
            _overlayBitmap = value;
            OnPropertyChanged();
            AvaloniaOverlayBitmap = new PixedImage(_overlayBitmap);
        }
    }

    public PixedImage AvaloniaImageBitmap
    {
        get => _renderImage;
        set
        {
            _renderImage = value;
            OnPropertyChanged();
        }
    }

    public PixedImage AvaloniaOverlayBitmap
    {
        get => _overlayImage;
        set
        {
            _overlayImage = value;
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

    public string ZoomText => GetZoomText();

    public PaintCanvasViewModel(ApplicationData applicationData, ToolSelector toolSelector, ToolMoveCanvas toolMoveCanvas, ToolZoom toolZoom)
    {
        _applicationData = applicationData;
        _toolSelector = toolSelector;
        _frame = new Frame(32, 32);
        LeftMouseDown = new ActionCommand<MouseEvent>(LeftMouseDownAction);
        LeftMouseUp = new ActionCommand<MouseEvent>(LeftMouseUpAction);
        RightMouseDown = new ActionCommand<MouseEvent>(RightMouseDownAction);
        RightMouseUp = new ActionCommand<MouseEvent>(RightMouseUpAction);
        MouseMove = new ActionCommand<MouseEvent>(MouseMoveAction);
        MouseWheel = new ActionCommand<double>(MouseWheelAction);
        MouseLeave = new ActionCommand(MouseLeaveAction);
        ZoomInCommand = new ActionCommand(ZoomInAction);
        ZoomOutCommand = new ActionCommand(ZoomOutAction);

        _projectModified = Subjects.ProjectModified.Subscribe(p =>
        {
            _frame = p.CurrentFrame;
            RecalculateFactor(_lastWindowSize);
        });

        _projectChanged = Subjects.ProjectChanged.Subscribe(p =>
        {
            _frame = p.CurrentFrame;
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
            AvaloniaOverlayBitmap = new PixedImage(Overlay);
        });

        _frameModified = Subjects.FrameModified.Subscribe(f =>
        {
            f.RefreshLayerRenderSources();
            f.RefreshCurrentLayerRenderSource([]);
            ReloadFrameRender();
        });

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
                tool.Previous.ResetProperties();
                ResetOverlay();
            }
        });

        _keyState = Subjects.KeyState.Subscribe(state =>
        {
            _currentKeyState = state;
        });

        _overlayChanged = Subjects.OverlayModified.Subscribe(overlay =>
        {
            ReloadOverlay();
        });

        _currentLayerRenderModified = Subjects.CurrentLayerRenderModified.Subscribe(pixels =>
        {
            _applicationData.CurrentFrame.RefreshLayerRenderSources(pixels);
            AvaloniaImageBitmap = _applicationData.CurrentFrame.RenderSource;
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
        var factor = Math.Min((double)windowSize.X, (double)windowSize.Y - 40.0d) / Math.Min((double)_frame.Width, (double)_frame.Height);
        _minimumImageFactor = Math.Min((double)windowSize.X / 2, (double)(windowSize.Y - 40) / 2) / Math.Min((double)_frame.Width, (double)_frame.Height);
        Zoom(factor);
        GridBrush = GetGridBrush();
        _lastWindowSize = windowSize;
        ResetOverlay();
    }

    public void ResetOverlay()
    {
        Overlay ??= new SKBitmap(_frame.Width, _frame.Height, true);

        if (Overlay.Width == _frame.Width && Overlay.Height == _frame.Height)
        {
            Overlay.Clear();
        }
        else
        {
            Overlay = new SKBitmap(_frame.Width, _frame.Height, true);
        }
        AvaloniaOverlayBitmap = new PixedImage(Overlay);
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
                _overlayChanged?.Dispose();
                _currentLayerRenderModified?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void LeftMouseDownAction(MouseEvent mouseEvent)
    {
        int imageX = (int)(mouseEvent.Point.X / _imageFactor);
        int imageY = (int)(mouseEvent.Point.Y / _imageFactor);

        if (!_frame.ContainsPixel(imageX, imageY) || _toolSelector.ToolSelected == null)
        {
            return;
        }

        _leftPressed = true;
        var cursorPoint = GetCursorPoint(mouseEvent.Point, _toolSelector.ToolSelected.GridMovement, out _);
        _toolSelector.ToolSelected?.ApplyTool(cursorPoint.X, cursorPoint.Y, _frame, ref _overlayBitmap, _currentKeyState);
        DebugTouchPointer(mouseEvent);
        Subjects.LayerModified.OnNext(_frame.CurrentLayer);
        Subjects.FrameModified.OnNext(_frame);
    }

    private void LeftMouseUpAction(MouseEvent mouseEvent)
    {
        int imageX = (int)(mouseEvent.Point.X / _imageFactor);
        int imageY = (int)(mouseEvent.Point.Y / _imageFactor);

        if (!_frame.ContainsPixel(imageX, imageY) || _toolSelector.ToolSelected == null)
        {
            return;
        }

        _leftPressed = false;

        var cursorPoint = GetCursorPoint(mouseEvent.Point, _toolSelector.ToolSelected.GridMovement, out _);
        _toolSelector.ToolSelected?.ReleaseTool(cursorPoint.X, cursorPoint.Y, _frame, ref _overlayBitmap, _currentKeyState);

        if (_toolSelector.ToolSelected != null && _toolSelector.ToolSelected.AddToHistory)
        {
            _applicationData.CurrentModel.AddHistory();
        }

        DebugTouchPointer(mouseEvent);
        Subjects.LayerModified.OnNext(_frame.CurrentLayer);
        Subjects.FrameModified.OnNext(_frame);
    }

    private void RightMouseDownAction(MouseEvent mouseEvent)
    {
        int imageX = (int)(mouseEvent.Point.X / _imageFactor);
        int imageY = (int)(mouseEvent.Point.Y / _imageFactor);

        if (!_frame.ContainsPixel(imageX, imageY) || _toolSelector.ToolSelected == null)
        {
            return;
        }

        _rightPressed = true;

        var cursorPoint = GetCursorPoint(mouseEvent.Point, _toolSelector.ToolSelected.GridMovement, out _);
        _toolSelector.ToolSelected?.ApplyTool(cursorPoint.X, cursorPoint.Y, _frame, ref _overlayBitmap, _currentKeyState);
        DebugTouchPointer(mouseEvent);
    }

    private void RightMouseUpAction(MouseEvent mouseEvent)
    {
        int imageX = (int)(mouseEvent.Point.X / _imageFactor);
        int imageY = (int)(mouseEvent.Point.Y / _imageFactor);

        if (!_frame.ContainsPixel(imageX, imageY) || _toolSelector.ToolSelected == null)
        {
            return;
        }

        _rightPressed = false;

        var cursorPoint = GetCursorPoint(mouseEvent.Point, _toolSelector.ToolSelected.GridMovement, out _);
        _toolSelector.ToolSelected?.ReleaseTool(cursorPoint.X, cursorPoint.Y, _frame, ref _overlayBitmap, _currentKeyState);

        if (_toolSelector.ToolSelected != null && _toolSelector.ToolSelected.AddToHistory)
        {
            _applicationData.CurrentModel.AddHistory();
        }

        DebugTouchPointer(mouseEvent);
        Subjects.LayerModified.OnNext(_frame.CurrentLayer);
        Subjects.FrameModified.OnNext(_frame);
    }

    private void MouseMoveAction(MouseEvent mouseEvent)
    {
        int imageX = (int)(mouseEvent.Point.X / _imageFactor);
        int imageY = (int)(mouseEvent.Point.Y / _imageFactor);

        MouseCoordinatesText = "[" + imageX + "x" + imageY + "]";

        if (!_frame.ContainsPixel(imageX, imageY) || _toolSelector.ToolSelected == null)
        {
            return;
        }

        var cursorPoint = GetCursorPoint(mouseEvent.Point, _toolSelector.ToolSelected.GridMovement, out bool ignore);

        if (ignore)
        {
            return;
        }

        if (_leftPressed || _rightPressed)
        {
            _toolSelector.ToolSelected.MoveTool(cursorPoint.X, cursorPoint.Y, _frame, ref _overlayBitmap, _currentKeyState);
        }
        else
        {
            _toolSelector.ToolSelected.UpdateHighlightedPixel(cursorPoint.X, cursorPoint.Y, _frame, ref _overlayBitmap);
        }


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
        var step = delta * Math.Max(_minimumImageFactor, Math.Abs(_imageFactor) / 15);
        var factor = Math.Max(_minimumImageFactor, _imageFactor + step);
        Zoom(factor);
    }

    private void Zoom(double factor)
    {
        _imageFactor = Math.Clamp(factor, _minimumImageFactor, 60);
        OnPropertyChanged(nameof(ZoomText));
        GridWidth = _frame.Width * _imageFactor;
        GridHeight = _frame.Height * _imageFactor;
        Subjects.MouseWheel.OnNext(_imageFactor);
    }

    private void MouseLeaveAction()
    {
        if (_rightPressed || _leftPressed)
        {
            _toolSelector.ToolSelected?.ReleaseTool(0, 0, _frame, ref _overlayBitmap, _currentKeyState);
        }
        _rightPressed = false;
        _leftPressed = false;
    }

    private void ReloadOverlay()
    {
        AvaloniaOverlayBitmap = new PixedImage(Overlay);
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

        _frame.RefreshCurrentLayerRenderSource(pixels);
        AvaloniaImageBitmap = new PixedImage(_frame.RenderSource.Source);
    }

    private Point GetCursorPoint(Point point, bool gridMovenent, out bool ignore)
    {
        ignore = false;
        int argX = point.X;
        int argY = point.Y;

        if (gridMovenent)
        {
            argX = (int)(point.X / _imageFactor);
            argY = (int)(point.Y / _imageFactor);

            if (_prevX == argX && _prevY == argY)
            {
                ignore = true;
            }
            else
            {
                _prevX = argX;
                _prevY = argY;
            }
        }

        return new Point(argX, argY);
    }

    private string GetZoomText()
    {
        double zoom = _imageFactor * 100d;
        bool needRound = zoom % 1 != 0;
        return "Zoom: " + zoom.ToString(needRound ? "#.0" : "#");
    }

    private void DebugTouchPointer(MouseEvent mouseEvent)
    {
#if DEBUG
        int imageX = (int)(mouseEvent.Point.X / _imageFactor);
        int imageY = (int)(mouseEvent.Point.Y / _imageFactor);
        _overlayBitmap.SetPixel(imageX, imageY, UniColor.Black);
        Overlay = _overlayBitmap;
#endif
    }
}
