using System;
using FF9;
using Memoria;
using Memoria.Data;

public class ff9feqp
{
	public static void FF9FEqp_Equip(Byte charPosID, ref Int32 currentItemIndex)
	{
		ff9feqp._FF9FEqp.player = charPosID;
		ff9feqp._FF9FEqp.equip = 0;
		ff9feqp._FF9FEqp.item[0] = new FF9ITEM(Byte.MaxValue, 0);
		FF9StateGlobal ff = FF9StateSystem.Common.FF9;
		Int32 num = 0;
		UInt16[] array = new UInt16[]
		{
			2048,
			1024,
			512,
			256,
			128,
			64,
			32,
			16,
			8,
			4,
			2,
			1
		};
		Byte[] array2 = new Byte[]
		{
			128,
			32,
			64,
			16,
			8
		};
	    PLAYER play = FF9StateSystem.Common.FF9.party.member[(Int32)ff9feqp._FF9FEqp.player];
	    UInt16 num2 = array[(CharacterId)ff9play.FF9Play_GetCharID2(play.Index, play.IsSubCharacter)];
		Byte b = array2[(Int32)ff9feqp._FF9FEqp.equip];
		for (Int32 i = 0; i < 256; i++)
		{
			if (ff.item[i].count != 0)
			{
				FF9ITEM_DATA ff9ITEM_DATA = ff9item._FF9Item_Data[(Int32)ff.item[i].id];
				if ((ff9ITEM_DATA.equip & num2) != 0 && (ff9ITEM_DATA.type & b) != 0)
				{
					ff9feqp._FF9FEqp.item[num++] = ff.item[i];
				}
			}
		}
		if (currentItemIndex >= num)
		{
			currentItemIndex = 0;
		}
		else if (currentItemIndex < 0)
		{
			currentItemIndex = num - 1;
		}
		PLAYER player = FF9StateSystem.Common.FF9.party.member[(Int32)ff9feqp._FF9FEqp.player];
		Int32 itemId1 = (Int32)player.equip[(Int32)ff9feqp._FF9FEqp.equip];
		Int32 itemId2 = (Int32)ff9feqp._FF9FEqp.item[currentItemIndex].id;
		if (itemId1 != CharacterEquipment.EmptyItemId)
		{
			ff9item.FF9Item_Add(itemId1, 1);
		}
		if (ff9item.FF9Item_Remove(itemId2, 1) != 0)
		{
			player.equip[(Int32)ff9feqp._FF9FEqp.equip] = (Byte)itemId2;
			ff9feqp.FF9FEqp_UpdatePlayer(player);
		}
	}

	private static void FF9FEqp_UpdatePlayer(PLAYER play)
	{
		ff9feqp.FF9FEqp_UpdateSA(play);
		ff9play.FF9Play_Update(play);
		play.info.serial_no = (Byte)ff9play.FF9Play_GetSerialID(play.info.slot_no, play.IsSubCharacter, play.equip);
	}

	private static void FF9FEqp_UpdateSA(PLAYER play)
	{
		Boolean[] array = new Boolean[64];
		if (!ff9abil.FF9Abil_HasAp(new Character(play)))
		{
			return;
		}
		for (Int32 i = 0; i < 5; i++)
		{
			if (play.equip[i] != 255)
			{
				FF9ITEM_DATA ff9ITEM_DATA = ff9item._FF9Item_Data[(Int32)play.equip[i]];
				for (Int32 j = 0; j < 3; j++)
				{
					Int32 num;
					if ((num = (Int32)ff9ITEM_DATA.ability[j]) != 0 && 192 <= num)
					{
						array[num - 192] = true;
					}
				}
			}
		}
		CharacterAbility[] array2 = ff9abil._FF9Abil_PaData[play.PresetId];
		for (Int32 i = 0; i < 48; i++)
		{
			if (192 <= array2[i].Id && ff9abil.FF9Abil_GetEnableSA(play.Index, array2[i].Id) && !array[array2[i].Id - 192] && play.pa[i] < array2[i].Ap)
			{
				ff9abil.FF9Abil_SetEnableSA(play.Index, array2[i].Id, false);
				Int32 capa_val = ff9abil._FF9Abil_SaData[array2[i].Id - 192].GemsCount;
				if (play.max.capa - play.cur.capa >= capa_val)
				{
					POINTS cur = play.cur;
					cur.capa = (Byte)(cur.capa + (Byte)capa_val);
				}
				else
				{
					play.cur.capa = play.max.capa;
				}
			}
		}
	}

	private static FF9FEQP _FF9FEqp = new FF9FEQP();
}
