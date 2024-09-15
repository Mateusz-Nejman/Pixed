using Avalonia.Input;
using System;

namespace Pixed.Services.Keyboard;

internal class KeyState(Key key, bool isShift, bool isCtrl, bool isAlt) : IEquatable<KeyState>
{
    public Key Key { get; } = key;
    public bool IsShift { get; } = isShift;
    public bool IsCtrl { get; } = isCtrl;
    public bool IsAlt { get; } = isAlt;

    public override bool Equals(object? obj)
    {
        if (obj is KeyState state)
        {
            return Equals(state);
        }

        return false;
    }

    public bool Equals(KeyState? other)
    {
        if (other == null)
        {
            return false;
        }
        return other.Key == Key && other.IsShift == IsShift && other.IsCtrl == IsCtrl && other.IsAlt == IsAlt;
    }
}
