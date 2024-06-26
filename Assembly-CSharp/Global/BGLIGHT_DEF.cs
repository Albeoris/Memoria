using System;
using System.IO;

public class BGLIGHT_DEF
{
	public BGLIGHT_DEF()
	{
		this.pos = new Int16[3];
		this.lightColor = new BGLIGHTCLR_DEF();
	}

	public void ReadData(BinaryReader reader)
	{
		this.pos[0] = reader.ReadInt16();
		this.pos[1] = reader.ReadInt16();
		this.pos[2] = reader.ReadInt16();
		this.pad = reader.ReadUInt16();
		this.lightColor.ReadData(reader);
	}

	public Int16[] pos;

	public UInt16 pad;

	public BGLIGHTCLR_DEF lightColor;
}
