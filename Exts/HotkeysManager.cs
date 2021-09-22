using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AmongUsHack.Exts
{
    public static class HotkeysManager
    {
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static LowLevelKeyboardProc lowLevelProc = HookCallback;

        private static List<GlobalHotkeys> hotkeys { get; set; }

        private const int WH_KEYBOARD_LL = 13;
        private static IntPtr hookId = IntPtr.Zero;
        public static bool isHookSetup { get; set; }
        static HotkeysManager()
        {
            hotkeys = new List<GlobalHotkeys>();
        }
        public static void SetupSystemHook()
        {
            if (isHookSetup) return;
            hookId = SetHook(lowLevelProc);
            isHookSetup = true;
        }
        public static void ShutdownSystemHook()
        {
            if (!isHookSetup) return;
            UnhookWindowsHookEx(hookId);
            isHookSetup = false;
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using(Process currProc = Process.GetCurrentProcess())
            {
                using(ProcessModule currMod = currProc.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(currMod.ModuleName), 0);
                }
            }
        }

        public static void AddHotkey(GlobalHotkeys hotkey)
        {
            hotkeys.Add(hotkey);
        }
        public static void RemoveHotkey(GlobalHotkeys hotkey)
        {
            hotkeys.Remove(hotkey);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Hotkeys will be scanned here
            if(nCode >= 0)
            {
                // Check hotkeys
                foreach(GlobalHotkeys glh in hotkeys)
                {
                    if(Keyboard.Modifiers == glh.modifier && Keyboard.IsKeyDown(glh.key))
                    {
                        if (glh.canExecute)
                        {
                            glh.callback?.Invoke();
                        }
                    }
                }
            }
            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        // Set up hooks
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        // Get the module's handle
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

    }
}
