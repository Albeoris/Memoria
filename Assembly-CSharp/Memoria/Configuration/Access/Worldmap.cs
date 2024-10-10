using Memoria.Prime;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Worldmap
        {
            public static Int32 MistViewDistance
            {
                get => Mathf.Clamp(Instance._worldmap.MistViewDistance, 10, 450);
                set => Instance._worldmap.MistViewDistance.Value = Mathf.Clamp(value, 10, 450);
            }
            public static Int32 MistStartDistance_base => Instance._worldmap.MistStartDistance_base;
            public static Int32 MistStartDistance => Instance._worldmap.MistStartDistance;
            public static Int32 MistEndDistance => Instance._worldmap.MistEndDistance;
            public static Int32 MistDensity
            {
                get => Mathf.Clamp(Instance._worldmap.MistDensity, 0, 1000);
                set => Instance._worldmap.MistDensity.Value = Mathf.Clamp(value, 0, 1000);
            }
            public static Int32 NoMistViewDistance
            {
                get => Mathf.Clamp(Instance._worldmap.NoMistViewDistance, 10, 450);
                set => Instance._worldmap.NoMistViewDistance.Value = Mathf.Clamp(value, 10, 450);
            }
            public static Int32 FogStartDistance => Instance._worldmap.FogStartDistance;
            public static Int32 FogEndDistance => Instance._worldmap.FogEndDistance;
            public static Int32 FieldOfView
            {
                get => Mathf.Clamp(Instance._worldmap.FieldOfView, 1, 160);
                set => Instance._worldmap.FieldOfView.Value = Mathf.Clamp(value, 1, 160);
            }
            public static Int32 FieldOfViewSpeedBoost => Instance._worldmap.FieldOfViewSpeedBoost;
            public static Int32 CameraDistance
            {
                get => Mathf.Clamp(Instance._worldmap.CameraDistance, -250, 250);
                set => Instance._worldmap.CameraDistance.Value = Mathf.Clamp(value, -250, 250);
            }
            public static Int32 CameraHeight => Instance._worldmap.CameraHeight;
            public static Int32 CameraAimHeight => Instance._worldmap.CameraAimHeight;
            public static Int32 CameraTiltShip
            {
                get => Mathf.Clamp(Instance._worldmap.CameraTiltShip, 0, 200);
                set => Instance._worldmap.CameraTiltShip.Value = Mathf.Clamp(value, 0, 200);
            }
        }
    }
}
