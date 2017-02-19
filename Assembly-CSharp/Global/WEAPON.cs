using System;
using System.Collections.Generic;
using System.Linq;

public class WEAPON
{
	public WEAPON(Byte category, Byte add_no, String model_name, BTL_REF Ref, Int16[] offset)
	{
		this.category = category;
		this.add_no = add_no;
		this.model_no = (UInt16)FF9BattleDB.GEO.FirstOrDefault((KeyValuePair<Int32, String> x) => x.Value == model_name).Key;
		this.Ref = Ref;
		this.offset = offset;
	}

	public Byte category;

	public Byte add_no;

	public UInt16 model_no;

	public BTL_REF Ref;

	public Int16[] offset;
}
