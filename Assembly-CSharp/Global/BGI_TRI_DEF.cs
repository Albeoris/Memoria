using System;
using System.IO;

public class BGI_TRI_DEF
{
	public BGI_TRI_DEF()
	{
		this.vertexNdx = new Int16[3];
		this.edgeNdx = new Int16[3];
		this.neighborNdx = new Int16[3];
		this.center = new BGI_VEC_DEF();
		this.triIdx = -1;
	}

	public void ReadData(BinaryReader reader)
	{
		this.triFlags = reader.ReadUInt16();
		this.triData = reader.ReadUInt16();
		this.floorNdx = reader.ReadInt16();
		this.normalNdx = reader.ReadInt16();
		this.thetaX = reader.ReadInt16();
		this.thetaZ = reader.ReadInt16();
		this.vertexNdx[0] = reader.ReadInt16();
		this.vertexNdx[1] = reader.ReadInt16();
		this.vertexNdx[2] = reader.ReadInt16();
		this.edgeNdx[0] = reader.ReadInt16();
		this.edgeNdx[1] = reader.ReadInt16();
		this.edgeNdx[2] = reader.ReadInt16();
		this.neighborNdx[0] = reader.ReadInt16();
		this.neighborNdx[1] = reader.ReadInt16();
		this.neighborNdx[2] = reader.ReadInt16();
		this.center.ReadData(reader);
		this.d = reader.ReadInt32();
	}

	public UInt16 triFlags;

	public UInt16 triData;

	public Int16 floorNdx;

	public Int16 normalNdx;

	public Int16 thetaX;

	public Int16 thetaZ;

	public Int16[] vertexNdx;

	public Int16[] edgeNdx;

	public Int16[] neighborNdx;

	public BGI_VEC_DEF center;

	public Int32 d;

	public Int32 triIdx;
}
