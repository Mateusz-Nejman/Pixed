using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.Services.Keyboard
{
    internal class KeyState
    {
        public Key Key { get; }
        public bool IsShift { get; }
        public bool IsCtrl { get; }
        public bool IsAlt { get; }

        public KeyState(Key key, bool isShift, bool isCtrl, bool isAlt)
        {
            Key = key;
            IsShift = isShift;
            IsCtrl = isCtrl;
            IsAlt = isAlt;
        }

        public bool Equals(KeyState other)
        {
            return other.Key == Key && other.IsShift == IsShift && other.IsCtrl == IsCtrl && other.IsAlt == IsAlt;
        }
    }
}
