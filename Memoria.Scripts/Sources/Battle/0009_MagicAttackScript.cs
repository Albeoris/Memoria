using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    [BattleScript(Id)]
    public sealed class MagicAttackScript : IBattleScript, IEstimateBattleScript
    {
        public const Int32 Id = 0009;

        private readonly BattleCalculator _v;

        public MagicAttackScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.NormalMagicParams();
            _v.Caster.PenaltyMini();
            _v.Target.PenaltyShellAttack();
            _v.PenaltyCommandDividedAttack();
            _v.CasterCommand.BonusElement();

            if (_v.CanAttackMagic())
            {
                _v.CalcHpDamage();
                _v.TryAlterMagicStatuses();
            }
        }

        public Single RateTarget()
        {
            _v.NormalMagicParams();
            _v.Caster.PenaltyMini();
            _v.Target.PenaltyShellAttack();
            _v.PenaltyCommandDividedAttack();
            _v.CasterCommand.BonusElement();

            if (!_v.CanAttackMagic())
                return 0;

            if (_v.Target.IsUnderAnyStatus(BattleStatus.Reflect) && !_v.Command.IsReflectNull)
                return 0;

            _v.CalcHpDamage();

            Single rate = Math.Min(_v.Target.HpDamage, _v.Target.CurrentHp);

            if ((_v.Target.Flags & CalcFlag.HpRecovery) == CalcFlag.HpRecovery)
                rate *= -1;
            if (_v.Target.IsPlayer)
                rate *= -1;

            return rate;
        }
    }
}