using Memoria.Prime;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class ControllerWatcher : PersistenSingleton<ControllerWatcher>
{
    private const int WM_DEVICECHANGE = 0x0219;
    private const int DBT_DEVICEARRIVAL = 0x8000;
    private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
    private const int GWL_WNDPROC = -4;

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, WindowProcDelegate newProc);
    [DllImport("user32.dll")]
    private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    private WindowProcDelegate newWndProc;
    private IntPtr oldWndProc;

    private delegate IntPtr WindowProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    private String message;

    public void Initialize()
    {
        try
        {
            IntPtr windowHandle = GetActiveWindow();
            if (windowHandle != IntPtr.Zero)
            {
                newWndProc = new WindowProcDelegate(WindowProc);
                oldWndProc = SetWindowLongPtr(windowHandle, GWL_WNDPROC, newWndProc);

                Log.Message("[ControllerWatcher] USB device notification registered");
            }
            StartCoroutine("RefreshControllers");
        }
        catch (Exception e)
        {
            Log.Error("[ControllerWatcher] Failed to register device notifications: " + e.Message);
        }
    }

    private IntPtr WindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == WM_DEVICECHANGE)
        {
            switch (wParam.ToInt32())
            {
                case DBT_DEVICEARRIVAL:
                    message = "Device connected";
                    StopCoroutine("RefreshControllers");
                    StartCoroutine("RefreshControllers");
                    break;
                case DBT_DEVICEREMOVECOMPLETE:
                    message = "Device disconnected";
                    StopCoroutine("RefreshControllers");
                    StartCoroutine("RefreshControllers");
                    break;
            }
        }

        return CallWindowProc(oldWndProc, hWnd, msg, wParam, lParam);
    }

    private IEnumerator RefreshControllers()
    {
        Boolean connect = message == "Device connected";
        Int32 tries = connect ? 10 : 1;
        Boolean hasChanged = false;
        while (tries-- > 0)
        {
            yield return new WaitForSeconds(0.5f);
            hasChanged = XInputDotNetPure.GamePad.RefreshDevices();
            if (hasChanged) break;
        }

        if (hasChanged) Log.Message($"[ControllerWatcher] {message}");
        else if (connect) Log.Message($"[ControllerWatcher] Couldn't connect device");

        if (!connect && hasChanged && !PersistenSingleton<UIManager>.Instance.IsPause)
        {
            // Pause the game if a controller has been disconnected
            UIManager.Instance.GetSceneFromState(UIManager.Instance.State).OnKeyPause(null);
        }
        if (hasChanged && connect)
        {
            yield return new WaitForSeconds(0.5f);
            // Refresh one last time for Dualsense over buetooth
            XInputDotNetPure.GamePad.RefreshDevices();
        }
    }
}
