using Avalonia.Media;
using Avalonia.Platform;
using Pixed.Application.Controls;
using Pixed.Application.Input;
using Pixed.Application.Zoom;
using Pixed.Common;
using Pixed.Common.Selection;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Tools;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using Frame = Pixed.Core.Models.Frame;

namespace Pixed.Application.ViewModels;

internal class PaintCanvasViewModel : PixedViewModel, IDisposable
{
    private readonly ApplicationData _applicationData;
    private readonly ToolSelector _toolSelector;
    private SKBitmap _overlayBitmap;
    private PixedImage _renderImage = new(null);
    private PixedImage _overlayImage = new(null);
    private ImageBrush _transparentBrush;
    private readonly Avalonia.Media.Imaging.Bitmap _transparentBitmap;
    private double _gridWidth;
    private double _gridHeight;
    private bool _leftPressed;
    private bool _rightPressed;
    private Frame _frame;
    private Point _lastWindowSize;
    private bool _disposedValue;
    private KeyState _currentKeyState = new(Avalonia.Input.Key.None, false, false, false);
    private string _projectSizeText;
    private string _mouseCoordinatesText;
    private int _toolSize = 1;
    private bool _isPinchEnabled = false;
    private int _prevX = -1;
    private int _prevY = -1;
    private bool _gestureZoomEnabled;
    private double _zoomValue = 1;
    private bool _overlayVisible = false;

    private readonly IDisposable _projectModified;
    private readonly IDisposable _projectChanged;
    private readonly IDisposable _projectAdded;
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
    private readonly IDisposable _zoomChanged;
    private readonly IDisposable _selectionCreating;
    private readonly IDisposable _selectionDismissed;
    private readonly IDisposable _selectionCreated;
    private readonly IDisposable _selectionStarted;

    public ActionCommand<MouseEvent> LeftMouseDown { get; }
    public ActionCommand<MouseEvent> LeftMouseUp { get; }
    public ActionCommand<MouseEvent> MouseMove { get; }
    public ActionCommand<MouseEvent> RightMouseDown { get; }
    public ActionCommand<MouseEvent> RightMouseUp { get; }
    public ActionCommand<MouseEvent> MiddleMouseDown { get; }
    public ActionCommand<MouseEvent> MiddleMouseUp { get; }
    public ActionCommand<double> MouseWheel { get; }
    public ActionCommand MouseLeave { get; }

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

    public bool OverlayVisible
    {
        get => _overlayVisible;
        set
        {
            _overlayVisible = value;
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

    public bool IsPinchEnabled
    {
        get => _isPinchEnabled;
        set
        {
            _isPinchEnabled = value;
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
            OnPropertyChanged(nameof(ScaledGridWidth));
        }
    }

    public double GridHeight
    {
        get => _gridHeight;
        set
        {
            _gridHeight = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ScaledGridHeight));
        }
    }

    public double ScaledGridWidth => GridWidth * ZoomValue;
    public double ScaledGridHeight => GridHeight * ZoomValue;

    public bool GestureZoomEnabled
    {
        get => _gestureZoomEnabled;
        set
        {
            _gestureZoomEnabled = value;
            OnPropertyChanged();
        }
    }

    public double ZoomValue
    {
        get => _zoomValue;
        set
        {
            _zoomValue = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ScaledGridWidth));
            OnPropertyChanged(nameof(ScaledGridHeight));
        }
    }
    public double ZoomOffsetX { get; set; }
    public double ZoomOffsetY { get; set; }

    public string ZoomText { get; set; }
    public ZoomBorder ZoomContainer { get; set; }
    public ImageGrid GridCanvas { get; set; }
    public SelectionOverlay SelectionOverlay { get; set; }
    public ImageBrush TransparentBrush
    {
        get => _transparentBrush;
        set
        {
            _transparentBrush = value;
            OnPropertyChanged();
        }
    }

    public PaintCanvasViewModel(ApplicationData applicationData, ToolSelector toolSelector, ToolMoveCanvas toolMoveCanvas, SelectionManager selectionManager)
    {
        _applicationData = applicationData;
        _toolSelector = toolSelector;
        _frame = new Frame(32, 32);
        LeftMouseDown = new ActionCommand<MouseEvent>(LeftMouseDownAction);
        LeftMouseUp = new ActionCommand<MouseEvent>(LeftMouseUpAction);
        RightMouseDown = new ActionCommand<MouseEvent>(RightMouseDownAction);
        RightMouseUp = new ActionCommand<MouseEvent>(RightMouseUpAction);
        MouseMove = new ActionCommand<MouseEvent>(MouseMoveAction);
        MouseLeave = new ActionCommand(MouseLeaveAction);

        var stream = AssetLoader.Open(new Uri("avares://Pixed.Application/Resources/transparentBackground.png"));
        _transparentBitmap = new Avalonia.Media.Imaging.Bitmap(stream);

        TransparentBrush = GetTransparentBackgroundBrush();

        _projectModified = Subjects.ProjectModified.Subscribe(p =>
        {
            _frame = p.CurrentFrame;
            RecalculateFactor(_lastWindowSize);
        });

        _projectChanged = Subjects.ProjectChanged.Subscribe(p =>
        {
            _frame = p.CurrentFrame;
            RecalculateFactor(_lastWindowSize);
            RefreshGridCanvas();
            ProjectSizeText = "[" + p.Width + "x" + p.Height + "]";
        });

        _projectAdded = Subjects.ProjectAdded.Subscribe(p =>
        {
            foreach (var frame in p.Frames)
            {
                frame.RefreshCurrentLayerRenderSource([]);
            }
        });

        _frameChanged = Subjects.FrameChanged.Subscribe(f =>
        {
            _frame = f;
            _frame.RefreshLayerRenderSources();
            ReloadFrameRender();
            ClearOverlay();
            GridWidth = _frame.Width;
            GridHeight = _frame.Height;
        });

        _frameModified = Subjects.FrameModified.Subscribe(f =>
        {
            f.RefreshLayerRenderSources();
            ReloadFrameRender();
        });

        _layerModified = Subjects.LayerModified.Subscribe(l =>
        {
            l.Rerender();
            ReloadFrameRender();
        });

        _gridChanged = Subjects.GridChanged.Subscribe(enabled =>
        {
            RefreshGridCanvas();
        });

        _toolChanged = Subjects.ToolChanged.Subscribe(tool =>
        {
            if (tool.Previous != null)
            {
                tool.Previous.Reset();
                tool.Previous.ResetProperties();
                ClearOverlay();
            }
        });

        _keyState = Subjects.KeyState.Subscribe(state =>
        {
            _currentKeyState = state;
        });

        _overlayChanged = Subjects.OverlayModified.Subscribe(overlay =>
        {
            ReloadOverlay();
            OverlayVisible = true;
        });

        _currentLayerRenderModified = Subjects.CurrentLayerRenderModified.Subscribe(pixels =>
        {
            _applicationData.CurrentFrame.RefreshLayerRenderSources(pixels);
            AvaloniaImageBitmap = _applicationData.CurrentFrame.RenderSource;
        });

        _zoomChanged = ZoomBorder.ZoomChanged.Subscribe(entry =>
        {
            ZoomValue = entry.Zoom;
            ZoomOffsetX = entry.OffsetX;
            ZoomOffsetY = entry.OffsetY;
            if (GridCanvas != null)
            {
                GridCanvas.Zoom = entry.Zoom;
            }

            if (SelectionOverlay != null)
            {
                SelectionOverlay.Zoom = entry.Zoom;
            }
            RefreshZoomText();
            TransparentBrush = GetTransparentBackgroundBrush();
        });

        _selectionCreating = Subjects.SelectionCreating.Subscribe(selection =>
        {
            SelectionOverlay?.UpdateSelection(selection);
        });

        _selectionDismissed = Subjects.SelectionDismissed.Subscribe(_ =>
        {
            if (SelectionOverlay != null)
            {
                SelectionOverlay.UpdateSelection(null);
                SelectionOverlay.DrawLines = false;
            }
        });

        _selectionCreated = Subjects.SelectionCreated.Subscribe(selection =>
        {
            if (SelectionOverlay != null)
            {
                SelectionOverlay.DrawLines = true;
                SelectionOverlay.UpdateSelection(selection);
            }
        });

        _selectionStarted = Subjects.SelectionStarted.Subscribe(_ =>
        {
            if (SelectionOverlay != null)
            {
                SelectionOverlay.DrawLines = false;
            }
        });

        toolMoveCanvas.SetGestureEnabledAction = isEnabled =>
        {
            GestureZoomEnabled = isEnabled;
        };
    }
    public void RecalculateFactor(Point windowSize)
    {
        RefreshGridCanvas();
        _lastWindowSize = windowSize;
        ClearOverlay();
    }
    public void RefreshZoomText()
    {
        double zoom = ZoomValue * 100d;
        bool needRound = zoom % 1 != 0;
        ZoomText = "Zoom: " + zoom.ToString(needRound ? "#.0" : "#");
        OnPropertyChanged(nameof(ZoomText));
    }
    public void ClearOverlay()
    {
        OverlayVisible = false;
        if (_overlayBitmap == null || _overlayBitmap.Width != _frame.Width || _overlayBitmap.Height != _frame.Height)
        {
            _overlayBitmap?.Dispose();
            _overlayBitmap = new SKBitmap(_frame.Width, _frame.Height, true);
        }
        else
        {
            _overlayBitmap.Clear();
        }

        AvaloniaOverlayBitmap.UpdateBitmap(_overlayBitmap);
        OnPropertyChanged(nameof(AvaloniaOverlayBitmap));
    }

    public void RefreshGridCanvas()
    {
        GridCanvas.GridWidth = _applicationData.UserSettings.GridWidth;
        GridCanvas.GridHeight = _applicationData.UserSettings.GridHeight;
        GridCanvas.GridEnabled = _applicationData.UserSettings.GridEnabled;
        GridCanvas.GridColor = _applicationData.UserSettings.GridColor;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _projectAdded?.Dispose();
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
                _zoomChanged?.Dispose();
                _currentLayerRenderModified?.Dispose();
                _selectionCreated?.Dispose();
                _selectionCreating?.Dispose();
                _selectionDismissed?.Dispose();
                _selectionStarted?.Dispose();
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
        if (!_frame.ContainsPixel(mouseEvent.Point) || _toolSelector.ToolSelected == null)
        {
            return;
        }

        _leftPressed = true;
        _toolSelector.ToolSelected?.ApplyTool(mouseEvent.Point, _frame, ref _overlayBitmap, _currentKeyState);
        Subjects.FrameModified.OnNext(_frame);
    }

    private void LeftMouseUpAction(MouseEvent mouseEvent)
    {
        if (!_frame.ContainsPixel(mouseEvent.Point) || _toolSelector.ToolSelected == null)
        {
            return;
        }

        _leftPressed = false;
        _toolSelector.ToolSelected?.ReleaseTool(mouseEvent.Point, _frame, ref _overlayBitmap, _currentKeyState);

        if (_toolSelector.ToolSelected != null && _toolSelector.ToolSelected.AddToHistory)
        {
            _applicationData.CurrentModel.AddHistory();
        }

        Subjects.FrameModified.OnNext(_frame);
    }

    private void RightMouseDownAction(MouseEvent mouseEvent)
    {
        if (!_frame.ContainsPixel(mouseEvent.Point) || _toolSelector.ToolSelected == null)
        {
            return;
        }

        _rightPressed = true;
        _toolSelector.ToolSelected?.ApplyTool(mouseEvent.Point, _frame, ref _overlayBitmap, _currentKeyState);
    }

    private void RightMouseUpAction(MouseEvent mouseEvent)
    {
        if (!_frame.ContainsPixel(mouseEvent.Point) || _toolSelector.ToolSelected == null)
        {
            return;
        }

        _rightPressed = false;
        _toolSelector.ToolSelected?.ReleaseTool(mouseEvent.Point, _frame, ref _overlayBitmap, _currentKeyState);

        if (_toolSelector.ToolSelected != null && _toolSelector.ToolSelected.AddToHistory)
        {
            _applicationData.CurrentModel.AddHistory();
        }

        Subjects.FrameModified.OnNext(_frame);
    }

    private void MouseMoveAction(MouseEvent mouseEvent)
    {
        MouseCoordinatesText = "[" + mouseEvent.Point.X + "x" + mouseEvent.Point.Y + "]";

        if (!_frame.ContainsPixel(mouseEvent.Point) || _toolSelector.ToolSelected == null)
        {
            return;
        }

        if (!CanProcess(mouseEvent.Point))
        {
            return;
        }

        if (_leftPressed || _rightPressed)
        {
            _toolSelector.ToolSelected.MoveTool(mouseEvent.Point, _frame, ref _overlayBitmap, _currentKeyState);
        }
        else
        {
            _toolSelector.ToolSelected.UpdateHighlightedPixel(mouseEvent.Point, _frame, ref _overlayBitmap);
        }


    }

    private void MouseLeaveAction()
    {
        if (_rightPressed || _leftPressed)
        {
            _toolSelector.ToolSelected?.ReleaseTool(new Point(), _frame, ref _overlayBitmap, _currentKeyState);
        }
        _rightPressed = false;
        _leftPressed = false;
    }

    private void ReloadOverlay()
    {
        AvaloniaOverlayBitmap.UpdateBitmap(_overlayBitmap);
        OnPropertyChanged(nameof(AvaloniaOverlayBitmap));
    }

    private void ReloadFrameRender()
    {
        List<Pixel>? pixels = null;

        if (_toolSelector.ToolSelected is ToolPen pen)
        {
            pixels = pen.GetPixels();
        }

        _frame.RefreshCurrentLayerRenderSource(pixels);
        AvaloniaImageBitmap.UpdateBitmap(_frame.RenderSource.Source);
        OnPropertyChanged(nameof(AvaloniaImageBitmap));
    }

    private bool CanProcess(Point point)
    {
        var prevX = _prevX;
        var prevY = _prevY;
        _prevX = point.X;
        _prevY = point.Y;

        return point.X != prevX || point.Y != prevY;
    }

    private ImageBrush GetTransparentBackgroundBrush()
    {
        double defaultValue = 2d;

        double delta = defaultValue;
        if (ZoomContainer != null)
        {
            Avalonia.Matrix matrix = ZoomContainer.Matrix.Invert();
            delta = matrix.Prepend(new Avalonia.Matrix(defaultValue, 0, 0, defaultValue, 0, 0)).M11;
        }
        return new(_transparentBitmap)
        {
            TileMode = TileMode.Tile,
            Transform = new ScaleTransform(delta, delta)
        };
    }
}
