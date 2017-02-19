using System;
using System.IO;

public class BGI_FLOOR_DEF
{
	public BGI_FLOOR_DEF()
	{
		this.orgPos = new BGI_VEC_DEF();
		this.curPos = new BGI_VEC_DEF();
		this.minPos = new BGI_VEC_DEF();
		this.maxPos = new BGI_VEC_DEF();
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
		reader.BaseStream.Seek((Int64)(4 + this.triNdxOffset), SeekOrigin.Begin);
		this.triNdxList = new Int32[(Int32)this.triCount];
		for (UInt16 num = 0; num < this.triCount; num = (UInt16)(num + 1))
		{
			this.triNdxList[(Int32)num] = reader.ReadInt32();
		}
		reader.BaseStream.Seek(position, SeekOrigin.Begin);
	}

	public UInt16 floorFlags;

	public UInt16 floorNdx;

	public BGI_VEC_DEF orgPos;

	public BGI_VEC_DEF curPos;

	public BGI_VEC_DEF minPos;

	public BGI_VEC_DEF maxPos;

	public UInt16 triCount;

	public UInt16 triNdxOffset;

	public Int32[] triNdxList;
}
