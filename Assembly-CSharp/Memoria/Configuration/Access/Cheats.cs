using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Cheats
        {
            public static Boolean Enabled => Instance._cheats.Enabled;
            public static Boolean SpeedMode => Instance._cheats.SpeedMode;
            public static Int32 SpeedFactor => Instance._cheats.SpeedFactor;
            public static Boolean SpeedTimer => Instance._cheats.SpeedTimer;
            public static Boolean BattleAssistance => Instance._cheats.BattleAssistance;
            public static Boolean Attack9999 => Instance._cheats.Attack9999;
            public static Boolean NoRandomEncounter => Instance._cheats.NoRandomEncounter;
            public static Boolean MasterSkill => Instance._cheats.MasterSkill;
            public static Boolean LvMax => Instance._cheats.LvMax;
            public static Boolean GilMax => Instance._cheats.GilMax;
        }
    }
}
