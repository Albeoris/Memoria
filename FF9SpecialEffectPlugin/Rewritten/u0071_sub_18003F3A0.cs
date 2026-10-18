namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// PSX GPU packet helpers over the render context int array at off_180066C68: [8] PSX address of the ordering table,
/// [9] packet cursor (PSX address, bumped by each allocation), [10] depth base subtracted before the OT lookup.
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>
    /// AddPrim: packet a3 gets length byte a1 &gt;&gt; 24 and the OT entry's link, the OT entry a2 points to a3. When a4 is
    /// not 255, an 8-byte DR_MODE packet (0xE1000200 | (a4 &amp; 3) &lt;&lt; 5, length 1) is taken from the cursor and linked
    /// in front of it. Returns the XOR delta applied to the OT entry's low 24 bits.
    /// </summary>
    public static long PsxAddPrimWithMode(int a1, uint* a2, ulong a3, int a4)
    {
        Recompiler.Runtime.Tracer.Enter("PsxAddPrimWithMode(a1, a2, a3, a4)", a1, (nint)a2, a3, a4);
        *(byte*)(a3 + 3) = (byte)((uint)a1 >> 24);
        *(uint*)a3 ^= (*(uint*)a3 ^ *a2) & 0xFFFFFF;
        long result = (*a2 ^ (uint)PsxPtrToAddr((long)unk_180576A10, a3)) & 0xFFFFFF;
        *a2 ^= (uint)result;
        if (a4 != 255)
        {
            uint* context = (uint*)off_180066C68;
            uint cursor = context[9];
            byte* mode = ResolveGlobalPsxAddress(cursor);
            Mode2* mode2 = (Mode2*)mode;
            context[9] = cursor + 8;
            mode[3] = 1;
            uint tag = mode2->union_0_4;
            mode2->field_4 = (uint)(32 * (a4 & 3)) | 0xE1000200;
            mode2->union_0_4 ^= (*a2 ^ tag) & 0xFFFFFF;
            result = (*a2 ^ (uint)PsxPtrToAddr((long)unk_180576A10, (ulong)mode)) & 0xFFFFFF;
            *a2 ^= (uint)result;
        }

        return result;
    }

    /// <summary>
    /// AddPrim of packet a3 (length byte a1 &gt;&gt; 24) at OT entry (a2 - ctx[10]) &gt;&gt; 2 of the table at ctx[8], when that
    /// index is below 4096. Returns the XOR delta of the entry, or the context pointer when the index is out of range.
    /// </summary>
    public static int* PsxAddPrimAtDepth(int a1, int a2, ulong a3)
    {
        Recompiler.Runtime.Tracer.Enter("PsxAddPrimAtDepth(a1, a2, a3)", a1, a2, a3);
        uint* context = (uint*)off_180066C68;
        long index = (uint)((a2 - (int)context[10]) >> 2);
        if ((uint)index >= 0x1000)
        {
            return (int*)context;
        }

        uint* entry = (uint*)(ResolveGlobalPsxAddress(context[8]) + 4 * index);
        *(byte*)(a3 + 3) = (byte)((uint)a1 >> 24);
        *(uint*)a3 ^= (*entry ^ *(uint*)a3) & 0xFFFFFF;
        uint delta = (*entry ^ (uint)PsxPtrToAddr((long)unk_180576A10, a3)) & 0xFFFFFF;
        *entry ^= delta;
        return (int*)delta;
    }

    /// <summary>PsxAddPrimWithMode at OT entry (a2 - ctx[10]) &gt;&gt; 2 when below 4096; otherwise returns the context pointer.</summary>
    public static int* PsxAddPrimAtDepthWithMode(int a1, int a2, ulong a3, int a4)
    {
        Recompiler.Runtime.Tracer.Enter("PsxAddPrimAtDepthWithMode(a1, a2, a3, a4)", a1, a2, a3, a4);
        uint* context = (uint*)off_180066C68;
        long index = (uint)((a2 - (int)context[10]) >> 2);
        if ((uint)index >= 0x1000)
        {
            return (int*)context;
        }

        var entry = (uint*)(ResolveGlobalPsxAddress(context[8]) + 4 * index);
        return (int*)PsxAddPrimWithMode(a1, entry, a3, a4);
    }

    /// <summary>Takes 8 bytes from the packet cursor and sets word 1 to a1 | (a2 &amp; 3) &lt;&lt; 5 | 0xE1000200; returns the packet.</summary>
    public static byte* PsxAllocDrawMode(int a1, sbyte a2)
    {
        Recompiler.Runtime.Tracer.Enter("PsxAllocDrawMode(a1, a2)", a1, a2);
        uint* context = (uint*)off_180066C68;
        uint cursor = context[9];
        byte* packet = ResolveGlobalPsxAddress(cursor);
        Mode3* mode3 = (Mode3*)packet;
        context[9] = cursor + 8;
        mode3->field_4 = (uint)(a1 | (32 * (a2 & 3))) | 0xE1000200;
        return packet;
    }

    /// <summary>
    /// Eight 16-byte TILE packets from the cursor (128 bytes): 80x110 tiles on a 4x2 grid, colour (a3, a4, a5), code 0x62
    /// (semi-transparent) or 0x60 when a2 == 255. Each goes to OT entry 1 when dword_180367754 == 186, otherwise to the
    /// entry a1, through PsxAddPrimWithMode (length 3, mode a2). Returns the last result.
    /// </summary>
    public static long PsxAddScreenTiles(long a1, int a2, byte a3, byte a4, byte a5)
    {
        Recompiler.Runtime.Tracer.Enter("PsxAddScreenTiles(a1, a2, a3, a4, a5)", a1, a2, a3, a4, a5);
        uint* context = (uint*)off_180066C68;
        uint cursor = context[9];
        byte* packets = ResolveGlobalPsxAddress(cursor);
        int scene = dword_180367754;
        byte code = (byte)((a2 == 255 ? 0 : 2) | 0x60);
        context[9] = cursor + 128;

        long result = 0;
        var tile = (PsxTile*)packets;
        for (int i = 0; i < 8; i++, tile++)
        {
            tile->code = code;
            tile->r = a3;
            tile->g = a4;
            tile->b = a5;
            *(uint*)&tile->w = 7209040;
            tile->x = (short)(80 * (i & 3));
            tile->y = (short)(110 * (i >> 2));
            long entry = scene == 186 ? (long)ResolveGlobalPsxAddress(context[8]) + 4 : a1;
            result = PsxAddPrimWithMode(0x3000000, (uint*)entry, (ulong)tile, a2);
        }

        return result;
    }

    /// <summary>
    /// a4 rows x a3 columns of POLY_G4 (36 bytes each, from the cursor) spanning x 0..320, linked into OT entry a1 with
    /// mode a6 (semi-transparent unless 255). Row r: top y = a2 + r*a5 (row 0 flat), wave y offsets
    /// (a8 * (int)(sin(t / 4096.8 * 2pi) * 4096.8)) &gt;&gt; 12 with t advancing by a10 per column and a11 per row from a9;
    /// colours a7[r] (top) and a7[r+1] (bottom), whose code bytes are set to the packet code first.
    /// Returns a7 + 3 + 4 * a4 when rows were drawn, otherwise 9 * a4 * a3.
    /// </summary>
    public static ulong PsxAddWaveGradient(uint* a1, short a2, uint a3, int a4, short a5, int a6, long a7, int a8, int a9, int a10, int a11)
    {
        Recompiler.Runtime.Tracer.Enter("PsxAddWaveGradient(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, …)", (nint)a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
        uint* context = (uint*)off_180066C68;
        uint cursor = context[9];
        var poly = (PsxPolyG4*)ResolveGlobalPsxAddress(cursor);
        ulong result = unchecked((uint)(9 * a4) * a3);
        context[9] = unchecked(cursor + (uint)(36 * a4) * a3);
        if (a4 <= 0)
        {
            return result;
        }

        int columns = (int)a3;
        byte code = (byte)((a6 == 255 ? 0 : 2) | 0x38);
        byte* colourCode = (byte*)(a7 + 3);
        for (int row = 0; row < a4; row++)
        {
            result = (ulong)(colourCode + 4);
            colourCode[0] = code;
            colourCode[4] = code;
            int phase = a9 + a11 * row;
            if (columns > 0)
            {
                int right = 0;
                for (long column = columns; column != 0; column--)
                {
                    int left = right;
                    right += 320;
                    poly->x2 = (short)(left / columns);
                    poly->x0 = (short)(left / columns);
                    poly->x3 = (short)(right / columns);
                    poly->x1 = (short)(right / columns);
                    if (row != 0)
                    {
                        poly->y0 = (short)(a2 + ((a8 * WaveSin(phase)) >> 12));
                        poly->y1 = (short)(a2 + ((a8 * WaveSin(phase + a10)) >> 12));
                    }
                    else
                    {
                        poly->y1 = a2;
                        poly->y0 = a2;
                    }

                    int lower = a11 + phase;
                    poly->y2 = (short)(a5 + a2 + ((a8 * WaveSin(lower)) >> 12));
                    poly->y3 = (short)(a5 + a2 + ((a8 * WaveSin(lower + a10)) >> 12));
                    uint top = *(uint*)(colourCode - 3);
                    poly->rgb1 = top;
                    poly->rgb0 = top;
                    uint bottom = *(uint*)(colourCode + 1);
                    poly->rgb3 = bottom;
                    poly->rgb2 = bottom;
                    PsxAddPrimWithMode(0x8000000, a1, (ulong)poly, a6);
                    poly++;
                    phase = a10 - a11 + lower;
                }
            }

            a2 += a5;
            colourCode = (byte*)result;
        }

        return result;
    }

    /// <summary>(int)(sin(t / 4096.8 * 2 * pi) * 4096.8), all in double.</summary>
    private static int WaveSin(int t)
    {
        Recompiler.Runtime.Tracer.Enter("WaveSin(t)", t);
        return (int)(Math.Sin((double)t / 4096.8 * 2.0 * 3.141592653589793) * 4096.8);
    }
}
