using System;

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

            private const String DefaultBattleCharacterShader = "PSX/BattleMap_StatusEffect";
            private const String ToonBattleCharacterShader = "PSX/BattleMap_StatusEffect_Toon";
            private const String RealismBattleCharacterShader = "PSX/BattleMap_StatusEffect_RealLighting";

            private const String DefaultFieldCharacterShader = "PSX/FieldMapActor";
            private const String ToonFieldCharacterShader = "PSX/FieldMapActor_Toon";
            private const String RealismFieldCharacterShader = "PSX/FieldMapActor_RealLighting";

            public static String BattleCharacterShader
            {
                get
                {
                    if (Configuration.Shaders.CustomShaderEnabled == 1)
                    {
                        if (Configuration.Shaders.Shader_Battle_Realism == 1)
                            return RealismBattleCharacterShader;
                        else if (Configuration.Shaders.Shader_Battle_Toon == 1)
                            return ToonBattleCharacterShader;
                    }
                    return DefaultBattleCharacterShader;
                }
            }

            public static String FieldCharacterShader
            {
                get
                {
                    if (Configuration.Shaders.CustomShaderEnabled == 1)
                    {
                        if (Configuration.Shaders.Shader_Field_Realism == 1)
                            return RealismFieldCharacterShader;
                        else if (Configuration.Shaders.Shader_Field_Toon == 1)
                            return ToonFieldCharacterShader;
                    }
                    return DefaultFieldCharacterShader;
                }
            }
        }
    }
}
