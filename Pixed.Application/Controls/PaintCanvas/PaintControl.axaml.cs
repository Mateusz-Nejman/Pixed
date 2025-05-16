using Avalonia.Controls;
using Avalonia.Input;
using Pixed.Application.DependencyInjection;
using Pixed.Application.Input;
using Pixed.Application.Utils;
using Pixed.Application.ViewModels;
using Pixed.Application.Zoom;
using Pixed.Common.Input;
using Pixed.Core.Models;
using System;

namespace Pixed.Application.Controls.PaintCanvas;

internal partial class PaintControl : ExtendedControl<PaintControlViewModel>
{
    private readonly int _scrollBarSize = 18;
    private readonly IDisposable _zoomChanged;
    public PaintControl() : base()
    {
        InitializeComponent();
        SizeChanged += PaintCanvas_SizeChanged;
        zoomControl.GesturesEnabled = false;
        ViewModel.GridCanvas = gridCanvas;
        ViewModel.SelectionOverlay = selectionOverlay;
        ViewModel.CursorOverlay = cursorOverlay;
        ViewModel.TransparentBackground = transparentBackground;
        ViewModel.ZoomValue = zoomControl.Zoom;
        ViewModel.ZoomOffsetX = zoomControl.Offset.X;
        ViewModel.ZoomOffsetY = zoomControl.Offset.Y;
        ViewModel.ZoomContainer = zoomControl;
        Unloaded += PaintCanvas_Unloaded;

        _zoomChanged = ZoomControl.ZoomChanged.Subscribe(_ =>
        {
            var applicationData = this.GetServiceProvider().Get<ApplicationData>();
            zoomControl.ConfigureOffsetBounds(applicationData.CurrentFrame.Width, applicationData.CurrentFrame.Height);
            ViewModel.RefreshGridCanvas();
        });

        zoomControl.PointerPressedCommand = new Core.ActionCommand<Control, PointerPressedEventArgs>(OnPointerPressed);
        zoomControl.PointerReleasedCommand = new Core.ActionCommand<Control, PointerReleasedEventArgs>(OnPointerReleased);
        zoomControl.PointerWheelCommand = new Core.ActionCommand<Control, PointerWheelEventArgs>(OnPointerWheelChanged);
        zoomControl.PointerExitedCommand = new Core.ActionCommand<Control, PointerEventArgs>(OnPointerExited);
        zoomControl.PointerMovedCommand = new Core.ActionCommand<Control, PointerEventArgs>(OnPointerMoved);
    }

    private void PaintCanvas_Unloaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _zoomChanged?.Dispose();
    }

    private void PaintCanvas_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        ViewModel.RecalculateFactor(new Point((int)(e.NewSize.Width - _scrollBarSize), (int)(e.NewSize.Height - _scrollBarSize)));

        var applicationData = this.GetServiceProvider().Get<ApplicationData>();
        int projectWidth = applicationData.CurrentModel.Width;
        int projectHeight = applicationData.CurrentModel.Height;
        double deltaX = e.NewSize.Width / projectWidth;
        double deltaY = e.NewSize.Height / projectHeight;
        zoomControl.ZoomTo(Math.Min(deltaX, deltaY), new Avalonia.Point(projectWidth / 2, projectHeight / 2), Avalonia.Matrix.Identity);
    }

    private void OnPointerPressed(Control sender, PointerPressedEventArgs e)
    {
        var pos = e.GetPosition(sender);
        MouseMapper mapper = new(e, sender);

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

    private void OnPointerReleased(Control sender, PointerReleasedEventArgs e)
    {
        var pos = e.GetPosition(sender);
        MouseMapper mapper = new(e, sender);

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

    private void OnPointerWheelChanged(Control sender, PointerWheelEventArgs e)
    {
        ViewModel?.MouseWheel?.Execute(e.Delta.Y);
        //e.Handled = true;
    }

    private void OnPointerExited(Control sender, PointerEventArgs e)
    {
        ViewModel?.MouseLeave?.Execute();
    }

    private void OnPointerMoved(Control sender, PointerEventArgs e)
    {
        var pos = e.GetPosition(sender);

        ViewModel?.MouseMove?.Execute(new MouseEvent(pos.ToPixedPoint(), e.Pointer.Type == PointerType.Touch));
    }
}