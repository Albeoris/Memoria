using System;

namespace FF9
{
	public class Status
	{
		public static Boolean checkCurStat(BTL_DATA btl, UInt32 status)
		{
			return (btl.stat.cur & status) != 0u;
		}

		public const UInt32 STATUS_MASK_FIELD = 127u;

		public const UInt32 STATUS_MASK_COUNT = 4026466304u;

		public const UInt32 STAT_OPR_POISON2 = 0u;

		public const UInt32 STAT_OPR_POISON = 1u;

		public const UInt32 STAT_OPR_REGENE = 2u;
	}
}
