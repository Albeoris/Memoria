using System;
using System.IO;
using Memoria.Scripts;
using UnityEngine;
using UnityEngine.Rendering;

public static class PSXGPU
{
	static PSXGPU()
	{
		PSXGPU.InitMaterial();
		PSXGPU.p1 = default(Vector3);
		PSXGPU.uv0 = default(Vector3);
		PSXGPU.uv1 = default(Vector3);
		PSXGPU.constOffset = new Vector4(-9999f, -9999f, -9999f, 0.5f);
		PSXGPU.constTexSize = new Vector4(-9999f, -9999f, 0f, 0f);
		PSXGPU.constOffsetB = new Vector4(-9999f, -9999f, -9999f, 0.5f);
		PSXGPU.constTexSizeB = new Vector4(-9999f, -9999f, 0f, 0f);
		PSXGPU.isNeedSetPass = true;
		PSXGPU.isNeedEnd = false;
		PSXGPU.blendIndex = -1;
		PSXGPU.texKey = 4294967280u;
	}

	public static Int32 GetFlagTP(UInt16 obj)
	{
		return obj >> 7 & 3;
	}

	public static Int32 GetFlagABR(UInt16 obj)
	{
		return obj >> 5 & 3;
	}

	public static Int32 GetFlagTY(UInt16 obj)
	{
		return obj >> 4 & 1;
	}

	public static Int32 GetFlagTX(UInt16 obj)
	{
		return (Int32)(obj & 15);
	}

	public static Int32 GetFlagClutY(UInt16 obj)
	{
		return obj >> 6 & 511;
	}

	public static Int32 GetFlagClutX(UInt16 obj)
	{
		return (Int32)(obj & 63);
	}

	public static Int32 GetPixelCoorTY(Int32 ty)
	{
		return ty << 8;
	}

	public static Int32 GetPixelCoorTX(Int32 tx)
	{
		return tx << 6;
	}

	public static Int32 GetPixelCoorClutY(Int32 cluty)
	{
		return cluty;
	}

	public static Int32 GetPixelCoorClutX(Int32 clutx)
	{
		return clutx << 4;
	}

	public static Boolean IsSemiTrans(Byte code)
	{
		return (code & 2) != 0;
	}

	public static Boolean IsShadeTex(Byte code)
	{
		return (code & 1) != 0;
	}

	private static void InitMaterial()
	{
		PSXGPU.matPSXShaderNoTexture = new PSXMaterial(ShadersLoader.Find("PSXShaderNoTexture"));
		PSXGPU.matPSXShaderTextureNoShading = new PSXMaterial(ShadersLoader.Find("PSXShaderTextureNoShading"));
		PSXGPU.matPSXShaderTextureShading = new PSXMaterial(ShadersLoader.Find("PSXShaderTextureShading"));
	}

	public unsafe static void exePrim(void* addr)
	{
		PSXGPU.isNeedEnd = false;
		PSXGPU.curMaterial = (PSXMaterial)null;
		switch (PSXGPU.ff9debugGetPrimType((PSX_LIBGPU.P_TAG*)addr, true))
		{
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_POLY_F3:
			PSXGPU.exePolyF3((PSX_LIBGPU.POLY_F3*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_POLY_FT3:
			PSXGPU.exePolyFT3((PSX_LIBGPU.POLY_FT3*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_POLY_G3:
			PSXGPU.exePolyG3((PSX_LIBGPU.POLY_G3*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_POLY_GT3:
			PSXGPU.exePolyGT3((PSX_LIBGPU.POLY_GT3*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_POLY_F4:
			PSXGPU.exePolyF4((PSX_LIBGPU.POLY_F4*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_POLY_FT4:
			PSXGPU.exePolyFT4((PSX_LIBGPU.POLY_FT4*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_POLY_G4:
			PSXGPU.exePolyG4((PSX_LIBGPU.POLY_G4*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_POLY_GT4:
			PSXGPU.exePolyGT4((PSX_LIBGPU.POLY_GT4*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_SPRT_8:
			PSXGPU.exeSPRT_8((PSX_LIBGPU.SPRT_8*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_SPRT_16:
			PSXGPU.exeSPRT_16((PSX_LIBGPU.SPRT_16*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_SPRT:
			PSXGPU.exeSPRT((PSX_LIBGPU.SPRT*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_TILE_1:
			PSXGPU.exeTILE_1((PSX_LIBGPU.TILE_1*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_TILE_8:
			PSXGPU.exeTILE_8((PSX_LIBGPU.TILE_8*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_TILE_16:
			PSXGPU.exeTILE_16((PSX_LIBGPU.TILE_16*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_TILE:
			PSXGPU.exeTILE((PSX_LIBGPU.TILE*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_LINE_F2:
			PSXGPU.exeLineF2((PSX_LIBGPU.LINE_F2*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_LINE_G2:
			PSXGPU.exeLineG2((PSX_LIBGPU.LINE_G2*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_LINE_F3:
			PSXGPU.exeLineF3((PSX_LIBGPU.LINE_F3*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_LINE_G3:
			PSXGPU.exeLineG3((PSX_LIBGPU.LINE_G3*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_LINE_F4:
			PSXGPU.exeLineF4((PSX_LIBGPU.LINE_F4*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_LINE_G4:
			PSXGPU.exeLineG4((PSX_LIBGPU.LINE_G4*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_TWIN:
			PSXGPU.exeDR_TWIN((PSX_LIBGPU.DR_TWIN*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_AREA:
			PSXGPU.exeDR_AREA((PSX_LIBGPU.DR_AREA*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_OFFSET:
			PSXGPU.exeDR_OFFSET((PSX_LIBGPU.DR_OFFSET*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_MODE:
			PSXGPU.exeDR_MODE((PSX_LIBGPU.DR_MODE*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_ENV:
			PSXGPU.exeDR_ENV((PSX_LIBGPU.DR_ENV*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_LOAD:
			PSXGPU.exeDR_LOAD((PSX_LIBGPU.DR_LOAD*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_TPAGE:
			PSXGPU.exeDR_TPAGE((PSX_LIBGPU.DR_TPAGE*)addr);
			break;
		case PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_STP:
			PSXGPU.exeDR_STP((PSX_LIBGPU.DR_STP*)addr);
			break;
		}
		if (PSXGPU.isNeedEnd)
		{
		}
	}

	private unsafe static void exePolyF3(PSX_LIBGPU.POLY_F3* ObjPtr)
	{
		Boolean abe = PSXGPU.IsSemiTrans(ObjPtr->code);
		Color32 color = new Color32(ObjPtr->r0, ObjPtr->g0, ObjPtr->b0, Byte.MaxValue);
		Vector3 p = new Vector3((Single)ObjPtr->x0, (Single)ObjPtr->y0, PSXGPU.zDepth);
		Vector3 vector = new Vector3((Single)ObjPtr->x1, (Single)ObjPtr->y1, PSXGPU.zDepth);
		Vector3 p2 = new Vector3((Single)ObjPtr->x2, (Single)ObjPtr->y2, PSXGPU.zDepth);
		PSXGPU.DrawPolyG3(abe, color, p, color, vector, color, p2);
	}

	private unsafe static void exePolyF4(PSX_LIBGPU.POLY_F4* ObjPtr)
	{
		Boolean abe = PSXGPU.IsSemiTrans(ObjPtr->code);
		Color32 color = new Color32(ObjPtr->r0, ObjPtr->g0, ObjPtr->b0, Byte.MaxValue);
		Vector3 p = new Vector3((Single)ObjPtr->x0, (Single)ObjPtr->y0, PSXGPU.zDepth);
		Vector3 vector = new Vector3((Single)ObjPtr->x1, (Single)ObjPtr->y1, PSXGPU.zDepth);
		Vector3 vector2 = new Vector3((Single)ObjPtr->x2, (Single)ObjPtr->y2, PSXGPU.zDepth);
		Vector3 p2 = new Vector3((Single)ObjPtr->x3, (Single)ObjPtr->y3, PSXGPU.zDepth);
		PSXGPU.DrawPolyG3(abe, color, p, color, vector, color, vector2);
		PSXGPU.DrawPolyG3(abe, color, vector2, color, vector, color, p2);
	}

	private unsafe static void exePolyG3(PSX_LIBGPU.POLY_G3* ObjPtr)
	{
		Boolean abe = PSXGPU.IsSemiTrans(ObjPtr->code);
		Color32 c = new Color32(ObjPtr->r0, ObjPtr->g0, ObjPtr->b0, Byte.MaxValue);
		Color32 c2 = new Color32(ObjPtr->r1, ObjPtr->g1, ObjPtr->b1, Byte.MaxValue);
		Color32 c3 = new Color32(ObjPtr->r2, ObjPtr->g2, ObjPtr->b2, Byte.MaxValue);
		Vector3 p = new Vector3((Single)ObjPtr->x0, (Single)ObjPtr->y0, PSXGPU.zDepth);
		Vector3 vector = new Vector3((Single)ObjPtr->x1, (Single)ObjPtr->y1, PSXGPU.zDepth);
		Vector3 p2 = new Vector3((Single)ObjPtr->x2, (Single)ObjPtr->y2, PSXGPU.zDepth);
		PSXGPU.DrawPolyG3(abe, c, p, c2, vector, c3, p2);
	}

	private unsafe static void exePolyG4(PSX_LIBGPU.POLY_G4* ObjPtr)
	{
		Boolean abe = PSXGPU.IsSemiTrans(ObjPtr->code);
		Color32 c = new Color32(ObjPtr->r0, ObjPtr->g0, ObjPtr->b0, Byte.MaxValue);
		Color32 c2 = new Color32(ObjPtr->r1, ObjPtr->g1, ObjPtr->b1, Byte.MaxValue);
		Color32 color = new Color32(ObjPtr->r2, ObjPtr->g2, ObjPtr->b2, Byte.MaxValue);
		Color32 c3 = new Color32(ObjPtr->r3, ObjPtr->g3, ObjPtr->b3, Byte.MaxValue);
		Vector3 p = new Vector3((Single)ObjPtr->x0, (Single)ObjPtr->y0, PSXGPU.zDepth);
		Vector3 vector = new Vector3((Single)ObjPtr->x1, (Single)ObjPtr->y1, PSXGPU.zDepth);
		Vector3 vector2 = new Vector3((Single)ObjPtr->x2, (Single)ObjPtr->y2, PSXGPU.zDepth);
		Vector3 p2 = new Vector3((Single)ObjPtr->x3, (Single)ObjPtr->y3, PSXGPU.zDepth);
		PSXGPU.DrawPolyG3(abe, c, p, c2, vector, color, vector2);
		PSXGPU.DrawPolyG3(abe, color, vector2, c2, vector, c3, p2);
	}

	private unsafe static void exePolyFT3(PSX_LIBGPU.POLY_FT3* p)
	{
		Boolean tge = PSXGPU.IsShadeTex(p->code);
		PSXGPU.DrawPolyGT3(PSXGPU.DRAW_BEGIN_END, p->code, p->clut, p->tpage, tge, ref p->rgbc0, ref p->xy0, ref p->uv0, ref p->rgbc0, ref p->xy1, ref p->uv1, ref p->rgbc0, ref p->xy2, ref p->uv2);
	}

	private unsafe static void exePolyFT4(PSX_LIBGPU.POLY_FT4* p)
	{
		Boolean tge = PSXGPU.IsShadeTex(p->code);
		PSXGPU.DrawPolyGT3(PSXGPU.DRAW_BEGIN, p->code, p->clut, p->tpage, tge, ref p->rgbc0, ref p->xy0, ref p->uv0, ref p->rgbc0, ref p->xy1, ref p->uv1, ref p->rgbc0, ref p->xy2, ref p->uv2);
		PSXGPU.DrawPolyGT3(PSXGPU.DRAW_END, p->code, p->clut, p->tpage, tge, ref p->rgbc0, ref p->xy2, ref p->uv2, ref p->rgbc0, ref p->xy1, ref p->uv1, ref p->rgbc0, ref p->xy3, ref p->uv3);
	}

	private unsafe static void exePolyGT3(PSX_LIBGPU.POLY_GT3* p)
	{
		PSXGPU.DrawPolyGT3(PSXGPU.DRAW_BEGIN_END, p->code, p->clut, p->tpage, false, ref p->rgbc0, ref p->xy0, ref p->uv0, ref p->rgbc1, ref p->xy1, ref p->uv1, ref p->rgbc2, ref p->xy2, ref p->uv2);
	}

	private unsafe static void exePolyGT4(PSX_LIBGPU.POLY_GT4* p)
	{
		PSXGPU.DrawPolyGT3(PSXGPU.DRAW_BEGIN, p->code, p->clut, p->tpage, false, ref p->rgbc0, ref p->xy0, ref p->uv0, ref p->rgbc1, ref p->xy1, ref p->uv1, ref p->rgbc2, ref p->xy2, ref p->uv2);
		PSXGPU.DrawPolyGT3(PSXGPU.DRAW_END, p->code, p->clut, p->tpage, false, ref p->rgbc2, ref p->xy2, ref p->uv2, ref p->rgbc1, ref p->xy1, ref p->uv1, ref p->rgbc3, ref p->xy3, ref p->uv3);
	}

	private unsafe static void exeLineF2(PSX_LIBGPU.LINE_F2* ObjPtr)
	{
		Boolean abe = PSXGPU.IsSemiTrans(ObjPtr->code);
		Color32 color = new Color32(ObjPtr->r0, ObjPtr->g0, ObjPtr->b0, Byte.MaxValue);
		Vector3 p = new Vector3((Single)ObjPtr->x0, (Single)ObjPtr->y0, PSXGPU.zDepth);
		Vector3 vector = new Vector3((Single)ObjPtr->x1, (Single)ObjPtr->y1, PSXGPU.zDepth);
		PSXGPU.DrawLineG2(abe, color, p, color, vector);
	}

	private unsafe static void exeLineG2(PSX_LIBGPU.LINE_G2* ObjPtr)
	{
		Boolean abe = PSXGPU.IsSemiTrans(ObjPtr->code);
		Color32 c = new Color32(ObjPtr->r0, ObjPtr->g0, ObjPtr->b0, Byte.MaxValue);
		Color32 c2 = new Color32(ObjPtr->r1, ObjPtr->g1, ObjPtr->b1, Byte.MaxValue);
		Vector3 p = new Vector3((Single)ObjPtr->x0, (Single)ObjPtr->y0, PSXGPU.zDepth);
		Vector3 vector = new Vector3((Single)ObjPtr->x1, (Single)ObjPtr->y1, PSXGPU.zDepth);
		PSXGPU.DrawLineG2(abe, c, p, c2, vector);
	}

	private unsafe static void exeLineF3(PSX_LIBGPU.LINE_F3* ObjPtr)
	{
		Boolean abe = PSXGPU.IsSemiTrans(ObjPtr->code);
		Color32 color = new Color32(ObjPtr->r0, ObjPtr->g0, ObjPtr->b0, Byte.MaxValue);
		Vector3 p = new Vector3((Single)ObjPtr->x0, (Single)ObjPtr->y0, PSXGPU.zDepth);
		Vector3 p2 = new Vector3((Single)ObjPtr->x1, (Single)ObjPtr->y1, PSXGPU.zDepth);
		Vector3 vector = new Vector3((Single)ObjPtr->x2, (Single)ObjPtr->y2, PSXGPU.zDepth);
		PSXGPU.DrawLineG2(abe, color, p, color, p2);
		PSXGPU.DrawLineG2(abe, color, p2, color, vector);
	}

	private unsafe static void exeLineG3(PSX_LIBGPU.LINE_G3* ObjPtr)
	{
		Boolean abe = PSXGPU.IsSemiTrans(ObjPtr->code);
		Color32 c = new Color32(ObjPtr->r0, ObjPtr->g0, ObjPtr->b0, Byte.MaxValue);
		Color32 color = new Color32(ObjPtr->r1, ObjPtr->g1, ObjPtr->b1, Byte.MaxValue);
		Color32 c2 = new Color32(ObjPtr->r2, ObjPtr->g2, ObjPtr->b2, Byte.MaxValue);
		Vector3 p = new Vector3((Single)ObjPtr->x0, (Single)ObjPtr->y0, PSXGPU.zDepth);
		Vector3 p2 = new Vector3((Single)ObjPtr->x1, (Single)ObjPtr->y1, PSXGPU.zDepth);
		Vector3 vector = new Vector3((Single)ObjPtr->x2, (Single)ObjPtr->y2, PSXGPU.zDepth);
		PSXGPU.DrawLineG2(abe, c, p, color, p2);
		PSXGPU.DrawLineG2(abe, color, p2, c2, vector);
	}

	private unsafe static void exeLineF4(PSX_LIBGPU.LINE_F4* ObjPtr)
	{
		Boolean abe = PSXGPU.IsSemiTrans(ObjPtr->code);
		Color32 color = new Color32(ObjPtr->r0, ObjPtr->g0, ObjPtr->b0, Byte.MaxValue);
		Vector3 p = new Vector3((Single)ObjPtr->x0, (Single)ObjPtr->y0, PSXGPU.zDepth);
		Vector3 p2 = new Vector3((Single)ObjPtr->x1, (Single)ObjPtr->y1, PSXGPU.zDepth);
		Vector3 p3 = new Vector3((Single)ObjPtr->x2, (Single)ObjPtr->y2, PSXGPU.zDepth);
		Vector3 vector = new Vector3((Single)ObjPtr->x3, (Single)ObjPtr->y3, PSXGPU.zDepth);
		PSXGPU.DrawLineG2(abe, color, p, color, p2);
		PSXGPU.DrawLineG2(abe, color, p2, color, p3);
		PSXGPU.DrawLineG2(abe, color, p3, color, vector);
	}

	private unsafe static void exeLineG4(PSX_LIBGPU.LINE_G4* ObjPtr)
	{
		Boolean abe = PSXGPU.IsSemiTrans(ObjPtr->code);
		Color32 c = new Color32(ObjPtr->r0, ObjPtr->g0, ObjPtr->b0, Byte.MaxValue);
		Color32 color = new Color32(ObjPtr->r1, ObjPtr->g1, ObjPtr->b1, Byte.MaxValue);
		Color32 color2 = new Color32(ObjPtr->r2, ObjPtr->g2, ObjPtr->b2, Byte.MaxValue);
		Color32 c2 = new Color32(ObjPtr->r3, ObjPtr->g3, ObjPtr->b3, Byte.MaxValue);
		Vector3 p = new Vector3((Single)ObjPtr->x0, (Single)ObjPtr->y0, PSXGPU.zDepth);
		Vector3 p2 = new Vector3((Single)ObjPtr->x1, (Single)ObjPtr->y1, PSXGPU.zDepth);
		Vector3 p3 = new Vector3((Single)ObjPtr->x2, (Single)ObjPtr->y2, PSXGPU.zDepth);
		Vector3 vector = new Vector3((Single)ObjPtr->x3, (Single)ObjPtr->y3, PSXGPU.zDepth);
		PSXGPU.DrawLineG2(abe, c, p, color, p2);
		PSXGPU.DrawLineG2(abe, color, p2, color2, p3);
		PSXGPU.DrawLineG2(abe, color2, p3, c2, vector);
	}

	private unsafe static void exeSPRT(PSX_LIBGPU.SPRT* p)
	{
		Boolean abe = PSXGPU.IsSemiTrans(p->code);
		Boolean tge = PSXGPU.IsShadeTex(p->code);
		Int32 globalFlagTP = PSXGPU.GlobalFlagTP;
		Int32 globalFlagABR = PSXGPU.GlobalFlagABR;
		Int32 globalFlagTY = PSXGPU.GlobalFlagTY;
		Int32 globalFlagTX = PSXGPU.GlobalFlagTX;
		Int32 flagClutY = PSXGPU.GetFlagClutY(p->clut);
		Int32 flagClutX = PSXGPU.GetFlagClutX(p->clut);
		PSXGPU.DrawSprt(abe, tge, ref p->rgbc, ref p->xy0, ref p->uv0, ref p->wh, globalFlagTP, globalFlagABR, globalFlagTY, globalFlagTX, flagClutY, flagClutX);
	}

	private unsafe static void exeSPRT_16(PSX_LIBGPU.SPRT_16* p)
	{
		Boolean abe = PSXGPU.IsSemiTrans(p->code);
		Boolean tge = PSXGPU.IsShadeTex(p->code);
		PSX_LIBGPU.WH wh;
		wh.w = (wh.h = 16);
		Int32 globalFlagTP = PSXGPU.GlobalFlagTP;
		Int32 globalFlagABR = PSXGPU.GlobalFlagABR;
		Int32 globalFlagTY = PSXGPU.GlobalFlagTY;
		Int32 globalFlagTX = PSXGPU.GlobalFlagTX;
		Int32 flagClutY = PSXGPU.GetFlagClutY(p->clut);
		Int32 flagClutX = PSXGPU.GetFlagClutX(p->clut);
		PSXGPU.DrawSprt(abe, tge, ref p->rgbc, ref p->xy0, ref p->uv0, ref wh, globalFlagTP, globalFlagABR, globalFlagTY, globalFlagTX, flagClutY, flagClutX);
	}

	private unsafe static void exeSPRT_8(PSX_LIBGPU.SPRT_8* p)
	{
		Boolean abe = PSXGPU.IsSemiTrans(p->code);
		Boolean tge = PSXGPU.IsShadeTex(p->code);
		PSX_LIBGPU.WH wh;
		wh.w = (wh.h = 8);
		Int32 globalFlagTP = PSXGPU.GlobalFlagTP;
		Int32 globalFlagABR = PSXGPU.GlobalFlagABR;
		Int32 globalFlagTY = PSXGPU.GlobalFlagTY;
		Int32 globalFlagTX = PSXGPU.GlobalFlagTX;
		Int32 flagClutY = PSXGPU.GetFlagClutY(p->clut);
		Int32 flagClutX = PSXGPU.GetFlagClutX(p->clut);
		PSXGPU.DrawSprt(abe, tge, ref p->rgbc, ref p->xy0, ref p->uv0, ref wh, globalFlagTP, globalFlagABR, globalFlagTY, globalFlagTX, flagClutY, flagClutX);
	}

	private unsafe static void exeTILE(PSX_LIBGPU.TILE* ObjPtr)
	{
		Boolean abe = PSXGPU.IsSemiTrans(ObjPtr->code);
		Color32 c = new Color32(ObjPtr->r0, ObjPtr->g0, ObjPtr->b0, Byte.MaxValue);
		Vector3 p = new Vector3((Single)ObjPtr->x0, (Single)ObjPtr->y0, PSXGPU.zDepth);
		Vector2 size = new Vector2((Single)ObjPtr->w, (Single)ObjPtr->h);
		PSXGPU.DrawTile(abe, c, p, size);
	}

	private unsafe static void exeTILE_1(PSX_LIBGPU.TILE_1* ObjPtr)
	{
		Boolean abe = PSXGPU.IsSemiTrans(ObjPtr->code);
		Color32 c = new Color32(ObjPtr->r0, ObjPtr->g0, ObjPtr->b0, Byte.MaxValue);
		Vector3 p = new Vector3((Single)ObjPtr->x0, (Single)ObjPtr->y0, PSXGPU.zDepth);
		PSXGPU.DrawTile(abe, c, p, new Vector2(1f, 1f));
	}

	private unsafe static void exeTILE_8(PSX_LIBGPU.TILE_8* ObjPtr)
	{
		Boolean abe = PSXGPU.IsSemiTrans(ObjPtr->code);
		Color32 c = new Color32(ObjPtr->r0, ObjPtr->g0, ObjPtr->b0, Byte.MaxValue);
		Vector3 p = new Vector3((Single)ObjPtr->x0, (Single)ObjPtr->y0, PSXGPU.zDepth);
		PSXGPU.DrawTile(abe, c, p, new Vector2(8f, 8f));
	}

	private unsafe static void exeTILE_16(PSX_LIBGPU.TILE_16* ObjPtr)
	{
		Boolean abe = PSXGPU.IsSemiTrans(ObjPtr->code);
		Color32 c = new Color32(ObjPtr->r0, ObjPtr->g0, ObjPtr->b0, Byte.MaxValue);
		Vector3 p = new Vector3((Single)ObjPtr->x0, (Single)ObjPtr->y0, PSXGPU.zDepth);
		PSXGPU.DrawTile(abe, c, p, new Vector2(16f, 16f));
	}

	public unsafe static PSXGPU.FF9DebugPrimType ff9debugGetPrimType(PSX_LIBGPU.P_TAG* ObjPtr, Boolean CheckLength)
	{
		UInt32 len = ObjPtr->getLen();
		if (ObjPtr != null)
		{
			if (CheckLength && len == 0u)
			{
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_OTENTRY;
			}
			if (ObjPtr->code == 128 && len == 4u)
			{
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_OTEND;
			}
			if ((ObjPtr->code & 252) == 32)
			{
				if (CheckLength && len != 4u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_POLY_F3;
			}
			else if ((ObjPtr->code & 252) == 36)
			{
				if (CheckLength && len != 7u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_POLY_FT3;
			}
			else if ((ObjPtr->code & 252) == 48)
			{
				if (CheckLength && len != 6u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_POLY_G3;
			}
			else if ((ObjPtr->code & 252) == 52)
			{
				if (CheckLength && len != 9u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_POLY_GT3;
			}
			else if ((ObjPtr->code & 252) == 40)
			{
				if (CheckLength && len != 5u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_POLY_F4;
			}
			else if ((ObjPtr->code & 252) == 44)
			{
				if (CheckLength && len != 9u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_POLY_FT4;
			}
			else if ((ObjPtr->code & 252) == 56)
			{
				if (CheckLength && len != 8u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_POLY_G4;
			}
			else if ((ObjPtr->code & 252) == 60)
			{
				if (CheckLength && len != 12u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_POLY_GT4;
			}
			else if ((ObjPtr->code & 252) == 116)
			{
				if (CheckLength && len != 3u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_SPRT_8;
			}
			else if ((ObjPtr->code & 252) == 124)
			{
				if (CheckLength && len != 3u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_SPRT_16;
			}
			else if ((ObjPtr->code & 252) == 100)
			{
				if (CheckLength && len != 4u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_SPRT;
			}
			else if ((ObjPtr->code & 252) == 104)
			{
				if (CheckLength && len != 2u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_TILE_1;
			}
			else if ((ObjPtr->code & 252) == 112)
			{
				if (CheckLength && len != 2u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_TILE_8;
			}
			else if ((ObjPtr->code & 252) == 120)
			{
				if (CheckLength && len != 2u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_TILE_16;
			}
			else if ((ObjPtr->code & 252) == 96)
			{
				if (CheckLength && len != 3u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_TILE;
			}
			else if ((ObjPtr->code & 252) == 64)
			{
				if (CheckLength && len != 3u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_LINE_F2;
			}
			else if ((ObjPtr->code & 252) == 80)
			{
				if (CheckLength && len != 4u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_LINE_G2;
			}
			else if ((ObjPtr->code & 252) == 72)
			{
				if (CheckLength && len != 5u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_LINE_F3;
			}
			else if ((ObjPtr->code & 252) == 88)
			{
				if (CheckLength && len != 7u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_LINE_G3;
			}
			else if ((ObjPtr->code & 252) == 76)
			{
				if (CheckLength && len != 6u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_LINE_F4;
			}
			else if ((ObjPtr->code & 252) == 92)
			{
				if (CheckLength && len != 9u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
				}
				return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_LINE_G4;
			}
			else
			{
				if (ObjPtr->code == 226)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_TWIN;
				}
				if (ObjPtr->code == 228 && len == 2u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_AREA;
				}
				if (ObjPtr->code == 229)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_OFFSET;
				}
				if (ObjPtr->code == 225 && len == 2u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_MODE;
				}
				if (ObjPtr->code == 227 && len == 9u)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_ENV;
				}
				if (ObjPtr->code == 231)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_MOVE;
				}
				if (ObjPtr->code == 160)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_LOAD;
				}
				if (ObjPtr->code == 225)
				{
					if (CheckLength && len != 1u)
					{
						return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_MARGE;
					}
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_TPAGE;
				}
				else if (ObjPtr->code == 230)
				{
					return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_DR_STP;
				}
			}
		}
		return PSXGPU.FF9DebugPrimType.FF9DEBUG_PRIMTYPE_UNKNOWN;
	}

	public static void Start()
	{
	}

	public static void DrawPolyG3(Boolean abe, Color32 c0, Vector3 p0, Color32 c1, Vector3 p1, Color32 c2, Vector3 p2)
	{
		PSXGPU.SetMaterial(PSXGPU.matPSXShaderNoTexture);
		c0.a = (c1.a = (c2.a = PSXGPU.SetShader(abe, PSXGPU.GlobalFlagABR, PSXGPU.drOffset)));
		PSXGPU.SetPolygon(4);
		GL.Color(c0);
		GL.Vertex(p0);
		GL.Color(c1);
		GL.Vertex(p1);
		GL.Color(c2);
		GL.Vertex(p2);
		GL.End();
	}

	public static void DrawPolyGT3(Int32 drawMode, Byte code, UInt16 clut, UInt16 tpage, Boolean tge, ref Color32 c0, ref PSX_LIBGPU.XY p0, ref PSX_LIBGPU.UV t0, ref Color32 c1, ref PSX_LIBGPU.XY p1, ref PSX_LIBGPU.UV t1, ref Color32 c2, ref PSX_LIBGPU.XY p2, ref PSX_LIBGPU.UV t2)
	{
		Byte a = c0.a;
		Byte a2 = c1.a;
		Byte a3 = c2.a;
		Boolean abe = PSXGPU.IsSemiTrans(code);
		Int32 flagABR = PSXGPU.GetFlagABR(tpage);
		if ((drawMode & 1) != 0)
		{
			Int32 flagTP = PSXGPU.GetFlagTP(tpage);
			Int32 flagTY = PSXGPU.GetFlagTY(tpage);
			Int32 flagTX = PSXGPU.GetFlagTX(tpage);
			Int32 flagClutY = PSXGPU.GetFlagClutY(clut);
			Int32 flagClutX = PSXGPU.GetFlagClutX(clut);
			PSXGPU.SetMaterial(PSXGPU.matPSXShaderTextureShading);
			PSXGPU.SetTexture(flagTP, flagTY, flagTX, flagClutY, flagClutX, FilterMode.Point);
			c0.a = (c1.a = (c2.a = PSXGPU.SetShader(abe, flagABR, PSXGPU.drOffset)));
			PSXGPU.SetPolygon(4);
		}
		else
		{
			c0.a = (c1.a = (c2.a = PSXGPU.GetAlpha(abe, flagABR)));
		}
		GL.Color(c0);
		GL.TexCoord2((Single)t0.u, (Single)t0.v);
		GL.Vertex3((Single)p0.x, (Single)p0.y, PSXGPU.zDepth);
		GL.Color(c1);
		GL.TexCoord2((Single)t1.u, (Single)t1.v);
		GL.Vertex3((Single)p1.x, (Single)p1.y, PSXGPU.zDepth);
		GL.Color(c2);
		GL.TexCoord2((Single)t2.u, (Single)t2.v);
		GL.Vertex3((Single)p2.x, (Single)p2.y, PSXGPU.zDepth);
		if ((drawMode & 2) != 0)
		{
			GL.End();
		}
		c0.a = a;
		c1.a = a2;
		c2.a = a3;
	}

	public static void DrawLineG2(Boolean blend, Color32 beginColor, Vector3 begin, Color32 endColor, Vector3 end)
	{
		PSXGPU.SetMaterial(PSXGPU.matPSXShaderNoTexture);
		beginColor.a = (endColor.a = PSXGPU.SetShader(blend, PSXGPU.GlobalFlagABR, PSXGPU.drOffset));
		PSXGPU.SetPolygon(1);
		GL.Color(beginColor);
		GL.Vertex(begin);
		GL.Color(endColor);
		GL.Vertex(end);
		GL.End();
	}

	public static void DrawSprt(Boolean abe, Boolean tge, ref Color32 c0, ref PSX_LIBGPU.XY p0, ref PSX_LIBGPU.UV t0, ref PSX_LIBGPU.WH size, Int32 TP, Int32 ABR, Int32 TY, Int32 TX, Int32 clutY, Int32 clutX)
	{
		Byte a = c0.a;
		PSXGPU.p1.x = (Single)(p0.x + size.w);
		PSXGPU.p1.y = (Single)(p0.y + size.h);
		PSXGPU.p1.z = PSXGPU.zDepth;
		PSXGPU.uv0.x = (Single)t0.u + 0.5f;
		PSXGPU.uv0.y = (Single)t0.v + 0.5f;
		PSXGPU.uv1.x = (Single)((Int16)t0.u + size.w) - 0.5f;
		PSXGPU.uv1.y = (Single)((Int16)t0.v + size.h) - 0.5f;
		PSXGPU.SetMaterial(PSXGPU.matPSXShaderTextureShading);
		PSXGPU.SetTexture(TP, TY, TX, clutY, clutX, FilterMode.Bilinear);
		c0.a = PSXGPU.SetShader(abe, ABR, PSXGPU.drOffset);
		PSXGPU.SetPolygon(7);
		GL.Color(c0);
		GL.TexCoord(PSXGPU.uv0);
		GL.Vertex3((Single)p0.x, (Single)p0.y, PSXGPU.zDepth);
		GL.Color(c0);
		GL.TexCoord2(PSXGPU.uv1.x, PSXGPU.uv0.y);
		GL.Vertex3(PSXGPU.p1.x, (Single)p0.y, PSXGPU.zDepth);
		GL.Color(c0);
		GL.TexCoord(PSXGPU.uv1);
		GL.Vertex(PSXGPU.p1);
		GL.Color(c0);
		GL.TexCoord2(PSXGPU.uv0.x, PSXGPU.uv1.y);
		GL.Vertex3((Single)p0.x, PSXGPU.p1.y, PSXGPU.zDepth);
		GL.End();
		c0.a = a;
	}

	public static void DrawTile(Boolean abe, Color32 c0, Vector3 p0, Vector2 size)
	{
		PSXGPU.SetMaterial(PSXGPU.matPSXShaderNoTexture);
		c0.a = PSXGPU.SetShader(abe, PSXGPU.GlobalFlagABR, PSXGPU.drOffset);
		PSXGPU.SetPolygon(7);
		GL.Color(c0);
		GL.Vertex(p0);
		GL.Color(c0);
		GL.Vertex(p0 + new Vector3(size.x, 0f, 0f));
		GL.Color(c0);
		GL.Vertex(p0 + new Vector3(size.x, size.y, 0f));
		GL.Color(c0);
		GL.Vertex(p0 + new Vector3(0f, size.y, 0f));
		GL.End();
	}

	private unsafe static void exeDR_MODE(PSX_LIBGPU.DR_MODE* ObjPtr)
	{
		UInt16 obj = (UInt16)(ObjPtr->code[0] & 65535u);
		PSXGPU.GlobalFlagTP = PSXGPU.GetFlagTP(obj);
		PSXGPU.GlobalFlagABR = PSXGPU.GetFlagABR(obj);
		PSXGPU.GlobalFlagTY = PSXGPU.GetFlagTY(obj);
		PSXGPU.GlobalFlagTX = PSXGPU.GetFlagTX(obj);
	}

	private unsafe static void exeDR_TWIN(PSX_LIBGPU.DR_TWIN* ObjPtr)
	{
	}

	private unsafe static void exeDR_OFFSET(PSX_LIBGPU.DR_OFFSET* ObjPtr)
	{
        PSXGPU.drOffset.x = ObjPtr->code[1] & 65535u;
        PSXGPU.drOffset.y = ObjPtr->code[1] >> 16;
    }

	private unsafe static void exeDR_LOAD(PSX_LIBGPU.DR_LOAD* ObjPtr)
	{
	}

	private unsafe static void exeDR_TPAGE(PSX_LIBGPU.DR_TPAGE* ObjPtr)
	{
		UInt16 obj = (UInt16)(ObjPtr->code[0] & 65535u);
		PSXGPU.GlobalFlagTP = PSXGPU.GetFlagTP(obj);
		PSXGPU.GlobalFlagABR = PSXGPU.GetFlagABR(obj);
		PSXGPU.GlobalFlagTY = PSXGPU.GetFlagTY(obj);
		PSXGPU.GlobalFlagTX = PSXGPU.GetFlagTX(obj);
	}

	private unsafe static void exeDR_STP(PSX_LIBGPU.DR_STP* ObjPtr)
	{
	}

	private unsafe static void exeDR_ENV(PSX_LIBGPU.DR_ENV* ObjPtr)
	{
	}

	private unsafe static void exeDR_AREA(PSX_LIBGPU.DR_AREA* ObjPtr)
	{
		PSXGPU.drAreaX = (Int32)(ObjPtr->code[1] >> 16);
		if (PSXGPU.drAreaX != 0)
		{
			GL.Viewport(new Rect(0f, 0f, (Single)PSXTextureMgr.GEN_TEXTURE_W, (Single)PSXTextureMgr.GEN_TEXTURE_H));
			GL.LoadIdentity();
			Matrix4x4 mat = Matrix4x4.Ortho(0f, (Single)PSXTextureMgr.GEN_TEXTURE_W, 0f, (Single)PSXTextureMgr.GEN_TEXTURE_H, FieldMap.HalfScreenWidth, 65536f);
			mat.m22 = -1f;
			mat.m23 = 0f;
			GL.LoadProjectionMatrix(mat);
			PSXGPU.drOffset.x = PSXGPU.drOffset.x - (Single)PSXTextureMgr.GEN_TEXTURE_X;
			PSXGPU.drOffset.y = PSXGPU.drOffset.y - (Single)PSXTextureMgr.GEN_TEXTURE_Y;
			PSXGPU.curRenderTexture = RenderTexture.active;
			RenderTexture.active = PSXTextureMgr.genTexture;
		}
		else
		{
			SFX.ResetViewPort();
			if (SFX.isDebugPng)
			{
				Texture2D texture2D = new Texture2D(PSXTextureMgr.GEN_TEXTURE_W, PSXTextureMgr.GEN_TEXTURE_H, TextureFormat.ARGB32, false);
				texture2D.ReadPixels(new Rect(0f, 0f, (Single)PSXTextureMgr.GEN_TEXTURE_W, (Single)PSXTextureMgr.GEN_TEXTURE_H), 0, 0);
				texture2D.Apply();
				Byte[] bytes = texture2D.EncodeToPNG();
				File.WriteAllBytes("Temp/gen.png", bytes);
			}
			RenderTexture.active = PSXGPU.curRenderTexture;
		}
	}

	private static void DBG_COLOR(ref Color32 COL, Byte R, Byte G, Byte B)
	{
		if (PSXGPU.dbgColor)
		{
			COL.r = R;
			COL.g = G;
			COL.b = B;
		}
	}

	private static void SetMaterial(PSXMaterial mat)
	{
		if (PSXGPU.curMaterial != mat)
		{
			PSXGPU.curMaterial = mat;
			PSXGPU.constOffset.x = -9999f;
			PSXGPU.constTexSize.x = -9999f;
			PSXGPU.isNeedSetPass = true;
			PSXGPU.blendIndex = -1;
			PSXGPU.curPolygon = 0;
			PSXGPU.texKey = 4294967280u;
		}
	}

	private static void SetTexture(Int32 TP, Int32 TY, Int32 TX, Int32 clutY, Int32 clutX, FilterMode filter = FilterMode.Point)
	{
		if (TP != 2)
		{
			PSXGPU.constOffset.z = 0.5f;
			PSXGPU.constTexSize.x = 256f;
			PSXGPU.constTexSize.y = 256f;
			PSXTexture texture = PSXTextureMgr.GetTexture(TP, TY, TX, clutY, clutX);
			texture.SetFilter(filter);
			if (texture.key != PSXGPU.texKey)
			{
				PSXGPU.curMaterial.mainTexture = texture.texture;
				PSXGPU.texKey = texture.key;
				PSXGPU.isNeedSetPass = true;
			}
		}
		else
		{
			PSXGPU.constOffset.z = (Single)((TX << 6) - PSXTextureMgr.GEN_TEXTURE_X) + 0.5f;
			PSXGPU.constTexSize.x = (Single)PSXTextureMgr.GEN_TEXTURE_W;
			PSXGPU.constTexSize.y = (Single)PSXTextureMgr.GEN_TEXTURE_H;
			if (PSXGPU.texKey != 4294967295u)
			{
				PSXGPU.curMaterial.mainTexture = PSXTextureMgr.genTexture;
				PSXGPU.texKey = UInt32.MaxValue;
				PSXGPU.isNeedSetPass = true;
			}
		}
	}

	private static Byte GetAlpha(Boolean abe, Int32 ABR)
	{
		if (abe)
		{
			return PSXGPU.TransSets[ABR].alpha;
		}
		return Byte.MaxValue;
	}

	private static Byte SetShader(Boolean abe, Int32 ABR, Vector4 offset)
	{
		Int32 num;
		if (abe)
		{
			num = (Int32)((ABR == 2) ? 2 : 0);
		}
		else
		{
			num = 4;
		}
		if (PSXGPU.blendIndex != num)
		{
			PSXGPU.blendIndex = num;
			PSXGPU.isNeedSetPass = true;
		}
		Byte result;
		if (abe)
		{
			PSXGPU.curMaterial.SetInt("_BlendOp", (Int32)PSXGPU.TransSets[ABR].blendOp);
			PSXGPU.curMaterial.SetInt("_SrcFactor", (Int32)PSXGPU.TransSets[ABR].srcFac);
			PSXGPU.curMaterial.SetInt("_DstFactor", (Int32)PSXGPU.TransSets[ABR].dstFac);
			result = PSXGPU.TransSets[ABR].alpha;
		}
		else
		{
			ABR = 4;
			PSXGPU.curMaterial.SetInt("_BlendOp", 0);
			PSXGPU.curMaterial.SetInt("_SrcFactor", 5);
			PSXGPU.curMaterial.SetInt("_DstFactor", 10);
			result = Byte.MaxValue;
		}
		PSXGPU.constOffset.x = offset.x;
		PSXGPU.constOffset.y = offset.y;
		PSXGPU.curMaterial.SetVector("_Offset", PSXGPU.constOffset);
		PSXGPU.constOffsetB = PSXGPU.constOffset;
		PSXGPU.isNeedSetPass = true;
		PSXGPU.curMaterial.SetVector("_TexSize", PSXGPU.constTexSize);
		PSXGPU.constTexSizeB = PSXGPU.constTexSize;
		PSXGPU.isNeedSetPass = true;
		if (PSXGPU.isNeedEnd)
		{
			PSXGPU.isNeedEnd = false;
		}
		PSXGPU.curMaterial.SetPass(0);
		PSXGPU.isNeedSetPass = false;
		PSXGPU.curPolygon = 0;
		return result;
	}

	private static void SetPolygon(Int32 poly)
	{
		if (PSXGPU.isNeedEnd)
		{
		}
		GL.Begin(poly);
		PSXGPU.isNeedEnd = true;
		PSXGPU.curPolygon = poly;
	}

	public static void DrawLine()
	{
		GL.Begin(1);
		GL.Color(Color.green);
		GL.Vertex(new Vector3(0f, 0f, 0f));
		GL.Color(Color.blue);
		GL.Vertex(new Vector3(100f, 100f, 0f));
		GL.End();
	}

	public static void DrawTriangle()
	{
		GL.Begin(4);
		GL.Color(new Color32(128, 0, 0, Byte.MaxValue));
		GL.TexCoord(new Vector3(0f, 0f, 0f));
		GL.Vertex(new Vector3(100f, 100f, 0f));
		GL.Color(new Color32(0, 128, 0, Byte.MaxValue));
		GL.TexCoord(new Vector3(0f, 1f, 0f));
		GL.Vertex(new Vector3(100f, 200f, 0f));
		GL.Color(new Color32(0, 0, 128, Byte.MaxValue));
		GL.TexCoord(new Vector3(1f, 1f, 0f));
		GL.Vertex(new Vector3(200f, 200f, 0f));
		GL.End();
	}

	public static void DrawQuad(Vector2 offset)
	{
		GL.Begin(7);
		GL.Color(Color.red);
		GL.TexCoord(new Vector3(0f, 0f, 0f));
		GL.Vertex(new Vector3(offset.x + 100f, offset.y + 100f, 0f));
		GL.Color(Color.green);
		GL.TexCoord(new Vector3(0f, 1f, 0f));
		GL.Vertex(new Vector3(offset.x + 100f, offset.y + 200f, 0f));
		GL.Color(Color.blue);
		GL.TexCoord(new Vector3(1f, 1f, 0f));
		GL.Vertex(new Vector3(offset.x + 200f, offset.y + 200f, 0f));
		GL.Color(Color.white);
		GL.TexCoord(new Vector3(1f, 0f, 0f));
		GL.Vertex(new Vector3(offset.x + 200f, offset.y + 100f, 0f));
		GL.End();
	}

	public static void DrawPsxArea()
	{
		PSXGPU.matPSXShaderNoTexture.SetPass(0);
		GL.Begin(1);
		Single halfSceneWidth = FieldMap.HalfFieldWidth;
		Single halfSceneHeight = FieldMap.HalfFieldHeight;
		GL.Color(Color.red);
		GL.Vertex(new Vector3(-halfSceneWidth, -halfSceneHeight, 0f));
		GL.Vertex(new Vector3(halfSceneWidth, -halfSceneHeight, 0f));
		GL.Color(Color.green);
		GL.Vertex(new Vector3(-halfSceneWidth, halfSceneHeight, 0f));
		GL.Vertex(new Vector3(halfSceneWidth, halfSceneHeight, 0f));
		GL.Color(Color.blue);
		GL.Vertex(new Vector3(halfSceneWidth, halfSceneHeight, 0f));
		GL.Vertex(new Vector3(halfSceneWidth, -halfSceneHeight, 0f));
		GL.Color(Color.white);
		GL.Vertex(new Vector3(-halfSceneWidth, halfSceneHeight, 0f));
		GL.Vertex(new Vector3(-halfSceneWidth, -halfSceneHeight, 0f));
		GL.End();
	}

	public static void DrawDebugScreenDiagonal(Single hw, Single hh)
	{
		PSXGPU.matPSXShaderNoTexture.SetPass(0);
		GL.Begin(1);
		GL.Color(Color.red);
		GL.Vertex(new Vector3(-hw, -hh, 0f));
		GL.Vertex(new Vector3(hw, hh, 0f));
		GL.Color(Color.green);
		GL.Vertex(new Vector3(-hw, hh, 0f));
		GL.Vertex(new Vector3(hw, -hh, 0f));
		GL.End();
	}

	public static void DrawDebugXYZ(Single length)
	{
		PSXGPU.matPSXShaderNoTexture.SetPass(0);
		GL.Begin(1);
		GL.Color(Color.red);
		GL.Vertex(Vector3.zero);
		GL.Vertex(new Vector3(length, 0f, 0f));
		GL.Color(Color.green);
		GL.Vertex(Vector3.zero);
		GL.Vertex(new Vector3(0f, length, 0f));
		GL.Color(Color.blue);
		GL.Vertex(Vector3.zero);
		GL.Vertex(new Vector3(0f, 0f, length));
		GL.End();
	}

	public static void DrawDebugSquareInOrtho(Vector3 positionCenter, Color color, Single size)
	{
		Material material = PSXGPU.matPSXShaderNoTexture;
		material.SetPass(0);
		GL.Begin(7);
		GL.Color(color);
		GL.Vertex(positionCenter + new Vector3(-size, -size, 0f));
		GL.Color(color);
		GL.Vertex(positionCenter + new Vector3(size, -size, 0f));
		GL.Color(color);
		GL.Vertex(positionCenter + new Vector3(size, size, 0f));
		GL.Color(color);
		GL.Vertex(positionCenter + new Vector3(-size, size, 0f));
		GL.End();
	}

	public const Int32 NONE = -1;

	public const Single PSX_TPAGE_SIZE = 256f;

	private const Single uvOffset = 0.5f;

	private static Boolean dbgColor = false;

	public static Single zDepth = 0f;

	private static Vector4 drOffset;

	private static Int32 drAreaX;

	private static RenderTexture curRenderTexture;

	public static PSXMaterial matPSXShaderNoTexture;

	public static PSXMaterial matPSXShaderTextureNoShading;

	public static PSXMaterial matPSXShaderTextureShading;

	private static Int32 GlobalFlagTP;

	private static Int32 GlobalFlagABR;

	private static Int32 GlobalFlagTY;

	private static Int32 GlobalFlagTX;

	private static Vector3 p1;

	private static Vector2 uv0;

	private static Vector2 uv1;

	private static Int32 DRAW_BEGIN = 1;

	private static Int32 DRAW_END = 2;

	private static Int32 DRAW_BEGIN_END = 3;

	private static PSXMaterial curMaterial;

	private static Vector4 constOffset;

	private static Vector4 constTexSize;

	private static Vector4 constOffsetB;

	private static Vector4 constTexSizeB;

	public static Int32 curPolygon = 0;

	public static Boolean isNeedEnd = false;

	private static Boolean isNeedSetPass;

	private static Int32 blendIndex;

	private static UInt32 texKey;

	private static PSXGPU.SemiTransParams[] TransSets = new PSXGPU.SemiTransParams[]
	{
		new PSXGPU.SemiTransParams(BlendOp.Add, BlendMode.SrcAlpha, BlendMode.One, 128),
		new PSXGPU.SemiTransParams(BlendOp.Add, BlendMode.SrcAlpha, BlendMode.One, Byte.MaxValue),
		new PSXGPU.SemiTransParams(BlendOp.ReverseSubtract, BlendMode.One, BlendMode.One, Byte.MaxValue),
		new PSXGPU.SemiTransParams(BlendOp.Add, BlendMode.SrcAlpha, BlendMode.One, 64)
	};

	public enum FF9DebugPrimType
	{
		FF9DEBUG_PRIMTYPE_UNKNOWN,
		FF9DEBUG_PRIMTYPE_OTENTRY,
		FF9DEBUG_PRIMTYPE_OTEND,
		FF9DEBUG_PRIMTYPE_POLY_F3,
		FF9DEBUG_PRIMTYPE_POLY_FT3,
		FF9DEBUG_PRIMTYPE_POLY_G3,
		FF9DEBUG_PRIMTYPE_POLY_GT3,
		FF9DEBUG_PRIMTYPE_POLY_F4,
		FF9DEBUG_PRIMTYPE_POLY_FT4,
		FF9DEBUG_PRIMTYPE_POLY_G4,
		FF9DEBUG_PRIMTYPE_POLY_GT4,
		FF9DEBUG_PRIMTYPE_SPRT_8,
		FF9DEBUG_PRIMTYPE_SPRT_16,
		FF9DEBUG_PRIMTYPE_SPRT,
		FF9DEBUG_PRIMTYPE_TILE_1,
		FF9DEBUG_PRIMTYPE_TILE_8,
		FF9DEBUG_PRIMTYPE_TILE_16,
		FF9DEBUG_PRIMTYPE_TILE,
		FF9DEBUG_PRIMTYPE_LINE_F2,
		FF9DEBUG_PRIMTYPE_LINE_G2,
		FF9DEBUG_PRIMTYPE_LINE_F3,
		FF9DEBUG_PRIMTYPE_LINE_G3,
		FF9DEBUG_PRIMTYPE_LINE_F4,
		FF9DEBUG_PRIMTYPE_LINE_G4,
		FF9DEBUG_PRIMTYPE_DR_TWIN,
		FF9DEBUG_PRIMTYPE_DR_AREA,
		FF9DEBUG_PRIMTYPE_DR_OFFSET,
		FF9DEBUG_PRIMTYPE_DR_MODE,
		FF9DEBUG_PRIMTYPE_DR_ENV,
		FF9DEBUG_PRIMTYPE_DR_MOVE,
		FF9DEBUG_PRIMTYPE_DR_LOAD,
		FF9DEBUG_PRIMTYPE_DR_TPAGE,
		FF9DEBUG_PRIMTYPE_DR_STP,
		FF9DEBUG_PRIMTYPE_MARGE,
		FF9DEBUG_PRIMTYPE_COUNT
	}

	private struct SemiTransParams
	{
		public SemiTransParams(BlendOp blendOp, BlendMode srcFac, BlendMode dstFac, Byte alpha)
		{
			this.blendOp = blendOp;
			this.srcFac = srcFac;
			this.dstFac = dstFac;
			this.alpha = alpha;
		}

		public readonly BlendOp blendOp;

		public readonly BlendMode srcFac;

		public readonly BlendMode dstFac;

		public readonly Byte alpha;
	}
}
