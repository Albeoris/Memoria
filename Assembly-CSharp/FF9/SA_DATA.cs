using System;

namespace FF9
{
	public class SA_DATA
	{
		public SA_DATA(Byte category, Byte capa_val, UInt16 name, UInt16 help, UInt16 help_size)
		{
			this.category = category;
			this.capa_val = capa_val;
			this.name = name;
			this.help = help;
			this.help_size = help_size;
		}

		public Byte category;

		public Byte capa_val;

		public UInt16 name;

		public UInt16 help;

		public UInt16 help_size;
	}
}
