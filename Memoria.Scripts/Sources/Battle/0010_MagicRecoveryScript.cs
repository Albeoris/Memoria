using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
    [BattleScript(Id)]
    public sealed class MagicRecoveryScript : IBattleScript, IEstimateBattleScript
    {
        public const Int32 Id = 0010;

        private readonly BattleCalculator _v;

        public MagicRecoveryScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.NormalMagicParams();
            _v.Caster.EnemyTranceBonusAttack();
            _v.Caster.PenaltyMini();
            _v.PenaltyCommandDividedAttack();
            _v.CalcHpMagicRecovery();
        }

        public Single RateTarget()
        {
            _v.NormalMagicParams();
            _v.Caster.PenaltyMini();
            _v.PenaltyCommandDividedAttack();

            if (_v.Target.IsUnderAnyStatus(BattleStatusConst.ApplyReflect) && !_v.Command.IsReflectNull)
                return 0;

            _v.CalcHpMagicRecovery();

            Single rate = _v.Target.HpDamage * BattleScriptDamageEstimate.RateHpMp((Int32)_v.Target.CurrentHp, (Int32)_v.Target.MaximumHp);

            if (!_v.Target.IsPlayer)
                rate = 0;

            return rate;
        }
    }
}
