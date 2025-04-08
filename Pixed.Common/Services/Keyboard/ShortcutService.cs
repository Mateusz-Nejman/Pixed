using Avalonia.Input;
using Pixed.Common.Models;
using Pixed.Core.Models;
using System;
using System.Collections.Generic;

namespace Pixed.Common.Services.Keyboard;

public class ShortcutService
{
    private readonly ApplicationData _applicationData;
    private readonly struct ShortcutEntry(KeyState state, Action action)
    {
        public KeyState State { get; } = state;
        public Action Action { get; } = action;
    }

    private readonly List<ShortcutEntry> _shortcuts;
    private readonly IDisposable _shortcutSubject;

    public ShortcutService(ApplicationData applicationData)
    {
        _applicationData = applicationData;
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

        Add(KeyState.Control(Key.Z), () =>
        {
            _applicationData.CurrentModel.Undo();
            Subjects.ProjectModified.OnNext(_applicationData.CurrentModel);
        });
        Add(KeyState.Control(Key.Y), () =>
        {
            _applicationData.CurrentModel.Redo();
            Subjects.ProjectModified.OnNext(_applicationData.CurrentModel);
        });
    }

    public void Add(KeyState shortcut, Action action)
    {
        _shortcuts.Add(new ShortcutEntry(shortcut, action));
    }
}
