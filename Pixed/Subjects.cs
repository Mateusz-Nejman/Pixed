using Pixed.Services.Keyboard;
using System.Reactive.Subjects;

namespace Pixed
{
    internal static class Subjects
    {
        public static Subject<KeyState> KeyState { get; } = new Subject<KeyState>();
        public static Subject<bool> RefreshCanvas { get; } = new Subject<bool>();
    }
}
