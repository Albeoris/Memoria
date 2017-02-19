using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Life, Full-Life, Rebirth Flame, Revive
    /// </summary>
    [BattleScript(Id)]
    public sealed class ReviveScript : IBattleScript
    {
        public const Int32 Id = 0013;

        private readonly BattleCalculator _v;

        public ReviveScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.CanBeRevived() || (!HitRateForZombie() && !_v.TargetCommand.TryMagicHit()))
                return;

            if (_v.Target.IsZombie)
            {
                _v.Target.Kill();
                return;
            }

            if (!_v.Target.CheckIsPlayer())
                return;

            Int32 damage = _v.Target.MaximumHp * (_v.Target.Will + _v.Command.Power);
            damage = Math.Min(9999, _v.Caster.HasSupportAbility(SupportAbility2.Concentrate) ? damage / 50 : damage / 100);

            _v.Target.Flags |= CalcFlag.HpAlteration | CalcFlag.HpRecovery;
            _v.Target.HpDamage = (Int16)damage;
            _v.TargetCommand.TryRemoveAbilityStatuses();
        }

        private Boolean HitRateForZombie()
        {
            if (_v.Target.IsZombie)
            {
                _v.MagicAccuracy();
                return false;
            }
            return true;
        }

    }
}