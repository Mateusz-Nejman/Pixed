﻿using Avalonia.Input;
using System.Collections.Generic;

namespace Pixed.Input
{
    internal static class Keyboard
    {
        private static Dictionary<Key, MouseButtonState> _keys = [];
        public static KeyModifiers Modifiers { get; set; } = KeyModifiers.None;

        public static void ProcessPressed(Key key)
        {
            if (_keys.ContainsKey(key))
            {
                _keys[key] = MouseButtonState.Pressed;
            }
            else
            {
                _keys.Add(key, MouseButtonState.Pressed);
            }
        }

        public static void ProcessReleased(Key key)
        {
            if (_keys.ContainsKey(key))
            {
                _keys[key] = MouseButtonState.Released;
            }
            else
            {
                _keys.Add(key, MouseButtonState.Released);
            }
        }

        public static bool IsKeyDown(Key key)
        {
            return _keys.ContainsKey(key) && _keys[key] == MouseButtonState.Pressed;
        }

        public static bool IsKeyUp(Key key)
        {
            return !_keys.ContainsKey(key) || _keys[key] == MouseButtonState.Released;
        }
    }
}