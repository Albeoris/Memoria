using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using Object = System.Object;

public static class PSX_LIBGPU
{
    public static T ByteArrayToStructure<T>(Byte[] bytes) where T : struct
    {
        GCHandle gchandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        T result = (T)((Object)Marshal.PtrToStructure(gchandle.AddrOfPinnedObject(), typeof(T)));
        gchandle.Free();
        return result;
    }

    public unsafe static void setRECT(RECT* r, Int16 _x, Int16 _y, Int16 _w, Int16 _h)
    {
        r->x = _x;
        r->y = _y;
        r->w = _w;
        r->h = _h;
    }

    public unsafe static Int32 LoadImage(RECT* rect, UInt32* p)
    {
        if ((Int32)rect->w == 0 || (Int32)rect->h == 0)
            return 0;
        PSX_LIBGPU.isVramDirty = true;
        UInt16* numPtr1 = (UInt16*)p;
        Int32 w = (Int32)rect->w;
        Int32 h = (Int32)rect->h;
        for (Int32 index1 = 0; index1 < h; ++index1)
        {
            for (Int32 index2 = 0; index2 < w; ++index2)
            {
                Int32 index3 = index1 * w + index2;
                Int32 index4 = ((Int32)rect->y + index1) * 1024 + ((Int32)rect->x + index2);
                if (PSX_LIBGPU.originalVram == null || PSX_LIBGPU.originalVram.Length == 0)
                {
                    throw new AccessViolationException();
                }
                else
                    originalVram[index4] = numPtr1[index3];
            }
        }
        return 0;
    }

    public unsafe static void AddPrim(void* in_ot, void* in_p)
    {
        ((P_TAG*)in_p)->addr = ((P_TAG*)in_ot)->addr;
        ((P_TAG*)in_ot)->addr = ((UInt32)in_p & 16777215u);
        PSX_OT_MGR.AddPrim(in_ot, in_p);
    }

public unsafe static UInt32* ClearOTagR(UInt32* ot, UInt32 n)
    {
        PSX_OT_MGR.ClearOTag(ot, n);
        if (n <= 0u)
        {
            return null;
        }
        Int32 num = (Int32)(n - 1u);
        for (Int32 i = num; i > 0; i--)
        {
            ot[i] = (UInt32)(ot + (i - 1));
        }
        *ot = 16777215u;
        return ot;
    }

public static UInt16[] originalVram = new UInt16[524288];

public static Boolean isInitVram = false;

public static Boolean isVramDirty = true;

public struct RECT
    {
        public Int16 x;

        public Int16 y;

        public Int16 w;

        public Int16 h;
    }

public struct RECT32
    {
        public Int32 x;

        public Int32 y;

        public Int32 w;

        public Int32 h;
    }

public struct DR_ENV
    {
        public UInt32 tag;

        public unsafe fixed UInt32 code [15];
    }

public struct DRAWENV
    {
        public RECT clip;

        public unsafe fixed Int16 ofs [2];

        public RECT tw;

        public UInt16 tpage;

        public Byte dtd;

        public Byte dfe;

        public Byte isbg;

        public Byte r0;

        public Byte g0;

        public Byte b0;

        public DR_ENV dr_env;
    }

public struct DISPENV
    {
        public RECT disp;

        public RECT screen;

        public Byte isinter;

        public Byte isrgb24;

        public Byte pad0;

        public Byte pad1;
    }

public struct P_TAG
    {
        public UInt32 getAddr()
        {
            return this.addr_len & 16777215u;
        }

        public UInt32 getLen()
        {
            return this.addr_len >> 24 & 255u;
        }

        public UInt32 addr
        {
            get { return this.addr_len & 16777215u; }
            set
            {
                UInt32 num = this.addr_len & 4278190080u;
                UInt32 num2 = value & 268435455u;
                this.addr_len = (num | num2);
            }
        }

        public UInt32 len
        {
            get { return this.addr_len >> 24 & 255u; }
            set
            {
                UInt32 num = value << 24 & 4278190080u;
                UInt32 num2 = this.addr_len & 268435455u;
                this.addr_len = (num | num2);
            }
        }

        private UInt32 addr_len;

        public Byte r0;

        public Byte g0;

        public Byte b0;

        public Byte code;
    }

public struct P_CODE
    {
        public Byte r0;

        public Byte g0;

        public Byte b0;

        public Byte code;
    }

public struct POLY_F3
    {
        public UInt32 tag;

        public Byte r0;

        public Byte g0;

        public Byte b0;

        public Byte code;

        public Int16 x0;

        public Int16 y0;

        public Int16 x1;

        public Int16 y1;

        public Int16 x2;

        public Int16 y2;
    }

public struct POLY_F4
    {
        public UInt32 tag;

        public Byte r0;

        public Byte g0;

        public Byte b0;

        public Byte code;

        public Int16 x0;

        public Int16 y0;

        public Int16 x1;

        public Int16 y1;

        public Int16 x2;

        public Int16 y2;

        public Int16 x3;

        public Int16 y3;
    }

[StructLayout(LayoutKind.Explicit)]
    public struct POLY_FT3
    {
        [FieldOffset(0)] public UInt32 tag;

        [FieldOffset(4)] public Color32 rgbc0;

        [FieldOffset(4)] public Byte r0;

        [FieldOffset(5)] public Byte g0;

        [FieldOffset(6)] public Byte b0;

        [FieldOffset(7)] public Byte code;

        [FieldOffset(8)] public XY xy0;

        [FieldOffset(8)] public Int16 x0;

        [FieldOffset(10)] public Int16 y0;

        [FieldOffset(12)] public UV uv0;

        [FieldOffset(12)] public Byte u0;

        [FieldOffset(13)] public Byte v0;

        [FieldOffset(14)] public UInt16 clut;

        [FieldOffset(16)] public XY xy1;

        [FieldOffset(16)] public Int16 x1;

        [FieldOffset(18)] public Int16 y1;

        [FieldOffset(20)] public UV uv1;

        [FieldOffset(20)] public Byte u1;

        [FieldOffset(21)] public Byte v1;

        [FieldOffset(22)] public UInt16 tpage;

        [FieldOffset(24)] public XY xy2;

        [FieldOffset(24)] public Int16 x2;

        [FieldOffset(26)] public Int16 y2;

        [FieldOffset(28)] public UV uv2;

        [FieldOffset(28)] public Byte u2;

        [FieldOffset(29)] public Byte v2;

        [FieldOffset(30)] public UInt16 pad1;
    }

[StructLayout(LayoutKind.Explicit)]
    public struct POLY_FT4
    {
        [FieldOffset(0)] public UInt32 tag;

        [FieldOffset(4)] public Color32 rgbc0;

        [FieldOffset(4)] public Byte r0;

        [FieldOffset(5)] public Byte g0;

        [FieldOffset(6)] public Byte b0;

        [FieldOffset(7)] public Byte code;

        [FieldOffset(8)] public XY xy0;

        [FieldOffset(8)] public Int16 x0;

        [FieldOffset(10)] public Int16 y0;

        [FieldOffset(12)] public UV uv0;

        [FieldOffset(12)] public Byte u0;

        [FieldOffset(13)] public Byte v0;

        [FieldOffset(14)] public UInt16 clut;

        [FieldOffset(16)] public XY xy1;

        [FieldOffset(16)] public Int16 x1;

        [FieldOffset(18)] public Int16 y1;

        [FieldOffset(20)] public UV uv1;

        [FieldOffset(20)] public Byte u1;

        [FieldOffset(21)] public Byte v1;

        [FieldOffset(22)] public UInt16 tpage;

        [FieldOffset(24)] public XY xy2;

        [FieldOffset(24)] public Int16 x2;

        [FieldOffset(26)] public Int16 y2;

        [FieldOffset(28)] public UV uv2;

        [FieldOffset(28)] public Byte u2;

        [FieldOffset(29)] public Byte v2;

        [FieldOffset(30)] public UInt16 pad2;

        [FieldOffset(32)] public XY xy3;

        [FieldOffset(32)] public Int16 x3;

        [FieldOffset(34)] public Int16 y3;

        [FieldOffset(36)] public UV uv3;

        [FieldOffset(36)] public Byte u3;

        [FieldOffset(37)] public Byte v3;

        [FieldOffset(38)] public UInt16 pad3;
    }

public struct POLY_G3
    {
        public UInt32 tag;

        public Byte r0;

        public Byte g0;

        public Byte b0;

        public Byte code;

        public Int16 x0;

        public Int16 y0;

        public Byte r1;

        public Byte g1;

        public Byte b1;

        public Byte pad1;

        public Int16 x1;

        public Int16 y1;

        public Byte r2;

        public Byte g2;

        public Byte b2;

        public Byte pad2;

        public Int16 x2;

        public Int16 y2;
    }

public struct POLY_G4
    {
        public UInt32 tag;

        public Byte r0;

        public Byte g0;

        public Byte b0;

        public Byte code;

        public Int16 x0;

        public Int16 y0;

        public Byte r1;

        public Byte g1;

        public Byte b1;

        public Byte pad1;

        public Int16 x1;

        public Int16 y1;

        public Byte r2;

        public Byte g2;

        public Byte b2;

        public Byte pad2;

        public Int16 x2;

        public Int16 y2;

        public Byte r3;

        public Byte g3;

        public Byte b3;

        public Byte pad3;

        public Int16 x3;

        public Int16 y3;
    }

[StructLayout(LayoutKind.Explicit)]
    public struct POLY_GT3
    {
        [FieldOffset(0)] public UInt32 tag;

        [FieldOffset(4)] public Color32 rgbc0;

        [FieldOffset(4)] public Byte r0;

        [FieldOffset(5)] public Byte g0;

        [FieldOffset(6)] public Byte b0;

        [FieldOffset(7)] public Byte code;

        [FieldOffset(8)] public XY xy0;

        [FieldOffset(8)] public Int16 x0;

        [FieldOffset(10)] public Int16 y0;

        [FieldOffset(12)] public UV uv0;

        [FieldOffset(12)] public Byte u0;

        [FieldOffset(13)] public Byte v0;

        [FieldOffset(14)] public UInt16 clut;

        [FieldOffset(16)] public Color32 rgbc1;

        [FieldOffset(16)] public Byte r1;

        [FieldOffset(17)] public Byte g1;

        [FieldOffset(18)] public Byte b1;

        [FieldOffset(19)] public Byte p1;

        [FieldOffset(20)] public XY xy1;

        [FieldOffset(20)] public Int16 x1;

        [FieldOffset(22)] public Int16 y1;

        [FieldOffset(24)] public UV uv1;

        [FieldOffset(24)] public Byte u1;

        [FieldOffset(25)] public Byte v1;

        [FieldOffset(26)] public UInt16 tpage;

        [FieldOffset(28)] public Color32 rgbc2;

        [FieldOffset(28)] public Byte r2;

        [FieldOffset(29)] public Byte g2;

        [FieldOffset(30)] public Byte b2;

        [FieldOffset(31)] public Byte p2;

        [FieldOffset(32)] public XY xy2;

        [FieldOffset(32)] public Int16 x2;

        [FieldOffset(34)] public Int16 y2;

        [FieldOffset(36)] public UV uv2;

        [FieldOffset(36)] public Byte u2;

        [FieldOffset(37)] public Byte v2;

        [FieldOffset(38)] public UInt16 pad2;
    }

[StructLayout(LayoutKind.Explicit)]
    public struct POLY_GT4
    {
        [FieldOffset(0)] public UInt32 tag;

        [FieldOffset(4)] public Color32 rgbc0;

        [FieldOffset(4)] public Byte r0;

        [FieldOffset(5)] public Byte g0;

        [FieldOffset(6)] public Byte b0;

        [FieldOffset(7)] public Byte code;

        [FieldOffset(8)] public XY xy0;

        [FieldOffset(8)] public Int16 x0;

        [FieldOffset(10)] public Int16 y0;

        [FieldOffset(12)] public UV uv0;

        [FieldOffset(12)] public Byte u0;

        [FieldOffset(13)] public Byte v0;

        [FieldOffset(14)] public UInt16 clut;

        [FieldOffset(16)] public Color32 rgbc1;

        [FieldOffset(16)] public Byte r1;

        [FieldOffset(17)] public Byte g1;

        [FieldOffset(18)] public Byte b1;

        [FieldOffset(19)] public Byte p1;

        [FieldOffset(20)] public XY xy1;

        [FieldOffset(20)] public Int16 x1;

        [FieldOffset(22)] public Int16 y1;

        [FieldOffset(24)] public UV uv1;

        [FieldOffset(24)] public Byte u1;

        [FieldOffset(25)] public Byte v1;

        [FieldOffset(26)] public UInt16 tpage;

        [FieldOffset(28)] public Color32 rgbc2;

        [FieldOffset(28)] public Byte r2;

        [FieldOffset(29)] public Byte g2;

        [FieldOffset(30)] public Byte b2;

        [FieldOffset(31)] public Byte p2;

        [FieldOffset(32)] public XY xy2;

        [FieldOffset(32)] public Int16 x2;

        [FieldOffset(34)] public Int16 y2;

        [FieldOffset(36)] public UV uv2;

        [FieldOffset(36)] public Byte u2;

        [FieldOffset(37)] public Byte v2;

        [FieldOffset(38)] public UInt16 pad2;

        [FieldOffset(40)] public Color32 rgbc3;

        [FieldOffset(40)] public Byte r3;

        [FieldOffset(41)] public Byte g3;

        [FieldOffset(42)] public Byte b3;

        [FieldOffset(43)] public Byte p3;

        [FieldOffset(44)] public XY xy3;

        [FieldOffset(44)] public Int16 x3;

        [FieldOffset(46)] public Int16 y3;

        [FieldOffset(48)] public UV uv3;

        [FieldOffset(48)] public Byte u3;

        [FieldOffset(49)] public Byte v3;

        [FieldOffset(50)] public UInt16 pad3;
    }

public struct LINE_F2
    {
        public UInt32 tag;

        public Byte r0;

        public Byte g0;

        public Byte b0;

        public Byte code;

        public Int16 x0;

        public Int16 y0;

        public Int16 x1;

        public Int16 y1;
    }

public struct LINE_G2
    {
        public UInt32 tag;

        public Byte r0;

        public Byte g0;

        public Byte b0;

        public Byte code;

        public Int16 x0;

        public Int16 y0;

        public Byte r1;

        public Byte g1;

        public Byte b1;

        public Byte p1;

        public Int16 x1;

        public Int16 y1;
    }

public struct LINE_F3
    {
        public UInt32 tag;

        public Byte r0;

        public Byte g0;

        public Byte b0;

        public Byte code;

        public Int16 x0;

        public Int16 y0;

        public Int16 x1;

        public Int16 y1;

        public Int16 x2;

        public Int16 y2;

        public UInt32 pad;
    }

public struct LINE_G3
    {
        public UInt32 tag;

        public Byte r0;

        public Byte g0;

        public Byte b0;

        public Byte code;

        public Int16 x0;

        public Int16 y0;

        public Byte r1;

        public Byte g1;

        public Byte b1;

        public Byte p1;

        public Int16 x1;

        public Int16 y1;

        public Byte r2;

        public Byte g2;

        public Byte b2;

        public Byte p2;

        public Int16 x2;

        public Int16 y2;

        public UInt32 pad;
    }

public struct LINE_F4
    {
        public UInt32 tag;

        public Byte r0;

        public Byte g0;

        public Byte b0;

        public Byte code;

        public Int16 x0;

        public Int16 y0;

        public Int16 x1;

        public Int16 y1;

        public Int16 x2;

        public Int16 y2;

        public Int16 x3;

        public Int16 y3;

        public UInt32 pad;
    }

public struct LINE_G4
    {
        public UInt32 tag;

        public Byte r0;

        public Byte g0;

        public Byte b0;

        public Byte code;

        public Int16 x0;

        public Int16 y0;

        public Byte r1;

        public Byte g1;

        public Byte b1;

        public Byte p1;

        public Int16 x1;

        public Int16 y1;

        public Byte r2;

        public Byte g2;

        public Byte b2;

        public Byte p2;

        public Int16 x2;

        public Int16 y2;

        public Byte r3;

        public Byte g3;

        public Byte b3;

        public Byte p3;

        public Int16 x3;

        public Int16 y3;

        public UInt32 pad;
    }

public struct XY
    {
        public Int16 x;

        public Int16 y;
    }

public struct UV
    {
        public Byte u;

        public Byte v;
    }

public struct WH
    {
        public Int16 w;

        public Int16 h;
    }

[StructLayout(LayoutKind.Explicit)]
    public struct SPRT
    {
        [FieldOffset(0)] public UInt32 tag;

        [FieldOffset(4)] public Byte r0;

        [FieldOffset(5)] public Byte g0;

        [FieldOffset(6)] public Byte b0;

        [FieldOffset(7)] public Byte code;

        [FieldOffset(4)] public Color32 rgbc;

        [FieldOffset(8)] public Int16 x0;

        [FieldOffset(10)] public Int16 y0;

        [FieldOffset(8)] public XY xy0;

        [FieldOffset(12)] public Byte u0;

        [FieldOffset(13)] public Byte v0;

        [FieldOffset(12)] public UV uv0;

        [FieldOffset(14)] public UInt16 clut;

        [FieldOffset(16)] public Int16 w;

        [FieldOffset(18)] public Int16 h;

        [FieldOffset(16)] public WH wh;
    }

[StructLayout(LayoutKind.Explicit)]
    public struct SPRT_16
    {
        [FieldOffset(4)] public Byte r0;

        [FieldOffset(5)] public Byte g0;

        [FieldOffset(6)] public Byte b0;

        [FieldOffset(7)] public Byte code;

        [FieldOffset(4)] public Color32 rgbc;

        [FieldOffset(8)] public Int16 x0;

        [FieldOffset(10)] public Int16 y0;

        [FieldOffset(8)] public XY xy0;

        [FieldOffset(12)] public Byte u0;

        [FieldOffset(13)] public Byte v0;

        [FieldOffset(12)] public UV uv0;

        [FieldOffset(14)] public UInt16 clut;
    }

[StructLayout(LayoutKind.Explicit)]
    public struct SPRT_8
    {
        [FieldOffset(4)] public Byte r0;

        [FieldOffset(5)] public Byte g0;

        [FieldOffset(6)] public Byte b0;

        [FieldOffset(7)] public Byte code;

        [FieldOffset(4)] public Color32 rgbc;

        [FieldOffset(8)] public Int16 x0;

        [FieldOffset(10)] public Int16 y0;

        [FieldOffset(8)] public XY xy0;

        [FieldOffset(12)] public Byte u0;

        [FieldOffset(13)] public Byte v0;

        [FieldOffset(12)] public UV uv0;

        [FieldOffset(14)] public UInt16 clut;
    }

public struct TILE
    {
        public UInt32 tag;

        public Byte r0;

        public Byte g0;

        public Byte b0;

        public Byte code;

        public Int16 x0;

        public Int16 y0;

        public Int16 w;

        public Int16 h;
    }

public struct TILE_16
    {
        public UInt32 tag;

        public Byte r0;

        public Byte g0;

        public Byte b0;

        public Byte code;

        public Int16 x0;

        public Int16 y0;
    }

public struct TILE_8
    {
        public UInt32 tag;

        public Byte r0;

        public Byte g0;

        public Byte b0;

        public Byte code;

        public Int16 x0;

        public Int16 y0;
    }

public struct TILE_1
    {
        public UInt32 tag;

        public Byte r0;

        public Byte g0;

        public Byte b0;

        public Byte code;

        public Int16 x0;

        public Int16 y0;
    }

public struct DR_MODE
    {
        public UInt32 tag;

        public unsafe fixed UInt32 code [2];
    }

public struct DR_TWIN
    {
        public UInt32 tag;

        public unsafe fixed UInt32 code [2];
    }

public struct DR_AREA
    {
        public UInt32 tag;

        public unsafe fixed UInt32 code [2];
    }

public struct DR_OFFSET
    {
        public UInt32 tag;

        public unsafe fixed UInt32 code [2];
    }

public struct DR_MOVE
    {
        public UInt32 tag;

        public unsafe fixed UInt32 code [5];
    }

public struct DR_LOAD
    {
        public UInt32 tag;

        public unsafe fixed UInt32 code [3];

        public unsafe fixed UInt32 p [13];

    }

public struct DR_TPAGE
    {
        public UInt32 tag;

        public unsafe fixed UInt32 code [1];
    }

public struct DR_STP
    {
        public UInt32 tag;

        public unsafe fixed UInt32 code [2];
    }
}