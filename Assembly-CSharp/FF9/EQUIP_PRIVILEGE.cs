using System;

namespace FF9
{
	public class EQUIP_PRIVILEGE
	{
		public EQUIP_PRIVILEGE(Byte dex, Byte str, Byte mgc, Byte wpr, DEF_ATTR def_attr, Byte p_up_attr)
		{
			this.dex = dex;
			this.str = str;
			this.mgc = mgc;
			this.wpr = wpr;
			this.def_attr = def_attr;
			this.p_up_attr = p_up_attr;
		}

		public Byte dex;

		public Byte str;

		public Byte mgc;

		public Byte wpr;

		public DEF_ATTR def_attr;

		public Byte p_up_attr;
	}
}
