using System;
using Memoria.Data;

namespace FF9
{
	public class ff9shop
	{
		public static void FF9Shop_Init()
		{
			ff9shop._FF9Shop.type = Byte.Parse(ff9shop.FF9Shop_GetType(ff9shop._FF9Shop.shop_id).ToString());
			ff9shop._FF9Shop.mode = 0;
			ff9shop._FF9Shop.enter = 0;
			ff9shop._FF9Shop.come_count = 0;
		}

	    public static Int32 FF9Shop_GetDefence(Int32 part, CharacterEquipment equip)
	    {
	        Int32 result = 0;
	        ItemDefence def = new ItemDefence();
	        for (Int32 i = 1; i < 5; i++)
	        {
	            if (equip[i] != 255 && 224 > equip[i])
	            {
	                Int32 num = equip[i] - 88;
	                def.PhisicalDefence = (Byte)(def.PhisicalDefence + ff9armor.ArmorData[num].PhisicalDefence);
	                def.PhisicalEvade = (Byte)(def.PhisicalEvade + ff9armor.ArmorData[num].PhisicalEvade);
	                def.MagicalDefence = (Byte)(def.MagicalDefence + ff9armor.ArmorData[num].MagicalDefence);
	                def.MagicalEvade = (Byte)(def.MagicalEvade + ff9armor.ArmorData[num].MagicalEvade);
	            }
	        }
	        switch (part)
	        {
	            case 1:
	                result = def.MagicalDefence;
	                break;
	            case 2:
	                result = def.PhisicalEvade;
	                break;
	            case 3:
	                result = def.PhisicalDefence;
	                break;
	            case 4:
	                result = 0;
	                break;
	        }
	        return result;
	    }

	    public static ShopUI.ShopType FF9Shop_GetType(Int32 shopId)
		{
			return ((shopId < 32) ? ff9buy.FF9Buy_GetType(shopId) : ShopUI.ShopType.Synthesis);
		}

		public const UInt16 FF9SHOP_TYPE_ITEM = 0;

		public const UInt16 FF9SHOP_TYPE_WEAPON = 1;

		public const UInt16 FF9SHOP_TYPE_SMITH = 2;

		public const UInt16 FF9SHOP_TYPE_MAX = 3;

		public const UInt16 FF9SHOP_MODE_MAX = 3;

		public const UInt16 FF9SHOP_SHOP_MAX = 40;

		public const UInt16 FF9SHOP_ITEM_MAX = 32;

		public const UInt16 FF9SHOP_WEAPON_MAX = 32;

		public const UInt16 FF9SHOP_SMITH_MAX = 8;

		public const UInt16 FF9SHOP_SMITH_START = 32;

		public const UInt16 FF9SHOP_PARTY_MAX = 8;

		public static FF9SHOP_INFO _FF9Shop = new FF9SHOP_INFO();
	}
}
