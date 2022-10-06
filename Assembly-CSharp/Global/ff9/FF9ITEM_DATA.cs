using System;

public class FF9ITEM_DATA
{
	public FF9ITEM_DATA(UInt16 name, UInt16 help, UInt16 price, UInt64 equip, Byte shape, Byte color, Byte eq_lv, Byte bonus, Byte[] ability, Byte type, Byte sort, Byte pad)
	{
		this.name = name;
		this.help = help;
		this.price = price;
		this.equip = equip;
		this.shape = shape;
		this.color = color;
		this.eq_lv = eq_lv;
		this.bonus = bonus;
		this.ability = ability;
		this.type = type;
		this.sort = sort;
		this.pad = pad;
	}

	public UInt16 name;

	public UInt16 help;

	public UInt16 price;

	public UInt64 equip;

	public Byte shape;

	public Byte color;

	public Byte eq_lv;

	public Byte bonus;

	public Byte[] ability;

	public Byte type;

	public Byte sort;

	public Byte pad;
}
