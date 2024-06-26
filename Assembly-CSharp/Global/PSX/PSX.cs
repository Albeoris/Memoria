using System;
using UnityEngine;
using UnityEngine.Rendering;

internal static class PSX
{
	public static void ConvertColor16toARGB(UInt16 raw, out Byte a, out Byte r, out Byte g, out Byte b)
	{
		b = (Byte)((raw >> 10 & 31) << 3);
		g = (Byte)((raw >> 5 & 31) << 3);
		r = (Byte)((raw & 31) << 3);
		a = Byte.MaxValue;
		if (raw == 0)
		{
			a = 0;
		}
	}

	public static void ConvertColor16toColor32(UInt16 raw, out Color32 col)
	{
		Byte b = (Byte)((raw >> 10 & 31) << 3);
		Byte g = (Byte)((raw >> 5 & 31) << 3);
		Byte r = (Byte)((raw & 31) << 3);
		Byte a = Byte.MaxValue;
		if (raw == 0)
		{
			a = 0;
		}
		col.a = a;
		col.r = r;
		col.g = g;
		col.b = b;
	}

	public static Vector3 CalculateGTE_RTPT_POS(Vector3 vertex, Matrix4x4 localRTS, Matrix4x4 globalRT, Single viewDist, Vector2 offset, Boolean useAbsZ = true)
	{
		Vector3 v = localRTS.MultiplyPoint(vertex);
		v.y *= -1f;
		Vector3 result = globalRT.MultiplyPoint(v);
		result.y *= -1f;
		Single num = result.z;
		if (useAbsZ)
		{
			num = Mathf.Abs(result.z);
		}
		result.x *= viewDist / num;
		result.y *= viewDist / num;
		result.x += offset.x;
		result.y += offset.y;
		return result;
	}

	public static Single CalculateGTE_RTPTZ(Vector3 vertex, Matrix4x4 localRTS, Matrix4x4 globalRT, Single viewDist, Vector2 offset)
	{
		Vector3 v = localRTS.MultiplyPoint(vertex);
		v.y *= -1f;
		return globalRT.MultiplyPoint(v).z;
	}

	public static Vector3 CalculateGTE_RTPT(Vector3 vertex, Matrix4x4 localRTS, Matrix4x4 globalRT, Single viewDist, Vector2 offset)
	{
		Vector3 result = PSX.CalculateGTE_RTPT_POS(vertex, localRTS, globalRT, viewDist, offset, true);
		result.z = 0f;
		return result;
	}

	public static Vector3 CalculateGTE_RT(Vector3 vertex, Matrix4x4 localRTS, Matrix4x4 globalRT)
	{
		Vector3 v = localRTS.MultiplyPoint(vertex);
		v.y *= -1f;
		Vector3 result = globalRT.MultiplyPoint(v);
		result.y *= -1f;
		return result;
	}

	public static Color32 ConvertABGR16toABGR32(UInt16 vramPixel, Boolean enableSemiTransparent, Int32 abr)
	{
		Single num = (Single)(vramPixel >> 10 & 31) / 31f;
		Byte b = (Byte)(num * 255f);
		Single num2 = (Single)(vramPixel >> 5 & 31) / 31f;
		Byte g = (Byte)(num2 * 255f);
		Single num3 = (Single)(vramPixel & 31) / 31f;
		Byte r = (Byte)(num3 * 255f);
		Byte a = Byte.MaxValue;
		if (enableSemiTransparent)
		{
			Byte b2;
			if (abr == 0)
			{
				b2 = Byte.MaxValue;
			}
			else
			{
				b2 = 0;
			}
			if (vramPixel == 0)
			{
				a = b2;
			}
			else if ((vramPixel & 32768) != 0 && (vramPixel & 32767) != 0)
			{
				a = PSX.TransSets[abr].alpha;
			}
		}
		else if (vramPixel == 0)
		{
			a = 0;
		}
		return new Color32(r, g, b, a);
	}

	public static Single Fp12ToFloat(Int32 fp)
	{
		return PSX.Fp12ToFloat((Int16)(fp & 65535));
	}

	public static Single Fp12ToFloat(Int16 fp)
	{
		return (Single)fp / 4096f;
	}

	public static Int16 FloatToFp12(Single f)
	{
		Int32 num = (Int32)(f * 4096f);
		if (num > 32767)
		{
			num = 32767;
		}
		if (num < -32768)
		{
			num = -32768;
		}
		return (Int16)num;
	}

	public static Single hexToFloat(UInt32 hex)
	{
		return (Single)hex;
	}

	public static Int16 hexToShort(UInt32 hex)
	{
		return (Int16)hex;
	}

	public static Int32 hexToInt(UInt32 hex)
	{
		return (Int32)hex;
	}

	public static Matrix4x4 PerspectiveOffCenter(Single left, Single right, Single bottom, Single top, Single near, Single far)
	{
		Single value = 2f * near / (right - left);
		Single value2 = 2f * near / (top - bottom);
		Single value3 = (right + left) / (right - left);
		Single value4 = (top + bottom) / (top - bottom);
		Single value5 = -(far + near) / (far - near);
		Single value6 = -(2f * far * near) / (far - near);
		Single value7 = -1f;
		Matrix4x4 result = default(Matrix4x4);
		result[0, 0] = value;
		result[0, 1] = 0f;
		result[0, 2] = value3;
		result[0, 3] = 0f;
		result[1, 0] = 0f;
		result[1, 1] = value2;
		result[1, 2] = value4;
		result[1, 3] = 0f;
		result[2, 0] = 0f;
		result[2, 1] = 0f;
		result[2, 2] = value5;
		result[2, 3] = value6;
		result[3, 0] = 0f;
		result[3, 1] = 0f;
		result[3, 2] = value7;
		result[3, 3] = 0f;
		return result;
	}

	private static Matrix4x4 PsxRotTrans2UnityModelView(Matrix4x4 psxRot, Vector3 psxTrans)
	{
		Matrix4x4 lhs = Matrix4x4.TRS(psxTrans, Quaternion.identity, Vector3.one);
		Matrix4x4 rhs = lhs * psxRot;
		return Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, 1f, -1f)) * rhs;
	}

	private static Matrix4x4 PsxProj2UnityProj(Single psxGeomScreen, Single clipDistance)
	{
	    Single halfScreenWidth = FieldMap.HalfScreenWidth;
	    Single halfScreenHeight = FieldMap.HalfScreenHeight;
		Single far = psxGeomScreen + clipDistance;
		return PSX.PerspectiveOffCenter(-halfScreenWidth, halfScreenWidth, halfScreenHeight, -halfScreenHeight, psxGeomScreen, far);
	}

	public static void ConvertCameraPsx2Unity(Camera unityCam, Matrix4x4 psxRot, Vector3 psxTrans, Single psxGeomScreen, Single clipDistance)
	{
		unityCam.worldToCameraMatrix = PSX.PsxRotTrans2UnityModelView(psxRot, psxTrans) * unityCam.transform.worldToLocalMatrix;
		unityCam.projectionMatrix = PSX.PsxProj2UnityProj(psxGeomScreen, clipDistance);
	}

	public static Single AngleFp12ToDegree(Int16 fp)
	{
		return PSX.Fp12ToFloat(fp) * 360f;
	}

	public static Int16 AngleDegreeToFp12(Single f)
	{
		return PSX.FloatToFp12(f / 360f);
	}

	public const Single PIXEL_SCALE = 1f;

	public const Single MESH_SCALE = 1f;

	public const Int32 ORDER_TABLE_SCALE = 1;

	public const Single PSX_FIELD_SCREEN_WIDTH = 320f;

	public const Single PSX_FIELD_SCREEN_HEIGHT = 224f;

	public const Single PSX_FIELD_SCREEN_RATIO = 1.42857146f;

	public const Single PSX_FIELD_SCREEN_HALF_WIDTH = 160f;

	public const Single PSX_FIELD_SCREEN_HALF_HEIGHT = 112f;

	public const Int16 PSX_FIELD_SCREEN_HALF_WIDTH_SHORT = 160;

	public const Int16 PSX_FIELD_SCREEN_HALF_HEIGHT_SHORT = 112;

	public const Single PSX_SCREEN_WIDTH = 320f;

	public const Single PSX_SCREEN_HEIGHT = 220f;

	public const Single PSX_SCREEN_RATIO = 1.4545455f;

	public const Single PSX_SCREEN_HALF_WIDTH = 160f;

	public const Single PSX_SCREEN_HALF_HEIGHT = 110f;

	public const Int16 PSX_SCREEN_HALF_WIDTH_SHORT = 160;

	public const Int16 PSX_SCREEN_HALF_HEIGHT_SHORT = 110;

	public const UInt32 PSX_BYTES_PER_PIXEL = 2u;

	public const UInt32 FRAME_BUFFER_WIDTH = 1024u;

	public const UInt32 FRAME_BUFFER_HEIGHT = 512u;

	public const UInt32 CLUT_256_W = 256u;

	public const UInt32 CLUT_16_W = 16u;

	public const UInt32 CLUT_H = 1u;

	public const UInt16 STP_BIT = 32768;

	public const UInt16 MAX_RGB = 32767;

	public const UInt16 MIN_RGB = 0;

	public const UInt16 RGB_BITS = 31;

	public const UInt16 RGB_RED = 31744;

	public const UInt16 RGB_GREEN = 992;

	public const UInt16 RGB_BLUE = 31;

	public const UInt16 ALPHA_TRANSPARENT = 0;

	public const UInt16 ALPHA_SEMI_TRANSPARENT = 127;

	public const UInt16 ALPHA_NON_TRANSPARENT = 255;

	public const UInt16 PIXEL_NON_TRANSPARENT_BLACK = 32768;

	public const UInt32 TIM_HEADER_ID = 16u;

	public const UInt32 TIM_FLAG_PMODE_MASK = 7u;

	public const UInt32 TIM_FLAG_PMODE_4BIT_CLUT = 0u;

	public const UInt32 TIM_FLAG_PMODE_8BIT_CLUT = 1u;

	public const UInt32 TIM_FLAG_PMODE_16BIT_DIRECT = 2u;

	public const UInt32 TIM_FLAG_PMODE_24BIT_DIRECT = 3u;

	public const UInt32 TIM_FLAG_PMODE_MIXED = 4u;

	public const UInt32 TIM_FLAG_CF_MASK = 8u;

	public const UInt32 TIM_FLAG_CF_NOT_EXIST = 0u;

	public const UInt32 TIM_FLAG_CF_EXIST = 8u;

	public static readonly PSX.SemiTransParams[] TransSets = new PSX.SemiTransParams[]
	{
		new PSX.SemiTransParams(BlendOp.Add, BlendMode.SrcAlpha, BlendMode.SrcAlpha, 128),
		new PSX.SemiTransParams(BlendOp.Add, BlendMode.One, BlendMode.One, Byte.MaxValue),
		new PSX.SemiTransParams(BlendOp.ReverseSubtract, BlendMode.One, BlendMode.One, Byte.MaxValue),
		new PSX.SemiTransParams(BlendOp.Add, BlendMode.SrcAlpha, BlendMode.One, 64)
	};

	public struct SemiTransParams
	{
		public SemiTransParams(BlendOp blendEquation, BlendMode srcFac, BlendMode dstFac, Byte alpha)
		{
			this.blendEquation = blendEquation;
			this.srcFac = srcFac;
			this.dstFac = dstFac;
			this.alpha = alpha;
		}

		public readonly BlendOp blendEquation;

		public readonly BlendMode srcFac;

		public readonly BlendMode dstFac;

		public readonly Byte alpha;
	}
}
