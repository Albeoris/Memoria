using System;

public class SFXKey
{
	public static void SetCurrentTPage(UInt16 tpage)
	{
		SFXKey.currentTexPage = (UInt32)((UInt32)((Int32)(tpage & 31) | (tpage >> 2 & 96)) << 16);
		SFXKey.currentTexABR = (UInt32)(tpage >> 5 & 3);
	}

	public static UInt32 GetCurrentABR(Byte code)
	{
		SFXKey.tmpABR = SFXKey.currentTexABR;
		if ((code & 2) != 0)
		{
			return (UInt32)((SFXKey.currentTexABR == 2u) ? 16777216u : 8388608u);
		}
		return 0u;
	}

	public static UInt32 GetCurrentABRTex(Byte code, UInt16 clut)
	{
		SFXKey.tmpABR = SFXKey.currentTexABR;
		UInt32 num;
		if (SFXKey.currentTexPage >> 5 == 2u)
		{
			num = 32768u;
		}
		else
		{
			num = (UInt32)(clut | 32768);
		}
		if ((code & 2) != 0)
		{
			num |= (UInt32)((SFXKey.currentTexABR == 2u) ? 16777216u : 8388608u);
		}
		return num | SFXKey.currentTexPage;
	}

	public static UInt32 GetABRTex(Byte code, UInt16 clut, UInt16 tpage)
	{
		UInt32 num = (UInt32)((UInt32)((Int32)(tpage & 31) | (tpage >> 2 & 96)) << 16);
		UInt32 num2 = (UInt32)(tpage >> 5 & 3);
		SFXKey.tmpABR = num2;
		UInt32 num3;
		if (num >> 5 == 2u)
		{
			num3 = 32768u;
		}
		else
		{
			num3 = (UInt32)(clut | 32768);
		}
		if ((code & 2) != 0)
		{
			num3 |= (UInt32)((num2 == 2u) ? 16777216u : 8388608u);
		}
		return num3 | num;
	}

	public static UInt32 GenerateKey(Int32 TP, Int32 TX, Int32 TY, Int32 clutX, Int32 clutY)
	{
		UInt32 num = 32768u;
		num |= (UInt32)(clutX & 63);
		num |= (UInt32)((UInt32)(clutY & 511) << 6);
		num |= (UInt32)((UInt32)(TX & 15) << 16);
		num |= (UInt32)((UInt32)(TY & 1) << 20);
		return num | (UInt32)((UInt32)(TP & 3) << 21);
	}

	public static UInt32 GetBlendMode(UInt32 key)
	{
		return key >> 23 & 3u;
	}

	public static UInt32 GetTextureMode(UInt32 key)
	{
		return key >> 21 & 3u;
	}

	public static UInt32 GetFilter(UInt32 key)
	{
		return key & 100663296u;
	}

	public static Boolean isLinePolygon(UInt32 key)
	{
		return (key & 134217728u) != 0u;
	}

	public static UInt32 GetTextureKey(UInt32 key)
	{
		return key & 8388607u;
	}

	public static Boolean IsBlurTexture(UInt32 key)
	{
		return (key >> 16 & 15u) < 6u;
	}

	public static Boolean IsTexture(UInt32 key)
	{
		return (key & 32768u) != 0u;
	}

	public static UInt32 GetPositionX(UInt32 key)
	{
		return (key & 983040u) >> 10;
	}

	public const UInt32 HAS_TEXTURE = 32768u;

	public const UInt32 ABR_ADD = 8388608u;

	public const UInt32 ABR_SUB = 16777216u;

	public const UInt32 FILLTER_POINT = 33554432u;

	public const UInt32 FILLTER_BILINEAR = 67108864u;

	public const Int32 FILLTER_MASK = 100663296;

	public const UInt32 LINE_POLYGON = 134217728u;

	public const UInt32 GROUND_TXTURE = 268435456u;

	public const UInt32 FULL_BLUR_TXTURE = 536870912u;

	public const UInt32 TEX_KEY_MASK = 8388607u;

	public const UInt32 TX_MASK = 983040u;

	public static UInt32 tmpABR;

	public static UInt32 currentTexPage;

	public static UInt32 currentTexABR;
}
