using System;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;

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

                if (enemy.StealableItems[3] != Byte.MaxValue)
                    StealItem(enemy, 3);
                else if (enemy.StealableItems[2] != Byte.MaxValue)
                    StealItem(enemy, 2);
                else if (enemy.StealableItems[1] != Byte.MaxValue)
                    StealItem(enemy, 1);
                else
                    StealItem(enemy, 0);
                return;
            }
            
            if (_v.Caster.HasSupportAbility(SupportAbility1.MasterThief))
            {
                if (enemy.StealableItems[3] != Byte.MaxValue && GameRandom.Next8() < 32)
                    StealItem(enemy, 3);
                else if (enemy.StealableItems[2] != Byte.MaxValue && GameRandom.Next8() < 32)
                    StealItem(enemy, 2);
                else if (enemy.StealableItems[1] != Byte.MaxValue && GameRandom.Next8() < 64)
                    StealItem(enemy, 1);
                else
                    StealItem(enemy, 0);
            }
            else
            {
                if (GameRandom.Next8() < 1)
                    StealItem(enemy, 3);
                else if (GameRandom.Next8() < 16)
                    StealItem(enemy, 2);
                else if (GameRandom.Next8() < 64)
                    StealItem(enemy, 1);
                else
                    StealItem(enemy, 0);
            }
            
        }

        private static Boolean HasStealableItems(BattleEnemy enemy)
        {
            Boolean hasStealableItems = false;
            for (Int16 slot = 0; slot < 4; ++slot)
            {
                if (enemy.StealableItems[slot] != Byte.MaxValue)
                    hasStealableItems = true;
            }
            return hasStealableItems;
        }

        private void StealItem(BattleEnemy enemy, Int32 slot)
        {
            Byte itemId = enemy.StealableItems[slot];
            if (itemId == Byte.MaxValue)
            {
                UiState.SetBattleFollowFormatMessage(BattleMesages.CouldNotStealAnything);
                return;
            }

            enemy.StealableItems[slot] = Byte.MaxValue;
            GameState.Thefts++;

            BattleItem.AddToInventory(itemId);
            UiState.SetBattleFollowFormatMessage(BattleMesages.Stole, FF9TextTool.ItemName(itemId));
            if (_v.Caster.HasSupportAbility(SupportAbility2.Mug))
            {
                _v.Target.Flags |= CalcFlag.HpAlteration;
                _v.Target.HpDamage = GameRandom.Next16() % (_v.Caster.Level * _v.Target.Level >> 1);
            }

            if (_v.Caster.HasSupportAbility(SupportAbility1.StealGil))
            {
                GameState.Gil += (UInt32)(GameRandom.Next16() % (1 + _v.Caster.Level * _v.Target.Level / 4));
            }
        }
    }
}
