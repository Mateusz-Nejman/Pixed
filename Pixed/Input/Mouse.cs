using Avalonia.Input;

namespace Pixed.Input
{
    internal static class Mouse
    {
        public static MouseButton ButtonChanged { get; set; }
        public static MouseButtonState LeftButton { get; set; } = MouseButtonState.Released;
        public static MouseButtonState MiddleButton { get; set; } = MouseButtonState.Released;
        public static MouseButtonState RightButton { get; set; } = MouseButtonState.Released;
    }
}
