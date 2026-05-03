using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Elexir
    /// </summary>
    [BattleScript(Id)]
    public sealed class ItemElixirScript : IBattleScript, IEstimateBattleScript
    {
        public const Int32 Id = 0071;

        private readonly BattleCalculator _v;

        public ItemElixirScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.CanBeAttacked())
                return;

            if (_v.Target.IsZombie)
            {
                _v.Target.CurrentMp = 0;
                _v.Target.Kill(_v.Caster);
            }
            else
            {
                _v.Target.CurrentHp = _v.Target.MaximumHp;
                _v.Target.CurrentMp = _v.Target.MaximumMp;
            }
        }

        public Single RateTarget()
        {
            if (!_v.Target.CanBeAttacked())
                return 0;

            if (_v.Target.IsZombie)
            {
                Int32 rate = BattleScriptStatusEstimate.RateStatus(BattleStatusId.Death);
                if (!_v.Target.IsPlayer)
                    rate *= -1;

                return rate;
            }
            else
            {
                Single rate = 0;

                rate += _v.Target.MaximumHp * BattleScriptDamageEstimate.RateHpMp((Int32)_v.Target.CurrentHp, (Int32)_v.Target.MaximumHp);
                rate += _v.Target.MaximumMp * BattleScriptDamageEstimate.RateHpMp((Int32)_v.Target.CurrentMp, (Int32)_v.Target.MaximumMp);

                if (!_v.Target.IsPlayer)
                    rate *= -1;

                return rate;
            }
        }
    }
}
