using System;
using UnityEngine;

public class SFXMeshBase
{
	public virtual void Render(Int32 index)
	{
	}

	protected static Boolean isRenderTexture;

	public static Int32 drOffsetX;

	public static Int32 drOffsetY;

	public static Int32 ssOffsetX;

	public static Int32 ssOffsetY;

	protected static RenderTexture curRenderTexture;
}
