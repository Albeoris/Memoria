using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Battle
        {
            public static Boolean Enabled => Instance._battle.Enabled.Value;
            public static Boolean NoAutoTrance => Enabled && Instance._battle.NoAutoTrance.Value;
            public static Int32 Speed => GetBattleSpeed();
            public static Int32 EncounterInterval => Enabled ? Instance._battle.EncounterInterval.Value : 960;

            private static Int32 GetBattleSpeed()
            {
                if (Enabled && Instance._battle.Speed.Value > 0)
                    return Instance._battle.Speed.Value;
                if (Hacks.Enabled)
                    return Instance._hacks.BattleSpeed.Value;
                return 0;
            }
        }
    }
}