using Memoria.Prime;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class ControllerWatcher : PersistenSingleton<ControllerWatcher>
{
    private const int WM_DEVICECHANGE = 0x0219;
    private const int DBT_DEVICEARRIVAL = 0x8000;
    private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

    private static Boolean isConnect = true;
    private static Int32 tries = 1;

    private static Thread refreshThread = new Thread(RefreshControllers);

    public void Start()
    {
        WindowProc.OnMessage += OnMessage;
    }

    public void StartRefresh()
    {
        if(refreshThread.IsAlive)
            refreshThread.Abort();
        refreshThread = new Thread(RefreshControllers);
        refreshThread.Start();
    }

    private void OnMessage(uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == WM_DEVICECHANGE)
        {
            switch (wParam.ToInt32())
            {
                case DBT_DEVICEARRIVAL:
                    isConnect = true;
                    tries = 10;
                    StartRefresh();
                    break;
                case DBT_DEVICEREMOVECOMPLETE:
                    isConnect = false;
                    tries = 1;
                    StartRefresh();
                    break;
            }
        }
    }

    private static void RefreshControllers()
    {
        try
        {
            Boolean hasChanged = false;
            while (tries-- > 0)
            {
                Thread.Sleep(500);
                hasChanged = XInputDotNetPure.GamePad.RefreshDevices();
                if (hasChanged) break;
            }

            if (hasChanged) Log.Message($"[ControllerWatcher] Device {(isConnect ? "connected" : "disconnected")}");
            else if (isConnect) Log.Message($"[ControllerWatcher] Couldn't connect device");

            if (!isConnect && hasChanged && !PersistenSingleton<UIManager>.Instance.IsPause)
            {
                // Pause the game if a controller has been disconnected
                UIManager.Instance.GetSceneFromState(UIManager.Instance.State).OnKeyPause(null);
            }
            if (hasChanged && isConnect)
            {
                Thread.Sleep(500);
                // Refresh one last time for DualSense over Bluetooth
                XInputDotNetPure.GamePad.RefreshDevices(true);
            }
        }
        catch(ThreadAbortException) { }
        catch(Exception e)
        {
            Log.Warning($"[ControllerWatcher] Couldn't refresh devices");
            Log.Warning(e);
        }
    }
}
