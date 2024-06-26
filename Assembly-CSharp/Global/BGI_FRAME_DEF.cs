using System;
using System.Collections.Generic;
using System.IO;

public class BGI_FRAME_DEF
{
	public void ReadData(BinaryReader reader)
	{
		this.frameFlags = reader.ReadUInt16();
		this.value = reader.ReadInt16();
		this.triCount = reader.ReadUInt16();
		this.triNdxOffset = reader.ReadUInt16();
	}

	public void WriteData(BinaryWriter writer)
	{
		writer.Write(this.frameFlags);
		writer.Write(this.value);
		writer.Write(this.triCount);
		writer.Write(this.triNdxOffset);
	}

	public UInt16 frameFlags;
	public Int16 value;
	public UInt16 triCount;
	public UInt16 triNdxOffset;
	public List<Int32> triIdxList;
}
