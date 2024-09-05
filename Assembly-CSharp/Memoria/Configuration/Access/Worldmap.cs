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
            public static Int32 FieldOfView => Instance._worldmap.FieldOfView;
            public static Int32 FieldOfViewSpeedBoost => Instance._worldmap.FieldOfViewSpeedBoost;
            public static Int32 MistStartDistance => Instance._worldmap.MistStartDistance;
            public static Int32 MistEndDistance => Instance._worldmap.MistEndDistance;
            public static Int32 FogStartDistance => Instance._worldmap.FogStartDistance;
            public static Int32 FogEndDistance => Instance._worldmap.FogEndDistance;
            public static Int32 ViewDistance
            {
                get => Mathf.Clamp(Instance._worldmap.ViewDistance, 10, 450);
                set => Instance._worldmap.ViewDistance.Value = Mathf.Clamp(value, 10, 450);
            }
            public static Int32 ShipCameraTilt => Instance._worldmap.ShipCameraTilt;
        }
    }
}
