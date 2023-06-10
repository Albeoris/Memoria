using System;
using System.Collections.Generic;
using UnityEngine;
using Memoria.Data;
using Memoria.Prime;

public class SFXRender
{
	public static void Init()
	{
		SFXKey.currentTexPage = 0u;
		SFXKey.currentTexABR = 0u;
		SFXRender.meshOrigin = new SFXMesh[SFXRender.MESH_MAX];
		for (Int32 i = 0; i < (Int32)SFXRender.meshOrigin.Length; i++)
			SFXRender.meshOrigin[i] = new SFXMesh();
		SFXRender.commandBuffer = new List<SFXMeshBase>();
		SFXRender.exportSFXDataMesh = new Dictionary<UInt32, SFXDataMeshConverter>();
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
		SFXRender.exportSFXDataMesh.Clear();
		SFXRender.exportSFXDataMesh = null;
	}

	public unsafe static void Update()
	{
		SFXRender.commandBuffer.Clear();
		if (SFX.SFX_BeginRender())
		{
			SFXRender.primCount = 0;
			SFXMeshBase.drOffsetX = CalculateWidescreenOffsetX();
			SFXMeshBase.drOffsetY = 0;
			SFXRender.meshEmpty = new List<SFXMesh>(SFXRender.meshOrigin);
			for (Int32 i = 0; i < SFXRender.MESH_MAX; i++)
				SFXRender.meshEmpty[i].Begin();
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
			if (SFX.isDebugPrintCode)
				Log.Message("[SFX] SFX_GetPrim (start)");
			for (; ; )
			{
				Int32 num = 0;
				PSX_LIBGPU.P_TAG* ptr = (PSX_LIBGPU.P_TAG*)SFX.SFX_GetPrim(ref num);
				if (ptr == null)
					break;
				SFXMesh.GzDepth = -num;
				SFXRender.Add(ptr);
				SFXRender.primCount++;
			}
			if (SFX.isDebugPrintCode)
				Log.Message($"[SFX] SFX_GetPrim (end:{SFXRender.primCount})");
			SFXRender.PushCommandBuffer();
			if (SFX.IsDebugMesh || SFX.IsDebugObjMesh)
			{
				List<SFXMesh> currMeshes = new List<SFXMesh>();
				for (Int32 i = 0; i < SFXRender.commandBuffer.Count; i++)
					if (SFXRender.commandBuffer[i] is SFXMesh)
					{
						currMeshes.Add(SFXRender.commandBuffer[i] as SFXMesh);
						//currMeshes[currMeshes.Count - 1].TryFixDepth();
					}
				if (SFX.IsDebugObjMesh)
					SFXMesh.ExportObjPacked(currMeshes, PSXTextureMgr.GetDebugExportPath() + "meshfull_" + SFX.frameIndex);
				if (SFX.IsDebugMesh)
				{
					for (Int32 i = 0; i < currMeshes.Count; i++)
					{
						SFXMesh mesh = currMeshes[i];
						if (!SFXRender.exportSFXDataMesh.ContainsKey(mesh._key))
						{
							SFXDataMeshConverter newExportMesh = new SFXDataMeshConverter();
							newExportMesh.key = mesh._key;
							newExportMesh.texturePath = SFXKey.GetTextureMode(mesh._key) != 2u ? "texture" + mesh._key.ToString("X8") + ".png" : "";
							SFXRender.exportSFXDataMesh[mesh._key] = newExportMesh;
						}
						SFXDataMeshConverter exportMesh = SFXRender.exportSFXDataMesh[mesh._key];
						SFXDataMeshConverter.Frame dataMesh = new SFXDataMeshConverter.Frame(exportMesh);
						dataMesh.ConvertFromMesh(mesh);
						exportMesh.frameMesh[SFX.frameIndex] = dataMesh;
					}
				}
			}
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
		SFXDataMesh.SFXColor = null;
		for (Int32 i = 0; i < SFXRender.commandBuffer.Count; i++)
			SFXRender.commandBuffer[i].Render(i);
		SFXMesh.DummyRender();
		camera.worldToCameraMatrix = worldToCameraMatrix;
	}

	public static void SaveSFXDataMeshes()
	{
		if (SFXRender.exportSFXDataMesh.Count == 0)
			return;
		Int32 lastFrame = 0;
		foreach (KeyValuePair<UInt32, SFXDataMeshConverter> entry in SFXRender.exportSFXDataMesh)
		{
			UInt32 key = entry.Key;
			SFXDataMeshConverter data = entry.Value;
			foreach (Int32 sfxFrameNum in data.frameMesh.Keys)
				if (lastFrame < sfxFrameNum)
					lastFrame = sfxFrameNum;
			if (false) // last 2 saves: from camera above with units far away (y) and bone positions artificially splitted (x), find where pieces are placed wrt. caster and target
			{
				data.SwitchToWorldCoordinates();
				foreach (SFXDataMeshConverter.Frame sfxFrame in data.frameMesh.Values)
				{
					for (Int32 i = 0; i < sfxFrame.vertex.Length; i++)
					{
						Boolean isAtCaster = sfxFrame.vertex[i].z < -20000;
						Boolean isAtTarget = sfxFrame.vertex[i].z > 20000;
						Boolean isAtMonBone0 = sfxFrame.vertex[i].x > 20000;
						Boolean isAtMonBone1 = sfxFrame.vertex[i].y > 20000;
						Boolean isAtRoot = sfxFrame.vertex[i].y < -20000;
						Boolean isAtAvg = sfxFrame.vertex[i].x < -20000;
						Boolean isAtWeapon = sfxFrame.vertex[i].y > 20000 && sfxFrame.vertex[i].x > 20000;
						Boolean isAtTarBone = sfxFrame.vertex[i].y < -20000 && sfxFrame.vertex[i].x < -20000;
						Byte pos = (Byte)(isAtCaster ? 1 : isAtTarget ? 2 : 0);
						SByte bpos = (SByte)(isAtWeapon ? -4 : isAtTarBone ? -3 : isAtMonBone0 ? -5 : isAtMonBone1 ? -6 : isAtAvg ? -2 : isAtRoot ? 0 : -1);
						if (data.positionTypeUnit != 0 && pos != data.positionTypeUnit)
							data.positionTypeUnit = 3;
						else
							data.positionTypeUnit = pos;
						if (pos == 1 && data.positionTypeCasterBone == SByte.MinValue)
							data.positionTypeCasterBone = bpos;
						if (pos == 2 && data.positionTypeTargetBone == SByte.MinValue)
							data.positionTypeTargetBone = bpos;
					}
				}
			}
			if (false) // 4th save: glue "connected components" and convert absolute vertex positions to caster/target relative positions
			{
				data.TryFixDepth();
				data.SwitchToWorldCoordinates();
				//data.ComputePositionInformations();
			}
			//if (File.Exists(outputFName))
			//{
			//	SFXDataMeshConverter previousCaption = new SFXDataMeshConverter();
			//	previousCaption.Import(File.ReadAllBytes(outputFName));
			//	previousCaption.UpdateWithNewCaption(data, true, false, false);
			//	previousCaption.Export(outputFName);
			//}
			//else
			//{
			//	data.Export(outputFName);
			//}
		}
		SFXDataMeshConverter.ExportPositionType(SFXRender.exportSFXDataMesh.Values, PSXTextureMgr.GetDebugExportPath() + "MeshPositions.txt");
		String outputFName = PSXTextureMgr.GetDebugExportPath() + "Mesh" + UnifiedBattleSequencer.EXTENSION_SFXMESH_RAW;
		//if (File.Exists(outputFName))
		//{
		//	List<SFXDataMeshConverter> previousCaption = SFXDataMeshConverter.Import(File.ReadAllText(outputFName));
		//	SFXDataMeshConverter.Export(previousCaption, outputFName + "2");
		//}
		SFXDataMeshConverter.ExportAsSFXModel(SFXRender.exportSFXDataMesh.Values, outputFName);
		//for (Int32 i = 0; i < lastFrame; i++)
		//	SFXDataMeshConverter.ExportAsObj(PSXTextureMgr.GetDebugExportPath() + "meshfull_" + i, SFXRender.exportSFXDataMesh.Values, i);
		SFXRender.exportSFXDataMesh.Clear();
	}

	public unsafe static void Add(PSX_LIBGPU.P_TAG* tag)
	{
		switch (tag->code & 252)
		{
			case 32:
			{
				UInt32 meshKey = SFXKey.GetCurrentABR(tag->code);
				SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
				mesh.PolyF3((PSX_LIBGPU.POLY_F3*)tag);
				return;
			}
			case 36:
				//if (SFX.currentEffectID != SpecialEffect.Stop)
				SFXRender.PolyFT3((PSX_LIBGPU.POLY_FT3*)tag);
				return;
			case 40:
			{
				SFXRender.PolyF4((PSX_LIBGPU.POLY_F4*)tag);
				return;
			}
			case 44:
				SFXRender.PolyFT4((PSX_LIBGPU.POLY_FT4*)tag, 0u);
				return;
			case 48:
			{
				UInt32 meshKey = SFXKey.GetCurrentABR(tag->code);
				SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
				mesh.PolyG3((PSX_LIBGPU.POLY_G3*)tag);
				return;
			}
			case 52:
				SFXRender.PolyGT3((PSX_LIBGPU.POLY_GT3*)tag);
				return;
			case 56:
			{
				SFXRender.PolyG4((PSX_LIBGPU.POLY_G4*)tag);
				return;
			}
			case 60:
				//if (SFX.currentEffectID != SpecialEffect.Slow && SFX.currentEffectID != SpecialEffect.Stop && SFX.currentEffectID != SpecialEffect.Rippler)
				SFXRender.PolyGT4((PSX_LIBGPU.POLY_GT4*)tag);
				return;
			case 64:
			{
				UInt32 meshKey = SFXKey.GetCurrentABR(tag->code) | SFXKey.LINE_POLYGON;
				SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
				mesh.LineF2((PSX_LIBGPU.LINE_F2*)tag);
				return;
			}
			case 68:
				SFXRender.PolyBFT4((PSX_LIBGPU.POLY_FT4*)tag);
				return;
			case 72:
				SFXRender.PolyBGT4((PSX_LIBGPU.POLY_GT4*)tag);
				return;
			case 76:
				SFXRender.PolyFT4((PSX_LIBGPU.POLY_FT4*)tag, SFXKey.FILLTER_POINT);
				return;
			case 80:
			{
				UInt32 meshKey = SFXKey.GetCurrentABR(tag->code) | SFXKey.LINE_POLYGON;
				SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
				mesh.LineG2((PSX_LIBGPU.LINE_G2*)tag);
				return;
			}
			case 96:
				SFXRender.TILE((PSX_LIBGPU.TILE*)tag, (Int32)((PSX_LIBGPU.TILE*)tag)->w, (Int32)((PSX_LIBGPU.TILE*)tag)->h);
				return;
			case 100:
				SFXRender.SPRT((PSX_LIBGPU.SPRT*)tag, (Int32)((PSX_LIBGPU.SPRT*)tag)->w, (Int32)((PSX_LIBGPU.SPRT*)tag)->h);
				return;
			case 104:
				SFXRender.TILE((PSX_LIBGPU.TILE*)tag, 1, 1);
				return;
			case 112:
				SFXRender.TILE((PSX_LIBGPU.TILE*)tag, 8, 8);
				return;
			case 116:
				SFXRender.SPRT((PSX_LIBGPU.SPRT*)tag, 8, 8);
				return;
			case 120:
				SFXRender.TILE((PSX_LIBGPU.TILE*)tag, 16, 16);
				return;
			case 124:
				SFXRender.SPRT((PSX_LIBGPU.SPRT*)tag, 16, 16);
				return;
		}
		switch (tag->code)
		{
		case 225:
			if (tag->getLen() == 1u)
				SFXRender.DR_TPAGE((PSX_LIBGPU.DR_TPAGE*)tag);
			else
				SFXRender.DR_TPAGE((PSX_LIBGPU.DR_TPAGE*)tag);
			break;
		case 228:
			SFXRender.DR_AREA((PSX_LIBGPU.DR_AREA*)tag);
			break;
		case 229:
			SFXRender.DR_OFFSET((PSX_LIBGPU.DR_OFFSET*)tag);
			break;
		case 231:
			//if (SFX.currentEffectID != SpecialEffect.Rippler && SFX.currentEffectID != SpecialEffect.Slow)
			SFXRender.DR_MOVE((PSX_LIBGPU.DR_MOVE*)tag);
			break;
		}
		return;
	}

	private static unsafe void PolyG4(PSX_LIBGPU.POLY_G4* tag)
	{
		UInt32 meshKey = SFXKey.GetCurrentABR(tag->code);
		SFXRender.FixWidescreenFace(meshKey, ref tag->x0, ref tag->x1, ref tag->x2, ref tag->x3, tag->y0, tag->y1, tag->y2, tag->y3);
		SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
		mesh.PolyG4((PSX_LIBGPU.POLY_G4*)tag);
	}

	private static unsafe void PolyF4(PSX_LIBGPU.POLY_F4* tag)
	{
		UInt32 meshKey = SFXKey.GetCurrentABR(tag->code);
		SFXRender.FixWidescreenFace(meshKey, ref tag->x0, ref tag->x1, ref tag->x2, ref tag->x3, tag->y0, tag->y1, tag->y2, tag->y3);
		SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
		mesh.PolyF4(tag);
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
		SFXRender.FixWidescreenFace(abrtex, ref tag->x0, ref tag->x1, ref tag->x2, ref tag->x3, tag->y0, tag->y1, tag->y2, tag->y3);
		SFXMesh mesh = SFXRender.GetMesh(abrtex, tag->code);
		mesh.PolyGt4(tag);
	}

	private unsafe static void PolyFT4(PSX_LIBGPU.POLY_FT4* tag, UInt32 fillter = 0u)
	{
		UInt32 meshKey = SFXKey.GetABRTex(tag->code, tag->clut, tag->tpage) | fillter;
		SFXRender.FixWidescreenFace(meshKey, ref tag->x0, ref tag->x1, ref tag->x2, ref tag->x3, tag->y0, tag->y1, tag->y2, tag->y3);
		SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
		mesh.PolyFt4(tag);
	}

	private unsafe static void PolyBFT4(PSX_LIBGPU.POLY_FT4* tag)
	{
		UInt32 meshKey = SFXKey.GetABRTex(tag->code, tag->clut, tag->tpage) | SFXKey.FILLTER_BILINEAR | SFXKey.FULL_BLUR_TXTURE;
		SFXRender.FixWidescreenFace(meshKey, ref tag->x0, ref tag->x1, ref tag->x2, ref tag->x3, tag->y0, tag->y1, tag->y2, tag->y3);
		SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
		mesh.PolyBft4(tag);
	}

	private unsafe static void PolyBGT4(PSX_LIBGPU.POLY_GT4* tag)
	{
		UInt32 meshKey = SFXKey.GetABRTex(tag->code, tag->clut, tag->tpage) | SFXKey.FILLTER_BILINEAR | SFXKey.FULL_BLUR_TXTURE;
		SFXRender.FixWidescreenFace(meshKey, ref tag->x0, ref tag->x1, ref tag->x2, ref tag->x3, tag->y0, tag->y1, tag->y2, tag->y3);
		SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
		mesh.PolyBgt4(tag);
	}

	private unsafe static void SPRT(PSX_LIBGPU.SPRT* tag, Int32 w, Int32 h)
	{
		if (w == 80 && h == 110 && (tag->y0 == 0 || tag->y0 == 110) && (tag->x0 == 0 || tag->x0 == 80 || tag->x0 == 160 || tag->x0 == 240))
		{
			Int16 rightx = (Int16)(tag->x0 == 0 ? 80 :
								   tag->x0 == 80 ? 160 :
								   tag->x0 == 160 ? 240 : 320);
			Int16 widescreenOffset = (Int16)CalculateWidescreenOffsetX();
			Single aspectRatioDiffMultiplier = FieldMap.PsxScreenWidth / (Single)FieldMap.PsxScreenWidthNative;
			w = (Int32)(w * aspectRatioDiffMultiplier);
			tag->x0 = (Int16)(tag->x0 * aspectRatioDiffMultiplier - widescreenOffset);
			if (rightx == 320 || tag->x0 + w < (Int16)(rightx * aspectRatioDiffMultiplier - widescreenOffset))
				w++;
		}
		UInt32 meshKey = SFXKey.GetCurrentABRTex(tag->code, tag->clut) | SFXKey.FILLTER_BILINEAR;
		SFXMesh mesh = SFXRender.GetMesh(meshKey, tag->code);
		mesh.Sprite(tag, w, h);
	}

	private unsafe static void TILE(PSX_LIBGPU.TILE* tag, Int32 w, Int32 h)
	{
		// Fixes some instances of screen flashes (e.g. white screen when summoning Ifrit)
		if (w == 80 && h == 110 && (tag->y0 == 0 || tag->y0 == 110) && (tag->x0 == 0 || tag->x0 == 80 || tag->x0 == 160 || tag->x0 == 240))
		{
			Int16 rightx = (Int16)(tag->x0 == 0 ? 80 :
								   tag->x0 == 80 ? 160 :
								   tag->x0 == 160 ? 240 : 320);
			Int16 widescreenOffset = (Int16)CalculateWidescreenOffsetX();
			Single aspectRatioDiffMultiplier = FieldMap.PsxScreenWidth / (Single)FieldMap.PsxScreenWidthNative;
			w = (Int32)(w * aspectRatioDiffMultiplier);
			tag->x0 = (Int16)(tag->x0 * aspectRatioDiffMultiplier - widescreenOffset);
			if (rightx == 320 || tag->x0 + w < (Int16)(rightx * aspectRatioDiffMultiplier - widescreenOffset))
			{
				// Need to increment w to make up for rounding losses
				w++;
			}
		}
		UInt32 currentABR = SFXKey.GetCurrentABR(tag->code);
		SFXMesh mesh = SFXRender.GetMesh(currentABR, tag->code);
		mesh.Tile(tag, w, h);
	}

	private unsafe static void DR_TPAGE(PSX_LIBGPU.DR_TPAGE* obj)
	{
		SFXKey.SetCurrentTPage((UInt16)(obj->code[0] & 0xFFFFu));
	}

	private unsafe static void DR_OFFSET(PSX_LIBGPU.DR_OFFSET* obj)
	{
		SFXMeshBase.drOffsetX = (Int32)(obj->code[1] & 0xFFFFu) + CalculateWidescreenOffsetX();
		SFXMeshBase.drOffsetY = (Int32)(obj->code[1] >> 16);
	}

	private unsafe static void DR_AREA(PSX_LIBGPU.DR_AREA* obj)
	{
		SFXRender.PushCommandBuffer();
		Int32 num = (Int32)(obj->code[1] >> 16);
		if (num != 0)
		{
			SFXMeshBase.drOffsetX -= PSXTextureMgr.GEN_TEXTURE_X + CalculateWidescreenOffsetX();
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
		Int32 rx = (Int32)(obj->code[1] >> 16);
		Int32 ry = (Int32)((Int16)(obj->code[1] & 0xFFFFu));
		Int32 rw = (Int32)(obj->code[2] >> 16);
		Int32 rh = (Int32)(obj->code[2] & 0xFFFFu);
		Int32 x = (Int32)(obj->code[3] >> 16);
		Int32 y = (Int32)(obj->code[3] & 0xFFFFu);
		SFXRender.PushCommandBuffer();
		if (rx < 320 && ry < 240)
		{
			if (SFX.currentEffectID == SpecialEffect.Slow)
				ry -= 128;
			SFXRender.commandBuffer.Add(new SFXScreenShot(rx, ry, x, y));
		}
		else
		{
			SFXRender.commandBuffer.Add(new SFXMoveImage(rx, ry, rw, rh, x, y));
		}
	}

	private static void PushCommandBuffer()
	{
		if (SFX.subOrder == 0)
			SFXRender.PushCommandBufferSub();
		if (SFX.addOrder == 0)
			SFXRender.PushCommandBufferOpa();
		else
			SFXRender.PushCommandBufferAdd();
		if (SFX.subOrder == 1)
			SFXRender.PushCommandBufferSub();
		if (SFX.addOrder == 0)
			SFXRender.PushCommandBufferAdd();
		else
			SFXRender.PushCommandBufferOpa();
		if (SFX.subOrder == 2)
			SFXRender.PushCommandBufferSub();
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
		if (SFXKey.isLinePolygon(meshKey))
		{
			if (blendMode == 0)
				list = SFXRender.meshLineOpa;
			else if (blendMode == 1)
				list = SFXRender.meshLineAdd;
			else
				list = SFXRender.meshLineSub;
		}
		else if (SFXKey.IsTexture(meshKey))
		{
			if (blendMode == 0)
				list = SFXRender.meshTexOpa;
			else if (blendMode == 1)
			{
				UInt32 filter = SFXKey.GetFilter(meshKey);
				list = filter == SFXKey.FILLTER_POINT ? SFXRender.meshTexAddPS
					 : filter == SFXKey.FILLTER_BILINEAR ? SFXRender.meshTexAddBL
					 : SFX.isDebugFillter ? SFXRender.meshTexAddBL : SFXRender.meshTexAddPS;
			}
			else
				list = SFXRender.meshTexSub;
		}
		else
		{
			if (blendMode == 0)
				list = SFXRender.meshOpa;
			else if (blendMode == 1)
				list = SFXRender.meshAdd;
			else
				list = SFXRender.meshSub;
		}
		foreach (SFXMesh sfxmesh in list)
			if (sfxmesh.GetKey() == meshKey)
				return sfxmesh;
		SFXMesh sfxmesh2 = SFXRender.meshEmpty[SFXRender.meshEmpty.Count - 1];
		SFXRender.meshEmpty.RemoveAt(SFXRender.meshEmpty.Count - 1);
		list.Add(sfxmesh2);
		sfxmesh2.Setup(meshKey, code);
		return sfxmesh2;
	}

	public static Int32 GetRenderPriority(UInt32 meshKey)
	{
		// (SFX.subOrder, SFX.addOrder)
		// (0, 0) => Sub Opa Add
		// (1, 0) => Opa Sub Add
		// (2, 0) => Opa Add Sub
		// (0, 1) => Sub Add Opa
		// (1, 1) => Add Sub Opa
		// (2, 1) => Add Opa Sub
		UInt32 blendMode = SFXKey.GetBlendMode(meshKey);
		Int32 priority = 0;
		if (SFXKey.isLinePolygon(meshKey))
			priority += blendMode == 1 ? 3 : 2;
		else if (SFXKey.IsTexture(meshKey))
			priority += blendMode == 1 && (SFXKey.GetFilter(meshKey) == SFXKey.FILLTER_BILINEAR || SFX.isDebugFillter) ? 2 : 1;
		if (blendMode == 0) // Opa
		{
			if (SFX.subOrder == 0 || (SFX.subOrder == 1 && SFX.addOrder == 1)) // Sub comes before Opa
				priority += 3;
			if (SFX.addOrder == 1) // Add comes before Opa
				priority += 4;
		}
		else if (blendMode == 1) // Add
		{
			if (SFX.subOrder == 0 || (SFX.subOrder == 1 && SFX.addOrder == 0)) // Sub comes before Add
				priority += 3;
			if (SFX.addOrder == 0) // Opa comes before Add
				priority += 3;
		}
		else // Sub
		{
			if (SFX.subOrder == 2 || (SFX.subOrder == 1 && SFX.addOrder == 1)) // Add comes before Sub
				priority += 4;
			if (SFX.subOrder == 2 || (SFX.subOrder == 1 && SFX.addOrder == 0)) // Opa comes before Sub
				priority += 3;
		}
		return priority;
	}

	private static void FixWidescreenFace(UInt32 meshKey, ref Int16 x0, ref Int16 x1, ref Int16 x2, ref Int16 x3, Int16 y0, Int16 y1, Int16 y2, Int16 y3)
	{
		if (!Memoria.Configuration.Graphics.WidescreenSupport)
			return;
		UInt32 textureKey = SFXKey.GetTextureKey(meshKey);
		// Enlarge blur textures, whatever the size
		if (SFXKey.GetTextureMode(meshKey) == 2u && (SFXKey.IsBlurTexture(textureKey) || (SFX.currentEffectID == SpecialEffect.Devour && textureKey == 0x57FFFFu)))
		{
			Int16 widescreenOffset = (Int16)CalculateWidescreenOffsetX();
			Single aspectRatioDiffMultiplier = FieldMap.PsxScreenWidth / (Single)FieldMap.PsxScreenWidthNative;
			x0 = (Int16)(x0 * aspectRatioDiffMultiplier - widescreenOffset);
			x1 = (Int16)(x1 * aspectRatioDiffMultiplier - widescreenOffset);
			x2 = (Int16)(x2 * aspectRatioDiffMultiplier - widescreenOffset);
			x3 = (Int16)(x3 * aspectRatioDiffMultiplier - widescreenOffset);
			return;
		}
		// Enlarge faces expanding over the whole screen
		if (IsFullWidthRect(x0, x1, x2, x3))
		{
			Int16 widescreenOffset = (Int16)CalculateWidescreenOffsetX();
			x0 -= widescreenOffset;
			x1 += widescreenOffset;
			x2 -= widescreenOffset;
			x3 += widescreenOffset;
			return;
		}
		// Fix specific exceptions
		if (SFX.currentEffectID == SpecialEffect.Phoenix__Full && meshKey == 0x00800000u) // Fire on the foreground
		{
			// 0x00800000u is used by many meshes in many frames, potentially with parts that are widescreens and parts that are not; they are the trickiest
			Int16 widescreenOffset = (Int16)CalculateWidescreenOffsetX();
			if (x0 == 0 && x2 == 0)
			{
				x0 -= widescreenOffset;
				x2 -= widescreenOffset;
			}
			if (x1 == 320 && x3 == 320)
			{
				x1 += widescreenOffset;
				x3 += widescreenOffset;
			}
			return;
		}
		if (SFX.currentEffectID == SpecialEffect.Phoenix_Rebirth_Flame && meshKey == 0x00800000u) // Light from above
		{
			Int16 widescreenOffset = (Int16)CalculateWidescreenOffsetX();
			if (x0 == 0 && x2 == 0 && x1 == 80 && x3 == 80)
			{
				x0 -= widescreenOffset;
				x2 -= widescreenOffset;
			}
			if (x1 == 320 && x3 == 320 && x0 == 240 && x2 == 240)
			{
				x1 += widescreenOffset;
				x3 += widescreenOffset;
			}
			return;
		}
		if (SFX.currentEffectID == SpecialEffect.Madeen__Full && meshKey == 0x00800000u) // Light from ground explosion
		{
			Int16 widescreenOffset = (Int16)CalculateWidescreenOffsetX();
			if (x0 == 0 && x2 == 0 && x1 == 40 && x3 == 40)
			{
				x0 -= widescreenOffset;
				x2 -= widescreenOffset;
			}
			if (x1 == 320 && x3 == 320 && x0 == 280 && x2 == 280)
			{
				x1 += widescreenOffset;
				x3 += widescreenOffset;
			}
			return;
		}
		if (SFX.currentEffectID == SpecialEffect.Ark__Full)
		{
			if (meshKey == 0x00800000u)
			{
				Int16 widescreenOffset = (Int16)CalculateWidescreenOffsetX();
				// Beam-like effect when the hand opens
				if (x0 == 0 && x2 == 0 && x1 == 160 && x3 == 160 && (y0 == 112 && y1 == 112 || y2 == 112 && y3 == 112))
				{
					x0 -= widescreenOffset;
					x2 -= widescreenOffset;
				}
				if (x1 == 320 && x3 == 320 && x0 == 160 && x2 == 160 && (y0 == 112 && y1 == 112 || y2 == 112 && y3 == 112))
				{
					x1 += widescreenOffset;
					x3 += widescreenOffset;
				}
				// Filter at the start of atmospheric entry
				if (x0 == 0 && x2 == 0 && y0 == y1 && y2 == y3 && (y0 % 60) == 0 && (y2 % 60) == 0)
				{
					x0 -= widescreenOffset;
					x2 -= widescreenOffset;
				}
			}
			else if (meshKey == 0x00BBBEC0u) // NASA stock image of Florida
			{
				Int16 widescreenOffset = (Int16)CalculateWidescreenOffsetX();
				Single aspectRatioDiffMultiplier = 1f + widescreenOffset / 270f; // 342f is the extreme right coordinate but it needs extension too
				x0 = (Int16)(x0 * aspectRatioDiffMultiplier - widescreenOffset);
				x1 = (Int16)(x1 * aspectRatioDiffMultiplier - widescreenOffset);
				x2 = (Int16)(x2 * aspectRatioDiffMultiplier - widescreenOffset);
				x3 = (Int16)(x3 * aspectRatioDiffMultiplier - widescreenOffset);
			}
			return;
		}
	}

	private static Boolean IsFullWidthRect(Int16 x0, Int16 x1, Int16 x2, Int16 x3)
	{
		return x0 == 0 && (x1 == 320 || x1 == 319) && x2 == 0 && (x3 == 320 || x3 == 319);
	}

	private static Int32 CalculateWidescreenOffsetX()
	{
		return (FieldMap.PsxScreenWidth - FieldMap.PsxScreenWidthNative) / 2;
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

	public static List<SFXMeshBase> commandBuffer;

	private static Dictionary<UInt32, SFXDataMeshConverter> exportSFXDataMesh;
}
