using System;
using FF9;

public class ff9level
{
	public static Int32 FF9Level_GetDex(Int32 slot_id, Int32 lv, Boolean lvup)
	{
		PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
		FF9LEVEL_BONUS bonus = player.bonus;
		FF9LEVEL_BASE ff9LEVEL_BASE = ff9level._FF9Level_Base[ff9play.FF9Play_GetCharID((Int32)player.info.menu_type)];
		if (lvup)
		{
			Int32 num = 0;
			Int32 num2 = ff9level.FF9Level_GetEquipBonus(player.equip, 0);
			FF9LEVEL_BONUS ff9LEVEL_BONUS = bonus;
			ff9LEVEL_BONUS.dex = (UInt16)(ff9LEVEL_BONUS.dex + (UInt16)(num + num2));
		}
		Int32 num3 = (Int32)ff9LEVEL_BASE.dex + lv / 10 + (bonus.dex >> 5);
		if (num3 > 50)
		{
			num3 = 50;
		}
		return num3;
	}

	public static Int32 FF9Level_GetStr(Int32 slot_id, Int32 lv, Boolean lvup)
	{
		PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
		FF9LEVEL_BONUS bonus = player.bonus;
		FF9LEVEL_BASE ff9LEVEL_BASE = ff9level._FF9Level_Base[ff9play.FF9Play_GetCharID((Int32)player.info.menu_type)];
		if (lvup)
		{
			Int32 num = (Int32)((player.cur.capa != 0) ? 0 : 3);
			Int32 num2 = ff9level.FF9Level_GetEquipBonus(player.equip, 1);
			FF9LEVEL_BONUS ff9LEVEL_BONUS = bonus;
			ff9LEVEL_BONUS.str = (UInt16)(ff9LEVEL_BONUS.str + (UInt16)(num + num2));
		}
		Int32 num3 = (Int32)ff9LEVEL_BASE.str + lv * 3 / 10 + (bonus.str >> 5);
		if (num3 > 99)
		{
			num3 = 99;
		}
		return num3;
	}

	public static Int32 FF9Level_GetMgc(Int32 slot_id, Int32 lv, Boolean lvup)
	{
		PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
		FF9LEVEL_BONUS bonus = player.bonus;
		FF9LEVEL_BASE ff9LEVEL_BASE = ff9level._FF9Level_Base[ff9play.FF9Play_GetCharID((Int32)player.info.menu_type)];
		if (lvup)
		{
			Int32 num = (Int32)((player.cur.capa != 0) ? 0 : 3);
			Int32 num2 = ff9level.FF9Level_GetEquipBonus(player.equip, 2);
			FF9LEVEL_BONUS ff9LEVEL_BONUS = bonus;
			ff9LEVEL_BONUS.mgc = (UInt16)(ff9LEVEL_BONUS.mgc + (UInt16)(num + num2));
		}
		Int32 num3 = (Int32)ff9LEVEL_BASE.mgc + lv * 3 / 10 + (bonus.mgc >> 5);
		if (num3 > 99)
		{
			num3 = 99;
		}
		return num3;
	}

	public static Int32 FF9Level_GetWpr(Int32 slot_id, Int32 lv, Boolean lvup)
	{
		PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
		FF9LEVEL_BONUS bonus = player.bonus;
		FF9LEVEL_BASE ff9LEVEL_BASE = ff9level._FF9Level_Base[ff9play.FF9Play_GetCharID((Int32)player.info.menu_type)];
		if (lvup)
		{
			Int32 num = (Int32)((player.cur.capa != 0) ? 0 : 1);
			Int32 num2 = ff9level.FF9Level_GetEquipBonus(player.equip, 3);
			FF9LEVEL_BONUS ff9LEVEL_BONUS = bonus;
			ff9LEVEL_BONUS.wpr = (UInt16)(ff9LEVEL_BONUS.wpr + (UInt16)(num + num2));
		}
		Int32 num3 = (Int32)ff9LEVEL_BASE.wpr + lv * 3 / 20 + (bonus.wpr >> 5);
		if (num3 > 50)
		{
			num3 = 50;
		}
		return num3;
	}

	public static Int32 FF9Level_GetCap(Int32 slot_id, Int32 lv, Boolean lvup)
	{
		PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
		FF9LEVEL_BONUS bonus = player.bonus;
		FF9LEVEL_BASE ff9LEVEL_BASE = ff9level._FF9Level_Base[ff9play.FF9Play_GetCharID((Int32)player.info.menu_type)];
		if (lvup)
		{
			Int32 num = (Int32)((player.cur.capa != 0) ? 0 : 5);
			Int32 num2 = 0;
			FF9LEVEL_BONUS ff9LEVEL_BONUS = bonus;
			ff9LEVEL_BONUS.cap = (UInt16)(ff9LEVEL_BONUS.cap + (UInt16)(num + num2));
		}
		Int32 num3 = (Int32)ff9LEVEL_BASE.cap + lv * 4 / 10 + (bonus.cap >> 5);
		if (num3 > 99)
		{
			num3 = 99;
		}
		return num3;
	}

	public static Int32 FF9Level_GetHp(Int32 lv, Int32 str)
	{
		Int32 num = (Int32)ff9level._FF9Level_HpMp[lv - 1].hp * str / 50;
		if (num > 9999)
		{
			num = 9999;
		}
		return num;
	}

	public static Int32 FF9Level_GetMp(Int32 lv, Int32 mgc)
	{
		Int32 num = (Int32)ff9level._FF9Level_HpMp[lv - 1].mp * mgc / 100;
		if (num > 999)
		{
			num = 999;
		}
		return num;
	}

	public static Int32 FF9Level_GetEquipBonus(Byte[] equip, Int32 base_type)
	{
		Int32 num = 0;
		for (Int32 i = 0; i < 5; i++)
		{
			if (equip[i] != 255)
			{
				FF9ITEM_DATA ff9ITEM_DATA = ff9item._FF9Item_Data[(Int32)equip[i]];
				EQUIP_PRIVILEGE equip_PRIVILEGE = ff9equip._FF9EquipBonus_Data[(Int32)ff9ITEM_DATA.bonus];
				switch (base_type)
				{
				case 0:
					num += (Int32)equip_PRIVILEGE.dex;
					break;
				case 1:
					num += (Int32)equip_PRIVILEGE.str;
					break;
				case 2:
					num += (Int32)equip_PRIVILEGE.mgc;
					break;
				case 3:
					num += (Int32)equip_PRIVILEGE.wpr;
					break;
				}
			}
		}
		return num;
	}

	public const Byte FF9LEVEL_DEX_MAX = 50;

	public const Byte FF9LEVEL_STR_MAX = 99;

	public const Byte FF9LEVEL_MGC_MAX = 99;

	public const Byte FF9LEVEL_WPR_MAX = 50;

	public const Byte FF9LEVEL_CAP_MAX = 99;

	public const Int32 FF9LEVEL_HP_MAX = 9999;

	public const Int32 FF9LEVEL_MP_MAX = 999;

	public static FF9LEVEL_BASE[] _FF9Level_Base = new FF9LEVEL_BASE[]
	{
		new FF9LEVEL_BASE(23, 21, 18, 23, 18),
		new FF9LEVEL_BASE(16, 12, 24, 19, 14),
		new FF9LEVEL_BASE(21, 14, 23, 17, 14),
		new FF9LEVEL_BASE(18, 24, 12, 21, 17),
		new FF9LEVEL_BASE(20, 20, 16, 22, 18),
		new FF9LEVEL_BASE(14, 18, 20, 11, 15),
		new FF9LEVEL_BASE(19, 13, 21, 18, 12),
		new FF9LEVEL_BASE(22, 22, 13, 15, 18),
		new FF9LEVEL_BASE(19, 15, 16, 19, 8),
		new FF9LEVEL_BASE(20, 18, 11, 21, 8),
		new FF9LEVEL_BASE(23, 21, 12, 21, 12),
		new FF9LEVEL_BASE(24, 21, 19, 23, 10)
	};

	public static FF9LEVEL_HPMP[] _FF9Level_HpMp = new FF9LEVEL_HPMP[]
	{
		new FF9LEVEL_HPMP(250, 200),
		new FF9LEVEL_HPMP(314, 206),
		new FF9LEVEL_HPMP(382, 212),
		new FF9LEVEL_HPMP(454, 219),
		new FF9LEVEL_HPMP(530, 226),
		new FF9LEVEL_HPMP(610, 234),
		new FF9LEVEL_HPMP(694, 242),
		new FF9LEVEL_HPMP(782, 250),
		new FF9LEVEL_HPMP(874, 259),
		new FF9LEVEL_HPMP(970, 268),
		new FF9LEVEL_HPMP(1062, 277),
		new FF9LEVEL_HPMP(1150, 285),
		new FF9LEVEL_HPMP(1234, 293),
		new FF9LEVEL_HPMP(1314, 301),
		new FF9LEVEL_HPMP(1390, 308),
		new FF9LEVEL_HPMP(1462, 315),
		new FF9LEVEL_HPMP(1530, 321),
		new FF9LEVEL_HPMP(1594, 327),
		new FF9LEVEL_HPMP(1662, 333),
		new FF9LEVEL_HPMP(1734, 340),
		new FF9LEVEL_HPMP(1810, 347),
		new FF9LEVEL_HPMP(1890, 355),
		new FF9LEVEL_HPMP(1974, 363),
		new FF9LEVEL_HPMP(2062, 371),
		new FF9LEVEL_HPMP(2154, 380),
		new FF9LEVEL_HPMP(2250, 389),
		new FF9LEVEL_HPMP(2350, 399),
		new FF9LEVEL_HPMP(2454, 409),
		new FF9LEVEL_HPMP(2562, 419),
		new FF9LEVEL_HPMP(2674, 430),
		new FF9LEVEL_HPMP(2790, 441),
		new FF9LEVEL_HPMP(2910, 453),
		new FF9LEVEL_HPMP(3034, 465),
		new FF9LEVEL_HPMP(3162, 477),
		new FF9LEVEL_HPMP(3282, 489),
		new FF9LEVEL_HPMP(3394, 500),
		new FF9LEVEL_HPMP(3498, 510),
		new FF9LEVEL_HPMP(3594, 519),
		new FF9LEVEL_HPMP(3682, 527),
		new FF9LEVEL_HPMP(3762, 535),
		new FF9LEVEL_HPMP(3834, 542),
		new FF9LEVEL_HPMP(3898, 548),
		new FF9LEVEL_HPMP(3958, 554),
		new FF9LEVEL_HPMP(4014, 559),
		new FF9LEVEL_HPMP(4066, 564),
		new FF9LEVEL_HPMP(4114, 568),
		new FF9LEVEL_HPMP(4158, 572),
		new FF9LEVEL_HPMP(4198, 576),
		new FF9LEVEL_HPMP(4234, 579),
		new FF9LEVEL_HPMP(4266, 582),
		new FF9LEVEL_HPMP(4294, 584),
		new FF9LEVEL_HPMP(4317, 586),
		new FF9LEVEL_HPMP(4334, 587),
		new FF9LEVEL_HPMP(4344, 588),
		new FF9LEVEL_HPMP(4353, 589),
		new FF9LEVEL_HPMP(4361, 590),
		new FF9LEVEL_HPMP(4368, 591),
		new FF9LEVEL_HPMP(4374, 592),
		new FF9LEVEL_HPMP(4379, 593),
		new FF9LEVEL_HPMP(4383, 594),
		new FF9LEVEL_HPMP(4386, 595),
		new FF9LEVEL_HPMP(4388, 596),
		new FF9LEVEL_HPMP(4389, 597),
		new FF9LEVEL_HPMP(4390, 598),
		new FF9LEVEL_HPMP(4391, 599),
		new FF9LEVEL_HPMP(4392, 600),
		new FF9LEVEL_HPMP(4393, 601),
		new FF9LEVEL_HPMP(4394, 602),
		new FF9LEVEL_HPMP(4395, 603),
		new FF9LEVEL_HPMP(4396, 604),
		new FF9LEVEL_HPMP(4397, 605),
		new FF9LEVEL_HPMP(4398, 606),
		new FF9LEVEL_HPMP(4399, 607),
		new FF9LEVEL_HPMP(4400, 608),
		new FF9LEVEL_HPMP(4401, 609),
		new FF9LEVEL_HPMP(4402, 610),
		new FF9LEVEL_HPMP(4403, 611),
		new FF9LEVEL_HPMP(4404, 612),
		new FF9LEVEL_HPMP(4405, 613),
		new FF9LEVEL_HPMP(4406, 614),
		new FF9LEVEL_HPMP(4407, 615),
		new FF9LEVEL_HPMP(4408, 616),
		new FF9LEVEL_HPMP(4409, 617),
		new FF9LEVEL_HPMP(4410, 618),
		new FF9LEVEL_HPMP(4411, 619),
		new FF9LEVEL_HPMP(4412, 620),
		new FF9LEVEL_HPMP(4413, 621),
		new FF9LEVEL_HPMP(4414, 622),
		new FF9LEVEL_HPMP(4415, 623),
		new FF9LEVEL_HPMP(4416, 624),
		new FF9LEVEL_HPMP(4417, 625),
		new FF9LEVEL_HPMP(4418, 626),
		new FF9LEVEL_HPMP(4419, 627),
		new FF9LEVEL_HPMP(4420, 628),
		new FF9LEVEL_HPMP(4421, 629),
		new FF9LEVEL_HPMP(4422, 630),
		new FF9LEVEL_HPMP(4423, 631),
		new FF9LEVEL_HPMP(4424, 632),
		new FF9LEVEL_HPMP(4524, 642)
	};

	public static UInt64[] _FF9Level_Exp = new UInt64[]
	{
		0UL,
		16UL,
		47UL,
		101UL,
		186UL,
		314UL,
		496UL,
		746UL,
		1078UL,
		1510UL,
		2059UL,
		2745UL,
		3588UL,
		4612UL,
		5840UL,
		7298UL,
		9012UL,
		11012UL,
		13327UL,
		15989UL,
		19030UL,
		22486UL,
		26392UL,
		30786UL,
		35706UL,
		41194UL,
		47291UL,
		54041UL,
		61488UL,
		69680UL,
		78664UL,
		88490UL,
		99208UL,
		110872UL,
		123535UL,
		137253UL,
		152082UL,
		168082UL,
		185312UL,
		203834UL,
		223710UL,
		245006UL,
		267787UL,
		292121UL,
		318076UL,
		345724UL,
		375136UL,
		406386UL,
		439548UL,
		474700UL,
		511919UL,
		551285UL,
		592878UL,
		636782UL,
		683080UL,
		731858UL,
		783202UL,
		837202UL,
		893947UL,
		953529UL,
		1016040UL,
		1081576UL,
		1150232UL,
		1222106UL,
		1297296UL,
		1375904UL,
		1458031UL,
		1543781UL,
		1633258UL,
		1726570UL,
		1823824UL,
		1925130UL,
		2030598UL,
		2140342UL,
		2254475UL,
		2373113UL,
		2496372UL,
		2624372UL,
		2757232UL,
		2895074UL,
		3038020UL,
		3186196UL,
		3339727UL,
		3498741UL,
		3663366UL,
		3833734UL,
		4009976UL,
		4192226UL,
		4380618UL,
		4575290UL,
		4776379UL,
		4984025UL,
		5198368UL,
		5419552UL,
		5647720UL,
		5883018UL,
		6125592UL,
		6375592UL,
		6633167UL
	};

	public enum level_base
	{
		FF9LEVEL_BASE_DEX,
		FF9LEVEL_BASE_STR,
		FF9LEVEL_BASE_MGC,
		FF9LEVEL_BASE_WPR,
		FF9LEVEL_BASE_MAX
	}
}
