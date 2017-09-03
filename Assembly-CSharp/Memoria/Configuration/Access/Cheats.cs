using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
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
    }
}