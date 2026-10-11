using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Draws frame <paramref name="a5"/> (modulo the count) of sprite set <paramref name="a1"/> at the world point
    /// *<paramref name="a2"/> (VXY, VZ) with the current matrix (context word 5): RTV0TR gives the centre, RTPS its
    /// depth, and RT/TR are left as Ry * Rx * Rz(a3[0..2]) scaled by <paramref name="a4"/>/4096 (when a4 &gt;= 0),
    /// pre-multiplied by the old RT when a3[3] != 0, with TR = the centre. Packet size/code by set: no tpage → dots
    /// (12 bytes, 0x68), size 8x8 → 0x74, 16x16 → 0x7C (16 bytes), else SPRT 0x64 (20 bytes); bit 1 unless
    /// <paramref name="a6"/> is 255. The 40-byte parameter block (+0 uv table, +4 colour table, +8/+12 size - 1,
    /// +20 clut, +24 depth cue a7, +28 count, +36 code) goes to EmitSpriteBatch / EmitFadedDots / BuildSpritePrims
    /// at OT entry ((SZ3 &gt;&gt; 2) - ctx[10]) &gt;&gt; 2, then the draw-mode packet is linked there. Returns the
    /// link's xor mask, or a7 when the entry is out of range or the frame is empty. a3 null means zero angles.
    /// </summary>
    public static long EmitWorldSpriteFrame(long a1, long* a2, short* a3, int a4, int a5, int a6, uint a7)
    {
        Recompiler.Runtime.Tracer.Enter("EmitWorldSpriteFrame(a1, a2, a3, a4, a5, a6, a7)", a1, (nint)a2, (nint)a3, a4, a5, a6, a7);
        byte* set = (byte*)a1;
        long zeroAngles = 0;
        short* angles = a3 != null ? a3 : (short*)&zeroAngles;
        int frames = *(ushort*)set & 0x7FFF;
        int frame = a5 >= frames ? a5 % frames : a5;
        long uvCountOffset = (2 * frames) + 8;
        int uvCount = *(ushort*)(set + uvCountOffset);
        long context = (long)unk_180576A10;
        uint uvTable = (uint)PsxAddrMapperRegister(context, (ulong)(set + uvCountOffset + 2));
        int tpage = *(ushort*)(set + 2);
        uint* ctx = (uint*)off_180066C68;
        int size = *(ushort*)(set + 6);

        Span<uint> parameters = stackalloc uint[10];
        uint* parametersAddress = (uint*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(parameters));
        new Span<uint>(parametersAddress, 10).Clear();
        parameters[0] = uvTable;
        parameters[1] = uvTable + (uint)(2 * uvCount) + 2;
        parameters[5] = *(ushort*)(set + 4);
        parameters[3] = (uint)(set[6] - 1);
        parameters[2] = (uint)(set[7] - 1);
        long frameOffset = *(ushort*)(set + (2 * frame) + 8);
        int count = set[frameOffset];
        parameters[7] = (uint)count;
        byte* records = set + (int)(frameOffset + 1);

        GslLoadMatrix(ResolveGlobalPsxAddress(ctx[5]));
        qword_180211FC0 = *a2;
        int packetSize;
        uint code;
        if (tpage != 0)
        {
            if (size == 0x808)
            {
                packetSize = 16;
                code = 116;
            }
            else if (size == 0x1010)
            {
                packetSize = 16;
                code = 124;
            }
            else
            {
                packetSize = 20;
                code = 100;
            }
        }
        else
        {
            packetSize = 12;
            code = 104;
        }

        uint cursor = ctx[9];
        byte* packets = ResolveGlobalPsxAddress(cursor);
        ctx[9] = cursor + (uint)(count * packetSize);
        GteRtV0Tr();

        Stack20_DrawRingMeshEffect_v74 matrix = default;
        new Span<short>(((short*)&matrix), 10).Clear();
        matrix.union_0_2 = 4096;
        matrix.union_8_2 = 4096;
        matrix.union_16_2 = 4096;
        short* m = (short*)RotMatrixZ(angles[2], ((short*)&matrix));
        m = (short*)RotMatrixX(angles[0], m);
        RotMatrixY(angles[1], m);
        if (a4 >= 0)
        {
            for (int i = 0; i < 9; i++)
            {
                ((short*)&matrix)[i] = (short)((a4 * ((short*)&matrix)[i]) >> 12);
            }
        }

        int centerX = dword_180212024;
        int centerY = dword_180212028;
        int centerZ = dword_18021202C;
        if (a6 != 255)
        {
            code |= 2;
        }

        GteRtps(0);
        parameters[9] = code;
        byte* mode = PsxAllocDrawMode(tpage, (sbyte)a6);
        Mode3* mode3 = (Mode3*)mode;
        uint depth = (uint)dword_18021200C >> 2;
        if (angles[3] != 0)
        {
            byte* current = stackalloc byte[32];
            System.Buffer.MemoryCopy(xmmword_180211F40, current, 32, 32);
            MulMatrix0((short*)current, ((short*)&matrix), (ushort*)((short*)&matrix));
        }

        dword_180211F50 = (ushort)matrix.union_16_2;
        *(UInt128*)xmmword_180211F40 = *(UInt128*)((short*)&matrix);
        int* translation = (int*)Unsafe.AsPointer(ref qword_180211F54);
        translation[0] = centerX;
        translation[1] = centerY;
        translation[2] = centerZ;
        parameters[6] = a7;
        int index = (int)(depth - ctx[10]) >> 2;
        if ((uint)index >= 0x1000 || count == 0)
        {
            return a7;
        }

        long offset = 4L * index;
        long entry = PsxAddrToPtr(context, ctx[8]) + offset;
        if (packetSize == 20)
        {
            EmitSpriteBatch((ulong)records, (ulong)parametersAddress, (ulong)packets, (ulong)entry);
        }
        else if (packetSize == 12)
        {
            EmitFadedDots((ulong)records, (ulong)parametersAddress, (ulong)packets, (ulong)entry);
        }
        else
        {
            BuildSpritePrims((ulong)records, (ulong)parametersAddress, (ulong)packets, (ulong)entry);
        }

        uint* head = (uint*)(ResolveGlobalPsxAddress(((uint*)off_180066C68)[8]) + offset);
        mode[3] = 1;
        mode3->union_0_4 ^= (*head ^ mode3->union_0_4) & 0xFFFFFF;
        uint mask = (*head ^ (uint)PsxPtrToAddr(context, (ulong)mode)) & 0xFFFFFF;
        *head ^= mask;
        return mask;
    }
}
