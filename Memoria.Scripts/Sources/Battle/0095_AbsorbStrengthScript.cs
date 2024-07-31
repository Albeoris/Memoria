using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Absorb Strength
    /// </summary>
    [BattleScript(Id)]
    public sealed class AbsorbStrengthScript : IBattleScript
    {
        public const Int32 Id = 0095;

        private readonly BattleCalculator _v;

        public AbsorbStrengthScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.TryAlterSingleStatus(BattleStatusId.ChangeStat, true, _v.Caster, "Strength", _v.Target.Strength / 2);
            if ((_v.Context.Flags & (BattleCalcFlags.Miss | BattleCalcFlags.Guard)) == 0)
                btl_stat.AlterStatus(_v.Caster, BattleStatusId.ChangeStat, _v.Caster, false, "Strength", Math.Min(Byte.MaxValue, _v.Caster.Strength * 2));
        }
    }
}
