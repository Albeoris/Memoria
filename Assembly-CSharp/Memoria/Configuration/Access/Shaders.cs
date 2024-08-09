using Memoria.Prime;
using System;
using UnityEngine;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Shaders
        {
            public static Int32 CustomShaderEnabled => Instance._shaders.Enabled ? 1 : 0;
            public static Int32 Shader_Field_Realism => Instance._shaders.Shader_Field_Realism;
            public static Int32 Shader_Field_Toon => Instance._shaders.Shader_Field_Toon;
            public static Int32 Shader_Field_Outlines => Instance._shaders.Shader_Field_Outlines;
            public static Int32 Shader_Battle_Realism => Instance._shaders.Shader_Battle_Realism;
            public static Int32 Shader_Battle_Toon => Instance._shaders.Shader_Battle_Toon;
            public static Int32 Shader_Battle_Outlines => Instance._shaders.Shader_Battle_Outlines;
        }
    }
}
