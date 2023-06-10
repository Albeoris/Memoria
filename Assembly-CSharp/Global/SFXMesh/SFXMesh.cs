using System;
using System.IO;
using System.Collections.Generic;
using Memoria.Scripts;
using Memoria.Data;
using UnityEngine;

public class SFXMesh : SFXMeshBase
{
	public SFXMesh(Int32 indexCount = INDICES_MAX, Int32 vertexCount = VERTICES_MAX, Int32 texCount = VERTICES_MAX)
	{
		_key = 0u;
		_material = new Material(__shaders[0]);
		_mesh = new Mesh();
		_mesh.MarkDynamic();
		IbIndex = new Int32[indexCount];
		VbPos = new Vector3[vertexCount];
		VbCol = new Color32[vertexCount];
		VbTex = new Vector2[texCount];
		_constTexParam = new Vector4(HALF_PIXEL, HALF_PIXEL, 256f, 256f);
		faceType = new List<FaceType>();
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
		for (Int32 i = 0; i < 6; i++)
			__shaders[i] = ShadersLoader.Find(shaderNames[i]);
		__gPos = new Vector3[POS_MAX];
		for (Int32 i = 0; i < __gPos.Length; i++)
			__gPos[i] = default;
		__gTex = new Vector2[TEX_MAX];
		for (Int32 j = 0; j < __gTex.Length; j++)
			__gTex[j] = default;
		__gCol = new Color32[COL_MAX];
		for (Int32 k = 0; k < __gCol.Length; k++)
			__gCol[k] = default;
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
			__shaders[i] = null;
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
			_alpha = AbrAlphaData[SFXKey.tmpABR];
			_shaderIndex += SFXKey.GetBlendMode(meshKey);
			if (SFX.currentEffectID == SpecialEffect.Stop && _key == 0xD78000u)
				_shaderIndex = 2; // Black sprite: use SFX_SUB_GT
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
		if (SFX.currentEffectID == SpecialEffect.Detect && SFXMesh.DetectEyeFrameStart < 0 && _key == SFXMesh.DetectEyeKey)
			SFXMesh.DetectEyeFrameStart = SFX.frameIndex;
	}

	public Texture GetTexture(out PSXTextureMgr.Kind textureMode)
	{
		UInt32 textureKey = SFXKey.GetTextureKey(_key);
		if (SFXKey.GetTextureMode(_key) != 2u)
		{
			textureMode = PSXTextureMgr.Kind.IMAGE;
			if (SFX.currentEffectID == SpecialEffect.Special_Necron_Death)
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
			if ((_key & SFXKey.FULL_BLUR_TXTURE) != 0u)
			{
				_constTexParam.w = 240f;
			}
			else
			{
				_constTexParam.y = -240f;
				_constTexParam.w = -240f;
			}
			_constTexParam.z = 320f;
			textureMode = PSXTextureMgr.Kind.BLUR;
			return PSXTextureMgr.blurTexture;
		}
		if ((_key & 0x1F0000u) == PSXTextureMgr.bgKey)
		{
			textureMode = PSXTextureMgr.Kind.BACKGROUND;
			return PSXTextureMgr.bgTexture;
		}
		if (SFX.currentEffectID == SpecialEffect.Devour && textureKey == 0x57FFFFu)
		{
			_constTexParam.x = 176f;
			_constTexParam.y = 0f;
			_constTexParam.z = 480f;
			_constTexParam.w = 360f;
			textureMode = PSXTextureMgr.Kind.BLUR;
			return PSXTextureMgr.blurTexture;
		}
		if (SFX.currentEffectID == SpecialEffect.Slow && textureKey == 0x578000u)
		{
			textureMode = PSXTextureMgr.Kind.SCREENSHOT;
			return SFXScreenShot.screenshot;
		}
		if (SFX.currentEffectID == SpecialEffect.Silent_Voice && (textureKey == 0x578000u || textureKey == 0x598000u))
		{
			if (textureKey == 0x598000u)
				_constTexParam.x += 128f;
			textureMode = PSXTextureMgr.Kind.SCREENSHOT;
			return SFXScreenShot.screenshot;
		}
		if (SFX.currentEffectID == SpecialEffect.Stop && textureKey == 0x578000u)
		{
			textureMode = PSXTextureMgr.Kind.IMAGE;
			return PSXTextureMgr.GetTexture(_key).texture;
		}
		if (PSXTextureMgr.isCreateGenTexture)
		{
			_constTexParam.x = SFXKey.GetPositionX(_key) - (UInt64)PSXTextureMgr.GEN_TEXTURE_X + 0.5f;
			_constTexParam.z = PSXTextureMgr.GEN_TEXTURE_W;
			_constTexParam.w = PSXTextureMgr.GEN_TEXTURE_H;
			textureMode = PSXTextureMgr.Kind.GENERATED;
			return PSXTextureMgr.genTexture;
		}
		textureMode = PSXTextureMgr.Kind.IMAGE;
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
		faceType.Clear();
	}

	public void End()
	{
		_mesh.Clear();
		Int32[] indexArray = new Int32[IbOffset];
		Buffer.BlockCopy(IbIndex, 0, indexArray, 0, IbOffset << 2);
		_mesh.vertices = VbPos;
		_mesh.colors32 = VbCol;

		_mesh.uv = SFXKey.IsTexture(_key) ? VbTex : null;
		if (SFXScreenShot.IsSpecialSlowTexture(_key))
		{
			Array.Copy(SFXScreenShot.slowClockUV, VbTex, SFXScreenShot.slowClockUV.Length);
			for (Int32 i = 0; i < VbOffset; i++)
				VbCol[i] = new Color32(255, 255, 255, 255);
			_mesh.colors32 = VbCol;
			_mesh.uv = VbTex;
			_shaderIndex = 1;
		}

		if (SFX.isDebugLine)
			_mesh.SetIndices(indexArray, MeshTopology.Lines, 0);
		else if (SFXKey.isLinePolygon(_key))
			_mesh.SetIndices(indexArray, MeshTopology.Lines, 0);
		else
			_mesh.triangles = indexArray;
	}

	public override void Render(Int32 index)
	{
		_material.shader = __shaders[_shaderIndex];
		if (SFXKey.IsTexture(_key))
		{
			_material.mainTexture = GetTexture(out _);
			if (_material.mainTexture == null)
				return;
			UInt32 filter = SFXKey.GetFilter(_key);
			if (filter == SFXKey.FILLTER_POINT)
				_material.mainTexture.filterMode = FilterMode.Point;
			else if (filter == SFXKey.FILLTER_BILINEAR)
				_material.mainTexture.filterMode = FilterMode.Bilinear;
			else
				_material.mainTexture.filterMode = (!SFX.isDebugFillter) ? FilterMode.Point : FilterMode.Bilinear;
			_material.mainTexture.wrapMode = TextureWrapMode.Clamp;
			_material.SetVector(TexParam, _constTexParam);
		}
		else if (SFXScreenShot.IsSpecialSlowTexture(_key))
		{
			Texture2D psxtexture = PSXTextureMgr.GetTexture(1, 1, 8, 247, 0).texture;
			_material.mainTexture = psxtexture;
			if (_material.mainTexture == null)
				return;
			_material.mainTexture.filterMode = FilterMode.Point;
			_material.mainTexture.wrapMode = TextureWrapMode.Clamp;
			_material.SetVector(TexParam, new Vector4(HALF_PIXEL, HALF_PIXEL, 256f, 256f));
		}
		if (SFXDataMesh.SFXColor.HasValue)
			_material.SetColor(Color, ColorData[SFX.colIntensity] * SFXDataMesh.SFXColor.Value);
		else
			_material.SetColor(Color, ColorData[SFX.colIntensity]);
		_material.SetFloat(Threshold, (SFX.colThreshold != 0) ? 0.05f : 0.0295f);
		_material.SetPass(0);
		Graphics.DrawMeshNow(_mesh, Matrix4x4.identity);
		if (SFX.currentEffectID == SpecialEffect.Stop && SFXKey.GetTextureKey(_key) == 0x578000u)
			Graphics.DrawMeshNow(_mesh, Matrix4x4.identity); // Increase the black sprite's opacity
		if (SFX.IsDebugObjMesh)
			ExportObj(PSXTextureMgr.GetDebugExportPath() + "mesh" + _key.ToString("X8") + "_" + SFX.frameIndex);
	}

	private static String ExportObj_Header(String mtllib)
	{
		return "mtllib " + mtllib + ".mtl\n";
	}
	private static String ExportObj_Mesh(SFXMesh mesh, Int32 indexOffset, Int32 uvOffset)
	{
		Boolean isTexture = SFXKey.IsTexture(mesh._key);
		String obj = isTexture ? "usemtl " + mesh._key.ToString("X8") + "\n" : "";
		Int32 i;
		obj += "o " + mesh._key.ToString("X8") + "\n";
		for (i = 0; i < mesh.VbOffset; i++) // Colored vertices are supported by very few tools
			obj += "v " + mesh.VbPos[i].x + " " + mesh.VbPos[i].y + " " + mesh.VbPos[i].z
				 //+ " " + ((Single)mesh.VbCol[i].r / 256f) + " " + ((Single)mesh.VbCol[i].g / 256f) + " " + ((Single)mesh.VbCol[i].b / 256f)
				 + " " + (mesh.VbCol[i].r) + " " + (mesh.VbCol[i].g) + " " + (mesh.VbCol[i].b)
				 + "\n";
		if (isTexture)
			for (i = 0; i < mesh.VbOffset; i++)
				obj += "vt " + (mesh.VbTex[i].x / 256f) + " " + (mesh.VbTex[i].y / 256f) + "\n";
		if (SFXKey.isLinePolygon(mesh._key))
		{
			obj += "l";
			for (i = 0; i < mesh.IbOffset; i++)
				obj += " " + mesh.IbIndex[i] + 1;
			obj += "\n";
		}
		else
		{
			for (i = 0; i < mesh.IbOffset; i += 3)
				obj += "f " + (mesh.IbIndex[i] + indexOffset) + (isTexture ? "/" + (mesh.IbIndex[i] + uvOffset) : "")
					+ " " + (mesh.IbIndex[i + 1] + indexOffset) + (isTexture ? "/" + (mesh.IbIndex[i + 1] + uvOffset) : "")
					+ " " + (mesh.IbIndex[i + 2] + indexOffset) + (isTexture ? "/" + (mesh.IbIndex[i + 2] + uvOffset) : "") + "\n";
		}
		return obj;
	}

	public void ExportObj(String path)
	{
		String obj = ExportObj_Header(Path.GetFileName(path));
		obj += ExportObj_Mesh(this, 1, 1);
		File.WriteAllText(Path.ChangeExtension(path, ".obj"), obj);
		String pngPath = "texture" + _key.ToString("X8") + ".png";
		String mtl = "newmtl " + _key.ToString("X8") + "\nKa 1 1 1\nKd 1 1 1\nKs 0 0 0\nd 1\nmap_Ka " + pngPath + "\nmap_Kd " + pngPath + "\nmap_Ks " + pngPath + "\n";
		File.WriteAllText(Path.ChangeExtension(path, ".mtl"), mtl);
	}
	public static void ExportObjPacked(List<SFXMesh> pack, String path)
	{
		String obj = ExportObj_Header(Path.GetFileName(path));
		List<String> mat = new List<String>();
		String pngPath;
		Int32 voff = 1, uvoff = 1;
		for (Int32 i = 0; i < pack.Count; i++)
		{
			obj += ExportObj_Mesh(pack[i], voff, uvoff);
			voff += pack[i].VbOffset;
			uvoff += SFXKey.IsTexture(pack[i]._key) ? pack[i].VbOffset : 0;
			pngPath = "texture" + pack[i]._key.ToString("X8") + ".png";
			mat.Add("newmtl " + pack[i]._key.ToString("X8") + "\nKa 1 1 1\nKd 1 1 1\nKs 0 0 0\nd 1\nmap_Ka " + pngPath + "\nmap_Kd " + pngPath + "\nmap_Ks " + pngPath + "\n");
		}
		File.WriteAllText(Path.ChangeExtension(path, ".obj"), obj);
		File.WriteAllText(Path.ChangeExtension(path, ".mtl"), String.Join("\n\n", mat.ToArray()));
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
		faceType.Add(FaceType.POLY_F3);
		faceType.Add(FaceType.POLY_F3);
		faceType.Add(FaceType.POLY_F3);
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
		faceType.Add(FaceType.POLY_FT3);
		faceType.Add(FaceType.POLY_FT3);
		faceType.Add(FaceType.POLY_FT3);
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
		faceType.Add(FaceType.POLY_G3);
		faceType.Add(FaceType.POLY_G3);
		faceType.Add(FaceType.POLY_G3);
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
		faceType.Add(FaceType.POLY_GT3);
		faceType.Add(FaceType.POLY_GT3);
		faceType.Add(FaceType.POLY_GT3);
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
		faceType.Add(FaceType.POLY_F4);
		faceType.Add(FaceType.POLY_F4);
		faceType.Add(FaceType.POLY_F4);
		faceType.Add(FaceType.POLY_F4);
		faceType.Add(FaceType.POLY_F4);
		faceType.Add(FaceType.POLY_F4);
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
		faceType.Add(FaceType.POLY_FT4);
		faceType.Add(FaceType.POLY_FT4);
		faceType.Add(FaceType.POLY_FT4);
		faceType.Add(FaceType.POLY_FT4);
		faceType.Add(FaceType.POLY_FT4);
		faceType.Add(FaceType.POLY_FT4);
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
			num9 = 128f;
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
		faceType.Add(FaceType.POLY_BFT4);
		faceType.Add(FaceType.POLY_BFT4);
		faceType.Add(FaceType.POLY_BFT4);
		faceType.Add(FaceType.POLY_BFT4);
		faceType.Add(FaceType.POLY_BFT4);
		faceType.Add(FaceType.POLY_BFT4);
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
		faceType.Add(FaceType.POLY_G4);
		faceType.Add(FaceType.POLY_G4);
		faceType.Add(FaceType.POLY_G4);
		faceType.Add(FaceType.POLY_G4);
		faceType.Add(FaceType.POLY_G4);
		faceType.Add(FaceType.POLY_G4);
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
		if (SFX.currentEffectID == SpecialEffect.Detect && _key == SFXMesh.DetectEyeKey && SFXMesh.DetectEyeFrameStart >= 0)
		{
			// Make Detect's eye open; hacky fix
			Vector2 textureOffset = default(Vector2);
			Int32 eyeFrame = SFX.frameIndex - SFXMesh.DetectEyeFrameStart;
			if (eyeFrame >= SFXMesh.DetectEyeOpenFrame + 9)
				textureOffset.Set(-128, 0);
			else if (eyeFrame >= SFXMesh.DetectEyeOpenFrame + 6)
				textureOffset.Set(0, -64);
			else if (eyeFrame >= SFXMesh.DetectEyeOpenFrame + 3)
				textureOffset.Set(-128, -64);
			else if (eyeFrame >= SFXMesh.DetectEyeOpenFrame)
				textureOffset.Set(0, -128);
			VbTex[VbOffset] += textureOffset;
			VbTex[VbOffset + 1] += textureOffset;
			VbTex[VbOffset + 2] += textureOffset;
			VbTex[VbOffset + 3] += textureOffset;
		}
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
		faceType.Add(FaceType.POLY_GT4);
		faceType.Add(FaceType.POLY_GT4);
		faceType.Add(FaceType.POLY_GT4);
		faceType.Add(FaceType.POLY_GT4);
		faceType.Add(FaceType.POLY_GT4);
		faceType.Add(FaceType.POLY_GT4);
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
			num9 = 128f;
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
		faceType.Add(FaceType.POLY_BGT4);
		faceType.Add(FaceType.POLY_BGT4);
		faceType.Add(FaceType.POLY_BGT4);
		faceType.Add(FaceType.POLY_BGT4);
		faceType.Add(FaceType.POLY_BGT4);
		faceType.Add(FaceType.POLY_BGT4);
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
		faceType.Add(FaceType.SPRITE);
		faceType.Add(FaceType.SPRITE);
		faceType.Add(FaceType.SPRITE);
		faceType.Add(FaceType.SPRITE);
		faceType.Add(FaceType.SPRITE);
		faceType.Add(FaceType.SPRITE);
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
		faceType.Add(FaceType.TILE);
		faceType.Add(FaceType.TILE);
		faceType.Add(FaceType.TILE);
		faceType.Add(FaceType.TILE);
		faceType.Add(FaceType.TILE);
		faceType.Add(FaceType.TILE);
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
		faceType.Add(FaceType.LINE_F2);
		faceType.Add(FaceType.LINE_F2);
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
		faceType.Add(FaceType.LINE_G2);
		faceType.Add(FaceType.LINE_G2);
	}

	public Byte GetShadeAlpha(Byte code)
	{
		if ((code & 1) != 0)
			return _alpha;
		Int32 num = _alpha << 1;
		if (num > 255)
			return Byte.MaxValue;
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

	private static Shader[] __shaders;

	public static Int32 GPosIndex;
	public static Int32 GTexIndex;
	public static Int32 GColIndex;
	
	private static Vector3[] __gPos;
	private static Vector2[] __gTex;
	private static Color32[] __gCol;
	private static Material __dummyMaterial;

	public UInt32 _key;
	public Byte _alpha;
	public readonly Mesh _mesh;
	public readonly Material _material;

	public Int32[] IbIndex;
	public Vector3[] VbPos;
	public Color32[] VbCol;
	public Vector2[] VbTex;
	public Int32 VbOffset;
	public Int32 IbOffset;

	public UInt32 _shaderIndex;
	public Vector4 _constTexParam;
	public static readonly Int32 Threshold = Shader.PropertyToID("_Threshold");
	public static readonly Int32 Color = Shader.PropertyToID("_Color");
	public static readonly Int32 TexParam = Shader.PropertyToID("_TexParam");
	public static readonly String[] shaderNames = new String[]
	{
		"SFX_OPA_GT", "SFX_ADD_GT", "SFX_SUB_GT",
		"SFX_OPA_G", "SFX_ADD_G", "SFX_SUB_G"
	};

	public const Single HALF_PIXEL = 0.5f;
	private const Single INV_HALF_PIXEL = 0f;
	private const Int32 INDICES_MAX = 21000;
	private const Int32 VERTICES_MAX = 14000;
	private const Int32 POS_MAX = 22000;
	private const Int32 TEX_MAX = 19300;
	private const Int32 COL_MAX = 10000;

	public List<FaceType> faceType;

	public enum FaceType : Byte
	{
		POLY_F3,
		POLY_FT3,
		POLY_G3,
		POLY_GT3,
		POLY_F4,
		POLY_FT4,
		POLY_BFT4,
		POLY_G4,
		POLY_GT4,
		POLY_BGT4,
		SPRITE,
		TILE,
		LINE_F2,
		LINE_G2
	}
	public static Boolean IsFaceTypeParticle(FaceType ft)
	{
		return ft == FaceType.SPRITE || ft == FaceType.TILE;
	}
	public static Boolean IsFaceTypeLine(FaceType ft)
	{
		return ft == FaceType.LINE_F2 || ft == FaceType.LINE_G2;
	}
	public static Int32 GetFaceTypeVertexCount(FaceType ft)
	{
		if (ft <= FaceType.POLY_GT3)
			return 3;
		if (ft <= FaceType.TILE)
			return 4;
		return -1;
	}

	public const UInt32 DetectEyeKey = 0x003ABDC0u;
	public const Int32 DetectEyeOpenFrame = 20;
	public static Int32 DetectEyeFrameStart;
}
