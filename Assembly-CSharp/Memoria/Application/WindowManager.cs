using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Memoria
{
    public class WindowManager
    {
        [DllImport("user32.dll")]
        private static extern void MoveWindow(IntPtr hwnd, int X, int Y, int width, int height, bool repaint);

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);
        delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hmonitor, [In, Out] MONITORINFOEX info);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        public class MONITORINFOEX
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szDevice = new char[32];
        }

        public static List<MONITORINFOEX> Displays { get; private set; } = GetDisplays();

        public static void AlignWindow()
        {
            try
            {
                Int32 monitor = 0;
                Int32 windowMode = 0;

                Int32.TryParse(IniFile.SettingsIni.GetSetting("Settings", "ActiveMonitor", "0 -").Split(' ')[0], out monitor);
                Int32.TryParse(IniFile.SettingsIni.GetSetting("Settings", "WindowMode", "0"), out windowMode);

                String alignment = IniFile.SettingsIni.GetSetting("Settings", "WindowPosition", "center").ToLower();

                IntPtr hWindow = GetActiveWindow();
                RECT windowRect = new RECT();
                GetWindowRect(hWindow, ref windowRect);

                if (monitor > Displays.Count - 1) return;
                var display = Displays[monitor]; // Hopefully it's the same order

                RECT rect = windowMode == 1 || windowMode == 2 ? display.rcMonitor : display.rcWork;

                int displayWidth = rect.right - rect.left;
                int displayHeight = rect.bottom - rect.top;

                // In mode fullscreen or borderless we use the selected display resolution for the window size
                int width = displayWidth;
                int height = displayHeight;

                if (windowMode == 0 || windowMode == 3)
                {
                    Int32 targetWidth = 0;
                    Int32 targetHeight = 0;
                    String[] resolution = IniFile.SettingsIni.GetSetting("Settings", "ScreenResolution", "0x0").Split('x');
                    if (resolution.Length == 2)
                    {
                        Int32.TryParse(resolution[0], out targetWidth);
                        Int32.TryParse(resolution[1], out targetHeight);
                    }

                    int borderWidth = windowRect.right - windowRect.left - Screen.width;
                    int borderHeight = windowRect.bottom - windowRect.top - Screen.height;

                    // Auto resolution
                    if (targetWidth == 0 || targetHeight == 0)
                    {
                        width = displayWidth;
                        height = displayHeight;
                    }
                    else
                    {
                        // We make sure the window fits into the screen
                        height = Math.Min(displayHeight, windowRect.bottom - windowRect.top);
                        width = (height - borderHeight) * targetWidth / targetHeight + borderWidth; // We want to keep the propotions

                        if (width > displayWidth)
                        {
                            width = displayWidth;
                            height = (width - borderWidth) * targetHeight / targetWidth + borderHeight;
                        }
                    }
                }

                int x = rect.left + (displayWidth - width) / 2;
                int y = rect.top + (displayHeight - height) / 2;

                if (alignment.Contains("top"))
                    y = rect.top;
                else if (alignment.Contains("bottom"))
                    y = rect.top + (displayHeight - height);

                if(alignment.Contains("left"))
                    x = rect.left;
                else if (alignment.Contains("right"))
                    x = rect.left + (displayWidth - width);

                if (alignment.Contains(","))
                {
                    String[] tokens = alignment.Split(',');
                    Int32 newX, newY;
                    if (Int32.TryParse(tokens[0].Trim(), out newX) && Int32.TryParse(tokens[1].Trim(), out newY))
                    {
                        x = rect.left + newX;
                        y = rect.top + newY;
                    }
                }

                if (x != windowRect.left || y != windowRect.top)
                {
                    Log.Message($"[WindowManager] Moving window to ({x},{y}) with size ({width},{height}) on monitor {monitor}");
                    MoveWindow(hWindow, x, y, width, height, true);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private static List<MONITORINFOEX> GetDisplays()
        {
            List<MONITORINFOEX> col = new List<MONITORINFOEX>();

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
                {
                    MONITORINFOEX mi = new MONITORINFOEX();
                    mi.cbSize = Marshal.SizeOf(mi);
                    if (GetMonitorInfo(hMonitor, mi))
                    {
                        col.Add(mi);
                    }
                    return true;
                }, IntPtr.Zero);
            return col;
        }
    }
}
