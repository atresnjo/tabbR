using System;
using System.Runtime.InteropServices;

namespace tabbR.Application
{
    public class WinApi
    {
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        public static int SW_SHOWNORMAL = 1;
        public static int SW_FORCEMINIMIZE = 11;
    }
}
