namespace Pixed.Services.Keyboard
{
    internal class ShortcutService
    {
        private struct ShortcutEntry
        {
            public KeyState State { get; }
            public Action Action { get; }

            public ShortcutEntry(KeyState state, Action action)
            {
                State = state;
                Action = action;
            }
        }

        private readonly List<ShortcutEntry> _shortcuts;
        private readonly IDisposable _shortcutSubject;

        public ShortcutService()
        {
            _shortcuts = [];
            _shortcutSubject = Subjects.KeyState.Subscribe(state =>
            {
                foreach (var shortcut in _shortcuts)
                {
                    if (shortcut.State.Equals(state))
                    {
                        shortcut.Action?.Invoke();
                    }
                }
            });
        }

        public void Add(KeyState shortcut, Action action)
        {
            _shortcuts.Add(new ShortcutEntry(shortcut, action));
        }
    }
}
