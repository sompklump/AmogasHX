using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AmongUsHack.Exts
{
    public class GlobalHotkeys
    {
        public ModifierKeys modifier { get; set; }
        public Key key { get; set; }
        public Action callback { get; set; }
        public bool canExecute { get; set; }

        public GlobalHotkeys(ModifierKeys _modifier, Key _key, Action _callback, bool _canExecute = true)
        {
            modifier = _modifier;
            key = _key;
            canExecute = _canExecute;
            callback = _callback;
        }
    }
}
