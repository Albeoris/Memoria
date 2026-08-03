using Assets.Scripts.Common;
using Memoria.Prime;
using Memoria.Prime.WinAPI;
using System;
using System.ComponentModel;
using UnityEngine;
using Object = System.Object;

namespace Memoria
{
    public sealed class PlayerWindow
    {
        private static readonly Object _lock = new Object();
        private static volatile PlayerWindow _instance;
        private static string _currentwindowtitle;

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

        public static void UpdateTitle(Boolean InBattle = false) // Maybe something to handle FMV too ?
        {
            string TextTile = "";
            if (InBattle)
            {
                TextTile = _currentwindowtitle;
                TextTile += $" | Battle/Group: {FF9StateSystem.Common.FF9.btlMapNo}/{FF9StateSystem.Battle?.FF9Battle?.btl_scene?.PatNum}";
            }
            else
            {
                if (SceneDirector.IsFieldScene())
                {
                    String camIdxIfCam = (PersistenSingleton<EventEngine>.Instance?.fieldmap?.scene?.cameraList.Count > 1 && PersistenSingleton<EventEngine>.Instance?.fieldmap?.camIdx != -1) ? "-" + PersistenSingleton<EventEngine>.Instance.fieldmap.camIdx : "";
                    TextTile = $"Map: {FF9StateSystem.Common.FF9.fldMapNo}{camIdxIfCam} ({FF9StateSystem.Common.FF9.mapNameStr}) | Index/Counter: {PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR)}/{PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR)} | Loc: {FF9StateSystem.Common.FF9.fldLocNo}";
                }
                else if (SceneDirector.IsWorldScene())
                {
                    String wldLocName = ff9.w_worldLocationName();
                    if (String.IsNullOrEmpty(wldLocName))
                        TextTile = $"World Map: {FF9StateSystem.Common.FF9.wldMapNo}";
                    else
                        TextTile = $"World Map: {FF9StateSystem.Common.FF9.wldMapNo}, {wldLocName}";
                }
                _currentwindowtitle = TextTile;
            }


            Instance.SetTitle(TextTile);
        }
    }
}
