using System;

namespace FF9
{
	public class DEF_ATTR
	{
		public DEF_ATTR()
		{
			this.invalid = 0;
			this.absorb = 0;
			this.half = 0;
			this.weak = 0;
		}

		public DEF_ATTR(Byte invalid, Byte absorb, Byte half, Byte weak)
		{
			this.invalid = invalid;
			this.absorb = absorb;
			this.half = half;
			this.weak = weak;
		}

		public Byte invalid;

		public Byte absorb;

		public Byte half;

		public Byte weak;
	}
}
