using System;

public class SFXKey
{
    public static void SetCurrentTPage(UInt16 tpage)
    {
        SFXKey.currentTexPage = (UInt32)((UInt32)((Int32)(tpage & 0x1F) | (tpage >> 2 & 0x60)) << 16);
        SFXKey.currentTexABR = (UInt32)(tpage >> 5 & 3);
    }

    public static UInt32 GetCurrentABR(Byte code)
    {
        SFXKey.tmpABR = SFXKey.currentTexABR;
        if ((code & 2) != 0)
            return (UInt32)((SFXKey.currentTexABR == 2u) ? ABR_SUB : ABR_ADD);
        return 0u;
    }

    public static UInt32 GetCurrentABRTex(Byte code, UInt16 clut)
    {
        SFXKey.tmpABR = SFXKey.currentTexABR;
        UInt32 num;
        if (SFXKey.currentTexPage >> 5 == 2u)
            num = HAS_TEXTURE;
        else
            num = (UInt32)(clut | HAS_TEXTURE);
        if ((code & 2) != 0)
            num |= (UInt32)((SFXKey.currentTexABR == 2u) ? ABR_SUB : ABR_ADD);
        return num | SFXKey.currentTexPage;
    }

    public static UInt32 GetABRTex(Byte code, UInt16 clut, UInt16 tpage)
    {
        UInt32 num = (UInt32)((UInt32)((Int32)(tpage & 0x1F) | (tpage >> 2 & 0x60)) << 16);
        UInt32 num2 = (UInt32)(tpage >> 5 & 3);
        SFXKey.tmpABR = num2;
        UInt32 num3;
        if (num >> 5 == 2u)
            num3 = HAS_TEXTURE;
        else
            num3 = (UInt32)(clut | HAS_TEXTURE);
        if ((code & 2) != 0)
            num3 |= (UInt32)((num2 == 2u) ? ABR_SUB : ABR_ADD);
        return num3 | num;
    }

    public static UInt32 GenerateKey(Int32 TP, Int32 TX, Int32 TY, Int32 clutX, Int32 clutY)
    {
        UInt32 num = 0x8000u;
        num |= (UInt32)(clutX & 0x3F);
        num |= (UInt32)((UInt32)(clutY & 0x1FF) << 6);
        num |= (UInt32)((UInt32)(TX & 0xF) << 16);
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
        return key & FILTER_MASK;
    }

    public static Boolean isLinePolygon(UInt32 key)
    {
        return (key & LINE_POLYGON) != 0u;
    }

    public static UInt32 GetTextureKey(UInt32 key)
    {
        return key & TEX_KEY_MASK;
    }

    public static Boolean IsBlurTexture(UInt32 key)
    {
        return (key >> 16 & 0xFu) < 6u;
    }

    public static Boolean IsTexture(UInt32 key)
    {
        return (key & HAS_TEXTURE) != 0u;
    }

    public static UInt32 GetPositionX(UInt32 key)
    {
        return (key & TX_MASK) >> 10;
    }

    public const UInt32 HAS_TEXTURE = 0x8000u;

    public const UInt32 ABR_ADD = 0x800000u;

    public const UInt32 ABR_SUB = 0x1000000u;

    public const UInt32 FILTER_POINT = 0x2000000u;

    public const UInt32 FILTER_BILINEAR = 0x4000000u;

    public const Int32 FILTER_MASK = 0x6000000;

    public const UInt32 LINE_POLYGON = 0x8000000u;

    public const UInt32 GROUND_TXTURE = 0x10000000u;

    public const UInt32 FULL_BLUR_TXTURE = 0x20000000u;

    public const UInt32 TEX_KEY_MASK = 0x7FFFFFu;

    public const UInt32 TX_MASK = 0xF0000u;

    public static UInt32 tmpABR;

    public static UInt32 currentTexPage;

    public static UInt32 currentTexABR;
}
