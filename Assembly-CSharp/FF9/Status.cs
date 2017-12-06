using System;
using Memoria.Data;

namespace FF9
{
	public class Status
	{
	    public static Boolean checkCurStat(BTL_DATA btl, BattleStatus status)
	    {
	        return (btl.stat.cur & status) != 0u;
        }

	    public const UInt32 STATUS_MASK_FIELD = 127u;

		public const UInt32 STATUS_MASK_COUNT = 4026466304u;
	    public const BattleStatus STATUS_MASK = BattleStatus.Poison | BattleStatus.Sleep | BattleStatus.Regen | BattleStatus.Haste | BattleStatus.Slow | BattleStatus.Float | BattleStatus.Shell | BattleStatus.Protect | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Vanish | BattleStatus.Doom | BattleStatus.Reflect | BattleStatus.Jump | BattleStatus.GradualPetrify;

        public const UInt32 STAT_OPR_POISON2 = 0u;

		public const UInt32 STAT_OPR_POISON = 1u;

		public const UInt32 STAT_OPR_REGENE = 2u;
	}
}
