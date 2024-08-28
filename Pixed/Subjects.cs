using Pixed.Selection;
using Pixed.Services.Keyboard;
using System.Reactive.Subjects;

namespace Pixed
{
    internal static class Subjects
    {
        public static Subject<KeyState> KeyState { get; } = new Subject<KeyState>();
        public static Subject<bool> RefreshCanvas { get; } = new Subject<bool>();

        public static Subject<BaseSelection> SelectionCreated { get; } = new Subject<BaseSelection>();
        public static Subject<BaseSelection> SelectionDismissed { get; } = new Subject<BaseSelection>();
        public static Subject<BaseSelection> SelectionMoveRequest { get; } = new Subject<BaseSelection>();
        public static Subject<BaseSelection> ClipboardCopy { get; } = new Subject<BaseSelection>();
        public static Subject<BaseSelection> ClipboardCut { get; } = new Subject<BaseSelection>();
        public static Subject<BaseSelection> ClipboardPaste { get; } = new Subject<BaseSelection>();
    }
}
