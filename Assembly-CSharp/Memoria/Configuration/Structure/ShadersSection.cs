using Memoria.Prime;
using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class ShadersSection : IniSection
        {
            public readonly IniValue<Int32> Shader_Field_Realism;
            public readonly IniValue<Int32> Shader_Field_Toon;
            public readonly IniValue<Int32> Shader_Field_Outlines;
            public readonly IniValue<Int32> Shader_Battle_Realism;
            public readonly IniValue<Int32> Shader_Battle_Toon;
            public readonly IniValue<Int32> Shader_Battle_Outlines;

            public ShadersSection() : base(nameof(ShadersSection), false)
            {
                Shader_Field_Realism = BindInt32(nameof(Shader_Field_Realism), 0);
                Shader_Field_Toon = BindInt32(nameof(Shader_Field_Toon), 0);
                Shader_Field_Outlines = BindInt32(nameof(Shader_Field_Outlines), 0);
                Shader_Battle_Realism = BindInt32(nameof(Shader_Battle_Realism), 0);
                Shader_Battle_Toon = BindInt32(nameof(Shader_Battle_Toon), 0);
                Shader_Battle_Outlines = BindInt32(nameof(Shader_Battle_Outlines), 0);
            }
        }
    }
}
