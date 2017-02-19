using System;
using UnityEngine;

public class FF9Char
{
	public FF9Char()
	{
		this.evt = new PosObj();
	}

	public static Int32 ff9charptr_attr_test(FF9Char ff9char, Int32 attr)
	{
		return (Int32)((UInt64)ff9char.attr & (UInt64)((Int64)attr));
	}

	public static void ff9charptr_attr_set(FF9Char ff9char, Int32 attr)
	{
		ff9char.attr |= (UInt32)attr;
	}

	public static void ff9char_attr_set(Int32 charuid, Int32 attr)
	{
		FF9Char ff9char = FF9StateSystem.Common.FF9.charArray[charuid];
		FF9Char.ff9charptr_attr_set(ff9char, attr);
	}

	public static void ff9charptr_attr_clear(FF9Char ff9char, Int32 attr)
	{
		ff9char.attr = (UInt32)((UInt64)ff9char.attr & (UInt64)((Int64)(~(Int64)attr)));
	}

	public static void ff9char_attr_clear(Int32 charuid, Int32 attr)
	{
		FF9Char ff9char = FF9StateSystem.Common.FF9.charArray[charuid];
		FF9Char.ff9charptr_attr_clear(ff9char, attr);
	}

	public UInt32 attr;

	public GameObject geo;

	public PosObj evt;

	public BTL_DATA btl;
}
