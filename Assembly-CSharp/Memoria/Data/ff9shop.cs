using System;
using Memoria.Data;

namespace FF9
{
	public class ff9shop
	{
		public static void FF9Shop_Init()
		{
			ff9shop._FF9Shop.type = Byte.Parse(ff9shop.FF9Shop_GetType((Int32)ff9shop._FF9Shop.shop_id).ToString());
			ff9shop._FF9Shop.mode = 0;
			ff9shop._FF9Shop.enter = 0;
			ff9shop._FF9Shop.come_count = 0;
		}

		public static Int32 FF9Shop_GetDefence(Int32 part, CharacterEquipment equip)
		{
			Int32 result = 0;
			DEF_PARAMS def_PARAMS = new DEF_PARAMS(0, 0, 0, 0);
			for (Int32 i = 1; i < 5; i++)
			{
				if (equip[i] != 255 && 224 > equip[i])
				{
					Int32 num = (Int32)(equip[i] - 88);
					DEF_PARAMS def_PARAMS2 = def_PARAMS;
					def_PARAMS2.p_def = (Byte)(def_PARAMS2.p_def + ff9armor._FF9Armor_Data[num].p_def);
					DEF_PARAMS def_PARAMS3 = def_PARAMS;
					def_PARAMS3.p_ev = (Byte)(def_PARAMS3.p_ev + ff9armor._FF9Armor_Data[num].p_ev);
					DEF_PARAMS def_PARAMS4 = def_PARAMS;
					def_PARAMS4.m_def = (Byte)(def_PARAMS4.m_def + ff9armor._FF9Armor_Data[num].m_def);
					DEF_PARAMS def_PARAMS5 = def_PARAMS;
					def_PARAMS5.m_ev = (Byte)(def_PARAMS5.m_ev + ff9armor._FF9Armor_Data[num].m_ev);
				}
			}
			switch (part)
			{
			case 1:
				result = (Int32)def_PARAMS.m_def;
				break;
			case 2:
				result = (Int32)def_PARAMS.p_ev;
				break;
			case 3:
				result = (Int32)def_PARAMS.p_def;
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
