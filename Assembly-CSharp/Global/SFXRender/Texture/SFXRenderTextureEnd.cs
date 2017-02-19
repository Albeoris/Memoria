using System;
using System.IO;
using UnityEngine;

public class SFXRenderTextureEnd : SFXMeshBase
{
	public override void Render(Int32 index)
	{
		SFXMeshBase.isRenderTexture = false;
		PSXTextureMgr.isCreateGenTexture = true;
		SFX.ResetViewPort();
		if (SFX.isDebugPng)
		{
			Texture2D texture2D = new Texture2D(PSXTextureMgr.GEN_TEXTURE_W, PSXTextureMgr.GEN_TEXTURE_H, TextureFormat.ARGB32, false);
			texture2D.ReadPixels(new Rect(0f, 0f, (Single)PSXTextureMgr.GEN_TEXTURE_W, (Single)PSXTextureMgr.GEN_TEXTURE_H), 0, 0);
			texture2D.Apply();
			Byte[] bytes = texture2D.EncodeToPNG();
			File.WriteAllBytes("Temp/gen.png", bytes);
		}
		Graphics.SetRenderTarget((RenderTexture)null);
	}
}
