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

	public Vector3 ToVector3()
	{
		return new Vector3
		{
			x = Convert.ToSingle(this.coord[0]) * 1.52587891E-05f,
			y = Convert.ToSingle(this.coord[1]) * 1.52587891E-05f,
			z = Convert.ToSingle(this.coord[2]) * 1.52587891E-05f
		};
	}

	public Int32[] coord;

	public Int32 oneOverY;
}
