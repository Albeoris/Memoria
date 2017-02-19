using System;
using System.Collections.Generic;
using System.IO;

public class BGANIM_DEF
{
	public BGANIM_DEF()
	{
		this.frameList = new List<BGANIMFRAME_DEF>();
	}

	public void ReadData(BinaryReader reader)
	{
		UInt32 input = reader.ReadUInt32();
		Byte b = 0;
		this.flags = (Byte)BitUtil.ReadBits(input, ref b, 8);
		this.frameCount = (Int32)BitUtil.ReadBits(input, ref b, 24);
		input = reader.ReadUInt32();
		b = 0;
		this.camNdx = (Byte)BitUtil.ReadBits(input, ref b, 8);
		this.curFrame = (Int32)BitUtil.ReadBits(input, ref b, 24);
		this.frameRate = reader.ReadInt16();
		this.counter = reader.ReadUInt16();
		this.offset = reader.ReadUInt32();
	}

	public Byte flags;

	public Int32 frameCount;

	public Byte camNdx;

	public Int32 curFrame;

	public Int16 frameRate;

	public UInt16 counter;

	public UInt32 offset;

	public List<BGANIMFRAME_DEF> frameList;
}
