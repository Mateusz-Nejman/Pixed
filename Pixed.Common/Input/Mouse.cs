using Avalonia.Input;

namespace Pixed.Input;

internal static class Mouse
{
    public static MouseButton ButtonChanged { get; set; }
    public static MouseButtonState LeftButton { get; set; } = MouseButtonState.Released;
    public static MouseButtonState MiddleButton { get; set; } = MouseButtonState.Released;
    public static MouseButtonState RightButton { get; set; } = MouseButtonState.Released;

    public static void ProcessPoint(PointerPoint point)
    {
        var newLeftButton = point.Properties.IsLeftButtonPressed ? MouseButtonState.Pressed : MouseButtonState.Released;
        var newRightButton = point.Properties.IsRightButtonPressed ? MouseButtonState.Pressed : MouseButtonState.Released;
        var newMiddleButton = point.Properties.IsMiddleButtonPressed ? MouseButtonState.Pressed : MouseButtonState.Released;

        if (newLeftButton != LeftButton)
        {
            ButtonChanged = MouseButton.Left;
        }
        else if (newRightButton != RightButton)
        {
            ButtonChanged = MouseButton.Right;
        }
        else if (newMiddleButton != MiddleButton)
        {
            ButtonChanged = MouseButton.Middle;
        }
        LeftButton = newLeftButton;
        MiddleButton = newMiddleButton;
        RightButton = newRightButton;
    }
}
