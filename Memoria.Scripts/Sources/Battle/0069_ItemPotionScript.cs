using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Item, Hi-Potion
    /// </summary>
    [BattleScript(Id)]
    public sealed class ItemPotionScript : IBattleScript, IEstimateBattleScript
    {
        public const Int32 Id = 0069;

        private readonly BattleCalculator _v;

        public ItemPotionScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            BattleStatus status = _v.Command.ItemId != RegularItem.NoItem ? _v.Command.Item.Status : _v.Command.AbilityStatus;
            _v.Context.Attack = 15;
            _v.Context.AttackPower = _v.Command.ItemId != RegularItem.NoItem ? _v.Command.Item.Power : _v.Command.Power;
            _v.Context.DefensePower = 0;

            if (_v.Context.AttackPower > 0)
                _v.CalcHpMagicRecovery();
            if (status != 0)
            {
                Boolean statusEffectOnly = _v.Context.AttackPower == 0;
                Int32 rate = _v.Command.ItemId != RegularItem.NoItem ? _v.Command.Item.HitRate : _v.Command.HitRate;
                if (GameRandom.RandomInt(0, 100) < rate)
                    _v.Target.TryAlterStatuses(status, statusEffectOnly, _v.Caster);
                else if (statusEffectOnly)
                    _v.Context.Flags |= BattleCalcFlags.Miss;
            }
        }

        public Single RateTarget()
        {
            BattleStatus status = _v.Command.ItemId != RegularItem.NoItem ? _v.Command.Item.Status : _v.Command.AbilityStatus;
            _v.Context.Attack = 15;
            _v.Context.AttackPower = _v.Command.ItemId != RegularItem.NoItem ? _v.Command.Item.Power : _v.Command.Power;
            _v.Context.DefensePower = 0;

            _v.CalcHpMagicRecovery();

            Single rate = _v.Target.HpDamage * BattleScriptDamageEstimate.RateHpMp((Int32)_v.Target.CurrentHp, (Int32)_v.Target.MaximumHp);
            if ((_v.Target.Flags & CalcFlag.HpRecovery) != CalcFlag.HpRecovery)
                rate *= -1;
            if (!_v.Target.IsPlayer)
                rate *= -1;

            rate += BattleScriptStatusEstimate.RateStatuses(status);

            return rate;
        }
    }
}
