using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Cheats
        {
            public static Boolean Enabled => Instance._cheats.Enabled;
            public static Boolean Rotation => Instance._cheats.Rotation;
            public static Boolean Perspective => Instance._cheats.Perspective;
            public static Boolean SpeedMode => Instance._cheats.SpeedMode;
            public static Int32 SpeedFactor => Instance._cheats.SpeedFactor;
            public static Boolean BattleAssistance => Configuration.Mod.TranceSeek ? false : Instance._cheats.BattleAssistance;
            public static Boolean Attack9999 => Configuration.Mod.TranceSeek ? false : Instance._cheats.Attack9999;
            public static Boolean NoRandomEncounter => Configuration.Mod.TranceSeek ? false : Instance._cheats.NoRandomEncounter;
            public static Boolean MasterSkill => Configuration.Mod.TranceSeek ? false : Instance._cheats.MasterSkill;
            public static Boolean LvMax => Configuration.Mod.TranceSeek ? false : Instance._cheats.LvMax;
            public static Boolean GilMax => Configuration.Mod.TranceSeek ? false : Instance._cheats.GilMax;
            public static String SuperCheat9999 => Instance._cheats.SuperCheat9999; // TRANCE SEEK - DEBUG CHEAT
            public static Boolean AutoBattle => Instance._cheats.AutoBattle; // TRANCE SEEK - ENABLE/DISABLE AUTOBATTLE
        }
    }
}