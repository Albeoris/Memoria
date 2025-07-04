using Memoria;
using System;
using UnityEngine;

public class SoundLibWndProc : MonoBehaviour
{
    private const UInt32 WM_NCLBUTTONDOWN = 161u;
    private const UInt32 HTCAPTION = 2u;
    private const UInt32 WM_SYSKEYDOWN = 260u;
    private const UInt32 VK_RETURN = 13u;

    private Boolean isSEADSuspendedByTitleBar;

    private void Start()
    {
        WindowProc.OnMessage += WndProc;
    }

    private void Update()
    {
        if (this.isSEADSuspendedByTitleBar)
        {
            ISdLibAPIProxy.Instance.SdSoundSystem_Resume();
            this.isSEADSuspendedByTitleBar = false;
        }
    }

    private void WndProc(UInt32 msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == WM_NCLBUTTONDOWN && wParam.ToInt32() == HTCAPTION)
        {
            ISdLibAPIProxy.Instance.SdSoundSystem_Suspend();
            this.isSEADSuspendedByTitleBar = true;
        }
        if (msg == WM_SYSKEYDOWN && wParam.ToInt32() == VK_RETURN)
        {
            // Switching between fullscreen and windowed (Alt + Enter) may cause some visual artifacts (i.e. some body parts disappear)
            // This will prevent switching
            WindowProc.newMessage = 0u;
        }
    }
}
