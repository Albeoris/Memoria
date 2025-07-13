using Memoria.Prime;
using System;
using System.Collections;
using UnityEngine;

public class ControllerWatcher : PersistenSingleton<ControllerWatcher>
{
    private const int WM_DEVICECHANGE = 0x0219;
    private const int DBT_DEVICEARRIVAL = 0x8000;
    private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

    private Boolean isConnect = true;

    public void Start()
    {
        WindowProc.OnMessage += OnMessage;
    }

    private void OnMessage(uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == WM_DEVICECHANGE)
        {
            switch (wParam.ToInt32())
            {
                case DBT_DEVICEARRIVAL:
                    isConnect = true;
                    StopCoroutine("RefreshControllers");
                    StartCoroutine("RefreshControllers");
                    break;
                case DBT_DEVICEREMOVECOMPLETE:
                    isConnect = false;
                    StopCoroutine("RefreshControllers");
                    StartCoroutine("RefreshControllers");
                    break;
            }
        }
    }

    private IEnumerator RefreshControllers()
    {
        Int32 tries = isConnect ? 10 : 1;
        Boolean hasChanged = false;
        while (tries-- > 0)
        {
            yield return new WaitForSeconds(0.5f);
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
            yield return new WaitForSeconds(0.5f);
            // Refresh one last time for DualSense over Bluetooth
            XInputDotNetPure.GamePad.RefreshDevices(true);
        }
        yield break;
    }
}
