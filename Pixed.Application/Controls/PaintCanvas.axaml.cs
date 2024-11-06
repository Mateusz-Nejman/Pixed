using Avalonia.Controls;
using Avalonia.Input;
using Pixed.Application.DependencyInjection;
using Pixed.Application.Input;
using Pixed.Application.Utils;
using Pixed.Application.ViewModels;
using Pixed.Application.Zoom;
using Pixed.Common.Input;
using Pixed.Common.Models;
using System;

namespace Pixed.Application.Controls;

internal partial class PaintCanvas : PixedUserControl<PaintCanvasViewModel>
{
    private readonly int _scrollBarSize = 18;
    private readonly IDisposable _zoomChanged;
    public PaintCanvas() : base()
    {
        InitializeComponent();
        SizeChanged += PaintCanvas_SizeChanged;
        zoomBorder.GestureZoomEnabled = false;
        ViewModel.GridCanvas = gridCanvas;
        ViewModel.ZoomValue = zoomBorder.Zoom;
        ViewModel.ZoomOffsetX = zoomBorder.OffsetX;
        ViewModel.ZoomOffsetY = zoomBorder.OffsetY;
        Unloaded += PaintCanvas_Unloaded;

        _zoomChanged = ZoomBorder.ZoomChanged.Subscribe(_ =>
        {
            var applicationData = this.GetServiceProvider().Get<ApplicationData>();
            zoomBorder.ConfigureOffsetBounds(applicationData.CurrentFrame.Width, applicationData.CurrentFrame.Height);
            ViewModel.RefreshGridCanvas();
        });
    }

    private void PaintCanvas_Unloaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _zoomChanged?.Dispose();
    }

    private void PaintCanvas_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        ViewModel.RecalculateFactor(new Point((int)(e.NewSize.Width - _scrollBarSize), (int)(e.NewSize.Height - _scrollBarSize)));
    }

    private void Canvas_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Canvas canvas)
        {
            var pos = e.GetPosition(canvas);
            MouseMapper mapper = new(e, canvas);

            if (mapper.ChangedButton == MouseButton.Left && mapper.ButtonState == MouseButtonState.Pressed)
            {
                ViewModel.LeftMouseDown?.Execute(new MouseEvent(pos.ToPixedPoint(), e.Pointer.Type == PointerType.Touch));
            }
            else if (mapper.ChangedButton == MouseButton.Right && mapper.ButtonState == MouseButtonState.Pressed)
            {
                ViewModel.RightMouseDown?.Execute(new MouseEvent(pos.ToPixedPoint(), e.Pointer.Type == PointerType.Touch));
            }
            else if (mapper.ChangedButton == MouseButton.Middle && mapper.ButtonState == MouseButtonState.Pressed)
            {
                ViewModel.MiddleMouseDown?.Execute(new MouseEvent(pos.ToPixedPoint(), e.Pointer.Type == PointerType.Touch));
            }
        }
    }

    private void Canvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Canvas canvas)
        {
            var pos = e.GetPosition(canvas);
            MouseMapper mapper = new(e, canvas);

            if (mapper.ChangedButton == MouseButton.Left && mapper.ButtonState == MouseButtonState.Released)
            {
                ViewModel.LeftMouseUp?.Execute(new MouseEvent(pos.ToPixedPoint(), e.Pointer.Type == PointerType.Touch));
            }
            else if (mapper.ChangedButton == MouseButton.Right && mapper.ButtonState == MouseButtonState.Released)
            {
                ViewModel.RightMouseUp?.Execute(new MouseEvent(pos.ToPixedPoint(), e.Pointer.Type == PointerType.Touch));
            }
            else if (mapper.ChangedButton == MouseButton.Middle && mapper.ButtonState == MouseButtonState.Released)
            {
                ViewModel.MiddleMouseUp?.Execute(new MouseEvent(pos.ToPixedPoint(), e.Pointer.Type == PointerType.Touch));
            }
        }
    }

    private void Canvas_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        ViewModel?.MouseWheel?.Execute(e.Delta.Y);
        //e.Handled = true;
    }

    private void Canvas_PointerExited(object? sender, PointerEventArgs e)
    {
        ViewModel?.MouseLeave?.Execute();
    }

    private void Canvas_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is Canvas canvas)
        {
            var pos = e.GetPosition(canvas);

            ViewModel?.MouseMove?.Execute(new MouseEvent(pos.ToPixedPoint(), e.Pointer.Type == PointerType.Touch));
        }
    }
}