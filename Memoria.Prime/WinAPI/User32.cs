using System;
using System.Runtime.InteropServices;

namespace Memoria.Prime.WinAPI
{
    public static class User32
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern Boolean SetWindowText(IntPtr hwnd, String lpString);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindWindow(String className, String windowName);
    }
}