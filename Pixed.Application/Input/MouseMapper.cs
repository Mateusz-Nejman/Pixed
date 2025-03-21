﻿using Avalonia.Controls;
using Avalonia.Input;
using Pixed.Common.Input;

namespace Pixed.Application.Input;

internal class MouseMapper
{
    public MouseButton ChangedButton { get; }
    public MouseButtonState ButtonState { get; }

    public MouseMapper(PointerEventArgs eventArgs, Control control)
    {
        var point = eventArgs.GetCurrentPoint(control);
        Mouse.ProcessPoint(point);
        ChangedButton = Mouse.ButtonChanged;
        MouseButtonState state = MouseButtonState.Released;

        SetButtonState(ref state, MouseButton.Left, point.Properties.IsLeftButtonPressed);
        SetButtonState(ref state, MouseButton.Right, point.Properties.IsRightButtonPressed);
        SetButtonState(ref state, MouseButton.Middle, point.Properties.IsMiddleButtonPressed);
        ButtonState = state;
    }

    private void SetButtonState(ref MouseButtonState state, MouseButton mouseButton, bool value)
    {
        if (ChangedButton == mouseButton)
        {
            state = value ? MouseButtonState.Pressed : MouseButtonState.Released;
        }
    }
}
