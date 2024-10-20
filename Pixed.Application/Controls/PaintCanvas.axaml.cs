﻿using Avalonia.Controls;
using Avalonia.Input;
using Pixed.Application.Input;
using Pixed.Application.Utils;
using Pixed.Application.ViewModels;
using Pixed.Common;
using Pixed.Common.Input;
using System.Drawing;

namespace Pixed.Application.Controls;

internal partial class PaintCanvas : PixedUserControl<PaintCanvasViewModel>
{
    private readonly int _scrollBarSize = 18;
    public PaintCanvas() : base()
    {
        InitializeComponent();
        SizeChanged += PaintCanvas_SizeChanged;
        this.AddHandler(Avalonia.Input.Gestures.PinchEvent, PinchHandler);
    }

    private void PinchHandler(object? sender, PinchEventArgs e)
    {
        Subjects.ZoomChanged.OnNext(e.Scale);
    }

    private void PaintCanvas_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        ViewModel.RecalculateFactor(new Point((int)(e.NewSize.Width - _scrollBarSize), (int)(e.NewSize.Height - _scrollBarSize)));
    }

    private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border)
        {
            var pos = e.GetPosition(border);
            MouseMapper mapper = new(e, border);

            if (mapper.ChangedButton == MouseButton.Left && mapper.ButtonState == MouseButtonState.Pressed)
            {
                ViewModel.LeftMouseDown?.Execute(new MouseEvent(pos.ToSystemPoint(), e.Pointer.Type == PointerType.Touch));
            }
            else if (mapper.ChangedButton == MouseButton.Right && mapper.ButtonState == MouseButtonState.Pressed)
            {
                ViewModel.RightMouseDown?.Execute(new MouseEvent(pos.ToSystemPoint(), e.Pointer.Type == PointerType.Touch));
            }
            else if (mapper.ChangedButton == MouseButton.Middle && mapper.ButtonState == MouseButtonState.Pressed)
            {
                ViewModel.MiddleMouseDown?.Execute(new MouseEvent(pos.ToSystemPoint(), e.Pointer.Type == PointerType.Touch));
            }
        }
    }

    private void Border_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Border border)
        {
            var pos = e.GetPosition(border);
            MouseMapper mapper = new(e, border);

            if (mapper.ChangedButton == MouseButton.Left && mapper.ButtonState == MouseButtonState.Released)
            {
                ViewModel.LeftMouseUp?.Execute(new MouseEvent(pos.ToSystemPoint(), e.Pointer.Type == PointerType.Touch));
            }
            else if (mapper.ChangedButton == MouseButton.Right && mapper.ButtonState == MouseButtonState.Released)
            {
                ViewModel.RightMouseUp?.Execute(new MouseEvent(pos.ToSystemPoint(), e.Pointer.Type == PointerType.Touch));
            }
            else if (mapper.ChangedButton == MouseButton.Middle && mapper.ButtonState == MouseButtonState.Released)
            {
                ViewModel.MiddleMouseUp?.Execute(new MouseEvent(pos.ToSystemPoint(), e.Pointer.Type == PointerType.Touch));
            }
        }
    }

    private void Border_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        ViewModel?.MouseWheel?.Execute(e.Delta.Y);
        e.Handled = true;
    }

    private void Border_PointerExited(object? sender, PointerEventArgs e)
    {
        ViewModel?.MouseLeave?.Execute();
    }

    private void Border_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is Border border)
        {
            var pos = e.GetPosition(border);

            ViewModel?.MouseMove?.Execute(new MouseEvent(pos.ToSystemPoint(), e.Pointer.Type == PointerType.Touch));
        }
    }
}