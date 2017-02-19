using System;
using System.IO;
using System.Text;
using UnityEngine;
using Object = System.Object;

public class GEOTEXANIMHEADER
{
	public void ReadData(BinaryReader reader)
	{
		this.flags = reader.ReadByte();
		this.numframes = reader.ReadByte();
		this.rate = reader.ReadInt16();
		this.randmin = reader.ReadUInt16();
		this.randrange = reader.ReadUInt16();
		this.frame = reader.ReadInt32();
		this.count = reader.ReadByte();
		this.texID = reader.ReadByte();
		this.lastframe = reader.ReadInt16();
		this.geotexanimoffset = reader.ReadUInt32();
		Int64 position = reader.BaseStream.Position;
		this.lastframe = 1;
		reader.BaseStream.Seek((Int64)((UInt64)this.geotexanimoffset), SeekOrigin.Begin);
		this.target = new Rect
		{
			x = (Single)(reader.ReadUInt16() * 2),
			y = (Single)reader.ReadUInt16(),
			width = (Single)(reader.ReadUInt16() * 2),
			height = (Single)reader.ReadUInt16()
		};
		this.targetuv = this.target;
		UInt32 num = reader.ReadUInt32();
		this.coords = new Vector2[(Int32)this.numframes];
		this.rectuvs = new Rect[(Int32)this.numframes];
		reader.BaseStream.Seek((Int64)((UInt64)num), SeekOrigin.Begin);
		for (Int32 i = 0; i < (Int32)this.numframes; i++)
		{
			Int32 num2 = (Int32)(reader.ReadInt16() * 2);
			Int32 num3 = (Int32)reader.ReadInt16();
			this.coords[i] = new Vector2((Single)num2, (Single)num3);
			this.rectuvs[i] = new Rect((Single)num2, (Single)num3, this.target.width, this.target.height);
		}
		reader.BaseStream.Seek(position, SeekOrigin.Begin);
	}

	public void DumpData()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("flags = " + this.flags + "\n");
		stringBuilder.Append("numframes = " + this.numframes + "\n");
		stringBuilder.Append("rate = " + this.rate + "\n");
		stringBuilder.Append("randmin = " + this.randmin + "\n");
		stringBuilder.Append("randrange = " + this.randrange + "\n");
		stringBuilder.Append("frame = " + this.frame + "\n");
		stringBuilder.Append("count = " + this.count + "\n");
		stringBuilder.Append("texID = " + this.texID + "\n");
		stringBuilder.Append("lastframe = " + this.lastframe + "\n");
		stringBuilder.Append("target = " + this.target + "\n");
		for (Int32 i = 0; i < (Int32)this.numframes; i++)
		{
			stringBuilder.Append(String.Concat(new Object[]
			{
				"coords[",
				i,
				"] = ",
				this.coords[i],
				"\n"
			}));
			stringBuilder.Append(String.Concat(new Object[]
			{
				"rectuvs[",
				i,
				"] = ",
				this.rectuvs[i],
				"\n"
			}));
		}
		stringBuilder.Append("-----------");
		global::Debug.Log(stringBuilder.ToString());
	}

	public Byte flags;

	public Byte numframes;

	public Int16 rate;

	public UInt16 randmin;

	public UInt16 randrange;

	public Int32 frame;

	public Byte count;

	public Byte texID;

	public Int16 lastframe;

	public UInt32 geotexanimoffset;

	public Rect target;

	public Vector2[] coords;

	public Rect targetuv;

	public Rect[] rectuvs;

	public static Material texAnimMat = new Material(Shader.Find("PSX/BattleMap_TexAnim"));
}
