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
		return (Int32)ff9char.attr & attr;
	}

	public static void ff9charptr_attr_set(FF9Char ff9char, Int32 attr)
	{
		ff9char.attr |= (UInt32)attr;
	}

	public static void ff9char_attr_set(Int32 charuid, Int32 attr)
	{
		FF9Char.ff9charptr_attr_set(FF9StateSystem.Common.FF9.charArray[charuid], attr);
	}

	public static void ff9charptr_attr_clear(FF9Char ff9char, Int32 attr)
	{
		ff9char.attr &= (UInt32)~attr;
	}

	public static void ff9char_attr_clear(Int32 charuid, Int32 attr)
	{
		FF9Char.ff9charptr_attr_clear(FF9StateSystem.Common.FF9.charArray[charuid], attr);
	}

	public UInt32 attr;

	public GameObject geo;

	public PosObj evt;

	public BTL_DATA btl;
}
