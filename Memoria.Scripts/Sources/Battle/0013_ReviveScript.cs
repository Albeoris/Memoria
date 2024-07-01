using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Life, Full-Life, Rebirth Flame, Revive
    /// </summary>
    [BattleScript(Id)]
    public sealed class ReviveScript : IBattleScript, IEstimateBattleScript
    {
        public const Int32 Id = 0013;

        private readonly BattleCalculator _v;

        public ReviveScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.CanBeRevived())
                return;

            if (HitRateForZombie() && !_v.TryMagicHit())
                return;

            if (_v.Target.IsZombie)
            {
                _v.Target.Kill(_v.Caster);
                return;
            }

            if (!_v.Target.CheckIsPlayer())
                return;

            _v.Target.Flags |= CalcFlag.HpAlteration | CalcFlag.HpRecovery;
            _v.Target.HpDamage = (Int32)(_v.Target.MaximumHp * (_v.Target.Will + _v.Command.Power) / 100);
            _v.TryRemoveAbilityStatuses();
        }

        private Boolean HitRateForZombie()
        {
            if (_v.Target.IsZombie)
            {
                _v.MagicAccuracy();
                return true;
            }
            return false;
        }

        public Single RateTarget()
        {
            if (!_v.Target.CanBeRevived())
                return 0;

            if (_v.Target.IsZombie)
            {
                _v.MagicAccuracy();

                Single hitRate = BattleScriptAccuracyEstimate.RatePlayerAttackHit(_v.Context.HitRate);
                Single evaRate = BattleScriptAccuracyEstimate.RatePlayerAttackEvade(_v.Context.Evade);

                Single result = BattleScriptStatusEstimate.RateStatus(BattleStatus.Death) * hitRate * evaRate;
                if (!_v.Target.IsPlayer)
                    result *= -1;
                return result;
            }

            if (!_v.Target.IsPlayer)
                return 0;

            BattleStatus playerStatus = _v.Target.CurrentStatus;
            BattleStatus removeStatus = _v.Command.AbilityStatus;
            BattleStatus removedStatus = playerStatus & removeStatus;
            Int32 rating = BattleScriptStatusEstimate.RateStatuses(removedStatus);

            return -1 * rating;
        }
    }
}
