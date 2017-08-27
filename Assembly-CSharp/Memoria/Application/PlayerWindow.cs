using System;
using System.ComponentModel;
using Memoria.Prime;
using Memoria.Prime.WinAPI;
using UnityEngine;
using Object = System.Object;

namespace Memoria
{
    public sealed class PlayerWindow
    {
        private static readonly Object _lock = new Object();
        private static volatile PlayerWindow _instance;

        public static PlayerWindow Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_lock)
                    _instance = new PlayerWindow();

                return _instance;
            }
        }

        private const String OriginalTitle = "FINAL FANTASY IX";
        private readonly IntPtr _windowHandle;

        public PlayerWindow()
        {
            if (Application.platform != RuntimePlatform.WindowsPlayer)
                return;

            try
            {
                _windowHandle = User32.FindWindow(null, OriginalTitle);
                if (_windowHandle == IntPtr.Zero)
                    throw new Win32Exception();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Cannot find Unity Player Window.");
            }
        }

        public void SetTitle(String message)
        {
            if (_windowHandle != IntPtr.Zero)
                User32.SetWindowText(_windowHandle, OriginalTitle + " - " + message);
        }
    }
}