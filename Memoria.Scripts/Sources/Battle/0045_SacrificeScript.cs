using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Sacrifice
    /// </summary>
    [BattleScript(Id)]
    public sealed class SacrificeScript : IBattleScript
    {
        public const Int32 Id = 0045;

        private readonly BattleCalculator _v;

        public SacrificeScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.Target.IsUnderStatus(BattleStatus.Death) || _v.Caster.IsUnderStatus(BattleStatus.Petrify | BattleStatus.Death))
            {
                _v.Context.Flags |= BattleCalcFlags.Miss;
                return;
            }

            _v.Target.CurrentHp = _v.Target.MaximumHp;
            _v.Target.CurrentMp = _v.Target.MaximumMp;
            _v.Caster.CurrentMp = 0;
            _v.Caster.Kill();
        }
    }
}