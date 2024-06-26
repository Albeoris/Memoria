using System;
using UnityEngine;

public class SFXRenderTextureBegin : SFXMeshBase
{
	public static void PrepareCamera()
	{
		GL.Viewport(new Rect(0f, 0f, (Single)PSXTextureMgr.GEN_TEXTURE_W, (Single)PSXTextureMgr.GEN_TEXTURE_H));
		GL.LoadIdentity();
		Matrix4x4 mat = Matrix4x4.Ortho(0f, (Single)PSXTextureMgr.GEN_TEXTURE_W, 0f, (Single)PSXTextureMgr.GEN_TEXTURE_H, SFX.fxNearZ, SFX.fxFarZ);
		PsxCamera.SetOrthoZ(ref mat, SFX.fxNearZ, SFX.fxFarZ);
		GL.LoadProjectionMatrix(mat);
	}

	public override void Render(Int32 index)
	{
		SFXRenderTextureBegin.PrepareCamera();
		SFXMeshBase.curRenderTexture = RenderTexture.active;
		RenderTexture.active = PSXTextureMgr.genTexture;
		if (!PSXTextureMgr.isCreateGenTexture)
			GL.Clear(false, true, new Color(0f, 0f, 0f));
		SFXMeshBase.isRenderTexture = true;
	}
}
