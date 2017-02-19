using System;

namespace FF9
{
	public class FF9MIX_DATA
	{
		public FF9MIX_DATA(UInt16 price, Byte[] src, Byte dst, Byte shop)
		{
			this.price = price;
			this.src = src;
			this.dst = dst;
			this.shop = shop;
		}

		public UInt16 price;

		public Byte[] src = new Byte[2];

		public Byte dst;

		public Byte shop;
	}
}
