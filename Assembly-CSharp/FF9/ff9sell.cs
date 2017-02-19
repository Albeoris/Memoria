using System;

namespace FF9
{
	public class ff9sell
	{
		public static void FF9Sell_Sell(Byte item_id, UInt16 sell_ct)
		{
			FF9ITEM_DATA ff9ITEM_DATA = ff9item._FF9Item_Data[(Int32)item_id];
			Int64 num = (Int64)(ff9ITEM_DATA.price >> 1);
			Int32 num2;
			if ((num2 = ff9item.FF9Item_Remove((Int32)item_id, (Int32)sell_ct)) > 0)
			{
				FF9StateSystem.Common.FF9.party.gil += UInt32.Parse((num * (Int64)num2).ToString());
				if (FF9StateSystem.Common.FF9.party.gil > 9999999u)
				{
					FF9StateSystem.Common.FF9.party.gil = 9999999u;
				}
			}
		}
	}
}
