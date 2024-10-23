using Memoria.Prime.Ini;
using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class WorldmapSection : IniSection
        {
            public readonly IniValue<Int32> MistViewDistance;
            public readonly IniValue<Int32> MistStartDistance_base;
            public readonly IniValue<Int32> MistStartDistance;
            public readonly IniValue<Int32> MistEndDistance;
            public readonly IniValue<Int32> MistDensity;
            public readonly IniValue<Int32> NoMistViewDistance;
            public readonly IniValue<Int32> FogStartDistance;
            public readonly IniValue<Int32> FogEndDistance;
            public readonly IniValue<Int32> FieldOfView;
            public readonly IniValue<Int32> FieldOfViewSpeedBoost;
            public readonly IniValue<Int32> CameraDistance;
            public readonly IniValue<Int32> CameraHeight;
            public readonly IniValue<Int32> CameraAimHeight;
            public readonly IniValue<Int32> CameraTiltShip;
            public readonly IniValue<Boolean> AlternateControls;

            public WorldmapSection() : base(nameof(WorldmapSection), false)
            {
                MistViewDistance = BindInt32(nameof(MistViewDistance), 290);
                MistStartDistance_base = BindInt32(nameof(MistStartDistance_base), 40);
                MistStartDistance = BindInt32(nameof(MistStartDistance), 25);
                MistEndDistance = BindInt32(nameof(MistEndDistance), 250);
                MistDensity = BindInt32(nameof(MistDensity), 15);
                NoMistViewDistance = BindInt32(nameof(NoMistViewDistance), 300);
                FogStartDistance = BindInt32(nameof(FogStartDistance), 250);
                FogEndDistance = BindInt32(nameof(FogEndDistance), 350);
                FieldOfView = BindInt32(nameof(FieldOfView), 44);
                FieldOfViewSpeedBoost = BindInt32(nameof(FieldOfViewSpeedBoost), 100);
                CameraDistance = BindInt32(nameof(CameraDistance), 100);
                CameraHeight = BindInt32(nameof(CameraHeight), 100);
                CameraAimHeight = BindInt32(nameof(CameraAimHeight), 100);
                CameraTiltShip = BindInt32(nameof(CameraTiltShip), 100);
                AlternateControls = BindBoolean(nameof(AlternateControls), false);
            }
        }
    }
}
