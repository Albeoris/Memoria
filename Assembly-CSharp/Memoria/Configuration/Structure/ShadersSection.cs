using Memoria.Prime;
using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class ShadersSection : IniSection
        {
            public readonly IniValue<Int32> EnableRealismShadingForField;
            public readonly IniValue<Int32> EnableToonShadingForField;
            public readonly IniValue<Int32> EnableRealismShadingForBattle;
            public readonly IniValue<Int32> EnableToonShadingForBattle;
            public readonly IniValue<Int32> OutlineForFieldCharacter;
            public readonly IniValue<Int32> OutlineForBattleCharacter;

            public ShadersSection() : base(nameof(ShadersSection), false)
            {
                EnableRealismShadingForField = BindInt32(nameof(EnableRealismShadingForField), 0);
                EnableToonShadingForField = BindInt32(nameof(EnableToonShadingForField), 0);
                EnableRealismShadingForBattle = BindInt32(nameof(EnableRealismShadingForBattle), 0);
                EnableToonShadingForBattle = BindInt32(nameof(EnableToonShadingForBattle), 0);
                OutlineForFieldCharacter = BindInt32(nameof(OutlineForFieldCharacter), 0);
                OutlineForBattleCharacter = BindInt32(nameof(OutlineForBattleCharacter), 0);
                Log.Message("custom shader value = " + (Enabled.Value ? "1" : "0"));
            }
        }
    }
}
