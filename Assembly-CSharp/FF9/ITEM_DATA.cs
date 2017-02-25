using System;

namespace FF9
{
	public class ITEM_DATA
	{
		public ITEM_DATA(BattleCommandInfo info, BTL_REF Ref, UInt32 status)
		{
			this.info = info;
			this.Ref = Ref;
			this.status = status;
		}

		public BattleCommandInfo info;

		public BTL_REF Ref;

		public UInt32 status;
	}
}
