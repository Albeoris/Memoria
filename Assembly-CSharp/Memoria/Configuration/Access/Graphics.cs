using System;
using UnityEngine;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Graphics
        {
            public static Boolean Enabled => Instance._graphics.Enabled.Value;
            public static Int32 BattleFPS => Enabled ? Instance._graphics.BattleFPS.Value : 15;
            public static Int32 MovieFPS => Enabled ? Instance._graphics.MovieFPS.Value : 15;
            public static Int32 BattleSwirlFrames => Enabled ? Instance._graphics.BattleSwirlFrames.Value : 115;
            public static Int32 SkipIntros = Enabled ? Instance._graphics.SkipIntros.Value : 0;
            public static Int32 GarnetHair => Enabled ? Instance._graphics.GarnetHair.Value : 0;

            private static volatile Boolean _widescreenSupport = InitializeWidescreenSupport();

            private static Boolean InitializeWidescreenSupport()
            {
                if (!Enabled)
                    return false;

                if (!Instance._graphics.WidescreenSupport.Value)
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