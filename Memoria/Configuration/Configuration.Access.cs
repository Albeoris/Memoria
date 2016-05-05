using System;

namespace Memoria
{
    public sealed partial class Configuration : Ini
    {
        public static class Font
        {
            public static Boolean Enabled => Instance._font.Enabled.Value;
            public static String[] Names => Instance._font.Names.Value;
            public static Int32 Size => Instance._font.Size.Value;
        }

        public static class Cheats
        {
            public static Boolean Enabled => Instance._cheats.Enabled.Value;
            public static Boolean Rotation => Instance._cheats.Rotation.Value;
            public static Boolean Perspective => Instance._cheats.Perspective.Value;
            public static Boolean SpeedMode => Instance._cheats.SpeedMode.Value;
            public static Int32 SpeedFactor => Instance._cheats.SpeedFactor.Value;
            public static Boolean BattleAssistance => Instance._cheats.BattleAssistance.Value;
            public static Boolean Attack9999 => Instance._cheats.Attack9999.Value;
            public static Boolean NoRandomEncounter => Instance._cheats.NoRandomEncounter.Value;
            public static Boolean MasterSkill => Instance._cheats.MasterSkill.Value;
            public static Boolean LvMax => Instance._cheats.LvMax.Value;
            public static Boolean GilMax => Instance._cheats.GilMax.Value;
        }

        public static class Import
        {
            public static Boolean Enabled => Instance._import.Enabled.Value;
            public static String Path => Instance._import.Path.Value;
            public static Boolean Text => Instance._import.Text.Value;
            public static Boolean Graphics => Instance._import.Graphics.Value;
        }

        public static class Export
        {
            public static Boolean Enabled => Instance._export.Enabled.Value;
            public static String Path => Instance._export.Path.Value;
            public static String[] Languages => Instance._export.Languages.Value;
            public static Boolean Text => Instance._export.Text.Value;
            public static Int32 TextFormat = 1;
            public static Boolean Graphics => Instance._export.Graphics.Value;
            public static Boolean Field => Instance._export.Field.Value;
        }

        public static class Hacks
        {
            public static Boolean Enabled => Instance._hacks.Enabled.Value;
            public static Boolean AllCharactersAvailable => Instance._hacks.AllCharactersAvailable.Value;

            public static Boolean IsAllCharactersAvailable => Enabled && AllCharactersAvailable;
        }
    }
}