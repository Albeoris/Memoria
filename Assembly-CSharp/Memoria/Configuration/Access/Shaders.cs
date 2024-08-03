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
            public static Int32 EnableRealismShadingField => Instance._shaders.EnableRealismShadingForField;
            public static Int32 EnableToonShadingField => Instance._shaders.EnableToonShadingForField;
            public static Int32 EnableRealismShadingBattle => Instance._shaders.EnableRealismShadingForBattle;
            public static Int32 EnableToonShadingBattle => Instance._shaders.EnableToonShadingForBattle;
            public static Int32 OutlineForFieldCharacter => Instance._shaders.OutlineForFieldCharacter;
            public static Int32 OutlineForBattleCharacter => Instance._shaders.OutlineForBattleCharacter;
        }
    }
}
