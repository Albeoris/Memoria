using Recompiler.Runtime;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Builds the 44-byte strip descriptor on the stack and emits a depth-cued ribbon through the effect context
    /// off_180066C68 (u32 PSX addresses: [5] matrix, [8] ordering table, [9] packet cursor, [10] OTZ bias,
    /// [11] object whose s16 at +0x50 is the roll angle). <paramref name="a9"/> (4096 when negative) scales the RGB of
    /// the near color <paramref name="a10"/> and the far color <paramref name="a11"/> (staged in dword_180211FE0).
    /// <paramref name="a7"/> &gt;= 0: textured POLY_GT4 strip (52 bytes per segment, code 0x3C | 2 unless
    /// <paramref name="a12"/> is 255), tpage = a7 | (a12 &amp; 3) &lt;&lt; 5, clut = a8, V base/scale a3/a5, U a4 &lt;&lt; 8 /
    /// (a4 + a6) &lt;&lt; 8, half-width from sin/cos of (1024 - roll) scaled by 4096.8. Otherwise a LINE_G2 strip
    /// (28 bytes per segment, code 0x50 | 2). Loads the context matrix into the GTE, then, for a non-zero segment count
    /// <paramref name="a2"/>, sets IR0 start/step to 4096 / a2 and calls the strip emitter. With a2 == 0 the return is
    /// the low byte left in eax by the matrix address resolution.
    /// </summary>
    public static sbyte EmitDepthCuedRibbon(ulong a1, int a2, short a3, short a4, short a5, short a6, int a7, int a8, int a9, byte* a10, byte* a11, int a12)
    {
        Tracer.Enter("EmitDepthCuedRibbon(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, …)", a1, a2, a3, a4, a5, a6, a7, a8, a9, (nint)a10);
        int scale = a9 < 0 ? 4096 : a9;
        uint* ctx = (uint*)off_180066C68;

        byte* color = (byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref dword_180211FE0);
        color[0] = (byte)((scale * a10[0]) >> 12);
        color[1] = (byte)((scale * a10[1]) >> 12);
        color[2] = (byte)((scale * a10[2]) >> 12);
        color[3] = a10[3];
        int nearColor = dword_180211FE0;
        color[0] = (byte)((scale * a11[0]) >> 12);
        color[1] = (byte)((scale * a11[1]) >> 12);
        color[2] = (byte)((scale * a11[2]) >> 12);
        color[3] = a11[3];

        Stack44_EmitDepthCuedRibbon_desc desc = default;
        byte code;
        uint cursor = ctx[9];
        byte* packet;
        if (a7 >= 0)
        {
            packet = ResolveGlobalPsxAddress(cursor);
            uint obj = ctx[11];
            ctx[9] = cursor + (uint)(52 * a2);
            desc.field_36 = (short)(a4 << 8);
            desc.field_38 = (short)((a4 + a6) << 8);

            double unit = BitConverter.Int64BitsToDouble(qword_18004B6C8);
            double two = BitConverter.Int64BitsToDouble(qword_18004B698);
            double pi = BitConverter.Int64BitsToDouble(qword_18004B6A0);
            int roll = *(short*)(ResolveGlobalPsxAddress(obj) + 0x50);
            int sinPart = X86.Cvttsd2si(Math.Sin((double)(1024 - roll) / unit * two * pi) * unit);
            roll = *(short*)(ResolveGlobalPsxAddress(obj) + 0x50);
            int cosPart = X86.Cvttsd2si(Math.Cos((double)(1024 - roll) / unit * two * pi) * unit);
            desc.field_40 = (ushort)sinPart | (int)((uint)(cosPart << 16) & 0xFFFF0000u);

            code = (byte)((a12 == 255 ? 0 : 2) | 0x3C);
            desc.field_24 = (a7 | (32 * (a12 & 3))) << 16;
            desc.field_28 = a8 << 16;
            desc.field_20 = a3;
            desc.field_22 = a5;
        }
        else
        {
            packet = ResolveGlobalPsxAddress(cursor);
            ctx[9] = cursor + (uint)(28 * a2);
            desc.field_40 = 32 * (a12 & 3);
            code = (byte)((a12 == 255 ? 0 : 2) | 0x50);
        }

        desc.union_12_4 = nearColor;
        desc.field_16 = dword_180211FE0;
        desc.union_15_1 = code;

        byte* matrix = ResolveGlobalPsxAddress(ctx[5]);
        Matrix2* matrix2 = (Matrix2*)matrix;
        byte* gte = xmmword_180211F40;
        *(int*)gte = matrix2->field_0;
        *(int*)(gte + 4) = matrix2->field_4;
        *(int*)(gte + 8) = matrix2->field_8;
        *(int*)(gte + 12) = matrix2->field_12;
        dword_180211F50 = matrix2->field_16;
        uint matrixAddress = ctx[5];
        byte* translation = ResolveGlobalPsxAddress(matrixAddress);
        Translation3* translation3 = (Translation3*)translation;
        *(int*)(gte + 20) = translation3->field_20;
        *(int*)(gte + 24) = translation3->field_24;
        *(int*)(gte + 28) = translation3->field_28;

        if (a2 == 0)
        {
            return EmitDepthCuedRibbonResidualAl(matrixAddress);
        }

        int step = 4096 / a2;
        desc.field_32 = step;
        desc.field_8 = step;
        desc.field_4 = (int)PsxAddrMapperRegister((long)unk_180576A10, a1);
        desc.field_0 = ((uint*)off_180066C68)[10];
        byte* ot = ResolveGlobalPsxAddress(((uint*)off_180066C68)[8]);
        if (a7 >= 0)
        {
            return (sbyte)EmitDepthCuedQuadStrip(a2, (ulong)((byte*)&desc), (ulong)packet, (ulong)ot);
        }

        return EmitDepthCuedLineStrip(a2, (ulong)((byte*)&desc), (ulong)packet, (ulong)ot);
    }

    /// <summary>
    /// Low byte of eax after the inlined resolution of the matrix address (the original returns it uninitialised when
    /// the segment count is 0): the segment index &lt;&lt; 5 for a 0x..C0xxxx segment address, otherwise the address's
    /// low byte (the RAM mask and the scratchpad offset keep it).
    /// </summary>
    private static sbyte EmitDepthCuedRibbonResidualAl(uint address)
    {
        Tracer.Enter("EmitDepthCuedRibbonResidualAl(address)", address);
        uint segment = address >> 24;
        if (segment != 128 && (address & 0xC00000) == 0xC00000)
        {
            return (sbyte)(byte)(segment << 5);
        }

        return (sbyte)(byte)address;
    }
}
