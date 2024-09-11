using Avalonia.Input;
using System;

namespace Pixed.Services.Keyboard
{
    internal class Shortcut
    {
        private readonly string _id;
        private readonly string _description;
        private readonly Key _key;
        private readonly bool _shift;
        private readonly bool _control;
        private readonly bool _alt;

        public Shortcut(string id, string description, Key key, bool shift, bool control, bool alt)
        {
            _id = id;
            _description = description;
            _key = key;
            _shift = shift;
            _control = control;
            _alt = alt;
        }

        public string GetDisplayShortcut()
        {
            string prefix = "";

            if (_control)
            {
                prefix += "Ctrl+";
            }

            if (_alt)
            {
                prefix += "Alt+";
            }

            if (_shift)
            {
                prefix += "Shift+";
            }

            return prefix + Enum.GetName<Key>(_key);
        }
    }
}
