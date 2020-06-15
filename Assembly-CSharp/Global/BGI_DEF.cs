using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BGI_DEF
{
	public BGI_DEF()
	{
		this.name = String.Empty;
		this.orgPos = new BGI_VEC_DEF();
		this.curPos = new BGI_VEC_DEF();
		this.minPos = new BGI_VEC_DEF();
		this.maxPos = new BGI_VEC_DEF();
		this.charPos = new BGI_VEC_DEF();
		this.triList = new List<BGI_TRI_DEF>();
		this.edgeList = new List<BGI_EDGE_DEF>();
		this.anmList = new List<BGI_ANM_DEF>();
		this.floorList = new List<BGI_FLOOR_DEF>();
		this.normalList = new List<BGI_FVEC_DEF>();
		this.vertexList = new List<BGI_VEC_DEF>();
	}

	public void ReadData(BinaryReader reader)
	{
		reader.BaseStream.Seek(4L, SeekOrigin.Begin);
		this.dataSize = reader.ReadUInt16();
		this.orgPos.ReadData(reader);
		this.curPos.ReadData(reader);
		this.minPos.ReadData(reader);
		this.maxPos.ReadData(reader);
		this.charPos.ReadData(reader);
		this.activeFloor = reader.ReadInt16();
		this.activeTri = reader.ReadInt16();
		this.triCount = reader.ReadUInt16();
		this.triOffset = reader.ReadUInt16();
		this.edgeCount = reader.ReadUInt16();
		this.edgeOffset = reader.ReadUInt16();
		this.anmCount = reader.ReadUInt16();
		this.anmOffset = reader.ReadUInt16();
		this.floorCount = reader.ReadUInt16();
		this.floorOffset = reader.ReadUInt16();
		this.normalCount = reader.ReadUInt16();
		this.normalOffset = reader.ReadUInt16();
		this.vertexCount = reader.ReadUInt16();
		this.vertexOffset = reader.ReadUInt16();
		reader.BaseStream.Seek((Int64)(4 + this.triOffset), SeekOrigin.Begin);
		for (UInt16 num = 0; num < this.triCount; num = (UInt16)(num + 1))
		{
			BGI_TRI_DEF bgi_TRI_DEF = new BGI_TRI_DEF();
			bgi_TRI_DEF.ReadData(reader);
			bgi_TRI_DEF.triIdx = (Int32)num;
			this.triList.Add(bgi_TRI_DEF);
		}
		reader.BaseStream.Seek((Int64)(4 + this.edgeOffset), SeekOrigin.Begin);
		for (UInt16 num2 = 0; num2 < this.edgeCount; num2 = (UInt16)(num2 + 1))
		{
			BGI_EDGE_DEF bgi_EDGE_DEF = new BGI_EDGE_DEF();
			bgi_EDGE_DEF.ReadData(reader);
			this.edgeList.Add(bgi_EDGE_DEF);
		}
		reader.BaseStream.Seek((Int64)(4 + this.anmOffset), SeekOrigin.Begin);
		for (UInt16 num3 = 0; num3 < this.anmCount; num3 = (UInt16)(num3 + 1))
		{
			BGI_ANM_DEF bgi_ANM_DEF = new BGI_ANM_DEF();
			bgi_ANM_DEF.ReadData(reader);
			this.anmList.Add(bgi_ANM_DEF);
		}
		for (Int32 i = 0; i < (Int32)this.anmCount; i++)
		{
			BGI_ANM_DEF bgi_ANM_DEF2 = this.anmList[i];
			bgi_ANM_DEF2.frameList = new List<BGI_FRAME_DEF>();
			reader.BaseStream.Seek((Int64)((UInt64)(4u + bgi_ANM_DEF2.frameOffset)), SeekOrigin.Begin);
			for (Int32 j = 0; j < (Int32)bgi_ANM_DEF2.frameCount; j++)
			{
				BGI_FRAME_DEF bgi_FRAME_DEF = new BGI_FRAME_DEF();
				bgi_FRAME_DEF.ReadData(reader);
				bgi_ANM_DEF2.frameList.Add(bgi_FRAME_DEF);
			}
		}
		for (Int32 k = 0; k < (Int32)this.anmCount; k++)
		{
			BGI_ANM_DEF bgi_ANM_DEF3 = this.anmList[k];
			for (Int32 l = 0; l < (Int32)bgi_ANM_DEF3.frameCount; l++)
			{
				BGI_FRAME_DEF bgi_FRAME_DEF2 = bgi_ANM_DEF3.frameList[l];
				reader.BaseStream.Seek((Int64)(4 + bgi_FRAME_DEF2.triNdxOffset), SeekOrigin.Begin);
				bgi_FRAME_DEF2.triIdxList = new List<Int32>();
				for (Int32 m = 0; m < (Int32)bgi_FRAME_DEF2.triCount; m++)
				{
					Int32 item = reader.ReadInt32();
					bgi_FRAME_DEF2.triIdxList.Add(item);
				}
			}
		}
		reader.BaseStream.Seek((Int64)(4 + this.floorOffset), SeekOrigin.Begin);
		for (UInt16 num4 = 0; num4 < this.floorCount; num4 = (UInt16)(num4 + 1))
		{
			BGI_FLOOR_DEF bgi_FLOOR_DEF = new BGI_FLOOR_DEF();
			bgi_FLOOR_DEF.ReadData(reader);
			this.floorList.Add(bgi_FLOOR_DEF);
		}
		reader.BaseStream.Seek((Int64)(4 + this.normalOffset), SeekOrigin.Begin);
		for (UInt16 num5 = 0; num5 < this.normalCount; num5 = (UInt16)(num5 + 1))
		{
			BGI_FVEC_DEF bgi_FVEC_DEF = new BGI_FVEC_DEF();
			bgi_FVEC_DEF.ReadData(reader);
			this.normalList.Add(bgi_FVEC_DEF);
		}
		reader.BaseStream.Seek((Int64)(4 + this.vertexOffset), SeekOrigin.Begin);
		for (UInt16 num6 = 0; num6 < this.vertexCount; num6 = (UInt16)(num6 + 1))
		{
			BGI_VEC_DEF bgi_VEC_DEF = new BGI_VEC_DEF();
			bgi_VEC_DEF.ReadData(reader);
			this.vertexList.Add(bgi_VEC_DEF);
		}
	}

	public void LoadBGI(FieldMap fieldMap, String path, String name)
	{
		this.name = name;
		String[] bgiInfo;
		Byte[] binAsset = AssetManager.LoadBytes(path + name + ".bgi", out bgiInfo, false);
		if (FF9StateSystem.Common.FF9.fldMapNo == 70)
		{
			return;
		}
		using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(binAsset)))
		{
			this.ReadData(binaryReader);
		}
	}

	public const Int32 MAGIC_SIZE = 4;

	public UInt16 dataSize;

	public BGI_VEC_DEF orgPos;

	public BGI_VEC_DEF curPos;

	public BGI_VEC_DEF minPos;

	public BGI_VEC_DEF maxPos;

	public BGI_VEC_DEF charPos;

	public Int16 activeFloor;

	public Int16 activeTri;

	public UInt16 triCount;

	public UInt16 triOffset;

	public UInt16 edgeCount;

	public UInt16 edgeOffset;

	public UInt16 anmCount;

	public UInt16 anmOffset;

	public UInt16 floorCount;

	public UInt16 floorOffset;

	public UInt16 normalCount;

	public UInt16 normalOffset;

	public UInt16 vertexCount;

	public UInt16 vertexOffset;

	public String name;

	public Byte attributeMask;

	public List<BGI_TRI_DEF> triList;

	public List<BGI_EDGE_DEF> edgeList;

	public List<BGI_ANM_DEF> anmList;

	public List<BGI_FLOOR_DEF> floorList;

	public List<BGI_FVEC_DEF> normalList;

	public List<BGI_VEC_DEF> vertexList;
}
