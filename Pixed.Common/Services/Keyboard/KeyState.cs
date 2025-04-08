using Avalonia.Input;
using System;

namespace Pixed.Common.Services.Keyboard;

public class KeyState(Key key, bool pressed, bool isShift, bool isCtrl, bool isAlt) : IEquatable<KeyState>
{
    public Key Key { get; } = key;
    public bool Pressed { get; } = pressed;
    public bool IsShift { get; } = isShift;
    public bool IsCtrl { get; } = isCtrl;
    public bool IsAlt { get; } = isAlt;

    public KeyState() : this(Key.None, false, false, false, false)
    {
    }

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
        return other.Key == Key && other.IsShift == IsShift && other.IsCtrl == IsCtrl && other.IsAlt == IsAlt && other.Pressed == Pressed;
    }

    public override int GetHashCode()
    {
        HashCode hc = new();
        hc.Add(Key);
        hc.Add(IsShift);
        hc.Add(IsCtrl);
        hc.Add(IsAlt);
        hc.Add(Pressed);

        return hc.ToHashCode();
    }

    public static KeyState Control(Key key)
    {
        return new KeyState(key, true, false, true, false);
    }
}
