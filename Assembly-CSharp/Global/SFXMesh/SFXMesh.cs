using System;
using Memoria.Scripts;
using UnityEngine;

public class SFXMesh : SFXMeshBase
{
	public SFXMesh()
	{
		this.key = 0u;
		this.material = new Material(SFXMesh.shaders[0]);
		this.mesh = new Mesh();
		this.mesh.MarkDynamic();
		this.ibIndex = new Int32[SFXMesh.INDICES_MAX];
		this.vbPos = new Vector3[SFXMesh.VERTICES_MAX];
		this.vbCol = new Color32[SFXMesh.VERTICES_MAX];
		this.vbTex = new Vector2[SFXMesh.VERTICES_MAX];
		this.constTexParam = new Vector4(SFXMesh.HALF_PIXEL, SFXMesh.HALF_PIXEL, 256f, 256f);
	}

	public static void DummyRender()
	{
		Color32 c = new Color32(0, 0, 0, 0);
		Vector3 v = new Vector3(0f, 0f, 0f);
		SFXMesh.dummyMaterial.SetPass(0);
		GL.Begin(1);
		GL.Color(c);
		GL.Vertex(v);
		GL.Color(c);
		GL.Vertex(v);
		GL.End();
	}

	public static void Init()
	{
		SFXMesh.shaders = new Shader[6];
		SFXMesh.shaders[0] = ShadersLoader.Find("SFX_OPA_GT");
		SFXMesh.shaders[1] = ShadersLoader.Find("SFX_ADD_GT");
		SFXMesh.shaders[2] = ShadersLoader.Find("SFX_SUB_GT");
		SFXMesh.shaders[3] = ShadersLoader.Find("SFX_OPA_G");
		SFXMesh.shaders[4] = ShadersLoader.Find("SFX_ADD_G");
	    SFXMesh.shaders[5] = ShadersLoader.Find("SFX_SUB_G");
		SFXMesh.gPos = new Vector3[SFXMesh.POS_MAX];
		for (Int32 i = 0; i < (Int32)SFXMesh.gPos.Length; i++)
		{
			SFXMesh.gPos[i] = default(Vector3);
		}
		SFXMesh.gTex = new Vector2[SFXMesh.TEX_MAX];
		for (Int32 j = 0; j < (Int32)SFXMesh.gTex.Length; j++)
		{
			SFXMesh.gTex[j] = default(Vector2);
		}
		SFXMesh.gCol = new Color32[SFXMesh.COL_MAX];
		for (Int32 k = 0; k < (Int32)SFXMesh.gCol.Length; k++)
		{
			SFXMesh.gCol[k] = default(Color32);
		}
		SFXMesh.colorData = new Color[3];
		SFXMesh.colorData[0] = new Color(1f, 1f, 1f, 1f);
		SFXMesh.colorData[1] = new Color(1.5f, 1.5f, 1.5f, 1f);
		SFXMesh.colorData[2] = new Color(2f, 2f, 2f, 1f);
		SFXMesh.dummyMaterial = new Material(SFXMesh.shaders[4]);
	}

	public static void Release()
	{
		UnityEngine.Object.Destroy(SFXMesh.dummyMaterial);
		for (Int32 i = 0; i < 6; i++)
		{
			SFXMesh.shaders[i] = (Shader)null;
		}
		SFXMesh.shaders = null;
		SFXMeshBase.curRenderTexture = (RenderTexture)null;
	}

	public void ClearObject()
	{
		UnityEngine.Object.Destroy(this.material);
		UnityEngine.Object.Destroy(this.mesh);
		this.ibIndex = null;
		this.vbPos = null;
		this.vbCol = null;
		this.vbTex = null;
	}

	public void Setup(UInt32 meshKey, Byte code)
	{
		this.key = meshKey;
		Boolean flag = SFXKey.IsTexture(meshKey);
		this.shaderIndex = (UInt32)((!flag) ? 3u : 0u);
		if ((code & 2) != 0)
		{
			this.alpha = SFXMesh.abrAlphaData[(Int32)((UIntPtr)SFXKey.tmpABR)];
			this.shaderIndex += SFXKey.GetBlendMode(meshKey);
		}
		else
		{
			this.alpha = 128;
		}
		if (flag)
		{
			if (SFX.pixelOffset == 0)
			{
				this.constTexParam.x = SFXMesh.HALF_PIXEL;
				this.constTexParam.y = SFXMesh.HALF_PIXEL;
			}
			else if (SFX.pixelOffset == 1)
			{
				this.constTexParam.x = SFXMesh.INV_HALF_PIXEL;
				this.constTexParam.y = SFXMesh.INV_HALF_PIXEL;
			}
			else
			{
				this.constTexParam.x = SFXMesh.HALF_PIXEL;
				this.constTexParam.y = SFXMesh.INV_HALF_PIXEL;
			}
			this.constTexParam.z = 256f;
			this.constTexParam.w = 256f;
		}
	}

	private Texture GetTexture()
	{
		UInt32 textureKey = SFXKey.GetTextureKey(this.key);
		if (SFXKey.GetTextureMode(this.key) != 2u)
		{
			if (SFX.currentEffectID == 435)
			{
				Single[] array = new Single[2];
				Texture texture = PSXTextureMgr.GetTexture435(textureKey, array);
				if (texture != (UnityEngine.Object)null)
				{
					this.constTexParam.y = array[0];
					this.constTexParam.w = array[1];
					return texture;
				}
			}
			return PSXTextureMgr.GetTexture(this.key).texture;
		}
		if (SFXKey.IsBlurTexture(textureKey))
		{
			if ((this.key & 536870912u) != 0u)
			{
				this.constTexParam.w = 240f;
			}
			else
			{
				this.constTexParam.y = -240f;
				this.constTexParam.w = -240f;
			}
			this.constTexParam.z = 320f;
			return PSXTextureMgr.blurTexture;
		}
		if ((this.key & 2031616u) == PSXTextureMgr.bgKey)
		{
			return PSXTextureMgr.bgTexture;
		}
		if (PSXTextureMgr.isCreateGenTexture)
		{
			this.constTexParam.x = (Single)((UInt64)SFXKey.GetPositionX(this.key) - (UInt64)((Int64)PSXTextureMgr.GEN_TEXTURE_X)) + 0.5f;
			this.constTexParam.z = (Single)PSXTextureMgr.GEN_TEXTURE_W;
			this.constTexParam.w = (Single)PSXTextureMgr.GEN_TEXTURE_H;
			return PSXTextureMgr.genTexture;
		}
		if (SFX.currentEffectID == 274 && textureKey == 5767167u)
		{
			this.constTexParam.x = 176f;
			this.constTexParam.y = 0f;
			this.constTexParam.z = 480f;
			this.constTexParam.w = 360f;
			return PSXTextureMgr.blurTexture;
		}
		return PSXTextureMgr.GetTexture(this.key).texture;
	}

	public UInt32 GetKey()
	{
		return this.key;
	}

	public void Begin()
	{
		this.key = 0u;
		this.ibOffset = 0;
		this.vbOffset = 0;
	}

	public void End()
	{
		this.mesh.Clear();
		Int32[] array = new Int32[this.ibOffset];
		Buffer.BlockCopy(this.ibIndex, 0, array, 0, this.ibOffset << 2);
		this.mesh.vertices = this.vbPos;
		this.mesh.colors32 = this.vbCol;
		if (SFXKey.IsTexture(this.key))
		{
			this.mesh.uv = this.vbTex;
		}
		else
		{
			this.mesh.uv = null;
		}
		if (SFX.isDebugLine)
		{
			this.mesh.SetIndices(array, MeshTopology.Lines, 0);
		}
		else if (SFXKey.isLinePolygon(this.key))
		{
			this.mesh.SetIndices(array, MeshTopology.Lines, 0);
		}
		else
		{
			this.mesh.triangles = array;
		}
	}

	public override void Render(Int32 index)
	{
		this.material.shader = SFXMesh.shaders[(Int32)((UIntPtr)this.shaderIndex)];
		if (SFXKey.IsTexture(this.key))
		{
			this.material.mainTexture = this.GetTexture();
			if (this.material.mainTexture == (UnityEngine.Object)null)
			{
				return;
			}
			UInt32 fillter = SFXKey.GetFillter(this.key);
			if (fillter != 33554432u)
			{
				if (fillter != 67108864u)
				{
					this.material.mainTexture.filterMode = (FilterMode)((!SFX.isDebugFillter) ? FilterMode.Point : FilterMode.Bilinear);
				}
				else
				{
					this.material.mainTexture.filterMode = FilterMode.Bilinear;
				}
			}
			else
			{
				this.material.mainTexture.filterMode = FilterMode.Point;
			}
			this.material.mainTexture.wrapMode = TextureWrapMode.Clamp;
			this.material.SetVector("_TexParam", this.constTexParam);
		}
		this.material.SetColor("_Color", SFXMesh.colorData[SFX.colIntensity]);
		this.material.SetFloat("_Threshold", (SFX.colThreshold != 0) ? 0.05f : 0.0295f);
		this.material.SetPass(0);
		Graphics.DrawMeshNow(this.mesh, Matrix4x4.identity);
	}

	public unsafe void PolyF3(PSX_LIBGPU.POLY_F3* obj)
	{
		Int32 num = (Int32)obj->x0 + SFXMeshBase.drOffsetX;
		Int32 num2 = (Int32)obj->y0 + SFXMeshBase.drOffsetY;
		Int32 num3 = (Int32)obj->x1 + SFXMeshBase.drOffsetX;
		Int32 num4 = (Int32)obj->y1 + SFXMeshBase.drOffsetY;
		Int32 num5 = (Int32)obj->x2 + SFXMeshBase.drOffsetX;
		Int32 num6 = (Int32)obj->y2 + SFXMeshBase.drOffsetY;
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num, (Single)num2, SFXMesh.zDepth);
		this.vbPos[this.vbOffset] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num3, (Single)num4, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 1] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num5, (Single)num6, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 2] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r0;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g0;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b0;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.alpha;
		this.vbCol[this.vbOffset] = (this.vbCol[this.vbOffset + 1] = (this.vbCol[this.vbOffset + 2] = SFXMesh.gCol[SFXMesh.gColIndex++]));
		this.ibIndex[this.ibOffset++] = this.vbOffset;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.vbOffset += 3;
	}

	public unsafe void PolyFT3(PSX_LIBGPU.POLY_FT3* obj)
	{
		Int32 num = (Int32)obj->x0 + SFXMeshBase.drOffsetX;
		Int32 num2 = (Int32)obj->y0 + SFXMeshBase.drOffsetY;
		Int32 num3 = (Int32)obj->x1 + SFXMeshBase.drOffsetX;
		Int32 num4 = (Int32)obj->y1 + SFXMeshBase.drOffsetY;
		Int32 num5 = (Int32)obj->x2 + SFXMeshBase.drOffsetX;
		Int32 num6 = (Int32)obj->y2 + SFXMeshBase.drOffsetY;
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num, (Single)num2, SFXMesh.zDepth);
		this.vbPos[this.vbOffset] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num3, (Single)num4, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 1] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num5, (Single)num6, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 2] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u0, (Single)obj->v0);
		this.vbTex[this.vbOffset] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u1, (Single)obj->v1);
		this.vbTex[this.vbOffset + 1] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u2, (Single)obj->v2);
		this.vbTex[this.vbOffset + 2] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r0;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g0;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b0;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.GetShadeAlpha(obj->code);
		this.vbCol[this.vbOffset] = (this.vbCol[this.vbOffset + 1] = (this.vbCol[this.vbOffset + 2] = SFXMesh.gCol[SFXMesh.gColIndex++]));
		this.ibIndex[this.ibOffset++] = this.vbOffset;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.vbOffset += 3;
	}

	public unsafe void PolyG3(PSX_LIBGPU.POLY_G3* obj)
	{
		Int32 num = (Int32)obj->x0 + SFXMeshBase.drOffsetX;
		Int32 num2 = (Int32)obj->y0 + SFXMeshBase.drOffsetY;
		Int32 num3 = (Int32)obj->x1 + SFXMeshBase.drOffsetX;
		Int32 num4 = (Int32)obj->y1 + SFXMeshBase.drOffsetY;
		Int32 num5 = (Int32)obj->x2 + SFXMeshBase.drOffsetX;
		Int32 num6 = (Int32)obj->y2 + SFXMeshBase.drOffsetY;
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num, (Single)num2, SFXMesh.zDepth);
		this.vbPos[this.vbOffset] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num3, (Single)num4, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 1] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num5, (Single)num6, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 2] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r0;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g0;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b0;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.alpha;
		this.vbCol[this.vbOffset] = SFXMesh.gCol[SFXMesh.gColIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r1;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g1;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b1;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.alpha;
		this.vbCol[this.vbOffset + 1] = SFXMesh.gCol[SFXMesh.gColIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r2;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g2;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b2;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.alpha;
		this.vbCol[this.vbOffset + 2] = SFXMesh.gCol[SFXMesh.gColIndex++];
		this.ibIndex[this.ibOffset++] = this.vbOffset;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.vbOffset += 3;
	}

	public unsafe void PolyGT3(PSX_LIBGPU.POLY_GT3* obj)
	{
		Int32 num = (Int32)obj->x0 + SFXMeshBase.drOffsetX;
		Int32 num2 = (Int32)obj->y0 + SFXMeshBase.drOffsetY;
		Int32 num3 = (Int32)obj->x1 + SFXMeshBase.drOffsetX;
		Int32 num4 = (Int32)obj->y1 + SFXMeshBase.drOffsetY;
		Int32 num5 = (Int32)obj->x2 + SFXMeshBase.drOffsetX;
		Int32 num6 = (Int32)obj->y2 + SFXMeshBase.drOffsetY;
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num, (Single)num2, SFXMesh.zDepth);
		this.vbPos[this.vbOffset] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num3, (Single)num4, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 1] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num5, (Single)num6, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 2] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u0, (Single)obj->v0);
		this.vbTex[this.vbOffset] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u1, (Single)obj->v1);
		this.vbTex[this.vbOffset + 1] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u2, (Single)obj->v2);
		this.vbTex[this.vbOffset + 2] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		Byte shadeAlpha = this.GetShadeAlpha(obj->code);
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r0;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g0;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b0;
		SFXMesh.gCol[SFXMesh.gColIndex].a = shadeAlpha;
		this.vbCol[this.vbOffset] = SFXMesh.gCol[SFXMesh.gColIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r1;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g1;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b1;
		SFXMesh.gCol[SFXMesh.gColIndex].a = shadeAlpha;
		this.vbCol[this.vbOffset + 1] = SFXMesh.gCol[SFXMesh.gColIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r2;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g2;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b2;
		SFXMesh.gCol[SFXMesh.gColIndex].a = shadeAlpha;
		this.vbCol[this.vbOffset + 2] = SFXMesh.gCol[SFXMesh.gColIndex++];
		this.ibIndex[this.ibOffset++] = this.vbOffset;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.vbOffset += 3;
	}

	public unsafe void PolyF4(PSX_LIBGPU.POLY_F4* obj)
	{
		Int32 num = (Int32)obj->x0 + SFXMeshBase.drOffsetX;
		Int32 num2 = (Int32)obj->y0 + SFXMeshBase.drOffsetY;
		Int32 num3 = (Int32)obj->x1 + SFXMeshBase.drOffsetX;
		Int32 num4 = (Int32)obj->y1 + SFXMeshBase.drOffsetY;
		Int32 num5 = (Int32)obj->x2 + SFXMeshBase.drOffsetX;
		Int32 num6 = (Int32)obj->y2 + SFXMeshBase.drOffsetY;
		Int32 num7 = (Int32)obj->x3 + SFXMeshBase.drOffsetX;
		Int32 num8 = (Int32)obj->y3 + SFXMeshBase.drOffsetY;
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num, (Single)num2, SFXMesh.zDepth);
		this.vbPos[this.vbOffset] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num3, (Single)num4, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 1] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num5, (Single)num6, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 2] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num7, (Single)num8, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 3] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r0;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g0;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b0;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.alpha;
		this.vbCol[this.vbOffset] = (this.vbCol[this.vbOffset + 1] = (this.vbCol[this.vbOffset + 2] = (this.vbCol[this.vbOffset + 3] = SFXMesh.gCol[SFXMesh.gColIndex++])));
		this.ibIndex[this.ibOffset++] = this.vbOffset;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 3;
		this.vbOffset += 4;
	}

	public unsafe void PolyFT4(PSX_LIBGPU.POLY_FT4* obj)
	{
		Int32 num = (Int32)obj->x0 + SFXMeshBase.drOffsetX;
		Int32 num2 = (Int32)obj->y0 + SFXMeshBase.drOffsetY;
		Int32 num3 = (Int32)obj->x1 + SFXMeshBase.drOffsetX;
		Int32 num4 = (Int32)obj->y1 + SFXMeshBase.drOffsetY;
		Int32 num5 = (Int32)obj->x2 + SFXMeshBase.drOffsetX;
		Int32 num6 = (Int32)obj->y2 + SFXMeshBase.drOffsetY;
		Int32 num7 = (Int32)obj->x3 + SFXMeshBase.drOffsetX;
		Int32 num8 = (Int32)obj->y3 + SFXMeshBase.drOffsetY;
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num, (Single)num2, SFXMesh.zDepth);
		this.vbPos[this.vbOffset] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num3, (Single)num4, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 1] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num5, (Single)num6, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 2] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num7, (Single)num8, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 3] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u0, (Single)obj->v0);
		this.vbTex[this.vbOffset] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u1, (Single)obj->v1);
		this.vbTex[this.vbOffset + 1] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u2, (Single)obj->v2);
		this.vbTex[this.vbOffset + 2] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u3, (Single)obj->v3);
		this.vbTex[this.vbOffset + 3] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r0;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g0;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b0;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.GetShadeAlpha(obj->code);
		this.vbCol[this.vbOffset] = (this.vbCol[this.vbOffset + 1] = (this.vbCol[this.vbOffset + 2] = (this.vbCol[this.vbOffset + 3] = SFXMesh.gCol[SFXMesh.gColIndex++])));
		this.ibIndex[this.ibOffset++] = this.vbOffset;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 3;
		this.vbOffset += 4;
	}

	public unsafe void PolyBFT4(PSX_LIBGPU.POLY_FT4* obj)
	{
		Int32 num = (Int32)obj->x0 + SFXMeshBase.drOffsetX;
		Int32 num2 = (Int32)obj->y0 + SFXMeshBase.drOffsetY;
		Int32 num3 = (Int32)obj->x1 + SFXMeshBase.drOffsetX;
		Int32 num4 = (Int32)obj->y1 + SFXMeshBase.drOffsetY;
		Int32 num5 = (Int32)obj->x2 + SFXMeshBase.drOffsetX;
		Int32 num6 = (Int32)obj->y2 + SFXMeshBase.drOffsetY;
		Int32 num7 = (Int32)obj->x3 + SFXMeshBase.drOffsetX;
		Int32 num8 = (Int32)obj->y3 + SFXMeshBase.drOffsetY;
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num, (Single)num2, SFXMesh.zDepth);
		this.vbPos[this.vbOffset] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num3, (Single)num4, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 1] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num5, (Single)num6, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 2] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num7, (Single)num8, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 3] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		Single num9 = 0f;
		Single num10 = 240f;
		if ((this.key >> 16 & 31u) != 0u)
		{
			num9 = 128f;
		}
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u0 + num9, num10 - (Single)obj->v0);
		this.vbTex[this.vbOffset] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u1 + num9, num10 - (Single)obj->v1);
		this.vbTex[this.vbOffset + 1] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u2 + num9, num10 - (Single)obj->v2);
		this.vbTex[this.vbOffset + 2] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u3 + num9, num10 - (Single)obj->v3);
		this.vbTex[this.vbOffset + 3] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r0;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g0;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b0;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.alpha;
		this.vbCol[this.vbOffset] = (this.vbCol[this.vbOffset + 1] = (this.vbCol[this.vbOffset + 2] = (this.vbCol[this.vbOffset + 3] = SFXMesh.gCol[SFXMesh.gColIndex++])));
		this.ibIndex[this.ibOffset++] = this.vbOffset;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 3;
		this.vbOffset += 4;
	}

	public unsafe void PolyG4(PSX_LIBGPU.POLY_G4* obj)
	{
		Int32 num = (Int32)obj->x0 + SFXMeshBase.drOffsetX;
		Int32 num2 = (Int32)obj->y0 + SFXMeshBase.drOffsetY;
		Int32 num3 = (Int32)obj->x1 + SFXMeshBase.drOffsetX;
		Int32 num4 = (Int32)obj->y1 + SFXMeshBase.drOffsetY;
		Int32 num5 = (Int32)obj->x2 + SFXMeshBase.drOffsetX;
		Int32 num6 = (Int32)obj->y2 + SFXMeshBase.drOffsetY;
		Int32 num7 = (Int32)obj->x3 + SFXMeshBase.drOffsetX;
		Int32 num8 = (Int32)obj->y3 + SFXMeshBase.drOffsetY;
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num, (Single)num2, SFXMesh.zDepth);
		this.vbPos[this.vbOffset] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num3, (Single)num4, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 1] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num5, (Single)num6, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 2] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num7, (Single)num8, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 3] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r0;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g0;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b0;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.alpha;
		this.vbCol[this.vbOffset] = SFXMesh.gCol[SFXMesh.gColIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r1;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g1;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b1;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.alpha;
		this.vbCol[this.vbOffset + 1] = SFXMesh.gCol[SFXMesh.gColIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r2;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g2;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b2;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.alpha;
		this.vbCol[this.vbOffset + 2] = SFXMesh.gCol[SFXMesh.gColIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r3;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g3;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b3;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.alpha;
		this.vbCol[this.vbOffset + 3] = SFXMesh.gCol[SFXMesh.gColIndex++];
		this.ibIndex[this.ibOffset++] = this.vbOffset;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 3;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.vbOffset += 4;
	}

	public unsafe void PolyGT4(PSX_LIBGPU.POLY_GT4* obj)
	{
		Int32 num = (Int32)obj->x0 + SFXMeshBase.drOffsetX;
		Int32 num2 = (Int32)obj->y0 + SFXMeshBase.drOffsetY;
		Int32 num3 = (Int32)obj->x1 + SFXMeshBase.drOffsetX;
		Int32 num4 = (Int32)obj->y1 + SFXMeshBase.drOffsetY;
		Int32 num5 = (Int32)obj->x2 + SFXMeshBase.drOffsetX;
		Int32 num6 = (Int32)obj->y2 + SFXMeshBase.drOffsetY;
		Int32 num7 = (Int32)obj->x3 + SFXMeshBase.drOffsetX;
		Int32 num8 = (Int32)obj->y3 + SFXMeshBase.drOffsetY;
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num, (Single)num2, SFXMesh.zDepth);
		this.vbPos[this.vbOffset] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num3, (Single)num4, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 1] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num5, (Single)num6, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 2] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num7, (Single)num8, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 3] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u0, (Single)obj->v0);
		this.vbTex[this.vbOffset] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u1, (Single)obj->v1);
		this.vbTex[this.vbOffset + 1] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u2, (Single)obj->v2);
		this.vbTex[this.vbOffset + 2] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u3, (Single)obj->v3);
		this.vbTex[this.vbOffset + 3] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		Byte shadeAlpha = this.GetShadeAlpha(obj->code);
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r0;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g0;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b0;
		SFXMesh.gCol[SFXMesh.gColIndex].a = shadeAlpha;
		this.vbCol[this.vbOffset] = SFXMesh.gCol[SFXMesh.gColIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r1;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g1;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b1;
		SFXMesh.gCol[SFXMesh.gColIndex].a = shadeAlpha;
		this.vbCol[this.vbOffset + 1] = SFXMesh.gCol[SFXMesh.gColIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r2;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g2;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b2;
		SFXMesh.gCol[SFXMesh.gColIndex].a = shadeAlpha;
		this.vbCol[this.vbOffset + 2] = SFXMesh.gCol[SFXMesh.gColIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r3;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g3;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b3;
		SFXMesh.gCol[SFXMesh.gColIndex].a = shadeAlpha;
		this.vbCol[this.vbOffset + 3] = SFXMesh.gCol[SFXMesh.gColIndex++];
		this.ibIndex[this.ibOffset++] = this.vbOffset;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 3;
		this.vbOffset += 4;
	}

	public unsafe void PolyBGT4(PSX_LIBGPU.POLY_GT4* obj)
	{
		Int32 num = (Int32)obj->x0 + SFXMeshBase.drOffsetX;
		Int32 num2 = (Int32)obj->y0 + SFXMeshBase.drOffsetY;
		Int32 num3 = (Int32)obj->x1 + SFXMeshBase.drOffsetX;
		Int32 num4 = (Int32)obj->y1 + SFXMeshBase.drOffsetY;
		Int32 num5 = (Int32)obj->x2 + SFXMeshBase.drOffsetX;
		Int32 num6 = (Int32)obj->y2 + SFXMeshBase.drOffsetY;
		Int32 num7 = (Int32)obj->x3 + SFXMeshBase.drOffsetX;
		Int32 num8 = (Int32)obj->y3 + SFXMeshBase.drOffsetY;
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num, (Single)num2, SFXMesh.zDepth);
		this.vbPos[this.vbOffset] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num3, (Single)num4, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 1] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num5, (Single)num6, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 2] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num7, (Single)num8, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 3] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		Single num9 = 0f;
		Single num10 = 240f;
		if ((this.key >> 16 & 31u) != 0u)
		{
			num9 = 128f;
		}
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u0 + num9, num10 - (Single)obj->v0);
		this.vbTex[this.vbOffset] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u1 + num9, num10 - (Single)obj->v1);
		this.vbTex[this.vbOffset + 1] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u2 + num9, num10 - (Single)obj->v2);
		this.vbTex[this.vbOffset + 2] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u3 + num9, num10 - (Single)obj->v3);
		this.vbTex[this.vbOffset + 3] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		Byte a = this.alpha;
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r0;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g0;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b0;
		SFXMesh.gCol[SFXMesh.gColIndex].a = a;
		this.vbCol[this.vbOffset] = SFXMesh.gCol[SFXMesh.gColIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r1;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g1;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b1;
		SFXMesh.gCol[SFXMesh.gColIndex].a = a;
		this.vbCol[this.vbOffset + 1] = SFXMesh.gCol[SFXMesh.gColIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r2;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g2;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b2;
		SFXMesh.gCol[SFXMesh.gColIndex].a = a;
		this.vbCol[this.vbOffset + 2] = SFXMesh.gCol[SFXMesh.gColIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r3;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g3;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b3;
		SFXMesh.gCol[SFXMesh.gColIndex].a = a;
		this.vbCol[this.vbOffset + 3] = SFXMesh.gCol[SFXMesh.gColIndex++];
		this.ibIndex[this.ibOffset++] = this.vbOffset;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 3;
		this.vbOffset += 4;
	}

	public unsafe void SPRT(PSX_LIBGPU.SPRT* obj, Int32 w, Int32 h)
	{
		Single num = (Single)((Int32)obj->x0 + SFXMeshBase.drOffsetX);
		Single num2 = (Single)((Int32)obj->y0 + SFXMeshBase.drOffsetY);
		Single num3 = (Single)w - 1f;
		Single num4 = (Single)h - 1f;
		SFXMesh.gPos[SFXMesh.gPosIndex].Set(num, num2, SFXMesh.zDepth);
		this.vbPos[this.vbOffset] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set(num + (Single)w, num2, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 1] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set(num, num2 + (Single)h, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 2] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set(num + (Single)w, num2 + (Single)h, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 3] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u0, (Single)obj->v0);
		this.vbTex[this.vbOffset] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u0 + num3, (Single)obj->v0);
		this.vbTex[this.vbOffset + 1] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u0, (Single)obj->v0 + num4);
		this.vbTex[this.vbOffset + 2] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gTex[SFXMesh.gTexIndex].Set((Single)obj->u0 + num3, (Single)obj->v0 + num4);
		this.vbTex[this.vbOffset + 3] = SFXMesh.gTex[SFXMesh.gTexIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r0;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g0;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b0;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.GetShadeAlpha(obj->code);
		this.vbCol[this.vbOffset] = (this.vbCol[this.vbOffset + 1] = (this.vbCol[this.vbOffset + 2] = (this.vbCol[this.vbOffset + 3] = SFXMesh.gCol[SFXMesh.gColIndex++])));
		this.ibIndex[this.ibOffset++] = this.vbOffset;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 3;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.vbOffset += 4;
	}

	public unsafe void LineF2(PSX_LIBGPU.LINE_F2* obj)
	{
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)((Int32)obj->x0 + SFXMeshBase.drOffsetX), (Single)((Int32)obj->y0 + SFXMeshBase.drOffsetY), SFXMesh.zDepth);
		this.vbPos[this.vbOffset] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)((Int32)obj->x1 + SFXMeshBase.drOffsetX), (Single)((Int32)obj->y1 + SFXMeshBase.drOffsetY), SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 1] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r0;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g0;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b0;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.alpha;
		this.vbCol[this.vbOffset] = (this.vbCol[this.vbOffset + 1] = SFXMesh.gCol[SFXMesh.gColIndex++]);
		this.ibIndex[this.ibOffset++] = this.vbOffset;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.vbOffset += 2;
	}

	public unsafe void LineG2(PSX_LIBGPU.LINE_G2* obj)
	{
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)((Int32)obj->x0 + SFXMeshBase.drOffsetX), (Single)((Int32)obj->y0 + SFXMeshBase.drOffsetY), SFXMesh.zDepth);
		this.vbPos[this.vbOffset] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)((Int32)obj->x1 + SFXMeshBase.drOffsetX), (Single)((Int32)obj->y1 + SFXMeshBase.drOffsetY), SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 1] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r0;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g0;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b0;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.alpha;
		this.vbCol[this.vbOffset] = SFXMesh.gCol[SFXMesh.gColIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r1;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g1;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b1;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.alpha;
		this.vbCol[this.vbOffset + 1] = SFXMesh.gCol[SFXMesh.gColIndex++];
		this.ibIndex[this.ibOffset++] = this.vbOffset;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.vbOffset += 2;
	}

	public unsafe void TILE(PSX_LIBGPU.TILE* obj, Int32 w, Int32 h)
	{
		Int32 num = (Int32)obj->x0 + SFXMeshBase.drOffsetX;
		Int32 num2 = (Int32)obj->y0 + SFXMeshBase.drOffsetY;
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num, (Single)num2, SFXMesh.zDepth);
		this.vbPos[this.vbOffset] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)(num + w), (Single)num2, SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 1] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)num, (Single)(num2 + h), SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 2] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gPos[SFXMesh.gPosIndex].Set((Single)(num + w), (Single)(num2 + h), SFXMesh.zDepth);
		this.vbPos[this.vbOffset + 3] = SFXMesh.gPos[SFXMesh.gPosIndex++];
		SFXMesh.gCol[SFXMesh.gColIndex].r = obj->r0;
		SFXMesh.gCol[SFXMesh.gColIndex].g = obj->g0;
		SFXMesh.gCol[SFXMesh.gColIndex].b = obj->b0;
		SFXMesh.gCol[SFXMesh.gColIndex].a = this.alpha;
		this.vbCol[this.vbOffset] = (this.vbCol[this.vbOffset + 1] = (this.vbCol[this.vbOffset + 2] = (this.vbCol[this.vbOffset + 3] = SFXMesh.gCol[SFXMesh.gColIndex++])));
		this.ibIndex[this.ibOffset++] = this.vbOffset;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 1;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 3;
		this.ibIndex[this.ibOffset++] = this.vbOffset + 2;
		this.vbOffset += 4;
	}

	public Byte GetShadeAlpha(Byte code)
	{
		if ((code & 1) != 0)
		{
			return this.alpha;
		}
		Int32 num = (Int32)this.alpha << 1;
		if (num > 255)
		{
			return Byte.MaxValue;
		}
		return (Byte)num;
	}

	public static Byte[] abrAlphaData = new Byte[]
	{
		63,
		127,
		127,
		31
	};

	public static Color[] colorData;

	public static Single zDepth;

	private static Single HALF_PIXEL = 0.5f;

	private static Single INV_HALF_PIXEL = 0f;

	private static Int32 INDICES_MAX = 21000;

	private static Int32 VERTICES_MAX = 14000;

	private static Shader[] shaders;

	private static Int32 POS_MAX = 22000;

	private static Int32 TEX_MAX = 19300;

	private static Int32 COL_MAX = 10000;

	public static Int32 gPosIndex;

	public static Int32 gTexIndex;

	public static Int32 gColIndex;

	private static Vector3[] gPos;

	private static Vector2[] gTex;

	private static Color32[] gCol;

	private static Material dummyMaterial;

	private UInt32 key;

	private Byte alpha;

	private Mesh mesh;

	private Material material;

	public MeshTopology polyType;

	public Int32[] ibIndex;

	public Vector3[] vbPos;

	public Color32[] vbCol;

	public Vector2[] vbTex;

	public Int32 vbOffset;

	public Int32 ibOffset;

	private UInt32 shaderIndex;

	private Vector4 constTexParam;
}
