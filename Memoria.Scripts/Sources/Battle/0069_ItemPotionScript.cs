using System;
using Memoria.Data;

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
            _v.Context.Attack = 15;
            _v.Context.AttackPower = _v.Command.Item.Power;
            _v.Context.DefensePower = 0;

            _v.CalcHpMagicRecovery();
        }

        public Single RateTarget()
        {
            _v.Context.Attack = 15;
            _v.Context.AttackPower = _v.Command.Item.Power;
            _v.Context.DefensePower = 0;

            _v.CalcHpMagicRecovery();

            Single rate = _v.Target.HpDamage * BattleScriptDamageEstimate.RateHpMp((Int32)_v.Target.CurrentHp, (Int32)_v.Target.MaximumHp);

            if ((_v.Target.Flags & CalcFlag.HpRecovery) != CalcFlag.HpRecovery)
                rate *= -1;
            if (!_v.Target.IsPlayer)
                rate *= -1;

            return rate;
        }
    }
}