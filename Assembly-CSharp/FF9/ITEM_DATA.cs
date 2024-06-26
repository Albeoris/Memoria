using System;
using Memoria.Data;

namespace FF9
{
	public class ITEM_DATA
	{
		public ITEM_DATA(BattleCommandInfo info, BTL_REF Ref, BattleStatus status)
		{
			this.info = info;
			this.Ref = Ref;
			this.status = status;
		}

		public BattleCommandInfo info;

		public BTL_REF Ref;

		public BattleStatus status;
	}
}
