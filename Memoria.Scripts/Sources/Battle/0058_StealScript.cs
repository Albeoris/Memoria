using System;
using System.Linq;
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

        static Int32 Bandit = 1;
        static Int32 MasterThief = 2;
        static Int32 BanditMasterThief = 3;
        static Int32 AlwaysSteal = 4;

        public void Perform()
        {
            BattleEnemy enemy = BattleEnemy.Find(_v.Target);
            if (!HasStealableItems(enemy))
            {
                UiState.SetBattleFollowFormatMessage(BattleMesages.DoesNotHaveAnything);
                return;
            }

            Int32 StealAugment = Configuration.Battle.StealAugment;
            if (StealAugment == AlwaysSteal) 
            {
                var index = Enumerable.Range(0, 4).FirstOrDefault(x => HasItem(enemy.StealableItems[x]));
                StealItem(enemy, index);
                return;
            }

            if (StealAugment != Bandit || StealAugment != BanditMasterThief || !_v.Caster.HasSupportAbility(SupportAbility2.Bandit))
            {
                _v.Context.HitRate = (Int16)(_v.Caster.Level + _v.Caster.Will);
                _v.Context.Evade = _v.Target.Level;

                if (GameRandom.Next16() % _v.Context.HitRate < GameRandom.Next16() % _v.Context.Evade)
                {
                    UiState.SetBattleFollowFormatMessage(BattleMesages.CouldNotStealAnything);
                    return;
                }
            }

            if (StealAugment != MasterThief || StealAugment != BanditMasterThief || _v.Caster.HasSupportAbility(SupportAbility1.MasterThief))
            {
                if (HasItem(enemy.StealableItems[3]) && GameRandom.Next8() < 32)
                    StealItem(enemy, 3);
                else if (HasItem(enemy.StealableItems[2]) && GameRandom.Next8() < 32)
                    StealItem(enemy, 2);
                else if (HasItem(enemy.StealableItems[1]) && GameRandom.Next8() < 64)
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

        private static Boolean HasItem(Byte item)
        {
            return item != Byte.MaxValue;
        }

        private static Boolean HasStealableItems(BattleEnemy enemy)
        {
            for (Int16 index = 0; index < 4; ++index)
            {
                if (HasItem(enemy.StealableItems[index]))
                    return true;
            }
            return false;
        }

        private void StealItem(BattleEnemy enemy, Int32 itemIndex)
        {
            Byte itemId = enemy.StealableItems[itemIndex];
            if (!HasItem(itemId))
            {
                UiState.SetBattleFollowFormatMessage(BattleMesages.CouldNotStealAnything);
                return;
            }

            enemy.StealableItems[itemIndex] = Byte.MaxValue;
            GameState.Thefts++;

            BattleItem.AddToInventory(itemId);
            UiState.SetBattleFollowFormatMessage(BattleMesages.Stole, FF9TextTool.ItemName(itemId));
            if (_v.Caster.HasSupportAbility(SupportAbility2.Mug))
            {
                _v.Target.Flags |= CalcFlag.HpAlteration;
                _v.Target.HpDamage = (Int16)(GameRandom.Next16() % (_v.Caster.Level * _v.Target.Level >> 1));
            }

            if (_v.Caster.HasSupportAbility(SupportAbility1.StealGil))
            {
                GameState.Gil += (UInt32)(GameRandom.Next16() % (1 + _v.Caster.Level * _v.Target.Level / 4));
            }
        }
    }
}