using System;
using FF9;

namespace Memoria.Scripts.Battle
{
    [BattleScript(Id)]
    public sealed class FearScript : IBattleScript
    {
        public const Int32 Id = 0110;

        private readonly BattleCalculator _v;

        public FearScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            byte trancefear = (byte)(Comn.random16() % ((_v.Caster.Will * 2) - _v.Target.Will));
            if (trancefear < _v.Target.Trance)
            {
                _v.Target.Trance -= trancefear;
            }
            else
            {
                _v.Target.Trance = 0;
            }
            if (_v.Command.Power > 0)
            {
                _v.NormalMagicParams();
                _v.Caster.PenaltyMini();
                _v.Target.PenaltyShellAttack();
                _v.PenaltyCommandDividedAttack();
                _v.CasterCommand.BonusElement();
                if (_v.CanAttackMagic())
                {
                    _v.CalcHpDamage();
                    _v.TryAlterMagicStatusesTranceSeek(_v.Caster.Will);
                }
            }
            else if (_v.Command.HitRate > 0 && _v.Command.Power == 0)
            {
                _v.MagicAccuracy();
                _v.Target.PenaltyShellHitRate();
                _v.PenaltyCommandDividedHitRate();
                if (_v.TryMagicHit())
                {
                    _v.TargetCommand.TryAlterCommandStatusesTranceSeek(_v.Caster.Will);
                }
            }
        }
    }
}