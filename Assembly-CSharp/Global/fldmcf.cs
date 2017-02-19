using System;

public class fldmcf
{
	public static DMSMapLight[] ff9fieldMCFGetLightDefault(DMSMapConf ObjPtr, ref DMSMapLight? DefaultPtr, ref DMSMapLight? LadderPtr)
	{
		if (ObjPtr.lightUse == 0)
		{
			return null;
		}
		DMSMapLight[] dmsmapLight = ObjPtr.DMSMapLight;
		Int32 num = (Int32)(ObjPtr.lightUse - 1);
		do
		{
			DMSMapLight value = dmsmapLight[num];
			if ((Int32)value.type == 1)
			{
				DefaultPtr = new DMSMapLight?(value);
			}
			else if ((Int32)value.type == 2)
			{
				LadderPtr = new DMSMapLight?(value);
			}
			num--;
		}
		while (num >= 0);
		return dmsmapLight;
	}

	public static DMSMapChar? ff9fieldMCFGetCharByID(DMSMapConf ObjPtr, Int32 model)
	{
		model &= 65535;
		if (ObjPtr.charUse == 0)
		{
			return null;
		}
		DMSMapChar[] dmsmapChar = ObjPtr.DMSMapChar;
		Int32 num = (Int32)(ObjPtr.charUse - 1);
		DMSMapChar value;
		for (;;)
		{
			value = dmsmapChar[num];
			if (value.geoNo == (UInt16)model)
			{
				break;
			}
			num--;
			if (num < 0)
			{
				goto Block_3;
			}
		}
		return new DMSMapChar?(value);
		Block_3:
		return null;
	}

	public static void ff9fieldMCFService()
	{
		DMSMapConf? mcfPtr = FF9StateSystem.Field.FF9Field.loc.map.mcfPtr;
		if (mcfPtr == null)
		{
			return;
		}
		DMSMapChar[] dmsmapChar = mcfPtr.Value.DMSMapChar;
		if (dmsmapChar != null)
		{
			DMSMapLight? dmsmapLight = null;
			DMSMapLight? dmsmapLight2 = null;
			DMSMapChar? dmsmapChar2 = fldmcf.ff9fieldMCFGetCharByID(mcfPtr.Value, -1);
			fldmcf.ff9fieldMCFGetLightDefault(mcfPtr.Value, ref dmsmapLight, ref dmsmapLight2);
			foreach (Int32 num in FF9StateSystem.Common.FF9.charArray.Keys)
			{
				FF9Char ff9Char = FF9StateSystem.Common.FF9.charArray[num];
				Actor activeActorByUID = PersistenSingleton<EventEngine>.Instance.getActiveActorByUID(num);
				FF9StateFieldMap map = FF9StateSystem.Field.FF9Field.loc.map;
				if (ff9Char != null && activeActorByUID != null)
				{
					FF9FieldCharState ff9FieldCharState = FF9StateSystem.Field.FF9Field.loc.map.charStateArray[num];
					Int16 num2 = 0;
					DMSMapChar? dmsmapChar3 = fldmcf.ff9fieldMCFGetCharByID(mcfPtr.Value, (Int32)activeActorByUID.model);
					DMSMapLight? dmsmapLight3 = fldmcf.ff9fieldMCFGetLightByCharFloor(mcfPtr.Value, num, ref num2);
					Int32 num3 = (Int32)(ff9Char.attr & 4u);
					Boolean flag = ((Int32)ff9FieldCharState.floor & 255) != (Int32)(num2 & 255) || ((Int64)num3 ^ (Int64)((UInt64)(ff9FieldCharState.attr & 4u))) != 0L || FF9Char.ff9charptr_attr_test(ff9Char, 256) == 0;
					if (flag)
					{
						if (FF9Char.ff9charptr_attr_test(ff9Char, 1024) == 0)
						{
							SByte[] array = new SByte[4];
							SByte[] clr;
							if (dmsmapChar3 != null)
							{
								clr = dmsmapChar3.Value.clr;
							}
							else
							{
								clr = dmsmapChar2.Value.clr;
							}
							if (num3 != 0 && dmsmapLight2 != null)
							{
								array = dmsmapLight2.Value.clr;
							}
							else if (dmsmapLight3 != null)
							{
								array = dmsmapLight3.Value.clr;
							}
							else if (dmsmapLight != null)
							{
								array = dmsmapLight.Value.clr;
							}
							FF9FieldCharColor ff9FieldCharColor = default(FF9FieldCharColor);
							ff9FieldCharColor.r = (Byte)((Int32)clr[0] + (Int32)array[0] << 3);
							ff9FieldCharColor.g = (Byte)((Int32)clr[1] + (Int32)array[1] << 3);
							ff9FieldCharColor.b = (Byte)((Int32)clr[2] + (Int32)array[2] << 3);
							ff9FieldCharColor.active = true;
							ff9FieldCharState.clr[0] = (ff9FieldCharState.clr[1] = ff9FieldCharColor);
						}
						Int32 num4;
						Int32 num5;
						if (dmsmapChar3 != null)
						{
							num4 = (Int32)dmsmapChar3.Value.shadowI;
							num5 = (Int32)dmsmapChar3.Value.shadowR;
						}
						else
						{
							num4 = (Int32)dmsmapChar2.Value.shadowI;
							num5 = (Int32)dmsmapChar2.Value.shadowR;
						}
						if (dmsmapLight3 != null)
						{
							num4 += (Int32)dmsmapLight3.Value.shadowI;
							num5 += (Int32)dmsmapLight3.Value.shadowR;
						}
						else if (dmsmapLight != null)
						{
							num4 += (Int32)dmsmapLight.Value.shadowI;
							num5 += (Int32)dmsmapLight.Value.shadowR;
						}
						num4 <<= 3;
						ff9shadow.FF9ShadowSetAmpField(num, num4);
						ff9shadow.FF9ShadowSetScaleField(num, num5, num5);
						ff9FieldCharState.floor = (SByte)num2;
						ff9FieldCharState.attr = (UInt32)(((UInt64)ff9FieldCharState.attr & 18446744073709551611UL) | (UInt64)num3);
						FF9Char.ff9charptr_attr_set(ff9Char, 256);
					}
				}
			}
		}
	}

	public static DMSMapChar[] ff9fieldMCFGetCharArray(DMSMapConf? ObjPtr)
	{
		if (ObjPtr != null && ObjPtr.Value.charUse > 0)
		{
			return ObjPtr.Value.DMSMapChar;
		}
		return null;
	}

	public static DMSMapLight? ff9fieldMCFGetLightByCharFloor(DMSMapConf ObjPtr, Int32 uid, ref Int16 FloorNoPtr)
	{
		Int16 num = 0;
		BGI.BGI_charGetInfo(uid, ref num, ref FloorNoPtr);
		if (ObjPtr.lightUse == 0)
		{
			return null;
		}
		if ((FloorNoPtr & 255) == 255)
		{
			return null;
		}
		DMSMapLight[] dmsmapLight = ObjPtr.DMSMapLight;
		Int32 num2 = (Int32)(ObjPtr.lightUse - 1);
		DMSMapLight value;
		for (;;)
		{
			value = dmsmapLight[num2];
			if ((Int32)value.type == 0)
			{
				for (Int32 i = 3; i >= 0; i--)
				{
					SByte b = value.floor[i];
					Int32 num3 = (Int32)b & 255;
					if (num3 == (Int32)FloorNoPtr)
					{
						goto Block_4;
					}
				}
			}
			num2--;
			if (num2 < 0)
			{
				goto Block_5;
			}
		}
		Block_4:
		return new DMSMapLight?(value);
		Block_5:
		return null;
	}

	public static Int32 FF9FieldMCFSetCharColor(Int32 uid, Int32 ClrR, Int32 ClrG, Int32 ClrB)
	{
		FF9Char charPtr = FF9StateSystem.Common.FF9.charArray[uid];
		return fldmcf.ff9fieldMCFSetCharColor(charPtr, ClrR, ClrG, ClrB);
	}

	private static Int32 ff9fieldMCFSetCharColor(FF9Char CharPtr, Int32 ClrR, Int32 ClrG, Int32 ClrB)
	{
		if (FF9Char.ff9charptr_attr_test(CharPtr, 2048) == 0)
		{
			FF9FieldCharState ff9FieldCharState = FF9StateSystem.Field.FF9Field.loc.map.charStateArray[(Int32)CharPtr.evt.uid];
			FF9FieldCharColor ff9FieldCharColor = default(FF9FieldCharColor);
			ff9FieldCharColor.r = (Byte)ClrR;
			ff9FieldCharColor.g = (Byte)ClrG;
			ff9FieldCharColor.b = (Byte)ClrB;
			ff9FieldCharColor.active = true;
			ff9FieldCharState.clr[0] = (ff9FieldCharState.clr[1] = ff9FieldCharColor);
			FF9Char.ff9charptr_attr_set(CharPtr, 1024);
			FF9Char.ff9charptr_attr_set(CharPtr, 2048);
			return 1;
		}
		FF9Char.ff9charptr_attr_clear(CharPtr, 2048);
		return 0;
	}
}
