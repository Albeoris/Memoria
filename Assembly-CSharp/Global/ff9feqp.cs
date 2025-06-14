using FF9;
using Memoria;
using Memoria.Data;
using System;
using System.Collections.Generic;

public static class ff9feqp
{
    public static UInt64 GetCharacterEquipMaskFromId(CharacterId id)
    {
        return (Int32)id <= 11 ? 1ul << (11 - (Int32)id) : 1ul << (Int32)id;
    }

    public static UInt64 GetCharacterEquipMask(PLAYER player)
    {
        return GetCharacterEquipMaskFromId(player.Index);
    }

    public static void FF9FEqp_UpdatePlayer(PLAYER play)
    {
        ff9feqp.FF9FEqp_UpdateSA(play);
        ff9play.FF9Play_Update(play);
        ff9play.FF9Play_UpdateSerialNumber(play);
    }

    public static void FF9FEqp_UpdateSA(PLAYER player)
    {
        List<SupportAbility> equipSAList = new List<SupportAbility>();
        if (!ff9abil.FF9Abil_HasSA(player))
            return;

        for (Int32 i = 0; i < 5; i++)
            if (player.equip[i] != RegularItem.NoItem)
                foreach (Int32 abilId in ff9item._FF9Item_Data[player.equip[i]].ability)
                    if (abilId != 0 && ff9abil.IsAbilitySupport(abilId))
                        equipSAList.Add(ff9abil.GetSupportAbilityFromAbilityId(abilId));

        CharacterAbility[] playerAbilList = ff9abil._FF9Abil_PaData[player.PresetId];
        if (Configuration.Battle.LockEquippedAbilities == 1 || Configuration.Battle.LockEquippedAbilities == 3)
        {
            foreach (CharacterAbility playerAbil in playerAbilList)
                if (playerAbil.IsPassive)
                    ff9abil.FF9Abil_SetEnableSA(player, playerAbil.PassiveId, equipSAList.Contains(playerAbil.PassiveId));
        }
        else
        {
            for (Int32 k = 0; k < playerAbilList.Length; k++)
            {
                if (!playerAbilList[k].IsPassive)
                    continue;
                SupportAbility saIndex = playerAbilList[k].PassiveId;
                if (ff9abil.FF9Abil_IsEnableSA(player, saIndex) && !equipSAList.Contains(saIndex) && player.pa[k] < playerAbilList[k].Ap)
                {
                    ff9abil.FF9Abil_SetEnableSA(player, saIndex, false);
                    Int32 capa_val = ff9abil.GetSAGemCostFromPlayer(player, saIndex);
                    if (player.max.capa - player.cur.capa >= capa_val)
                        player.cur.capa = (UInt32)(player.cur.capa + capa_val);
                    else
                        player.cur.capa = player.max.capa;
                }
            }
        }
    }

    public static void FF9FEqp_Equip(Byte charPosID, ref Int32 currentItemIndex)
    {
        ItemType[] partMask = new ItemType[]
        {
            ItemType.Weapon,
            ItemType.Helmet,
            ItemType.Armlet,
            ItemType.Armor,
            ItemType.Accessory
        };
        ff9feqp._FF9FEqp.player = charPosID;
        ff9feqp._FF9FEqp.equip = 0;
        ff9feqp._FF9FEqp.item[0] = new FF9ITEM(RegularItem.NoItem, 0);
        FF9StateGlobal ff = FF9StateSystem.Common.FF9;
        Int32 itemSlot = 0;
        PLAYER player = ff.party.member[ff9feqp._FF9FEqp.player];
        UInt64 characterMask = ff9feqp.GetCharacterEquipMask(player);
        ItemType equipSlotMask = partMask[ff9feqp._FF9FEqp.equip];
        foreach (FF9ITEM item in ff.item)
        {
            if (item.count > 0)
            {
                FF9ITEM_DATA itemData = ff9item._FF9Item_Data[item.id];
                if ((itemData.equip & characterMask) != 0 && (itemData.type & equipSlotMask) != 0)
                    ff9feqp._FF9FEqp.item[itemSlot++] = item;
            }
        }
        if (currentItemIndex >= itemSlot)
            currentItemIndex = 0;
        else if (currentItemIndex < 0)
            currentItemIndex = itemSlot - 1;
        RegularItem oldEquipId = player.equip[ff9feqp._FF9FEqp.equip];
        RegularItem newEquipId = ff9feqp._FF9FEqp.item[currentItemIndex].id;
        if (oldEquipId != RegularItem.NoItem)
            ff9item.FF9Item_Add(oldEquipId, 1);
        if (ff9item.FF9Item_Remove(newEquipId, 1) != 0)
        {
            player.equip[ff9feqp._FF9FEqp.equip] = newEquipId;
            ff9feqp.FF9FEqp_UpdatePlayer(player);
        }
    }

    private static FF9FEQP _FF9FEqp = new FF9FEQP();
}
