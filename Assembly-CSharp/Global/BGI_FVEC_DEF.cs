using System;
using System.IO;
using UnityEngine;

public class BGI_FVEC_DEF
{
	public BGI_FVEC_DEF()
	{
		this.coord = new Int32[3];
		this.coord[0] = 0;
		this.coord[1] = 0;
		this.coord[2] = 0;
		this.oneOverY = 0;
	}

	public void ReadData(BinaryReader reader)
	{
		this.coord[0] = reader.ReadInt32();
		this.coord[1] = reader.ReadInt32();
		this.coord[2] = reader.ReadInt32();
		this.oneOverY = reader.ReadInt32();
	}

	public void WriteData(BinaryWriter writer)
	{
		writer.Write(this.coord[0]);
		writer.Write(this.coord[1]);
		writer.Write(this.coord[2]);
		writer.Write(this.oneOverY);
	}

	public Vector3 ToVector3()
	{
		return new Vector3
		{
			x = Convert.ToSingle(this.coord[0]) / 65536f,
			y = Convert.ToSingle(this.coord[1]) / 65536f,
			z = Convert.ToSingle(this.coord[2]) / 65536f
		};
	}

	public static BGI_FVEC_DEF FromVector3(Vector3 v)
	{
		BGI_FVEC_DEF normal = new BGI_FVEC_DEF();
		normal.coord[0] = (Int32)Math.Round(v.x * 65536f);
		normal.coord[1] = (Int32)Math.Round(v.y * 65536f);
		normal.coord[2] = (Int32)Math.Round(v.z * 65536f);
		normal.oneOverY = Single.IsInfinity(1f/ v.y) ? 0 : (Int32)Math.Round(1f / v.y * 65536f);
		return normal;
	}

	public Int32[] coord;
	public Int32 oneOverY;
}
