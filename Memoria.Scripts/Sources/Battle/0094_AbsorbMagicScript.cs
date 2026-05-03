using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Absorb Magic
    /// </summary>
    [BattleScript(Id)]
    public sealed class AbsorbMagicScript : IBattleScript
    {
        public const Int32 Id = 0094;

        private readonly BattleCalculator _v;

        public AbsorbMagicScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.TryAlterSingleStatus(BattleStatusId.ChangeStat, true, _v.Caster, "Magic", _v.Target.Magic / 2);
            if ((_v.Context.Flags & (BattleCalcFlags.Miss | BattleCalcFlags.Guard)) == 0)
                btl_stat.AlterStatus(_v.Caster, BattleStatusId.ChangeStat, _v.Caster, false, "Magic", Math.Min(Byte.MaxValue, _v.Caster.Magic * 2));
        }
    }
}
