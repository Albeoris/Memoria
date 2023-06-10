using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Osmose, Absorb MP
    /// </summary>
    [BattleScript(Id)]
    public sealed class DrainMpScript : IBattleScript
    {
        public const Int32 Id = 0015;

        private readonly BattleCalculator _v;

        public DrainMpScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.IsCasterNotTarget() || !_v.Target.CanBeAttacked())
                return;

            _v.NormalMagicParams();
            _v.Caster.EnemyTranceBonusAttack();
            _v.Caster.PenaltyMini();
            _v.Target.PenaltyShellAttack();
            _v.Target.Flags |= CalcFlag.MpAlteration;
            _v.Caster.Flags |= CalcFlag.MpAlteration;
            _v.Context.IsDrain = true;

            _v.CalcMpDamage();
            Int32 damage = _v.Target.MpDamage;

            if (_v.Target.IsZombie)
            {
                _v.Target.Flags |= CalcFlag.MpRecovery;
                if (damage > _v.Caster.CurrentMp)
                    damage = (Int32)_v.Caster.CurrentMp;
            }
            else
            {
                _v.Caster.Flags |= CalcFlag.MpRecovery;
                if (damage > _v.Target.CurrentMp)
                    damage = (Int32)_v.Target.CurrentMp;
            }

            _v.Target.MpDamage = damage;
            _v.Caster.MpDamage = damage;
        }
    }
}