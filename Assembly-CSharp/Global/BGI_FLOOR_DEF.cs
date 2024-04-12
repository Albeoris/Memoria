using System;
using System.Collections.Generic;
using System.IO;

public class BGI_FLOOR_DEF
{
	public BGI_FLOOR_DEF()
	{
		this.orgPos = new BGI_VEC_DEF();
		this.curPos = new BGI_VEC_DEF();
		this.minPos = new BGI_VEC_DEF();
		this.maxPos = new BGI_VEC_DEF();
		this.triNdxList = new List<Int32>();
	}

	public void ReadData(BinaryReader reader)
	{
		this.floorFlags = reader.ReadUInt16();
		this.floorNdx = reader.ReadUInt16();
		this.orgPos.ReadData(reader);
		this.curPos.ReadData(reader);
		this.minPos.ReadData(reader);
		this.maxPos.ReadData(reader);
		this.triCount = reader.ReadUInt16();
		this.triNdxOffset = reader.ReadUInt16();
		Int64 position = reader.BaseStream.Position;
		reader.BaseStream.Seek(4 + this.triNdxOffset, SeekOrigin.Begin);
		for (UInt16 i = 0; i < this.triCount; i++)
			this.triNdxList.Add(reader.ReadInt32());
		reader.BaseStream.Seek(position, SeekOrigin.Begin);
	}

	public void WriteData(BinaryWriter writer)
	{
		writer.Write(this.floorFlags);
		writer.Write(this.floorNdx);
		this.orgPos.WriteData(writer);
		this.curPos.WriteData(writer);
		this.minPos.WriteData(writer);
		this.maxPos.WriteData(writer);
		writer.Write(this.triCount);
		writer.Write(this.triNdxOffset);
		Int64 position = writer.BaseStream.Position;
		writer.BaseStream.Seek(4 + this.triNdxOffset, SeekOrigin.Begin);
		for (UInt16 i = 0; i < this.triCount; i++)
			writer.Write(this.triNdxList[i]);
		writer.BaseStream.Seek(position, SeekOrigin.Begin);
	}

	public UInt16 floorFlags;
	public UInt16 floorNdx;

	public BGI_VEC_DEF orgPos;
	public BGI_VEC_DEF curPos;
	public BGI_VEC_DEF minPos;
	public BGI_VEC_DEF maxPos;

	public UInt16 triCount;
	public UInt16 triNdxOffset;
	public List<Int32> triNdxList;
}
