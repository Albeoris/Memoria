using System;
using UnityEngine;

public class SFXScreenShot : SFXMeshBase
{
	public SFXScreenShot(Int32 _rx, Int32 _ry, Int32 _x, Int32 _y)
	{
		SFXMeshBase.ssOffsetX = _rx;
		SFXMeshBase.ssOffsetY = _ry;
		this.x = _x;
		this.y = _y;
	}

	public override void Render(Int32 index)
	{
		GL.PushMatrix();
		GL.PopMatrix();
	}

	private Int32 x;

	private Int32 y;
}
