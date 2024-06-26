using System;
using System.Collections.Generic;

public class FF9StateFieldMap
{
	public FF9StateFieldMap()
	{
		for (Int32 i = 0; i < FF9Snd.FLDINT_CHAR_SOUNDCOUNT; i++)
		{
			this.charSoundArray[i] = new FF9FieldCharSound();
		}
	}

	public void ff9ResetStateFieldMap()
	{
		this.attr = 0;
		this.nextMapNo = -1;
		this.nextMode = 1;
		this.charStateArray.Clear();
		this.shadowArray.Clear();
		FF9StateFieldMap map = FF9StateSystem.Field.FF9Field.loc.map;
		for (Int32 i = 0; i < FF9Snd.FLDINT_CHAR_SOUNDCOUNT; i++)
		{
			map.charSoundArray[i] = new FF9FieldCharSound();
		}
		map.charSoundUse = 0;
	}

	private Int32 attr;

	public Int16 nextMapNo;

	public Byte[] evtPtr;

	public DMSMapConf? mcfPtr;

	public Dictionary<Int32, FF9Shadow> shadowArray = new Dictionary<Int32, FF9Shadow>();

	public Int32 charOTOffset;

	public Byte encStatus;

	public Byte nextMode;

	public Int32 charSoundUse;

	public Dictionary<Int32, FF9FieldCharState> charStateArray = new Dictionary<Int32, FF9FieldCharState>();

	public FF9FieldCharSound[] charSoundArray = new FF9FieldCharSound[FF9Snd.FLDINT_CHAR_SOUNDCOUNT];
}
