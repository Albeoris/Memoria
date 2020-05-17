using System;
using Memoria.Scripts;
using UnityEngine;

public class SFXMesh : SFXMeshBase
{
	public SFXMesh()
	{
		_key = 0u;
		_material = new Material(__shaders[0]);
		_mesh = new Mesh();
		_mesh.MarkDynamic();
		IbIndex = new Int32[INDICES_MAX];
		VbPos = new Vector3[VERTICES_MAX];
		VbCol = new Color32[VERTICES_MAX];
		VbTex = new Vector2[VERTICES_MAX];
		_constTexParam = new Vector4(HALF_PIXEL, HALF_PIXEL, 256f, 256f);
	}

	public static void DummyRender()
	{
		Color32 c = new Color32(0, 0, 0, 0);
		Vector3 v = new Vector3(0f, 0f, 0f);
		__dummyMaterial.SetPass(0);
		GL.Begin(1);
		GL.Color(c);
		GL.Vertex(v);
		GL.Color(c);
		GL.Vertex(v);
		GL.End();
	}

	public static void Init()
	{
		__shaders = new Shader[6];
		__shaders[0] = ShadersLoader.Find("SFX_OPA_GT");
		__shaders[1] = ShadersLoader.Find("SFX_ADD_GT");
		__shaders[2] = ShadersLoader.Find("SFX_SUB_GT");
		__shaders[3] = ShadersLoader.Find("SFX_OPA_G");
		__shaders[4] = ShadersLoader.Find("SFX_ADD_G");
	    __shaders[5] = ShadersLoader.Find("SFX_SUB_G");
		__gPos = new Vector3[POS_MAX];
		for (Int32 i = 0; i < __gPos.Length; i++)
		{
			__gPos[i] = default;
		}
		__gTex = new Vector2[TEX_MAX];
		for (Int32 j = 0; j < __gTex.Length; j++)
		{
			__gTex[j] = default;
		}
		__gCol = new Color32[COL_MAX];
		for (Int32 k = 0; k < __gCol.Length; k++)
		{
			__gCol[k] = default;
		}
		ColorData = new Color[3];
		ColorData[0] = new Color(1f, 1f, 1f, 1f);
		ColorData[1] = new Color(1.5f, 1.5f, 1.5f, 1f);
		ColorData[2] = new Color(2f, 2f, 2f, 1f);
		__dummyMaterial = new Material(__shaders[4]);
	}

	public static void Release()
	{
		UnityEngine.Object.Destroy(__dummyMaterial);
		for (Int32 i = 0; i < 6; i++)
		{
			__shaders[i] = null;
		}
		__shaders = null;
		curRenderTexture = null;
	}

	public void ClearObject()
	{
		UnityEngine.Object.Destroy(_material);
		UnityEngine.Object.Destroy(_mesh);
		IbIndex = null;
		VbPos = null;
		VbCol = null;
		VbTex = null;
	}

	public void Setup(UInt32 meshKey, Byte code)
	{
		_key = meshKey;
		Boolean flag = SFXKey.IsTexture(meshKey);
		_shaderIndex = (!flag) ? 3u : 0u;
		if ((code & 2) != 0)
		{
			_alpha = AbrAlphaData[(Int32)((UIntPtr)SFXKey.tmpABR)];
			_shaderIndex += SFXKey.GetBlendMode(meshKey);
		}
		else
		{
			_alpha = 128;
		}
		if (flag)
		{
			if (SFX.pixelOffset == 0)
			{
				_constTexParam.x = HALF_PIXEL;
				_constTexParam.y = HALF_PIXEL;
			}
			else if (SFX.pixelOffset == 1)
			{
				_constTexParam.x = INV_HALF_PIXEL;
				_constTexParam.y = INV_HALF_PIXEL;
			}
			else
			{
				_constTexParam.x = HALF_PIXEL;
				_constTexParam.y = INV_HALF_PIXEL;
			}
			_constTexParam.z = 256f;
			_constTexParam.w = 256f;
		}
	}

	private Texture GetTexture()
	{
		UInt32 textureKey = SFXKey.GetTextureKey(_key);
		if (SFXKey.GetTextureMode(_key) != 2u)
		{
			if (SFX.currentEffectID == 435)
			{
				Single[] array = new Single[2];
				Texture texture = PSXTextureMgr.GetTexture435(textureKey, array);
				if (texture != null)
				{
					_constTexParam.y = array[0];
					_constTexParam.w = array[1];
					return texture;
				}
			}
			return PSXTextureMgr.GetTexture(_key).texture;
		}
		if (SFXKey.IsBlurTexture(textureKey))
		{
			if ((_key & 536870912u) != 0u)
			{
				_constTexParam.w = 240f;
			}
			else
			{
				_constTexParam.y = -240f;
				_constTexParam.w = -240f;
			}
			_constTexParam.z = 320f;
			return PSXTextureMgr.blurTexture;
		}
		if ((_key & 2031616u) == PSXTextureMgr.bgKey)
		{
			return PSXTextureMgr.bgTexture;
		}
		if (PSXTextureMgr.isCreateGenTexture)
		{
			_constTexParam.x = SFXKey.GetPositionX(_key) - (UInt64)PSXTextureMgr.GEN_TEXTURE_X + 0.5f;
			_constTexParam.z = PSXTextureMgr.GEN_TEXTURE_W;
			_constTexParam.w = PSXTextureMgr.GEN_TEXTURE_H;
			return PSXTextureMgr.genTexture;
		}
		if (SFX.currentEffectID == 274 && textureKey == 5767167u)
		{
			_constTexParam.x = 176f;
			_constTexParam.y = 0f;
			_constTexParam.z = 480f;
			_constTexParam.w = 360f;
			return PSXTextureMgr.blurTexture;
		}
		return PSXTextureMgr.GetTexture(_key).texture;
	}

	public UInt32 GetKey()
	{
		return _key;
	}

	public void Begin()
	{
		_key = 0u;
		IbOffset = 0;
		VbOffset = 0;
	}

	public void End()
	{
		_mesh.Clear();
		Int32[] array = new Int32[IbOffset];
		Buffer.BlockCopy(IbIndex, 0, array, 0, IbOffset << 2);
		FixWideScreenOffset();
		_mesh.vertices = VbPos;
		_mesh.colors32 = VbCol;
		
		_mesh.uv = SFXKey.IsTexture(_key) ? VbTex : null;
		
		if (SFX.isDebugLine)
			_mesh.SetIndices(array, MeshTopology.Lines, 0);
		else if (SFXKey.isLinePolygon(_key))
			_mesh.SetIndices(array, MeshTopology.Lines, 0);
		else
			_mesh.triangles = array;
	}

	private void FixWideScreenOffset()
	{
		Int32 offsetValue = (FieldMap.PsxFieldWidth - 320) / 2;
		for (Int32 i = 0; i < VbOffset; i++)
			VbPos[i].x = VbPos[i].x + offsetValue;
	}

	public override void Render(Int32 index)
	{
		_material.shader = __shaders[(Int32)((UIntPtr)_shaderIndex)];
		if (SFXKey.IsTexture(_key))
		{
			_material.mainTexture = GetTexture();
			if (_material.mainTexture == null)
			{
				return;
			}
			UInt32 filter = SFXKey.GetFilter(_key);
			if (filter != 33554432u)
			{
				if (filter != 67108864u)
				{
					_material.mainTexture.filterMode = (!SFX.isDebugFillter) ? FilterMode.Point : FilterMode.Bilinear;
				}
				else
				{
					_material.mainTexture.filterMode = FilterMode.Bilinear;
				}
			}
			else
			{
				_material.mainTexture.filterMode = FilterMode.Point;
			}
			_material.mainTexture.wrapMode = TextureWrapMode.Clamp;
			_material.SetVector(TexParam, _constTexParam);
		}
		_material.SetColor(Color, ColorData[SFX.colIntensity]);
		_material.SetFloat(Threshold, (SFX.colThreshold != 0) ? 0.05f : 0.0295f);
		_material.SetPass(0);
		Graphics.DrawMeshNow(_mesh, Matrix4x4.identity);
	}

	public unsafe void PolyF3(PSX_LIBGPU.POLY_F3* obj)
	{
		Int32 num = obj->x0 + drOffsetX;
		Int32 num2 = obj->y0 + drOffsetY;
		Int32 num3 = obj->x1 + drOffsetX;
		Int32 num4 = obj->y1 + drOffsetY;
		Int32 num5 = obj->x2 + drOffsetX;
		Int32 num6 = obj->y2 + drOffsetY;
		__gPos[GPosIndex].Set(num, num2, GzDepth);
		VbPos[VbOffset] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num3, num4, GzDepth);
		VbPos[VbOffset + 1] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num5, num6, GzDepth);
		VbPos[VbOffset + 2] = __gPos[GPosIndex++];
		__gCol[GColIndex].r = obj->r0;
		__gCol[GColIndex].g = obj->g0;
		__gCol[GColIndex].b = obj->b0;
		__gCol[GColIndex].a = _alpha;
		VbCol[VbOffset] = (VbCol[VbOffset + 1] = (VbCol[VbOffset + 2] = __gCol[GColIndex++]));
		IbIndex[IbOffset++] = VbOffset;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 2;
		VbOffset += 3;
	}

	public unsafe void PolyFt3(PSX_LIBGPU.POLY_FT3* obj)
	{
		Int32 num = obj->x0 + drOffsetX;
		Int32 num2 = obj->y0 + drOffsetY;
		Int32 num3 = obj->x1 + drOffsetX;
		Int32 num4 = obj->y1 + drOffsetY;
		Int32 num5 = obj->x2 + drOffsetX;
		Int32 num6 = obj->y2 + drOffsetY;
		__gPos[GPosIndex].Set(num, num2, GzDepth);
		VbPos[VbOffset] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num3, num4, GzDepth);
		VbPos[VbOffset + 1] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num5, num6, GzDepth);
		VbPos[VbOffset + 2] = __gPos[GPosIndex++];
		__gTex[GTexIndex].Set(obj->u0, obj->v0);
		VbTex[VbOffset] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u1, obj->v1);
		VbTex[VbOffset + 1] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u2, obj->v2);
		VbTex[VbOffset + 2] = __gTex[GTexIndex++];
		__gCol[GColIndex].r = obj->r0;
		__gCol[GColIndex].g = obj->g0;
		__gCol[GColIndex].b = obj->b0;
		__gCol[GColIndex].a = GetShadeAlpha(obj->code);
		VbCol[VbOffset] = (VbCol[VbOffset + 1] = (VbCol[VbOffset + 2] = __gCol[GColIndex++]));
		IbIndex[IbOffset++] = VbOffset;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 2;
		VbOffset += 3;
	}

	public unsafe void PolyG3(PSX_LIBGPU.POLY_G3* obj)
	{
		Int32 num = obj->x0 + drOffsetX;
		Int32 num2 = obj->y0 + drOffsetY;
		Int32 num3 = obj->x1 + drOffsetX;
		Int32 num4 = obj->y1 + drOffsetY;
		Int32 num5 = obj->x2 + drOffsetX;
		Int32 num6 = obj->y2 + drOffsetY;
		__gPos[GPosIndex].Set(num, num2, GzDepth);
		VbPos[VbOffset] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num3, num4, GzDepth);
		VbPos[VbOffset + 1] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num5, num6, GzDepth);
		VbPos[VbOffset + 2] = __gPos[GPosIndex++];
		__gCol[GColIndex].r = obj->r0;
		__gCol[GColIndex].g = obj->g0;
		__gCol[GColIndex].b = obj->b0;
		__gCol[GColIndex].a = _alpha;
		VbCol[VbOffset] = __gCol[GColIndex++];
		__gCol[GColIndex].r = obj->r1;
		__gCol[GColIndex].g = obj->g1;
		__gCol[GColIndex].b = obj->b1;
		__gCol[GColIndex].a = _alpha;
		VbCol[VbOffset + 1] = __gCol[GColIndex++];
		__gCol[GColIndex].r = obj->r2;
		__gCol[GColIndex].g = obj->g2;
		__gCol[GColIndex].b = obj->b2;
		__gCol[GColIndex].a = _alpha;
		VbCol[VbOffset + 2] = __gCol[GColIndex++];
		IbIndex[IbOffset++] = VbOffset;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 2;
		VbOffset += 3;
	}

	public unsafe void PolyGt3(PSX_LIBGPU.POLY_GT3* obj)
	{
		Int32 num = obj->x0 + drOffsetX;
		Int32 num2 = obj->y0 + drOffsetY;
		Int32 num3 = obj->x1 + drOffsetX;
		Int32 num4 = obj->y1 + drOffsetY;
		Int32 num5 = obj->x2 + drOffsetX;
		Int32 num6 = obj->y2 + drOffsetY;
		__gPos[GPosIndex].Set(num, num2, GzDepth);
		VbPos[VbOffset] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num3, num4, GzDepth);
		VbPos[VbOffset + 1] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num5, num6, GzDepth);
		VbPos[VbOffset + 2] = __gPos[GPosIndex++];
		__gTex[GTexIndex].Set(obj->u0, obj->v0);
		VbTex[VbOffset] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u1, obj->v1);
		VbTex[VbOffset + 1] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u2, obj->v2);
		VbTex[VbOffset + 2] = __gTex[GTexIndex++];
		Byte shadeAlpha = GetShadeAlpha(obj->code);
		__gCol[GColIndex].r = obj->r0;
		__gCol[GColIndex].g = obj->g0;
		__gCol[GColIndex].b = obj->b0;
		__gCol[GColIndex].a = shadeAlpha;
		VbCol[VbOffset] = __gCol[GColIndex++];
		__gCol[GColIndex].r = obj->r1;
		__gCol[GColIndex].g = obj->g1;
		__gCol[GColIndex].b = obj->b1;
		__gCol[GColIndex].a = shadeAlpha;
		VbCol[VbOffset + 1] = __gCol[GColIndex++];
		__gCol[GColIndex].r = obj->r2;
		__gCol[GColIndex].g = obj->g2;
		__gCol[GColIndex].b = obj->b2;
		__gCol[GColIndex].a = shadeAlpha;
		VbCol[VbOffset + 2] = __gCol[GColIndex++];
		IbIndex[IbOffset++] = VbOffset;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 2;
		VbOffset += 3;
	}

	public unsafe void PolyF4(PSX_LIBGPU.POLY_F4* obj)
	{
		Int32 num = obj->x0 + drOffsetX;
		Int32 num2 = obj->y0 + drOffsetY;
		Int32 num3 = obj->x1 + drOffsetX;
		Int32 num4 = obj->y1 + drOffsetY;
		Int32 num5 = obj->x2 + drOffsetX;
		Int32 num6 = obj->y2 + drOffsetY;
		Int32 num7 = obj->x3 + drOffsetX;
		Int32 num8 = obj->y3 + drOffsetY;
		__gPos[GPosIndex].Set(num, num2, GzDepth);
		VbPos[VbOffset] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num3, num4, GzDepth);
		VbPos[VbOffset + 1] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num5, num6, GzDepth);
		VbPos[VbOffset + 2] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num7, num8, GzDepth);
		VbPos[VbOffset + 3] = __gPos[GPosIndex++];
		__gCol[GColIndex].r = obj->r0;
		__gCol[GColIndex].g = obj->g0;
		__gCol[GColIndex].b = obj->b0;
		__gCol[GColIndex].a = _alpha;
		VbCol[VbOffset] = (VbCol[VbOffset + 1] = (VbCol[VbOffset + 2] = (VbCol[VbOffset + 3] = __gCol[GColIndex++])));
		IbIndex[IbOffset++] = VbOffset;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 2;
		IbIndex[IbOffset++] = VbOffset + 2;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 3;
		VbOffset += 4;
	}

	public unsafe void PolyFt4(PSX_LIBGPU.POLY_FT4* obj)
	{
		Int32 num = obj->x0 + drOffsetX;
		Int32 num2 = obj->y0 + drOffsetY;
		Int32 num3 = obj->x1 + drOffsetX;
		Int32 num4 = obj->y1 + drOffsetY;
		Int32 num5 = obj->x2 + drOffsetX;
		Int32 num6 = obj->y2 + drOffsetY;
		Int32 num7 = obj->x3 + drOffsetX;
		Int32 num8 = obj->y3 + drOffsetY;
		__gPos[GPosIndex].Set(num, num2, GzDepth);
		VbPos[VbOffset] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num3, num4, GzDepth);
		VbPos[VbOffset + 1] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num5, num6, GzDepth);
		VbPos[VbOffset + 2] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num7, num8, GzDepth);
		VbPos[VbOffset + 3] = __gPos[GPosIndex++];
		__gTex[GTexIndex].Set(obj->u0, obj->v0);
		VbTex[VbOffset] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u1, obj->v1);
		VbTex[VbOffset + 1] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u2, obj->v2);
		VbTex[VbOffset + 2] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u3, obj->v3);
		VbTex[VbOffset + 3] = __gTex[GTexIndex++];
		__gCol[GColIndex].r = obj->r0;
		__gCol[GColIndex].g = obj->g0;
		__gCol[GColIndex].b = obj->b0;
		__gCol[GColIndex].a = GetShadeAlpha(obj->code);
		VbCol[VbOffset] = (VbCol[VbOffset + 1] = (VbCol[VbOffset + 2] = (VbCol[VbOffset + 3] = __gCol[GColIndex++])));
		IbIndex[IbOffset++] = VbOffset;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 2;
		IbIndex[IbOffset++] = VbOffset + 2;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 3;
		VbOffset += 4;
	}

	public unsafe void PolyBft4(PSX_LIBGPU.POLY_FT4* obj)
	{
		Int32 num = obj->x0 + drOffsetX;
		Int32 num2 = obj->y0 + drOffsetY;
		Int32 num3 = obj->x1 + drOffsetX;
		Int32 num4 = obj->y1 + drOffsetY;
		Int32 num5 = obj->x2 + drOffsetX;
		Int32 num6 = obj->y2 + drOffsetY;
		Int32 num7 = obj->x3 + drOffsetX;
		Int32 num8 = obj->y3 + drOffsetY;
		__gPos[GPosIndex].Set(num, num2, GzDepth);
		VbPos[VbOffset] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num3, num4, GzDepth);
		VbPos[VbOffset + 1] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num5, num6, GzDepth);
		VbPos[VbOffset + 2] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num7, num8, GzDepth);
		VbPos[VbOffset + 3] = __gPos[GPosIndex++];
		Single num9 = 0f;
		Single num10 = 240f;
		if ((_key >> 16 & 31u) != 0u)
		{
			num9 = 128f;
		}
		__gTex[GTexIndex].Set(obj->u0 + num9, num10 - obj->v0);
		VbTex[VbOffset] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u1 + num9, num10 - obj->v1);
		VbTex[VbOffset + 1] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u2 + num9, num10 - obj->v2);
		VbTex[VbOffset + 2] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u3 + num9, num10 - obj->v3);
		VbTex[VbOffset + 3] = __gTex[GTexIndex++];
		__gCol[GColIndex].r = obj->r0;
		__gCol[GColIndex].g = obj->g0;
		__gCol[GColIndex].b = obj->b0;
		__gCol[GColIndex].a = _alpha;
		VbCol[VbOffset] = (VbCol[VbOffset + 1] = (VbCol[VbOffset + 2] = (VbCol[VbOffset + 3] = __gCol[GColIndex++])));
		IbIndex[IbOffset++] = VbOffset;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 2;
		IbIndex[IbOffset++] = VbOffset + 2;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 3;
		VbOffset += 4;
	}

	public unsafe void PolyG4(PSX_LIBGPU.POLY_G4* obj)
	{
		Int32 num = obj->x0 + drOffsetX;
		Int32 num2 = obj->y0 + drOffsetY;
		Int32 num3 = obj->x1 + drOffsetX;
		Int32 num4 = obj->y1 + drOffsetY;
		Int32 num5 = obj->x2 + drOffsetX;
		Int32 num6 = obj->y2 + drOffsetY;
		Int32 num7 = obj->x3 + drOffsetX;
		Int32 num8 = obj->y3 + drOffsetY;
		__gPos[GPosIndex].Set(num, num2, GzDepth);
		VbPos[VbOffset] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num3, num4, GzDepth);
		VbPos[VbOffset + 1] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num5, num6, GzDepth);
		VbPos[VbOffset + 2] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num7, num8, GzDepth);
		VbPos[VbOffset + 3] = __gPos[GPosIndex++];
		__gCol[GColIndex].r = obj->r0;
		__gCol[GColIndex].g = obj->g0;
		__gCol[GColIndex].b = obj->b0;
		__gCol[GColIndex].a = _alpha;
		VbCol[VbOffset] = __gCol[GColIndex++];
		__gCol[GColIndex].r = obj->r1;
		__gCol[GColIndex].g = obj->g1;
		__gCol[GColIndex].b = obj->b1;
		__gCol[GColIndex].a = _alpha;
		VbCol[VbOffset + 1] = __gCol[GColIndex++];
		__gCol[GColIndex].r = obj->r2;
		__gCol[GColIndex].g = obj->g2;
		__gCol[GColIndex].b = obj->b2;
		__gCol[GColIndex].a = _alpha;
		VbCol[VbOffset + 2] = __gCol[GColIndex++];
		__gCol[GColIndex].r = obj->r3;
		__gCol[GColIndex].g = obj->g3;
		__gCol[GColIndex].b = obj->b3;
		__gCol[GColIndex].a = _alpha;
		VbCol[VbOffset + 3] = __gCol[GColIndex++];
		IbIndex[IbOffset++] = VbOffset;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 2;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 3;
		IbIndex[IbOffset++] = VbOffset + 2;
		VbOffset += 4;
	}

	public unsafe void PolyGt4(PSX_LIBGPU.POLY_GT4* obj)
	{
		Int32 num = obj->x0 + drOffsetX;
		Int32 num2 = obj->y0 + drOffsetY;
		Int32 num3 = obj->x1 + drOffsetX;
		Int32 num4 = obj->y1 + drOffsetY;
		Int32 num5 = obj->x2 + drOffsetX;
		Int32 num6 = obj->y2 + drOffsetY;
		Int32 num7 = obj->x3 + drOffsetX;
		Int32 num8 = obj->y3 + drOffsetY;
		__gPos[GPosIndex].Set(num, num2, GzDepth);
		VbPos[VbOffset] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num3, num4, GzDepth);
		VbPos[VbOffset + 1] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num5, num6, GzDepth);
		VbPos[VbOffset + 2] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num7, num8, GzDepth);
		VbPos[VbOffset + 3] = __gPos[GPosIndex++];
		__gTex[GTexIndex].Set(obj->u0, obj->v0);
		VbTex[VbOffset] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u1, obj->v1);
		VbTex[VbOffset + 1] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u2, obj->v2);
		VbTex[VbOffset + 2] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u3, obj->v3);
		VbTex[VbOffset + 3] = __gTex[GTexIndex++];
		Byte shadeAlpha = GetShadeAlpha(obj->code);
		__gCol[GColIndex].r = obj->r0;
		__gCol[GColIndex].g = obj->g0;
		__gCol[GColIndex].b = obj->b0;
		__gCol[GColIndex].a = shadeAlpha;
		VbCol[VbOffset] = __gCol[GColIndex++];
		__gCol[GColIndex].r = obj->r1;
		__gCol[GColIndex].g = obj->g1;
		__gCol[GColIndex].b = obj->b1;
		__gCol[GColIndex].a = shadeAlpha;
		VbCol[VbOffset + 1] = __gCol[GColIndex++];
		__gCol[GColIndex].r = obj->r2;
		__gCol[GColIndex].g = obj->g2;
		__gCol[GColIndex].b = obj->b2;
		__gCol[GColIndex].a = shadeAlpha;
		VbCol[VbOffset + 2] = __gCol[GColIndex++];
		__gCol[GColIndex].r = obj->r3;
		__gCol[GColIndex].g = obj->g3;
		__gCol[GColIndex].b = obj->b3;
		__gCol[GColIndex].a = shadeAlpha;
		VbCol[VbOffset + 3] = __gCol[GColIndex++];
		IbIndex[IbOffset++] = VbOffset;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 2;
		IbIndex[IbOffset++] = VbOffset + 2;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 3;
		VbOffset += 4;
	}

	public unsafe void PolyBgt4(PSX_LIBGPU.POLY_GT4* obj)
	{
		Int32 num = obj->x0 + drOffsetX;
		Int32 num2 = obj->y0 + drOffsetY;
		Int32 num3 = obj->x1 + drOffsetX;
		Int32 num4 = obj->y1 + drOffsetY;
		Int32 num5 = obj->x2 + drOffsetX;
		Int32 num6 = obj->y2 + drOffsetY;
		Int32 num7 = obj->x3 + drOffsetX;
		Int32 num8 = obj->y3 + drOffsetY;
		__gPos[GPosIndex].Set(num, num2, GzDepth);
		VbPos[VbOffset] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num3, num4, GzDepth);
		VbPos[VbOffset + 1] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num5, num6, GzDepth);
		VbPos[VbOffset + 2] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num7, num8, GzDepth);
		VbPos[VbOffset + 3] = __gPos[GPosIndex++];
		Single num9 = 0f;
		Single num10 = 240f;
		if ((_key >> 16 & 31u) != 0u)
		{
			num9 = 128f;
		}
		__gTex[GTexIndex].Set(obj->u0 + num9, num10 - obj->v0);
		VbTex[VbOffset] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u1 + num9, num10 - obj->v1);
		VbTex[VbOffset + 1] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u2 + num9, num10 - obj->v2);
		VbTex[VbOffset + 2] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u3 + num9, num10 - obj->v3);
		VbTex[VbOffset + 3] = __gTex[GTexIndex++];
		Byte a = _alpha;
		__gCol[GColIndex].r = obj->r0;
		__gCol[GColIndex].g = obj->g0;
		__gCol[GColIndex].b = obj->b0;
		__gCol[GColIndex].a = a;
		VbCol[VbOffset] = __gCol[GColIndex++];
		__gCol[GColIndex].r = obj->r1;
		__gCol[GColIndex].g = obj->g1;
		__gCol[GColIndex].b = obj->b1;
		__gCol[GColIndex].a = a;
		VbCol[VbOffset + 1] = __gCol[GColIndex++];
		__gCol[GColIndex].r = obj->r2;
		__gCol[GColIndex].g = obj->g2;
		__gCol[GColIndex].b = obj->b2;
		__gCol[GColIndex].a = a;
		VbCol[VbOffset + 2] = __gCol[GColIndex++];
		__gCol[GColIndex].r = obj->r3;
		__gCol[GColIndex].g = obj->g3;
		__gCol[GColIndex].b = obj->b3;
		__gCol[GColIndex].a = a;
		VbCol[VbOffset + 3] = __gCol[GColIndex++];
		IbIndex[IbOffset++] = VbOffset;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 2;
		IbIndex[IbOffset++] = VbOffset + 2;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 3;
		VbOffset += 4;
	}

	public unsafe void Sprite(PSX_LIBGPU.SPRT* obj, Int32 w, Int32 h)
	{
		Single num = obj->x0 + drOffsetX;
		Single num2 = obj->y0 + drOffsetY;
		Single num3 = w - 1f;
		Single num4 = h - 1f;
		__gPos[GPosIndex].Set(num, num2, GzDepth);
		VbPos[VbOffset] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num + w, num2, GzDepth);
		VbPos[VbOffset + 1] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num, num2 + h, GzDepth);
		VbPos[VbOffset + 2] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num + w, num2 + h, GzDepth);
		VbPos[VbOffset + 3] = __gPos[GPosIndex++];
		__gTex[GTexIndex].Set(obj->u0, obj->v0);
		VbTex[VbOffset] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u0 + num3, obj->v0);
		VbTex[VbOffset + 1] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u0, obj->v0 + num4);
		VbTex[VbOffset + 2] = __gTex[GTexIndex++];
		__gTex[GTexIndex].Set(obj->u0 + num3, obj->v0 + num4);
		VbTex[VbOffset + 3] = __gTex[GTexIndex++];
		__gCol[GColIndex].r = obj->r0;
		__gCol[GColIndex].g = obj->g0;
		__gCol[GColIndex].b = obj->b0;
		__gCol[GColIndex].a = GetShadeAlpha(obj->code);
		VbCol[VbOffset] = (VbCol[VbOffset + 1] = (VbCol[VbOffset + 2] = (VbCol[VbOffset + 3] = __gCol[GColIndex++])));
		IbIndex[IbOffset++] = VbOffset;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 2;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 3;
		IbIndex[IbOffset++] = VbOffset + 2;
		VbOffset += 4;
	}

	public unsafe void LineF2(PSX_LIBGPU.LINE_F2* obj)
	{
		__gPos[GPosIndex].Set(obj->x0 + drOffsetX, obj->y0 + drOffsetY, GzDepth);
		VbPos[VbOffset] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(obj->x1 + drOffsetX, obj->y1 + drOffsetY, GzDepth);
		VbPos[VbOffset + 1] = __gPos[GPosIndex++];
		__gCol[GColIndex].r = obj->r0;
		__gCol[GColIndex].g = obj->g0;
		__gCol[GColIndex].b = obj->b0;
		__gCol[GColIndex].a = _alpha;
		VbCol[VbOffset] = (VbCol[VbOffset + 1] = __gCol[GColIndex++]);
		IbIndex[IbOffset++] = VbOffset;
		IbIndex[IbOffset++] = VbOffset + 1;
		VbOffset += 2;
	}

	public unsafe void LineG2(PSX_LIBGPU.LINE_G2* obj)
	{
		__gPos[GPosIndex].Set(obj->x0 + drOffsetX, obj->y0 + drOffsetY, GzDepth);
		VbPos[VbOffset] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(obj->x1 + drOffsetX, obj->y1 + drOffsetY, GzDepth);
		VbPos[VbOffset + 1] = __gPos[GPosIndex++];
		__gCol[GColIndex].r = obj->r0;
		__gCol[GColIndex].g = obj->g0;
		__gCol[GColIndex].b = obj->b0;
		__gCol[GColIndex].a = _alpha;
		VbCol[VbOffset] = __gCol[GColIndex++];
		__gCol[GColIndex].r = obj->r1;
		__gCol[GColIndex].g = obj->g1;
		__gCol[GColIndex].b = obj->b1;
		__gCol[GColIndex].a = _alpha;
		VbCol[VbOffset + 1] = __gCol[GColIndex++];
		IbIndex[IbOffset++] = VbOffset;
		IbIndex[IbOffset++] = VbOffset + 1;
		VbOffset += 2;
	}

	public unsafe void Tile(PSX_LIBGPU.TILE* obj, Int32 w, Int32 h)
	{
		Int32 num = obj->x0 + drOffsetX;
		Int32 num2 = obj->y0 + drOffsetY;
		__gPos[GPosIndex].Set(num, num2, GzDepth);
		VbPos[VbOffset] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num + w, num2, GzDepth);
		VbPos[VbOffset + 1] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num, num2 + h, GzDepth);
		VbPos[VbOffset + 2] = __gPos[GPosIndex++];
		__gPos[GPosIndex].Set(num + w, num2 + h, GzDepth);
		VbPos[VbOffset + 3] = __gPos[GPosIndex++];
		__gCol[GColIndex].r = obj->r0;
		__gCol[GColIndex].g = obj->g0;
		__gCol[GColIndex].b = obj->b0;
		__gCol[GColIndex].a = _alpha;
		VbCol[VbOffset] = (VbCol[VbOffset + 1] = (VbCol[VbOffset + 2] = (VbCol[VbOffset + 3] = __gCol[GColIndex++])));
		IbIndex[IbOffset++] = VbOffset;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 2;
		IbIndex[IbOffset++] = VbOffset + 1;
		IbIndex[IbOffset++] = VbOffset + 3;
		IbIndex[IbOffset++] = VbOffset + 2;
		VbOffset += 4;
	}

	public Byte GetShadeAlpha(Byte code)
	{
		if ((code & 1) != 0)
		{
			return _alpha;
		}
		Int32 num = _alpha << 1;
		if (num > 255)
		{
			return Byte.MaxValue;
		}
		return (Byte)num;
	}

	public static readonly Byte[] AbrAlphaData = {
		63,
		127,
		127,
		31
	};

	public static Color[] ColorData;
	public static Single GzDepth;

	private static Single HALF_PIXEL = 0.5f;
	private static Single INV_HALF_PIXEL = 0f;
	private static Int32 INDICES_MAX = 21000;
	private static Int32 VERTICES_MAX = 14000;
	private static Shader[] __shaders;
	private static Int32 POS_MAX = 22000;
	private static Int32 TEX_MAX = 19300;
	private static Int32 COL_MAX = 10000;

	public static Int32 GPosIndex;
	public static Int32 GTexIndex;
	public static Int32 GColIndex;
	
	private static Vector3[] __gPos;
	private static Vector2[] __gTex;
	private static Color32[] __gCol;
	private static Material __dummyMaterial;

	private UInt32 _key;
	private Byte _alpha;
	private readonly Mesh _mesh;
	private readonly Material _material;

	public Int32[] IbIndex;
	public Vector3[] VbPos;
	public Color32[] VbCol;
	public Vector2[] VbTex;
	public Int32 VbOffset;
	public Int32 IbOffset;

	private UInt32 _shaderIndex;
	private Vector4 _constTexParam;
	private static readonly Int32 Threshold = Shader.PropertyToID("_Threshold");
	private static readonly Int32 Color = Shader.PropertyToID("_Color");
	private static readonly Int32 TexParam = Shader.PropertyToID("_TexParam");
}
