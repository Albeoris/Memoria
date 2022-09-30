using System;
using System.Collections.Generic;
using FF9;
using Memoria;
using Memoria.Data;

public static class ff9feqp
{
	public static UInt16 GetCharacterEquipMask(PLAYER player)
	{
		return (UInt16)((Int32)player.Index <= 11 ? 1 << (11 - (Int32)player.Index) : 1 << (Int32)player.Index);
	}

	public static void FF9FEqp_UpdatePlayer(PLAYER play)
	{
		ff9feqp.FF9FEqp_UpdateSA(play);
		ff9play.FF9Play_Update(play);
		play.info.serial_no = ff9play.FF9Play_GetSerialID(play.info.slot_no, play.equip);
	}

	public static void FF9FEqp_UpdateSA(PLAYER player)
	{
		List<Int32> equipSAList = new List<Int32>();
		if (!ff9abil.FF9Abil_HasAp(new Character(player)))
			return;

		for (Int32 i = 0; i < 5; i++)
		{
			if (player.equip[i] != 255)
			{
				FF9ITEM_DATA ff9ITEM_DATA = ff9item._FF9Item_Data[player.equip[i]];
				for (Int32 j = 0; j < 3; j++)
				{
					Int32 abilId = ff9ITEM_DATA.ability[j];
					if (abilId != 0 && 192 <= abilId)
						equipSAList.Add(abilId - 192);
				}
			}
		}

		CharacterAbility[] playerAbilList = ff9abil._FF9Abil_PaData[(Int32)player.PresetId];
		if (Configuration.Battle.LockEquippedAbilities == 1 || Configuration.Battle.LockEquippedAbilities == 3)
		{
			for (Int32 saIndex = 0; saIndex < 64; saIndex++)
				ff9abil.FF9Abil_SetEnableSA(player, 192 + saIndex, equipSAList.Contains(saIndex) && Array.Exists(playerAbilList, abil => abil.Id == 192 + saIndex));
		}
		else
		{
			for (Int32 k = 0; k < 48; k++)
			{
				if (192 <= playerAbilList[k].Id && ff9abil.FF9Abil_GetEnableSA(player, playerAbilList[k].Id) && !equipSAList.Contains(playerAbilList[k].Id - 192) && player.pa[k] < playerAbilList[k].Ap)
				{
					ff9abil.FF9Abil_SetEnableSA(player, playerAbilList[k].Id, false);
					Byte capa_val = ff9abil._FF9Abil_SaData[playerAbilList[k].Id - 192].GemsCount;
					if (player.max.capa - player.cur.capa >= capa_val)
						player.cur.capa += capa_val;
					else
						player.cur.capa = player.max.capa;
				}
			}
		}
	}

	public static void FF9FEqp_Equip(Byte charPosID, ref Int32 currentItemIndex)
	{
		Byte[] partMask = new Byte[]
		{
			128,
			32,
			64,
			16,
			8
		};
		ff9feqp._FF9FEqp.player = charPosID;
		ff9feqp._FF9FEqp.equip = 0;
		ff9feqp._FF9FEqp.item[0] = new FF9ITEM(Byte.MaxValue, 0);
		FF9StateGlobal ff = FF9StateSystem.Common.FF9;
		Int32 itemSlot = 0;
		PLAYER player = ff.party.member[ff9feqp._FF9FEqp.player];
		UInt16 characterMask = ff9feqp.GetCharacterEquipMask(player);
		Byte equipSlotMask = partMask[ff9feqp._FF9FEqp.equip];
		for (Int32 i = 0; i < 256; i++)
		{
			if (ff.item[i].count != 0)
			{
				FF9ITEM_DATA item = ff9item._FF9Item_Data[ff.item[i].id];
				if ((item.equip & characterMask) != 0 && (item.type & equipSlotMask) != 0)
					ff9feqp._FF9FEqp.item[itemSlot++] = ff.item[i];
			}
		}
		if (currentItemIndex >= itemSlot)
			currentItemIndex = 0;
		else if (currentItemIndex < 0)
			currentItemIndex = itemSlot - 1;
		Int32 itemId1 = player.equip[ff9feqp._FF9FEqp.equip];
		Int32 itemId2 = ff9feqp._FF9FEqp.item[currentItemIndex].id;
		if (itemId1 != CharacterEquipment.EmptyItemId)
			ff9item.FF9Item_Add(itemId1, 1);
		if (ff9item.FF9Item_Remove(itemId2, 1) != 0)
		{
			player.equip[ff9feqp._FF9FEqp.equip] = (Byte)itemId2;
			ff9feqp.FF9FEqp_UpdatePlayer(player);
		}
	}

	private static FF9FEQP _FF9FEqp = new FF9FEQP();
}
