using Memoria.Prime.Ini;
using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class WorldmapSection : IniSection
        {
            public readonly IniValue<Int32> FieldOfView;
            public readonly IniValue<Int32> FieldOfViewSpeedBoost;
            public readonly IniValue<Int32> MistStartDistance;
            public readonly IniValue<Int32> MistEndDistance;
            public readonly IniValue<Int32> FogStartDistance;
            public readonly IniValue<Int32> FogEndDistance;
            public readonly IniValue<Int32> ViewDistance;
            public readonly IniValue<Int32> ShipCameraTilt;

            public WorldmapSection() : base(nameof(WorldmapSection), false)
            {
                FieldOfView = BindInt32(nameof(FieldOfView), 100);
                FieldOfViewSpeedBoost = BindInt32(nameof(FieldOfViewSpeedBoost), 100);
                MistStartDistance = BindInt32(nameof(MistStartDistance), 100);
                MistEndDistance = BindInt32(nameof(MistEndDistance), 100);
                FogStartDistance = BindInt32(nameof(FogStartDistance), 100);
                FogEndDistance = BindInt32(nameof(FogEndDistance), 100);
                ViewDistance = BindInt32(nameof(ViewDistance), 100);
                ShipCameraTilt = BindInt32(nameof(ShipCameraTilt), 100);
            }
        }
    }
}
