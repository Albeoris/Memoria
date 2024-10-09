using Memoria.Prime;
using System;
using UnityEngine;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Graphics
        {
            public static Int32 BattleFPS => Instance._graphics.BattleFPS;
            public static Int32 BattleTPS => Instance._graphics.BattleTPS;
            public static Int32 FieldFPS => Instance._graphics.FieldFPS;
            public static Int32 FieldTPS => Instance._graphics.FieldTPS;
            public static Int32 WorldFPS => Instance._graphics.WorldFPS;
            public static Int32 WorldTPS => Instance._graphics.WorldTPS;
            public static Int32 MenuFPS => Instance._graphics.MenuFPS;
            public static Int32 MenuTPS => Instance._graphics.MenuTPS;
            public static Int32 BattleSwirlFrames => Instance._graphics.BattleSwirlFrames;
            public static Int32 SkipIntros => Instance._graphics.SkipIntros;
            public static Int32 GarnetHair => Instance._graphics.GarnetHair;
            public static Int32 TileSize => Instance._graphics.TileSize;
            public static Int32 AntiAliasing => Instance._graphics.AntiAliasing;
            public static Int32 CameraStabilizer => Instance._graphics.CameraStabilizer;
            public static Int32 FieldSmoothTexture => Instance._graphics.FieldSmoothTexture;
            public static Int32 WorldSmoothTexture => Instance._graphics.WorldSmoothTexture;
            public static Int32 BattleSmoothTexture => Instance._graphics.BattleSmoothTexture;
            public static Int32 ElementsSmoothTexture => Instance._graphics.ElementsSmoothTexture;
            public static Int32 SFXSmoothTexture => Instance._graphics.SFXSmoothTexture;
            public static Int32 UISmoothTexture => Instance._graphics.UISmoothTexture;

            private static volatile Boolean _widescreenSupport = InitializeWidescreenSupport();

            public static Boolean InitializeWidescreenSupport()
            {
                if (!Instance._graphics.WidescreenSupport || (Instance._debug.Enabled && Instance._debug.StartFieldCreator) || Math.Abs((Double)Screen.width / (Double)Screen.height) < 1.34)
                {
                    return false;
                }
                return true;
                // if ((Math.Abs(((Double)Screen.width / (Double)Screen.height) - (16d / 9d)) < 0.01) || (Math.Abs(((Double)Screen.width / (Double)Screen.height) - (16d / 10d)) < 0.01))
            }

            public static Boolean ScreenIs16to10()
            {
                if (Math.Abs(((Double)Screen.width / (Double)Screen.height) - (16d / 10d)) < 0.01)
                    return true;

                return false;
            }

            public static Boolean WidescreenSupport
            {
                get => _widescreenSupport;
                set
                {
                    _disableWidescreenSupportForSingleMap = false;
                    _widescreenSupport = value;
                    FieldMap.OnWidescreenSupportChanged();
                    PersistenSingleton<UIManager>.Instance.OnWidescreenSupportChanged();
                }
            }

            private static Boolean _disableWidescreenSupportForSingleMap = false;

            public static void RestoreDisabledWidescreenSupport()
            {
                if (WidescreenSupport)
                    return;

                if (!_disableWidescreenSupportForSingleMap)
                    return;

                WidescreenSupport = true;
            }

            public static void DisableWidescreenSupportForSingleMap()
            {
                if (!WidescreenSupport)
                    return;

                WidescreenSupport = false;
                _disableWidescreenSupportForSingleMap = true;
            }
        }
    }
}
