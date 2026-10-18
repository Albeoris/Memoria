namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Draws frame <paramref name="a3"/> (clamped to the last) of sprite list set <paramref name="a1"/> at the world
    /// point a2 (VXY, VZ) projected with the current matrix (context word 5, RTPS): the header gets the colour, the
    /// screen position SXY2, blend bits (a4 &amp; 3) &lt;&lt; 5, clut a1[1], the record count and the mode cursor
    /// after the packets; PsxAddSpriteList fills the packets at OT entry ((SZ3 &gt;&gt; 2) - ctx[10]) &gt;&gt; 2 and
    /// the packet cursor (ctx[9]) becomes its returned mode cursor. Returns that PSX address, or ctx[9] when the
    /// entry is out of range.
    /// </summary>
    public static long EmitWorldSpriteList(ushort* a1, int* a2, int a3, int a4, int a5)
    {
        Recompiler.Runtime.Tracer.Enter("EmitWorldSpriteList(a1, a2, a3, a4, a5)", (nint)a1, (nint)a2, a3, a4, a5);
        int frames = *a1 & 0x7FFF;
        if (a3 >= frames)
        {
            a3 = frames - 1;
        }

        SpriteListHeader header = default;
        header.clut = a1[1];
        int offset = a1[a3 + 3];
        int flags = *(ushort*)((byte*)a1 + offset);
        offset += 2;
        header.colorCode = EmitWorldSpriteListColor((byte*)a1, ref offset, flags, a5);
        if ((flags & 0x4000) != 0)
        {
            offset += 4;
        }

        uint* ctx = (uint*)off_180066C68;
        GslLoadMatrix(ResolveGlobalPsxAddress(ctx[5]));
        int count = flags & 0x3FFF;
        long* v0 = (long*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref qword_180211FC0);
        ((int*)v0)[0] = a2[0];
        ((int*)v0)[1] = a2[1];
        ulong records = (ulong)a1 + (ulong)(long)offset;
        GteRtps(0);
        uint cursor = ctx[9];
        byte* packets = ResolveGlobalPsxAddress(cursor);
        ctx[9] = cursor + (uint)(20 * count);
        int code = a4 == 255 ? 0 : 2;
        header.modeBits = (uint)(32 * (a4 & 3));
        header.colorCode |= (uint)((code | 0x64) << 24);
        header.x = (short)dword_180211FF8;
        header.y = dword_180211FF8 >> 16;
        header.count = count;
        long result = ctx[9];
        header.modeCursor = ctx[9];
        int index = (int)(((uint)dword_18021200C >> 2) - ctx[10]) >> 2;
        if ((uint)index < 0x1000)
        {
            byte* ot = ResolveGlobalPsxAddress(ctx[8]);
            byte* modeCursor = PsxAddSpriteList(records, (ulong)&header, (ulong)packets, (ulong)(ot + (4 * index)));
            result = PsxAddrMapperRegister((long)unk_180576A10, (ulong)modeCursor);
            ((uint*)off_180066C68)[9] = (uint)result;
        }

        return result;
    }

    /// <summary>
    /// Frame colour: with flag bit 15 the 24-bit colour that follows (4 bytes consumed), scaled by
    /// <paramref name="brightness"/>/4096 through dword_180211FE0 when brightness &gt;= 0; otherwise grey
    /// (brightness, or 4096 when negative) &gt;&gt; 5 in each channel.
    /// </summary>
    private static uint EmitWorldSpriteListColor(byte* set, ref int offset, int flags, int brightness)
    {
        Recompiler.Runtime.Tracer.Enter("EmitWorldSpriteListColor(set, offset, flags, brightness)", (nint)set, offset, flags, brightness);
        if ((flags & 0x8000) == 0)
        {
            int level = brightness < 0 ? 4096 : brightness;
            uint grey = (byte)(level >> 5);
            return grey | ((grey | (grey << 8)) << 8);
        }

        int at = offset;
        offset += 4;
        int rgb = *(ushort*)(set + at) | (set[at + 2] << 16);
        if (brightness < 0)
        {
            return (uint)rgb;
        }

        uint scaled = (byte)((brightness * (byte)rgb) >> 12)
            | ((uint)(byte)((brightness * (byte)(rgb >> 8)) >> 12) << 8)
            | ((uint)(byte)((brightness * (byte)(rgb >> 16)) >> 12) << 16)
            | ((uint)(rgb >> 24) << 24);
        dword_180211FE0 = (int)scaled;
        return scaled;
    }

    /// <summary>
    /// Draws frame <paramref name="a5"/> (clamped to the last) of sprite list set <paramref name="a2"/> at screen
    /// position (<paramref name="a3"/>, <paramref name="a4"/>) into OT entry <paramref name="a1"/> (when below 4096)
    /// with blend <paramref name="a6"/> and brightness <paramref name="a7"/>, like EmitWorldSpriteList without the
    /// projection. Returns the new packet cursor, or the code word ((2 unless a6 is 255) | 0x64) &lt;&lt; 24 when the
    /// entry is out of range.
    /// </summary>
    public static long EmitScreenSpriteList(int a1, ushort* a2, int a3, int a4, int a5, int a6, int a7)
    {
        Recompiler.Runtime.Tracer.Enter("EmitScreenSpriteList(a1, a2, a3, a4, a5, a6, a7)", a1, (nint)a2, a3, a4, a5, a6, a7);
        long index = a1;
        int frames = *a2 & 0x7FFF;
        int frame = a5 >= frames ? frames - 1 : a5;
        SpriteListHeader header = default;
        header.clut = a2[1];
        int offset = a2[frame + 3];
        int flags = *(ushort*)((byte*)a2 + offset);
        offset += 2;
        uint color = EmitWorldSpriteListColor((byte*)a2, ref offset, flags, a7);
        if ((flags & 0x4000) != 0)
        {
            offset += 4;
        }

        uint* ctx = (uint*)off_180066C68;
        int count = flags & 0x3FFF;
        uint cursor = ctx[9];
        ulong records = (ulong)a2 + (ulong)(long)offset;
        header.count = count;
        byte* packets = ResolveGlobalPsxAddress(cursor);
        header.x = a3;
        header.y = a4;
        ctx[9] = cursor + (uint)(20 * count);
        header.modeCursor = cursor + (uint)(20 * count);
        int code = a6 == 255 ? 0 : 2;
        long result = (uint)((code | 0x64) << 24);
        header.modeBits = (uint)(32 * (a6 & 3));
        header.colorCode = (uint)result | color;
        if ((uint)index < 0x1000)
        {
            byte* ot = ResolveGlobalPsxAddress(ctx[8]);
            byte* modeCursor = PsxAddSpriteList(records, (ulong)&header, (ulong)packets, (ulong)(ot + (4 * index)));
            result = PsxAddrMapperRegister((long)unk_180576A10, (ulong)modeCursor);
            ((uint*)off_180066C68)[9] = (uint)result;
        }

        return result;
    }
}
