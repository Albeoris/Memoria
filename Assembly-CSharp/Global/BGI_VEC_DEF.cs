using System;
using System.IO;
using UnityEngine;

public class BGI_VEC_DEF
{
	public BGI_VEC_DEF()
	{
		this.coord = new Int16[3];
	}

	public void ReadData(BinaryReader reader)
	{
		this.coord[0] = reader.ReadInt16();
		this.coord[1] = reader.ReadInt16();
		this.coord[2] = reader.ReadInt16();
	}

	public void WriteData(BinaryWriter writer)
	{
		writer.Write(this.coord[0]);
		writer.Write(this.coord[1]);
		writer.Write(this.coord[2]);
	}

	public Vector3 ToVector3()
	{
		return new Vector3
		{
			x = Convert.ToSingle(this.coord[0]),
			y = Convert.ToSingle(this.coord[1]),
			z = Convert.ToSingle(this.coord[2])
		};
	}

	public static BGI_VEC_DEF FromVector3(Vector3 v)
	{
		BGI_VEC_DEF vec = new BGI_VEC_DEF();
		vec.coord[0] = (Int16)Math.Round(v.x);
		vec.coord[1] = (Int16)Math.Round(v.y);
		vec.coord[2] = (Int16)Math.Round(v.z);
		return vec;
	}

	public Int16[] coord;
}
