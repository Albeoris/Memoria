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

            Int16 damage = 0;
            _v.NormalMagicParams();
            _v.Caster.PenaltyMini();
            _v.Target.PenaltyShellAttack();
            _v.Target.Flags |= CalcFlag.MpAlteration;
            _v.Caster.Flags |= CalcFlag.MpAlteration;

            Int32 diff = _v.Context.PowerDifference;
            if (diff > 0)
                damage = (Int16)Math.Min(9999, diff * _v.Context.EnsureAttack >> 2);

            if (_v.Target.IsZombie)
            {
                _v.Target.Flags |= CalcFlag.MpRecovery;
                if (damage > _v.Caster.CurrentMp)
                    damage = _v.Caster.CurrentMp;
            }
            else
            {
                _v.Caster.Flags |= CalcFlag.MpRecovery;
                if (damage > _v.Target.CurrentMp)
                    damage = _v.Target.CurrentMp;
            }

            _v.Target.MpDamage = damage;
            _v.Caster.MpDamage = damage;
        }
    }
}