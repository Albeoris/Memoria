using Memoria.Prime;
using System;
using System.Runtime.InteropServices;

public static class WindowProc
{
    private const int GWL_WNDPROC = -4;

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowLong(IntPtr hWnd, Int32 nIndex, WindowProcDelegate dwNewLong);
    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, Int32 nIndex, WindowProcDelegate newProc);
    [DllImport("user32.dll")]
    private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

    private static WindowProcDelegate newWndProc;
    private static IntPtr oldWndProc = Initialize();

    private delegate IntPtr WindowProcDelegate(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

    public delegate void OnMessageDelegate(UInt32 msg, IntPtr wParam, IntPtr lParam);

    public static event OnMessageDelegate OnMessage;
    public static UInt32 newMessage;

    private static IntPtr Initialize()
    {
        try
        {
            IntPtr windowHandle = GetActiveWindow();
            if (windowHandle != IntPtr.Zero)
            {
                newWndProc = new WindowProcDelegate(WndProc);

                Log.Message("[WindowProc] Windows procedure registered");
                if(Is64Bit())
                    return SetWindowLongPtr(windowHandle, GWL_WNDPROC, newWndProc);
                else
                    return SetWindowLong(windowHandle, GWL_WNDPROC, newWndProc);
            }
            Log.Message("[WindowProc] Couldn't get active window handle");
        }
        catch (Exception e)
        {
            Log.Error("[WindowProc] Failed to register Windows procedure");
            Log.Error(e);
        }
        return IntPtr.Zero;
    }

    private static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        newMessage = msg;
        OnMessage?.Invoke(msg, wParam, lParam);
        return CallWindowProc(oldWndProc, hWnd, newMessage, wParam, lParam);
    }

    private static Boolean Is64Bit()
    {
        String environmentVariable = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
        return !String.IsNullOrEmpty(environmentVariable) && !(environmentVariable.Substring(0, 3) == "x86");
    }
}
