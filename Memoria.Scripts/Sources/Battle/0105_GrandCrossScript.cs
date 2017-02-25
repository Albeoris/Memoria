using System;
using Memoria.Data;

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
            const UInt32 alteringStatuses = (UInt32)
            (BattleStatus.Petrify | BattleStatus.Silence | BattleStatus.Blind | BattleStatus.Trouble | BattleStatus.Zombie
             | BattleStatus.Death | BattleStatus.Confuse | BattleStatus.Berserk | BattleStatus.Stop | BattleStatus.Poison
             | BattleStatus.Sleep | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Doom | BattleStatus.Mini);

            if (!_v.Target.CanBeAttacked())
                return;

            UInt32 status = 1;
            for (Int32 i = 0; i < 32; i++, status <<= 1)
            {
                if (GameRandom.Next8() >> 5 != 0)
                    continue;

                if ((status & alteringStatuses) != 0)
                {
                    _v.Target.AlterStatus((BattleStatus)status);
                }
                else if ((status & (UInt32)BattleStatus.LowHP) != 0 && !_v.Target.IsUnderStatus(BattleStatus.Death))
                {
                    _v.Context.Flags |= BattleCalcFlags.DirectHP;
                    _v.Target.CurrentHp = (UInt16)(1 + GameRandom.Next8() % 9);
                }
            }
        }
    }
}