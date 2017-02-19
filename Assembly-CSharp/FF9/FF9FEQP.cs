using System;

namespace FF9
{
	public class FF9FEQP
	{
		public FF9FEQP()
		{
			this.item = new FF9ITEM[256];
			this.cur_capa = new Boolean[4];
		}

		public Byte mode;

		public Byte input;

		public Byte equip;

		public Byte player;

		public FF9ITEM[] item;

		public Boolean[] cur_capa;

		public Boolean is_win;

		public Byte old_input;
	}
}
