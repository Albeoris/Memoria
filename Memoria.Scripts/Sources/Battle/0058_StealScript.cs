using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Steal, Mug
    /// </summary>
    [BattleScript(Id)]
    public sealed class StealScript : IBattleScript
    {
        public const Int32 Id = 0058;

        private readonly BattleCalculator _v;

        public StealScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            BattleEnemy enemy = BattleEnemy.Find(_v.Target);
            if (!HasStealableItems(enemy))
            {
                UiState.SetBattleFollowFormatMessage(BattleMesages.DoesNotHaveAnything);
                return;
            }

            if (!_v.Caster.HasSupportAbility(SupportAbility2.Bandit) && Configuration.Hacks.StealingAlwaysWorks < 2)
            {
                _v.Context.HitRate = (Int16)(_v.Caster.Level + _v.Caster.Will);
                _v.Context.Evade = _v.Target.Level;

                if (GameRandom.Next16() % _v.Context.HitRate < GameRandom.Next16() % _v.Context.Evade)
                {
                    UiState.SetBattleFollowFormatMessage(BattleMesages.CouldNotStealAnything);
                    return;
                }
            }

            if (Configuration.Hacks.StealingAlwaysWorks >= 1) // cheat
            {
                if (enemy.StealableItems[3] != RegularItem.NoItem)
                    _v.StealItem(enemy, 3);
                else if (enemy.StealableItems[2] != RegularItem.NoItem)
                    _v.StealItem(enemy, 2);
                else if (enemy.StealableItems[1] != RegularItem.NoItem)
                    _v.StealItem(enemy, 1);
                else
                    _v.StealItem(enemy, 0);
                return;
            }

            if (_v.Caster.HasSupportAbility(SupportAbility1.MasterThief))
            {
                if (enemy.StealableItems[3] != RegularItem.NoItem && GameRandom.Next8() < Math.Max(32, (Int32)enemy.StealableItemRates[3]))
                    _v.StealItem(enemy, 3);
                else if (enemy.StealableItems[2] != RegularItem.NoItem && GameRandom.Next8() < Math.Max(32, (Int32)enemy.StealableItemRates[2]))
                    _v.StealItem(enemy, 2);
                else if (enemy.StealableItems[1] != RegularItem.NoItem && GameRandom.Next8() < Math.Max(32, (Int32)enemy.StealableItemRates[1]))
                    _v.StealItem(enemy, 1);
                else if (enemy.StealableItems[0] != RegularItem.NoItem && GameRandom.Next8() < Math.Max(32, (Int32)enemy.StealableItemRates[0]))
                    _v.StealItem(enemy, 0);
                else
                    UiState.SetBattleFollowFormatMessage(BattleMesages.CouldNotStealAnything);
            }
            else
            {
                if (GameRandom.Next8() < enemy.StealableItemRates[3])
                    _v.StealItem(enemy, 3);
                else if (GameRandom.Next8() < enemy.StealableItemRates[2])
                    _v.StealItem(enemy, 2);
                else if (GameRandom.Next8() < enemy.StealableItemRates[1])
                    _v.StealItem(enemy, 1);
                else if (GameRandom.Next8() < enemy.StealableItemRates[0])
                    _v.StealItem(enemy, 0);
                else
                    UiState.SetBattleFollowFormatMessage(BattleMesages.CouldNotStealAnything);
            }

        }

        private static Boolean HasStealableItems(BattleEnemy enemy)
        {
            for (Int16 slot = 0; slot < 4; ++slot)
                if (enemy.StealableItems[slot] != RegularItem.NoItem)
                    return true;
            return false;
        }
    }
}
