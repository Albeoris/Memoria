using Memoria.Data;
using System;

public class FF9ITEM_DATA
{
    public FF9ITEM_DATA(UInt32 price, Int32 selling_price, UInt64 equip, Byte shape, Byte color, Single eq_lv, Int32 bonus, Int32[] ability, ItemType type, Single sort, String use_condition, Int32 weapon = -1, Int32 armor = -1, Int32 effect = -1)
    {
        this.price = price;
        this.selling_price = selling_price;
        this.equip = equip;
        this.shape = shape;
        this.color = color;
        this.eq_lv = eq_lv;
        this.bonus = bonus;
        this.ability = ability;
        this.type = type;
        this.sort = sort;
        this.use_condition = use_condition;
        this.pad = 0;
        this.weapon_id = weapon;
        this.armor_id = armor;
        this.effect_id = effect;
    }

    public UInt32 price;
    public Int32 selling_price;

    public UInt64 equip;

    public Byte shape;
    public Byte color;

    public Single eq_lv;

    public Int32 bonus;

    public Int32[] ability;

    public ItemType type;

    public Single sort;

    public String use_condition;

    public Byte pad;

    public Int32 weapon_id;
    public Int32 armor_id;
    public Int32 effect_id;
}
