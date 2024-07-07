using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Gems: Garnet, Amethyst, Aquamarine, Diamond, Emerald, Moonstone, Ruby, Peridot, Sapphire, Opal, Topaz, Lapis Lazuli, Ore
    /// </summary>
    [BattleScript(Id)]
    public sealed class ItemGemScript : IBattleScript, IEstimateBattleScript
    {
        public const Int32 Id = 0074;

        private readonly BattleCalculator _v;

        public ItemGemScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= CalcFlag.HpAlteration | CalcFlag.HpRecovery;
            _v.Target.HpDamage = _v.Command.Item.Power * (ff9item.FF9Item_GetCount(_v.Command.ItemId) + 1);
        }

        public Single RateTarget()
        {
            Int32 recovery = _v.Command.Item.Power * (ff9item.FF9Item_GetCount(_v.Command.ItemId) + 1);

            Single rate = recovery * BattleScriptDamageEstimate.RateHpMp((Int32)_v.Target.CurrentHp, (Int32)_v.Target.MaximumHp);

            if (!_v.Target.IsPlayer)
                rate *= -1;

            return rate;
        }
    }
}
