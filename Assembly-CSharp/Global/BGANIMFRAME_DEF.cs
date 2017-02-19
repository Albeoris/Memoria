using System;
using System.IO;

public class BGANIMFRAME_DEF
{
	public void ReadData(BinaryReader reader)
	{
		this.target = reader.ReadByte();
		this.value = reader.ReadSByte();
	}

	public Byte target;

	public SByte value;
}
