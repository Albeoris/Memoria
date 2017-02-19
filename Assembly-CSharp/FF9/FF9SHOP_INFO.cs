using System;

namespace FF9
{
	public class FF9SHOP_INFO
	{
		public Byte type;

		public Byte mode;

		public Byte mode_max;

		public Byte enter;

		public Byte come_count;

		public Byte[] pad = new Byte[3];

		public Byte shop_id;

		public Byte wipe_z;

		public Byte[] talk_text;

		public Int64 party_ct;

		public Byte[] slot_id = new Byte[8];

		public Int64 talk_z;
	}
}
