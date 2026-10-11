namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Draws frame <paramref name="a4"/> (modulo the count) of the sprite set <paramref name="a1"/> at
    /// (<paramref name="a2"/>, <paramref name="a3"/>) as 20-byte SPRT packets (code 0x64, semi-transparent unless
    /// <paramref name="a5"/> is 255) behind one draw-mode packet (0xE1000000 | tpage | (a5 &amp; 3) &lt;&lt; 5), all
    /// at depth 4094. Set layout: +0 u16 frame count (bit 15 ignored), +2 tpage, +4 clut, +6/+7 height/width + 1,
    /// +8 u16 frame offsets, then a u16 count + u16 uv table and a 4-byte colour table. Frame: byte count, then
    /// 3-byte records (s8 dx, s8 dy, attr: low nibble uv index, high nibble colour index). Colours are scaled by
    /// <paramref name="a6"/>/4096 through dword_180211FE0 (left holding the last colour word). Returns the draw-mode
    /// AddPrim result.
    /// </summary>
    public static int* EmitSpriteFrame(long a1, short a2, short a3, int a4, int a5, int a6)
    {
        Recompiler.Runtime.Tracer.Enter("EmitSpriteFrame(a1, a2, a3, a4, a5, a6)", a1, a2, a3, a4, a5, a6);
        byte* set = (byte*)a1;
        int frames = *(ushort*)set & 0x7FFF;
        if (a4 >= frames)
        {
            a4 %= frames;
        }

        int tpage = *(ushort*)(set + 2);
        int width = set[7] - 1;
        short widthWord = (short)width;
        long uvCountOffset = (2 * frames) + 8;
        byte* uvs = set + uvCountOffset + 2;
        byte* colors = uvs + (2 * *(ushort*)(set + uvCountOffset)) + 2;
        int clut = *(ushort*)(set + 4) << 16;
        int height = set[6] - 1;
        long frameOffset = *(ushort*)(set + 8 + (2 * (long)a4));
        int count = set[frameOffset];
        sbyte* record = (sbyte*)(set + (int)frameOffset + 1);

        uint* context = (uint*)off_180066C68;
        uint cursor = context[9];
        byte* packets = ResolveGlobalPsxAddress(cursor);
        uint modeAddress = cursor + (uint)(20 * count);
        context[9] = modeAddress;
        byte* mode = ResolveGlobalPsxAddress(modeAddress);
        context[9] = modeAddress + 8;
        byte code = (byte)((a5 == 255 ? 0 : 2) | 0x64);
        ((uint*)mode)[1] = (uint)(tpage | (32 * (a5 & 3))) | 0xE1000000;
        int halfWidth = width >> 1;
        byte* packet = packets;
        for (int i = 0; i < count; i++)
        {
            int dx = record[0];
            int dy = record[1];
            byte attributes = (byte)record[2];
            record += 3;
            *(ushort*)(packet + 8) = (ushort)(a2 + dx - halfWidth);
            *(ushort*)(packet + 10) = (ushort)(a3 + dy - (ushort)(height >> 1));
            int colorOffset = (attributes >> 2) & 0x3C;
            *(uint*)(packet + 12) = (uint)(clut | *(ushort*)(uvs + (2 * (attributes & 0xF))));
            ushort redGreen = *(ushort*)(colors + colorOffset);
            int blue = colors[colorOffset + 2];
            uint color = (uint)(code << 24)
                | (byte)((a6 * (byte)redGreen) >> 12)
                | ((uint)(byte)((a6 * (redGreen >> 8)) >> 12) << 8)
                | ((uint)(byte)((a6 * blue) >> 12) << 16);
            dword_180211FE0 = (int)color;
            *(ushort*)(packet + 16) = (ushort)widthWord;
            *(ushort*)(packet + 18) = (ushort)height;
            *(uint*)(packet + 4) = color;
            PsxAddPrimAtDepth(0x4000000, 4094, (ulong)packet);
            packet += 20;
        }

        return PsxAddPrimAtDepth(0x1000000, 4094, (ulong)mode);
    }

    /// <summary>
    /// Emits a 40-byte POLY_FT4 (code 0x2C, semi-transparent unless a1[4] is 255) at depth 4094. a1: [1] tpage
    /// (ORed in place with (a1[4] &amp; 3) &lt;&lt; 5 unless a1[4] is 255), [2] clut, [4] blend mode. Corners
    /// (x0, y0) = (a2, a4), (x1, y1) = (a2 + a8 + a11, a5), (x2, y2) = (a3, a4 + a9 + a12), (x3, y3) = (a3 + a8 + a11,
    /// a5 + a9 + a12); u = a6 / a6 + a8 + a11, v = a7 / a7 + a9 - a12 (255 when the sum reaches 256); colour from
    /// the 3 bytes at a10. Returns the AddPrim result.
    /// </summary>
    public static int* EmitFt4Quad(ushort* a1, short a2, short a3, short a4, short a5, int a6, int a7, int a8, int a9, long a10, int a11, int a12)
    {
        Recompiler.Runtime.Tracer.Enter("EmitFt4Quad(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, …)", (nint)a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
        uint* context = (uint*)off_180066C68;
        uint cursor = context[9];
        byte* p = ResolveGlobalPsxAddress(cursor);
        Site_ResolveGlobalPsxAddress2* site_ResolveGlobalPsxAddress2 = (Site_ResolveGlobalPsxAddress2*)p;
        context[9] = cursor + 40;
        p[7] = (byte)((a1[4] == 255 ? 0 : 2) | 0x2C);
        site_ResolveGlobalPsxAddress2->field_14 = a1[2];
        if (a1[4] != 255)
        {
            a1[1] |= (ushort)(32 * (a1[4] & 3));
        }

        site_ResolveGlobalPsxAddress2->field_22 = a1[1];
        site_ResolveGlobalPsxAddress2->field_32 = (ushort)(a3 + a8 + a11);
        site_ResolveGlobalPsxAddress2->field_16 = (ushort)(a2 + a8 + a11);
        site_ResolveGlobalPsxAddress2->field_8 = (ushort)a2;
        site_ResolveGlobalPsxAddress2->field_26 = (ushort)(a4 + a9 + a12);
        site_ResolveGlobalPsxAddress2->field_34 = (ushort)(a5 + a9 + a12);
        site_ResolveGlobalPsxAddress2->field_24 = (ushort)a3;
        int uRight = a11 + a6 + a8;
        site_ResolveGlobalPsxAddress2->field_10 = (ushort)a4;
        site_ResolveGlobalPsxAddress2->field_18 = (ushort)a5;
        p[28] = (byte)a6;
        p[12] = (byte)a6;
        if (uRight >= 256)
        {
            uRight--;
        }

        p[36] = (byte)uRight;
        p[20] = (byte)uRight;
        p[21] = (byte)a7;
        p[13] = (byte)a7;
        int vBottom = a9 + a7 - a12;
        if (vBottom >= 256)
        {
            vBottom--;
        }

        p[37] = (byte)vBottom;
        p[29] = (byte)vBottom;
        site_ResolveGlobalPsxAddress2->field_4 = *(ushort*)a10;
        p[6] = *(byte*)(a10 + 2);
        return PsxAddPrimAtDepth(0x9000000, 4094, (ulong)p);
    }

    /// <summary>
    /// Samples the colour ramp <paramref name="a1"/> at time <paramref name="a3"/> (clamped to the total) into the
    /// RGB bytes <paramref name="a2"/>. Ramp: +0 total time, +4 key count (both built on first use from the keys),
    /// +8 8-byte keys (r, g, b, next length, u16 length, u16 start). Blends the last key whose start is not after
    /// a3 with the following one through GTE GPF/GPL (weight ((a3 - start) &lt;&lt; 12) / length). Returns the
    /// sign-extended IR3.
    /// </summary>
    public static long SampleColorRamp(long a1, byte* a2, int a3)
    {
        Recompiler.Runtime.Tracer.Enter("SampleColorRamp(a1, a2, a3)", a1, (nint)a2, a3);
        int count = *(int*)(a1 + 4);
        byte* keys = (byte*)(a1 + 8);
        if (*(int*)a1 == 0)
        {
            int length = keys[3];
            count = 0;
            int total = 0;
            if (length != 0)
            {
                byte* key = keys + 4;
                do
                {
                    *(ushort*)(key + 2) = (ushort)total;
                    *(ushort*)key = (ushort)length;
                    total += length;
                    length = key[7];
                    key += 8;
                    count++;
                }
                while (length != 0);
            }

            *(int*)a1 = total;
            *(int*)(a1 + 4) = count;
        }

        if (a3 > *(int*)a1)
        {
            a3 = *(int*)a1;
        }

        byte* current = keys + (8L * (count - 1));
        for (int i = 0; i < count; i++)
        {
            if (a3 >= *(ushort*)(current + 6))
            {
                break;
            }

            current -= 8;
        }

        int t = ((a3 - *(ushort*)(current + 6)) << 12) / *(ushort*)(current + 4);
        int* ir = (int*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref dword_180211FE4);
        dword_180211FE0 = 4096 - t;
        ir[0] = current[0];
        ir[1] = current[1];
        ir[2] = current[2];
        GteGpf();
        dword_180211FE0 = t;
        ir[0] = current[8];
        ir[1] = current[9];
        ir[2] = current[10];
        GteGpl();
        int red = (short)ir[0];
        ir[0] = red;
        a2[0] = (byte)red;
        int green = (short)ir[1];
        ir[1] = green;
        a2[1] = (byte)green;
        int blue = (short)ir[2];
        ir[2] = blue;
        a2[2] = (byte)blue;
        return (uint)blue;
    }
}
