using System;
using Memoria.Data;

namespace FF9
{
	public static class ff9shop
	{
	    public static Int32 FF9Shop_GetDefence(Int32 part, CharacterEquipment equip)
	    {
	        Int32 result = 0;
	        for (Int32 i = 0; i < 5; i++)
	        {
				if (equip[i] != RegularItem.NoItem && ff9item.HasItemArmor(equip[i]))
				{
					ItemDefence equipArmor = ff9item.GetItemArmor(equip[i]);
					switch (part)
					{
						case 1:
							result += equipArmor.MagicalDefence;
							break;
						case 2:
							result += equipArmor.PhysicalEvade;
							break;
						case 3:
							result += equipArmor.PhysicalDefence;
							break;
						case 4:
							break;
					}
	            }
	        }
	        return result;
		}

		public static ShopUI.ShopType FF9Shop_GetType(Int32 shopId)
		{
			return ff9buy.FF9Buy_GetType(shopId);
		}
	}
}
