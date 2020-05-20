using System;
using System.Collections.Generic;
using UnityEngine;

public class SFXRender
{
	public static void Init()
	{
		SFXKey.currentTexPage = 0u;
		SFXKey.currentTexABR = 0u;
		SFXRender.meshOrigin = new SFXMesh[SFXRender.MESH_MAX];
		for (Int32 i = 0; i < (Int32)SFXRender.meshOrigin.Length; i++)
		{
			SFXRender.meshOrigin[i] = new SFXMesh();
		}
		SFXRender.commandBuffer = new List<SFXMeshBase>();
	}

	public static void Release()
	{
		for (Int32 i = 0; i < (Int32)SFXRender.meshOrigin.Length; i++)
		{
			SFXRender.meshOrigin[i].ClearObject();
			SFXRender.meshOrigin[i] = (SFXMesh)null;
		}
		SFXRender.meshOrigin = null;
		SFXRender.meshEmpty = null;
		SFXRender.commandBuffer = null;
		SFXRender.meshOpa = null;
		SFXRender.meshAdd = null;
		SFXRender.meshSub = null;
		SFXRender.meshTexOpa = null;
		SFXRender.meshTexAddPS = null;
		SFXRender.meshTexAddBL = null;
		SFXRender.meshTexSub = null;
		SFXRender.meshLineOpa = null;
		SFXRender.meshLineAdd = null;
		SFXRender.meshLineSub = null;
	}

	public unsafe static void Update()
	{
		SFXRender.commandBuffer.Clear();
		if (SFX.SFX_BeginRender())
		{
			SFXRender.primCount = 0;
			SFXMeshBase.drOffsetX = (FieldMap.PsxFieldWidth - 320) / 2; // Widescreen offset
			SFXMeshBase.drOffsetY = 0;
			SFXRender.meshEmpty = new List<SFXMesh>(SFXRender.meshOrigin);
			for (Int32 i = 0; i < SFXRender.MESH_MAX; i++)
			{
				SFXRender.meshEmpty[i].Begin();
			}
			SFXRender.meshOpa = new List<SFXMesh>();
			SFXRender.meshAdd = new List<SFXMesh>();
			SFXRender.meshSub = new List<SFXMesh>();
			SFXRender.meshTexOpa = new List<SFXMesh>();
			SFXRender.meshTexAddPS = new List<SFXMesh>();
			SFXRender.meshTexAddBL = new List<SFXMesh>();
			SFXRender.meshTexSub = new List<SFXMesh>();
			SFXRender.meshLineOpa = new List<SFXMesh>();
			SFXRender.meshLineAdd = new List<SFXMesh>();
			SFXRender.meshLineSub = new List<SFXMesh>();
			SFXMesh.GPosIndex = 0;
			SFXMesh.GTexIndex = 0;
			SFXMesh.GColIndex = 0;
			for (;;)
			{
				Int32 num = 0;
				PSX_LIBGPU.P_TAG* ptr = (PSX_LIBGPU.P_TAG*)((void*)SFX.SFX_GetPrim(ref num));
				if (ptr == null)
				{
					break;
				}
				SFXMesh.GzDepth = (Single)(-(Single)num);
				SFXRender.Add(ptr);
				SFXRender.primCount++;
			}
			SFXRender.PushCommandBuffer();
		}
	}

	public static void Render()
	{
		RenderTexture.active = (RenderTexture)null;
		Camera camera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
		Matrix4x4 worldToCameraMatrix = camera.worldToCameraMatrix;
		PSXTextureMgr.BeginRender();
		PSXTextureMgr.CaptureBG();
		camera.worldToCameraMatrix = Matrix4x4.identity;
		for (Int32 i = 0; i < SFXRender.commandBuffer.Count; i++)
		{
			SFXRender.commandBuffer[i].Render(i);
		}
		SFXMesh.DummyRender();
		camera.worldToCameraMatrix = worldToCameraMatrix;
	}

	public unsafe static void Add(PSX_LIBGPU.P_TAG* tag)
	{
		Int32 num = (Int32)(tag->code & 252);
		if (num == 32)
		{
			UInt32 meshKey = SFXKey.GetCurrentABR(tag->code);
			SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
			mesh.PolyF3((PSX_LIBGPU.POLY_F3*)tag);
			return;
		}
		if (num == 36)
		{
			if (SFX.currentEffectID != 149)
			{
				SFXRender.PolyFT3((PSX_LIBGPU.POLY_FT3*)tag);
			}
			return;
		}
		if (num == 40)
		{
			UInt32 meshKey = SFXKey.GetCurrentABR(tag->code);
			SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
			mesh.PolyF4((PSX_LIBGPU.POLY_F4*)tag);
			return;
		}
		if (num == 44)
		{
			SFXRender.PolyFT4((PSX_LIBGPU.POLY_FT4*)tag, 0u);
			return;
		}
		if (num == 48)
		{
			UInt32 meshKey = SFXKey.GetCurrentABR(tag->code);
			SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
			mesh.PolyG3((PSX_LIBGPU.POLY_G3*)tag);
			return;
		}
		if (num == 52)
		{
			SFXRender.PolyGT3((PSX_LIBGPU.POLY_GT3*)tag);
			return;
		}
		if (num == 56)
		{
			UInt32 meshKey = SFXKey.GetCurrentABR(tag->code);
			SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
			mesh.PolyG4((PSX_LIBGPU.POLY_G4*)tag);
			return;
		}
		if (num == 60)
		{
			if (SFX.currentEffectID != 126 && SFX.currentEffectID != 149 && SFX.currentEffectID != 395)
			{
				SFXRender.PolyGT4((PSX_LIBGPU.POLY_GT4*)tag);
			}
			return;
		}
		if (num == 64)
		{
			UInt32 meshKey = SFXKey.GetCurrentABR(tag->code) | 134217728u;
			SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
			mesh.LineF2((PSX_LIBGPU.LINE_F2*)tag);
			return;
		}
		if (num == 68)
		{
			SFXRender.PolyBFT4((PSX_LIBGPU.POLY_FT4*)tag);
			return;
		}
		if (num == 72)
		{
			SFXRender.PolyBGT4((PSX_LIBGPU.POLY_GT4*)tag);
			return;
		}
		if (num == 76)
		{
			SFXRender.PolyFT4((PSX_LIBGPU.POLY_FT4*)tag, 33554432u);
			return;
		}
		if (num == 80)
		{
			UInt32 meshKey = SFXKey.GetCurrentABR(tag->code) | 134217728u;
			SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
			mesh.LineG2((PSX_LIBGPU.LINE_G2*)tag);
			return;
		}
		if (num == 96)
		{
			SFXRender.TILE((PSX_LIBGPU.TILE*)tag, (Int32)((PSX_LIBGPU.TILE*)tag)->w, (Int32)((PSX_LIBGPU.TILE*)tag)->h);
			return;
		}
		if (num == 100)
		{
			SFXRender.SPRT((PSX_LIBGPU.SPRT*)tag, (Int32)((PSX_LIBGPU.SPRT*)tag)->w, (Int32)((PSX_LIBGPU.SPRT*)tag)->h);
			return;
		}
		if (num == 104)
		{
			SFXRender.TILE((PSX_LIBGPU.TILE*)tag, 1, 1);
			return;
		}
		if (num == 112)
		{
			SFXRender.TILE((PSX_LIBGPU.TILE*)tag, 8, 8);
			return;
		}
		if (num == 116)
		{
			SFXRender.SPRT((PSX_LIBGPU.SPRT*)tag, 8, 8);
			return;
		}
		if (num == 120)
		{
			SFXRender.TILE((PSX_LIBGPU.TILE*)tag, 16, 16);
			return;
		}
		if (num != 124)
		{
			switch (tag->code)
			{
			case 225:
				if (tag->getLen() == 1u)
				{
					SFXRender.DR_TPAGE((PSX_LIBGPU.DR_TPAGE*)tag);
				}
				else
				{
					SFXRender.DR_TPAGE((PSX_LIBGPU.DR_TPAGE*)tag);
				}
				break;
			case 228:
				SFXRender.DR_AREA((PSX_LIBGPU.DR_AREA*)tag);
				break;
			case 229:
				SFXRender.DR_OFFSET((PSX_LIBGPU.DR_OFFSET*)tag);
				break;
			case 231:
				if (SFX.currentEffectID != 395 && SFX.currentEffectID != 126)
				{
					SFXRender.DR_MOVE((PSX_LIBGPU.DR_MOVE*)tag);
				}
				break;
			}
			return;
		}
		SFXRender.SPRT((PSX_LIBGPU.SPRT*)tag, 16, 16);
	}

	private unsafe static void PolyGT3(PSX_LIBGPU.POLY_GT3* tag)
	{
		UInt32 abrtex = SFXKey.GetABRTex(tag->code, tag->clut, tag->tpage);
		SFXMesh mesh = SFXRender.GetMesh(abrtex, tag->code);
		mesh.PolyGt3(tag);
	}

	private unsafe static void PolyFT3(PSX_LIBGPU.POLY_FT3* tag)
	{
		UInt32 abrtex = SFXKey.GetABRTex(tag->code, tag->clut, tag->tpage);
		SFXMesh mesh = SFXRender.GetMesh(abrtex, tag->code);
		mesh.PolyFt3(tag);
	}

	private unsafe static void PolyGT4(PSX_LIBGPU.POLY_GT4* tag)
	{
		UInt32 abrtex = SFXKey.GetABRTex(tag->code, tag->clut, tag->tpage);
		SFXMesh mesh = SFXRender.GetMesh(abrtex, tag->code);
		mesh.PolyGt4(tag);
	}

	private unsafe static void PolyFT4(PSX_LIBGPU.POLY_FT4* tag, UInt32 fillter = 0u)
	{
		UInt32 meshKey = SFXKey.GetABRTex(tag->code, tag->clut, tag->tpage) | fillter;
		SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
		mesh.PolyFt4(tag);
	}

	private unsafe static void PolyBFT4(PSX_LIBGPU.POLY_FT4* tag)
	{
		UInt32 meshKey = SFXKey.GetABRTex(tag->code, tag->clut, tag->tpage) | 67108864u | 536870912u;
		SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
		mesh.PolyBft4(tag);
	}

	private unsafe static void PolyBGT4(PSX_LIBGPU.POLY_GT4* tag)
	{
		UInt32 meshKey = SFXKey.GetABRTex(tag->code, tag->clut, tag->tpage) | 67108864u | 536870912u;
		SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
		mesh.PolyBgt4(tag);
	}

	private unsafe static void SPRT(PSX_LIBGPU.SPRT* tag, Int32 w, Int32 h)
	{
		UInt32 meshKey = SFXKey.GetCurrentABRTex(tag->code, tag->clut) | 67108864u;
		SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
		mesh.Sprite(tag, w, h);
	}

	private unsafe static void TILE(PSX_LIBGPU.TILE* tag, Int32 w, Int32 h)
	{
		UInt32 currentABR = SFXKey.GetCurrentABR(tag->code);
		SFXMesh mesh = SFXRender.GetMesh(currentABR, tag->code);
		mesh.Tile(tag, w, h);
	}

	private unsafe static void DR_TPAGE(PSX_LIBGPU.DR_TPAGE* obj)
	{
		SFXKey.SetCurrentTPage((UInt16)(obj->code[0] & 65535u));
	}

	private unsafe static void DR_OFFSET(PSX_LIBGPU.DR_OFFSET* obj)
	{
		SFXMeshBase.drOffsetX = (Int32)(obj->code[1] & 65535u);
		SFXMeshBase.drOffsetY = (Int32)(obj->code[1] >> 16);
	}

	private unsafe static void DR_AREA(PSX_LIBGPU.DR_AREA* obj)
	{
		SFXRender.PushCommandBuffer();
		Int32 num = (Int32)(obj->code[1] >> 16);
		if (num != 0)
		{
			SFXMeshBase.drOffsetX -= PSXTextureMgr.GEN_TEXTURE_X;
			SFXMeshBase.drOffsetY -= PSXTextureMgr.GEN_TEXTURE_Y;
			SFXRender.commandBuffer.Add(new SFXRenderTextureBegin());
		}
		else
		{
			SFXRender.commandBuffer.Add(new SFXRenderTextureEnd());
		}
	}

	private unsafe static void DR_MOVE(PSX_LIBGPU.DR_MOVE* obj)
	{
		Int32 num = (Int32)(obj->code[1] >> 16);
		Int32 num2 = (Int32)((Int16)(obj->code[1] & 65535u));
		Int32 rw = (Int32)(obj->code[2] >> 16);
		Int32 rh = (Int32)(obj->code[2] & 65535u);
		Int32 x = (Int32)(obj->code[3] >> 16);
		Int32 y = (Int32)(obj->code[3] & 65535u);
		SFXRender.PushCommandBuffer();
		if (num < 320 && num2 < 240)
		{
			if (SFX.currentEffectID == 126)
			{
				num2 -= 128;
			}
			SFXRender.commandBuffer.Add(new SFXScreenShot(num, num2, x, y));
		}
		else
		{
			SFXRender.commandBuffer.Add(new SFXMoveImage(num, num2, rw, rh, x, y));
		}
	}

	private static void PushCommandBuffer()
	{
		if (SFX.subOrder == 0)
		{
			SFXRender.PushCommandBufferSub();
		}
		if (SFX.addOrder == 0)
		{
			SFXRender.PushCommandBufferOpa();
		}
		else
		{
			SFXRender.PushCommandBufferAdd();
		}
		if (SFX.subOrder == 1)
		{
			SFXRender.PushCommandBufferSub();
		}
		if (SFX.addOrder == 0)
		{
			SFXRender.PushCommandBufferAdd();
		}
		else
		{
			SFXRender.PushCommandBufferOpa();
		}
		if (SFX.subOrder == 2)
		{
			SFXRender.PushCommandBufferSub();
		}
	}

	private static void PushCommandBufferOpa()
	{
		foreach (SFXMesh sfxmesh in SFXRender.meshOpa)
		{
			sfxmesh.End();
			SFXRender.commandBuffer.Add(sfxmesh);
		}
		SFXRender.meshOpa.Clear();
		foreach (SFXMesh sfxmesh2 in SFXRender.meshTexOpa)
		{
			sfxmesh2.End();
			SFXRender.commandBuffer.Add(sfxmesh2);
		}
		SFXRender.meshTexOpa.Clear();
		foreach (SFXMesh sfxmesh3 in SFXRender.meshLineOpa)
		{
			sfxmesh3.End();
			SFXRender.commandBuffer.Add(sfxmesh3);
		}
		SFXRender.meshLineOpa.Clear();
	}

	private static void PushCommandBufferAdd()
	{
		foreach (SFXMesh sfxmesh in SFXRender.meshAdd)
		{
			sfxmesh.End();
			SFXRender.commandBuffer.Add(sfxmesh);
		}
		SFXRender.meshAdd.Clear();
		foreach (SFXMesh sfxmesh2 in SFXRender.meshTexAddPS)
		{
			sfxmesh2.End();
			SFXRender.commandBuffer.Add(sfxmesh2);
		}
		SFXRender.meshTexAddPS.Clear();
		foreach (SFXMesh sfxmesh3 in SFXRender.meshTexAddBL)
		{
			sfxmesh3.End();
			SFXRender.commandBuffer.Add(sfxmesh3);
		}
		SFXRender.meshTexAddBL.Clear();
		foreach (SFXMesh sfxmesh4 in SFXRender.meshLineAdd)
		{
			sfxmesh4.End();
			SFXRender.commandBuffer.Add(sfxmesh4);
		}
		SFXRender.meshLineAdd.Clear();
	}

	private static void PushCommandBufferSub()
	{
		foreach (SFXMesh sfxmesh in SFXRender.meshSub)
		{
			sfxmesh.End();
			SFXRender.commandBuffer.Add(sfxmesh);
		}
		SFXRender.meshSub.Clear();
		foreach (SFXMesh sfxmesh2 in SFXRender.meshTexSub)
		{
			sfxmesh2.End();
			SFXRender.commandBuffer.Add(sfxmesh2);
		}
		SFXRender.meshTexSub.Clear();
		foreach (SFXMesh sfxmesh3 in SFXRender.meshLineSub)
		{
			sfxmesh3.End();
			SFXRender.commandBuffer.Add(sfxmesh3);
		}
		SFXRender.meshLineSub.Clear();
	}

	private static SFXMesh GetMesh(UInt32 meshKey, Byte code)
	{
		UInt32 blendMode = SFXKey.GetBlendMode(meshKey);
		List<SFXMesh> list;
		if (!SFXKey.isLinePolygon(meshKey))
		{
			if (SFXKey.IsTexture(meshKey))
			{
				UInt32 num = blendMode;
				if (num != 0u)
				{
					if (num != 1u)
					{
						list = SFXRender.meshTexSub;
					}
					else
					{
						UInt32 fillter = SFXKey.GetFilter(meshKey);
						if (fillter != 33554432u)
						{
							if (fillter != 67108864u)
							{
								list = ((!SFX.isDebugFillter) ? SFXRender.meshTexAddPS : SFXRender.meshTexAddBL);
							}
							else
							{
								list = SFXRender.meshTexAddBL;
							}
						}
						else
						{
							list = SFXRender.meshTexAddPS;
						}
					}
				}
				else
				{
					list = SFXRender.meshTexOpa;
				}
			}
			else
			{
				UInt32 num = blendMode;
				if (num != 0u)
				{
					if (num != 1u)
					{
						list = SFXRender.meshSub;
					}
					else
					{
						list = SFXRender.meshAdd;
					}
				}
				else
				{
					list = SFXRender.meshOpa;
				}
			}
		}
		else
		{
			UInt32 num = blendMode;
			if (num != 0u)
			{
				if (num != 1u)
				{
					list = SFXRender.meshLineSub;
				}
				else
				{
					list = SFXRender.meshLineAdd;
				}
			}
			else
			{
				list = SFXRender.meshLineOpa;
			}
		}
		foreach (SFXMesh sfxmesh in list)
		{
			if (sfxmesh.GetKey() == meshKey)
			{
				return sfxmesh;
			}
		}
		SFXMesh sfxmesh2 = SFXRender.meshEmpty[SFXRender.meshEmpty.Count - 1];
		SFXRender.meshEmpty.RemoveAt(SFXRender.meshEmpty.Count - 1);
		list.Add(sfxmesh2);
		sfxmesh2.Setup(meshKey, code);
		return sfxmesh2;
	}

	public static Int32 primCount;

	private static Int32 MESH_MAX = 64;

	private static SFXMesh[] meshOrigin;

	private static List<SFXMesh> meshEmpty;

	private static List<SFXMesh> meshOpa;

	private static List<SFXMesh> meshAdd;

	private static List<SFXMesh> meshSub;

	private static List<SFXMesh> meshTexOpa;

	private static List<SFXMesh> meshTexAddPS;

	private static List<SFXMesh> meshTexAddBL;

	private static List<SFXMesh> meshTexSub;

	private static List<SFXMesh> meshLineOpa;

	private static List<SFXMesh> meshLineAdd;

	private static List<SFXMesh> meshLineSub;

	private static List<SFXMeshBase> commandBuffer;
}
