using System;
using Memoria.Prime.Ini;

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

        public static class Graphics
        {
            public static Boolean Enabled => Instance._graphics.Enabled.Value;
            public static Int32 BattleFPS => Enabled ? Instance._graphics.BattleFPS.Value : 15;
            public static Int32 BattleSwirlFrames => Enabled ? Instance._graphics.BattleSwirlFrames.Value : 115;
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
            public static Boolean Audio => Enabled && Instance._import.Audio.Value;
        }

        public static class Export
        {
            public static Boolean Enabled => Instance._export.Enabled.Value;
            public static String Path => Instance._export.Path.Value;
            public static String[] Languages => Instance._export.Languages.Value;
            public static Boolean Text => Instance._export.Text.Value;
            public static Int32 TextFormat = 1;
            public static Boolean Graphics => Instance._export.Graphics.Value;
            public static Boolean Audio => Enabled && Instance._export.Audio.Value;
            public static Boolean Field => Instance._export.Field.Value;
            public static Boolean Battle => Instance._export.Battle.Value;
        }

        public static class TetraMaster
        {
            public static Boolean Enabled => Instance._tetraMaster.Enabled.Value;
            public static Boolean IsEasyWin => Enabled && Instance._tetraMaster.ReduceRandom.Value == 2;
            public static Boolean IsReduceRandom => Enabled && Instance._tetraMaster.ReduceRandom.Value == 1;
        }

        public static class Fixes
        {
            public static Boolean Enabled => Instance._fixes.Enabled.Value;

            public static Boolean IsKeepRestTimeInBattle => Enabled && Instance._fixes.KeepRestTimeInBattle.Value;
        }

        public static class Hacks
        {
            public static Boolean Enabled => Instance._hacks.Enabled.Value;
            public static Int32 AllCharactersAvailable => Enabled ? Instance._hacks.AllCharactersAvailable.Value : 0;
            public static Int32 RopeJumpingIncrement => Instance._hacks.RopeJumpingIncrement.Value;

            public static Int32 BattleSpeed => Enabled ? Math.Min(Math.Max(0, Instance._hacks.BattleSpeed.Value), 2) : 0;
        }

        public static class Debug
        {
            public static Boolean Enabled => Instance._debug.Enabled.Value;
            public static Boolean SigningEventObjects => Enabled && Instance._debug.SigningEventObjects.Value;
        }
    }
}