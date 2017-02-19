using System;

namespace FF9
{
	public class BTL_VFX_REQ
	{
		public BTL_VFX_REQ()
		{
			this.trg = new BTL_DATA[8];
			this.monbone = new Byte[2];
			this.rtrg = new BTL_DATA[4];
		}

		public const UInt16 BTL_REQ_TRG_MAX = 8;

		public const UInt16 BTL_REQ_RTRG_MAX = 4;

		public BTL_DATA exe;

		public BTL_DATA[] trg;

		public SByte trgno;

		public Byte[] monbone;

		public SByte rtrgno;

		public BTL_DATA[] rtrg;

		public PSX_LIBGTE.VECTOR trgcpos;

		public UInt16 flgs;

		public Int16 arg0;

		public BTL_DATA mexe;

		public Int32 dmy;
	}
}
