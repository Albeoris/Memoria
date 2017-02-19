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

	public Vector3 ToVector3()
	{
		return new Vector3
		{
			x = Convert.ToSingle(this.coord[0]) * 1f,
			y = Convert.ToSingle(this.coord[1]) * 1f,
			z = Convert.ToSingle(this.coord[2]) * 1f
		};
	}

	public Int16[] coord;
}
