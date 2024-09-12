using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Input
{
    internal class MouseMapper
    {
        public MouseButton ChangedButton { get; }
        public MouseButtonState ButtonState { get; }

        public MouseMapper(PointerEventArgs eventArgs, Control control)
        {
            ChangedButton = Mouse.ButtonChanged;
            var point = eventArgs.GetCurrentPoint(control);
            MouseButtonState state = MouseButtonState.Released;

            SetButtonState(ref state, MouseButton.Left, point.Properties.IsLeftButtonPressed);
            SetButtonState(ref state, MouseButton.Right, point.Properties.IsRightButtonPressed);
            SetButtonState(ref state, MouseButton.Middle, point.Properties.IsMiddleButtonPressed);
            ButtonState = state;
        }

        private void SetButtonState(ref MouseButtonState state, MouseButton mouseButton, bool value)
        {
            if(ChangedButton == mouseButton)
            {
                state = value ? MouseButtonState.Pressed : MouseButtonState.Released;
            }
        }
    }
}
