using System;

namespace FF9
{
	public class STAT_DATA
	{
		public STAT_DATA(Byte priority, Byte opr_cnt, UInt16 conti_cnt, UInt32 clear, UInt32 invalid)
		{
			this.priority = priority;
			this.opr_cnt = opr_cnt;
			this.conti_cnt = conti_cnt;
			this.clear = clear;
			this.invalid = invalid;
		}

		public Byte priority;

		public Byte opr_cnt;

		public UInt16 conti_cnt;

		public UInt32 clear;

		public UInt32 invalid;
	}
}
