using System;
using System.Collections.Generic;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class CheatsSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));

            public readonly IniValue<Boolean> Rotation = IniValue.Boolean(nameof(Rotation));
            public readonly IniValue<Boolean> Perspective = IniValue.Boolean(nameof(Perspective));

            public readonly IniValue<Boolean> SpeedMode = IniValue.Boolean(nameof(SpeedMode));
            public readonly IniValue<Int32> SpeedFactor = IniValue.Int32(nameof(SpeedFactor));

            public readonly IniValue<Boolean> BattleAssistance = IniValue.Boolean(nameof(BattleAssistance));
            public readonly IniValue<Boolean> Attack9999 = IniValue.Boolean(nameof(Attack9999));
            public readonly IniValue<Boolean> NoRandomEncounter = IniValue.Boolean(nameof(NoRandomEncounter));
            public readonly IniValue<Boolean> MasterSkill = IniValue.Boolean(nameof(MasterSkill));
            public readonly IniValue<Boolean> LvMax = IniValue.Boolean(nameof(LvMax));
            public readonly IniValue<Boolean> GilMax = IniValue.Boolean(nameof(GilMax));

            public CheatsSection() : base("Cheats")
            {
                Enabled.Value = false;

                Rotation.Value = true;
                Perspective.Value = true;

                SpeedMode.Value = true;
                SpeedFactor.Value = 5;

                BattleAssistance.Value = false;
                Attack9999.Value = false;
                NoRandomEncounter.Value = false;
                MasterSkill.Value = false;
                LvMax.Value = false;
                GilMax.Value = false;
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return Rotation;
                yield return Perspective;
                yield return SpeedMode;
                yield return SpeedFactor;
                yield return BattleAssistance;
                yield return Attack9999;
                yield return NoRandomEncounter;
                yield return MasterSkill;
                yield return LvMax;
                yield return GilMax;
            }
        }
    }
}