using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Grand Cross
    /// </summary>
    [BattleScript(Id)]
    public sealed class GrandCrossScript : IBattleScript
    {
        public const Int32 Id = 0105;

        private readonly BattleCalculator _v;

        public GrandCrossScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            const BattleStatus alteringStatuses =
            BattleStatus.Petrify | BattleStatus.Silence | BattleStatus.Blind | BattleStatus.Trouble | BattleStatus.Zombie
             | BattleStatus.Death | BattleStatus.Confuse | BattleStatus.Berserk | BattleStatus.Stop | BattleStatus.Poison
             | BattleStatus.Sleep | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Doom | BattleStatus.Mini
             | BattleStatus.LowHP;

            if (!_v.Target.CanBeAttacked())
                return;

            foreach (BattleStatusId statusId in alteringStatuses.ToStatusList())
            {
                if (GameRandom.Next8() >> 5 != 0)
                    continue;

                if (statusId == BattleStatusId.LowHP)
                {
                    if (!_v.Target.IsUnderStatus(BattleStatus.Death))
                    {
                        _v.Context.Flags |= BattleCalcFlags.DirectHP;
                        _v.Target.CurrentHp = (UInt32)(1 + GameRandom.Next8() % 9);
                    }
                }
                else
                {
                    _v.Target.AlterStatus(statusId.ToBattleStatus(), _v.Caster);
                }
            }
        }
    }
}
