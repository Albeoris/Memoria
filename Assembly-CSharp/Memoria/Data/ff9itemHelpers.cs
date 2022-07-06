using Assets.SiliconSocial;
using FF9;
using Memoria;
using Memoria.Data;
using System;
using System.Collections.Generic;
using System.Linq;

public static class ff9itemHelpers
{
    public static FF9ITEM FF9Item_GetPtr(Int32 id, FF9ITEM[] ff9Items)
    {
        return ff9Items.FirstOrDefault(x => x?.count != 0 && x?.id == id);
    }

    /// <summary>
    /// Defines, based on equipment type, where item should go (weapon, wrist, head ect.)
    /// </summary>
    /// <param name="item">Item under inspection</param>
    /// <returns><see cref="FF9FEQP_EQUIP"/> value of equipment slot</returns>
    public static Int32 FF9Item_GetEquipPart(FF9ITEM_DATA item)
    {
        if (item.type.HasFlag(ItemType.Weapon))
            return (Int32)FF9FEQP_EQUIP.FF9FEQP_EQUIP_WEAPON;

        if (item.type.HasFlag(ItemType.Armlet))
            return (Int32)FF9FEQP_EQUIP.FF9FEQP_EQUIP_WRIST;

        if (item.type.HasFlag(ItemType.Helmet))
            return (Int32)FF9FEQP_EQUIP.FF9FEQP_EQUIP_HEAD;

        if (item.type.HasFlag(ItemType.Armor))
            return (Int32)FF9FEQP_EQUIP.FF9FEQP_EQUIP_BODY;

        if (item.type.HasFlag(ItemType.Accessory))
            return (Int32)FF9FEQP_EQUIP.FF9FEQP_EQUIP_ACCESSORY;

        return -1;
    }

    /// <summary>
    /// Determines, how many times item is equipped across the entire party.
    /// </summary>
    /// <param name="itemId">Item to search</param>
    /// <param name="party">Collection of party members</param>
    /// <returns>Count of item occurrences across party</returns>
    public static Int32 FF9Item_GetEquipCount(Int32 itemId, PLAYER[] party)
    {
        IEnumerable<PLAYER> charsInParty = party.Where(c => c.info.party != 0);
        Int32 count = charsInParty.Select(c => c.GetEquipmentCount(itemId)).Sum();
        return count;
    }

    public static Int32 FF9Item_Remove(Int32 id, Int32 count, FF9ITEM[] ff9ItemArray)
    {
        FF9ITEM item = ff9ItemArray.FirstOrDefault(x => x.count != 0 && x.id == id);
        if (item == null)
            return 0;

        // Prevents removing more items than available.
        Int32 removeCount = item.count >= count ? count : item.count;
        item.count -= (Byte)removeCount;
        return removeCount;

    }

    public static Int32 FF9Item_GetCount(Int32 id, FF9ITEM[] items)
    {
        FF9ITEM item = FF9Item_GetPtr(id, items);
        return item == null ? 0 : item.count;
    }
}