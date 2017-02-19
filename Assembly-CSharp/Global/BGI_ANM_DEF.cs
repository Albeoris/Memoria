using System;
using System.Collections.Generic;
using System.IO;

public class BGI_ANM_DEF
{
	public void ReadData(BinaryReader reader)
	{
		this.anmFlags = reader.ReadUInt16();
		this.frameCount = reader.ReadUInt16();
		this.frameRate = reader.ReadInt16();
		this.counter = reader.ReadUInt16();
		this.curFrame = reader.ReadInt32();
		this.frameOffset = reader.ReadUInt32();
	}

	public UInt16 anmFlags;

	public UInt16 frameCount;

	public Int16 frameRate;

	public UInt16 counter;

	public Int32 curFrame;

	public UInt32 frameOffset;

	public List<BGI_FRAME_DEF> frameList;
}
