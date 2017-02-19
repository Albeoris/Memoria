using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Comet, Meteor, Twister, Meteorite, Meteorite Counter
    /// </summary>
    [BattleScript(Id)]
    public sealed class MeteoriteScript : IBattleScript
    {
        public const Int32 Id = 0018;

        private readonly BattleCalculator _v;

        public MeteoriteScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (IsMeteorMiss() || IsCometMiss())
            {
                _v.Context.Flags |= BattleCalcFlags.Miss;
                return;
            }

            InitializeAttackParams();

            _v.Caster.PenaltyMini();
            _v.Target.PenaltyShellAttack();
            _v.PenaltyCommandDividedAttack();
            _v.CasterCommand.BonusElement();

            if (_v.CanAttackMagic())
            {
                _v.TargetCommand.CalcHpDamage();
                _v.TargetCommand.TryAlterMagicStatuses();
            }
        }

        private void InitializeAttackParams()
        {
            _v.Context.Attack = (Int16)(GameRandom.Next16() % (_v.Caster.Magic + _v.Caster.Level));
            _v.SetCommandPower();
            _v.Context.DefensePower = 0;
        }

        private Boolean IsMeteorMiss()
        {
            return _v.Command.AbilityId == BattleAbilityId.Meteor && _v.Command.IsMeteorMiss;
        }

        private Boolean IsCometMiss()
        {
            return _v.Command.SpecialEffect == SpecialEffect.Comet && GameRandom.Next8() > 170;
        }
    }
}