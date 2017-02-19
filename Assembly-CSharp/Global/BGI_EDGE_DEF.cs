using System;
using System.IO;

public class BGI_EDGE_DEF
{
	public void ReadData(BinaryReader reader)
	{
		this.edgeFlags = reader.ReadUInt16();
		this.edgeClone = reader.ReadInt16();
	}

	public UInt16 edgeFlags;

	public Int16 edgeClone;
}
