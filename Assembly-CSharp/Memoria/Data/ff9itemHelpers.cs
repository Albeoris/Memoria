using FF9;
using Memoria;
using Memoria.Data;
using System;
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
            return (int)FF9FEQP_EQUIP.FF9FEQP_EQUIP_WEAPON;

        if (item.type.HasFlag(ItemType.Armlet))
            return (int)FF9FEQP_EQUIP.FF9FEQP_EQUIP_WRIST;

        if (item.type.HasFlag(ItemType.Helmet))
            return (int)FF9FEQP_EQUIP.FF9FEQP_EQUIP_HEAD;

        if (item.type.HasFlag(ItemType.Armor))
            return (int)FF9FEQP_EQUIP.FF9FEQP_EQUIP_BODY;

        if (item.type.HasFlag(ItemType.Accessory))
            return (int)FF9FEQP_EQUIP.FF9FEQP_EQUIP_ACCESSORY;

        return -1;
    }
}