using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.Services.Keyboard
{
    internal class Shortcut
    {
        private string _id;
        private string _description;
        private Key _key;
        private bool _shift;
        private bool _control;
        private bool _alt;

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

            if(_control)
            {
                prefix += "Ctrl+";
            }

            if(_alt)
            {
                prefix += "Alt+";
            }

            if(_shift)
            {
                prefix += "Shift+";
            }

            return prefix + Enum.GetName<Key>(_key);
        }
    }
}
