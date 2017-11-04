using System;
using UnityEngine;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Graphics
        {
            public static Boolean Enabled => Instance._graphics.Enabled;
            public static Int32 BattleFPS => Instance._graphics.BattleFPS;
            public static Int32 MovieFPS => Instance._graphics.MovieFPS;
            public static Int32 BattleSwirlFrames => Instance._graphics.BattleSwirlFrames;
            public static Int32 SkipIntros = Instance._graphics.SkipIntros;
            public static Int32 GarnetHair => Instance._graphics.GarnetHair;

            private static volatile Boolean _widescreenSupport = InitializeWidescreenSupport();

            private static Boolean InitializeWidescreenSupport()
            {
                if (!Enabled)
                    return false;

                if (!Instance._graphics.WidescreenSupport)
                    return false;

                if (Math.Abs(((Double)Screen.width / (Double)Screen.height) - (16d / 9d)) < 0.001)
                {
                    return true;
                }

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