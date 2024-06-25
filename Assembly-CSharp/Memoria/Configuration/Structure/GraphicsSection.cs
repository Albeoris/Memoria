using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class GraphicsSection : IniSection
        {
            public readonly IniValue<Int32> CustomShader;
            public readonly IniValue<Int32> EnableRealismShadingForField;
            public readonly IniValue<Int32> EnableToonShadingForField;
            public readonly IniValue<Int32> EnableRealismShadingForBattle;
            public readonly IniValue<Int32> EnableToonShadingForBattle;
            public readonly IniValue<Int32> OutlineForFieldCharacter;
            public readonly IniValue<Int32> OutlineForBattleCharacter;
            public readonly IniValue<Int32> EnableSSAO;
            public readonly IniValue<Int32> BattleFPS;
            public readonly IniValue<Int32> BattleTPS;
            public readonly IniValue<Int32> FieldFPS;
            public readonly IniValue<Int32> FieldTPS;
            public readonly IniValue<Int32> WorldFPS;
            public readonly IniValue<Int32> WorldTPS;
            public readonly IniValue<Int32> MenuFPS;
            public readonly IniValue<Int32> MenuTPS;
            public readonly IniValue<Int32> BattleSwirlFrames;
            public readonly IniValue<Boolean> WidescreenSupport;
            public readonly IniValue<Int32> TileSize;
            public readonly IniValue<Int32> AntiAliasing;
            public readonly IniValue<Int32> SkipIntros;
            public readonly IniValue<Int32> GarnetHair;
            public readonly IniValue<Int32> CameraStabilizer;

            public GraphicsSection() : base(nameof(GraphicsSection), false)
            {
                CustomShader = BindInt32(nameof(CustomShader), 0);
                EnableRealismShadingForField = BindInt32(nameof(EnableRealismShadingForField), 0);
                EnableToonShadingForField = BindInt32(nameof(EnableToonShadingForField), 0);
                EnableRealismShadingForBattle = BindInt32(nameof(EnableRealismShadingForBattle), 0);
                EnableToonShadingForBattle = BindInt32(nameof(EnableToonShadingForBattle), 0);
                OutlineForFieldCharacter = BindInt32(nameof(OutlineForFieldCharacter), 0);
                OutlineForBattleCharacter = BindInt32(nameof(OutlineForBattleCharacter), 0);
                EnableSSAO = BindInt32(nameof(EnableSSAO), 0);
                BattleFPS = BindInt32(nameof(BattleFPS), 30);
                BattleTPS = BindInt32(nameof(BattleTPS), 15);
                FieldFPS = BindInt32(nameof(FieldFPS), 30);
                FieldTPS = BindInt32(nameof(FieldTPS), 30);
                WorldFPS = BindInt32(nameof(WorldFPS), 20);
                WorldTPS = BindInt32(nameof(WorldTPS), 20);
                MenuFPS = BindInt32(nameof(MenuFPS), 60);
                MenuTPS = BindInt32(nameof(MenuTPS), 60);
                BattleSwirlFrames = BindInt32(nameof(BattleSwirlFrames), 115);
                WidescreenSupport = BindBoolean(nameof(WidescreenSupport), true);
                TileSize = BindInt32(nameof(TileSize), 32);
                AntiAliasing = BindInt32(nameof(AntiAliasing), 0);
                SkipIntros = BindInt32(nameof(SkipIntros), 0);
                GarnetHair = BindInt32(nameof(GarnetHair), 0);
                CameraStabilizer = BindInt32(nameof(CameraStabilizer), 0);
            }
        }
    }
}
