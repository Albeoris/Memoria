using System;
using System.IO;
using UnityEngine;

public class MBG_CAM_DEF
{
	public MBG_CAM_DEF()
	{
		this.r = new Int16[3, 3];
		this.t = new Int32[3];
		this.centerOffset = new Int16[2];
	}

	public void ReadBinary(BinaryReader reader)
	{
		this.proj = reader.ReadUInt16();
		for (Int32 i = 0; i < 3; i++)
		{
			for (Int32 j = 0; j < 3; j++)
			{
				this.r[i, j] = reader.ReadInt16();
			}
		}
		for (Int32 k = 0; k < 3; k++)
		{
			this.t[k] = reader.ReadInt32();
		}
		for (Int32 l = 0; l < 2; l++)
		{
			this.centerOffset[l] = reader.ReadInt16();
		}
	}

	public Matrix4x4 GetMatrixRT()
	{
		Matrix4x4 result = default(Matrix4x4);
		result.SetRow(0, new Vector4((Single)this.r[0, 0] * 0.000244140625f, (Single)this.r[0, 1] * 0.000244140625f, (Single)this.r[0, 2] * 0.000244140625f, (Single)this.t[0] * 1f));
		result.SetRow(1, new Vector4((Single)this.r[1, 0] * 0.000244140625f, (Single)this.r[1, 1] * 0.000244140625f, (Single)this.r[1, 2] * 0.000244140625f, (Single)this.t[1] * 1f));
		result.SetRow(2, new Vector4((Single)this.r[2, 0] * 0.000244140625f, (Single)this.r[2, 1] * 0.000244140625f, (Single)this.r[2, 2] * 0.000244140625f, (Single)this.t[2] * 1f));
		result.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		return result;
	}

	public Vector2 GetCenterOffset()
	{
		Vector2 result = new Vector2((Single)this.centerOffset[0], (Single)this.centerOffset[1]);
		return result;
	}

	public Single GetViewDistance()
	{
		return (Single)this.proj;
	}

	public Vector3 GetCamPos()
	{
		return new Vector3((Single)this.t[0] * 1f, (Single)this.t[1] * 1f, (Single)this.t[2] * 1f);
	}

	public UInt16 proj;

	private Int16[,] r;

	private Int32[] t;

	public Int16[] centerOffset;
}
