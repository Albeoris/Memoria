using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Memoria.Launcher.Controller
{
    /// <summary>
    /// Maps controller actions to the native Win32 MessageBox owned by Launcher.
    /// </summary>
    internal static class NativeDialogControllerBridge
    {
        private const UInt32 Command = 0x0111;
        private const UInt32 KeyDown = 0x0100;
        private const UInt32 KeyUp = 0x0101;
        private const Int32 OkButton = 1;
        private const Int32 CancelButton = 2;
        private const Int32 NoButton = 7;
        private const Int32 Enter = 0x0D;
        private const Int32 Escape = 0x1B;
        private const Int32 ArrowLeft = 0x25;
        private const Int32 ArrowUp = 0x26;
        private const Int32 ArrowRight = 0x27;
        private const Int32 ArrowDown = 0x28;
        private const String DialogWindowClass = "#32770";

        public static Boolean IsMessageBoxActiveForCurrentProcess()
        {
            IntPtr window = GetForegroundWindow();
            if (window == IntPtr.Zero || !String.Equals(GetWindowClass(window), DialogWindowClass, StringComparison.Ordinal))
                return false;

            GetWindowThreadProcessId(window, out UInt32 processId);
            return processId == (UInt32)Process.GetCurrentProcess().Id;
        }

        public static void Send(ControllerButton actions)
        {
            if ((actions & ControllerButton.Cancel) != 0)
                Cancel();
            else if ((actions & ControllerButton.Confirm) != 0)
                SendKey(Enter);
            else if ((actions & ControllerButton.Up) != 0)
                SendKey(ArrowUp);
            else if ((actions & ControllerButton.Down) != 0)
                SendKey(ArrowDown);
            else if ((actions & ControllerButton.Left) != 0)
                SendKey(ArrowLeft);
            else if ((actions & ControllerButton.Right) != 0)
                SendKey(ArrowRight);
        }

        private static String GetWindowClass(IntPtr window)
        {
            StringBuilder result = new StringBuilder(64);
            return GetClassName(window, result, result.Capacity) > 0 ? result.ToString() : String.Empty;
        }

        private static void Cancel()
        {
            IntPtr window = GetForegroundWindow();
            if (window == IntPtr.Zero)
                return;

            if (TryInvokeDialogButton(window, CancelButton) ||
                TryInvokeDialogButton(window, NoButton) ||
                TryInvokeDialogButton(window, OkButton))
            {
                return;
            }

            SendKey(window, Escape);
        }

        private static Boolean TryInvokeDialogButton(IntPtr window, Int32 buttonId)
        {
            IntPtr button = GetDlgItem(window, buttonId);
            return button != IntPtr.Zero && PostMessage(window, Command, new IntPtr(buttonId), button);
        }

        private static void SendKey(Int32 virtualKey)
        {
            IntPtr window = GetForegroundWindow();
            if (window != IntPtr.Zero)
                SendKey(window, virtualKey);
        }

        private static void SendKey(IntPtr window, Int32 virtualKey)
        {
            PostMessage(window, KeyDown, new IntPtr(virtualKey), IntPtr.Zero);
            PostMessage(window, KeyUp, new IntPtr(virtualKey), IntPtr.Zero);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern Int32 GetClassName(IntPtr window, StringBuilder className, Int32 maximumCount);

        [DllImport("user32.dll")]
        private static extern UInt32 GetWindowThreadProcessId(IntPtr window, out UInt32 processId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDlgItem(IntPtr dialog, Int32 itemId);

        [DllImport("user32.dll")]
        private static extern Boolean PostMessage(IntPtr window, UInt32 message, IntPtr wParam, IntPtr lParam);
    }
}
