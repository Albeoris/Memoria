using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Draws frame <paramref name="a5"/> (clamped to the last) of the scaled sprite set <paramref name="a1"/> at
    /// world point *<paramref name="a2"/>: TR = RTV0TR of the point with the current matrix (context word 5), RT =
    /// Ry * Rx * Rz(a3[0..2]) with columns scaled by the frame's x/y/z scales (y 4096; with <paramref name="a4"/>
    /// &gt;= 0 all three pass through GTE GPF with IR0 = a4), pre-multiplied by the old RT when a3[3] != 0. Set: +0
    /// frame count, +2 s16 offset / 4, +4 mode (0: textured quad via EmitSpriteQuad, 40 bytes; else ground
    /// triangle fan via PsxAddGroundTriFan, 92 bytes), +6 colour count, +8 frame record, scale table (x, z u16
    /// pairs) at +20 + 2 * (mode ? 4 : 2) * colours, uv pairs after it. Header: code 0x2C/0x30 (+2 unless
    /// <paramref name="a6"/> is 255), depth base ctx[10], depth cue <paramref name="a7"/>, blend bits, offset,
    /// colour index frame * (colours - 1) / (frames - 1). Returns the emitter's result. a3 null means zero angles.
    /// </summary>
    public static long EmitScaledSpriteFrame(ushort* a1, long* a2, short* a3, int a4, int a5, int a6, int a7)
    {
        Recompiler.Runtime.Tracer.Enter("EmitScaledSpriteFrame(a1, a2, a3, a4, a5, a6, a7)", (nint)a1, (nint)a2, (nint)a3, a4, a5, a6, a7);
        long zeroAngles = 0;
        short* angles = a3 != null ? a3 : (short*)&zeroAngles;
        int frame = a5;
        int colors = a1[3];
        int last = a1[0] - 1;
        if (a5 > last)
        {
            frame = last;
        }

        int colorIndex = last != 0 ? frame * (colors - 1) / last : 0;
        int offset = 4 * (short)a1[1];
        int tableWords = a1[2] != 0 ? 4 * colors : 2 * colors;
        int scaleY = 4096;
        byte* scales = (byte*)(a1 + tableWords + 10);
        long entry = 4L * frame;
        int scaleZ = *(ushort*)(scales + entry);
        int scaleX = *(ushort*)(scales + entry + 2);
        if (a4 >= 0)
        {
            int* ir = (int*)Unsafe.AsPointer(ref dword_180211FE4);
            dword_180211FE0 = a4;
            ir[0] = scaleX;
            ir[1] = 4096;
            ir[2] = scaleZ;
            GteGpf();
            scaleZ = (short)ir[2];
            scaleX = (short)ir[0];
            ir[0] = scaleX;
            scaleY = (short)ir[1];
            ir[1] = scaleY;
            ir[2] = scaleZ;
        }

        uint* ctx = (uint*)off_180066C68;
        GslLoadMatrix(ResolveGlobalPsxAddress(ctx[5]));
        qword_180211FC0 = *a2;
        GteRtV0Tr();
        Stack20_DrawRingMeshEffect_v74 matrix = default;
        new Span<short>(((short*)&matrix), 10).Clear();
        matrix.union_0_2 = 4096;
        matrix.union_8_2 = 4096;
        matrix.union_16_2 = 4096;
        short* m = (short*)RotMatrixZ(angles[2], ((short*)&matrix));
        m = (short*)RotMatrixX(angles[0], m);
        RotMatrixY(angles[1], m);
        for (int row = 0; row < 3; row++)
        {
            ((short*)&matrix)[(3 * row) + 0] = (short)((scaleX * ((short*)&matrix)[(3 * row) + 0]) >> 12);
            ((short*)&matrix)[(3 * row) + 1] = (short)((scaleY * ((short*)&matrix)[(3 * row) + 1]) >> 12);
            ((short*)&matrix)[(3 * row) + 2] = (short)((scaleZ * ((short*)&matrix)[(3 * row) + 2]) >> 12);
        }

        int trX = dword_180212024;
        int trY = dword_180212028;
        int trZ = dword_18021202C;
        if (angles[3] != 0)
        {
            byte* current = stackalloc byte[32];
            System.Buffer.MemoryCopy(xmmword_180211F40, current, 32, 32);
            MulMatrix0((short*)current, ((short*)&matrix), (ushort*)((short*)&matrix));
        }

        int* translation = (int*)Unsafe.AsPointer(ref qword_180211F54);
        translation[0] = trX;
        translation[1] = trY;
        translation[2] = trZ;
        dword_180211F50 = (ushort)matrix.union_16_2;
        *(UInt128*)xmmword_180211F40 = *(UInt128*)((short*)&matrix);

        Span<uint> header = stackalloc uint[10];
        uint* headerAddress = (uint*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(header));
        new Span<uint>(headerAddress, 10).Clear();
        uint code = a6 == 255 ? 0u : 2u;
        if (a1[2] == 0)
        {
            uint uvAddress = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)(scales + (2 * frame) + (4 * last) + 4));
            header[3] = (uint)offset;
            header[4] = uvAddress;
            header[5] = (uint)colorIndex;
            header[6] = (uint)(32 * (a6 & 3));
            header[0] = (code | 0x2C) << 24;
            header[2] = (uint)a7;
            header[1] = ((uint*)off_180066C68)[10];
            uint cursor = ((uint*)off_180066C68)[9];
            byte* packet = ResolveGlobalPsxAddress(cursor);
            uint ot = ((uint*)off_180066C68)[8];
            ((uint*)off_180066C68)[9] = cursor + 40;
            return EmitSpriteQuad((ulong)(a1 + 4), (ulong)headerAddress, (ulong)packet, (ulong)ResolveGlobalPsxAddress(ot));
        }

        uint fanCursor = ctx[9];
        header[4] = (uint)offset;
        header[5] = (uint)colorIndex;
        header[3] = (uint)(32 * (a6 & 3));
        header[0] = (code | 0x30) << 24;
        header[2] = (uint)a7;
        header[1] = ctx[10];
        byte* fan = ResolveGlobalPsxAddress(fanCursor);
        uint fanOt = ctx[8];
        ctx[9] = fanCursor + 92;
        return PsxAddGroundTriFan((ulong)(a1 + 4), (ulong)headerAddress, (ulong)fan, (ulong)ResolveGlobalPsxAddress(fanOt));
    }
}
