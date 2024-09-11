using Pixed.Models;
using Pixed.Selection;
using Pixed.Services.Keyboard;
using System.Reactive.Subjects;

namespace Pixed
{
    internal static class Subjects
    {
        public static Subject<KeyState> KeyState { get; } = new Subject<KeyState>();
        public static Subject<bool> RefreshCanvas { get; } = new Subject<bool>();
        public static Subject<int> FrameChanged { get; } = new Subject<int>();

        public static Subject<BaseSelection> SelectionCreated { get; } = new Subject<BaseSelection>();
        public static Subject<BaseSelection> SelectionDismissed { get; } = new Subject<BaseSelection>();
        public static Subject<BaseSelection> SelectionMoveRequest { get; } = new Subject<BaseSelection>();
        public static Subject<BaseSelection> ClipboardCopy { get; } = new Subject<BaseSelection>();
        public static Subject<BaseSelection> ClipboardCut { get; } = new Subject<BaseSelection>();
        public static Subject<BaseSelection> ClipboardPaste { get; } = new Subject<BaseSelection>();
        public static Subject<UniColor> PrimaryColorChanged { get; } = new Subject<UniColor>();
        public static Subject<UniColor> PrimaryColorChange { get; } = new Subject<UniColor>();
        public static Subject<UniColor> SecondaryColorChanged { get; } = new Subject<UniColor>();
        public static Subject<PaletteModel> PaletteAdded { get; } = new Subject<PaletteModel>();
        public static Subject<PaletteModel> PaletteSelected { get; } = new Subject<PaletteModel>();
    }
}
