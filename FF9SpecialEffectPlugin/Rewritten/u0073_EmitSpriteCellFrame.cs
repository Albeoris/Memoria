using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Emits frame <paramref name="a5"/> (clamped to the last of the low-15-bit count at +0) of the cell sprite
    /// <paramref name="a1"/> through EmitSpriteQuadList. Frame header (at the u16 offset a1[3 + frame]): u16 flags
    /// (bit 15 = 24-bit colour follows, bit 14 = u16 x/y scale follows, low 14 bits = cell count), then the optional
    /// fields and the 10-byte cells. Colour: the frame colour scaled by <paramref name="a7"/> (4.12, negative = unscaled;
    /// computed in the IR0 bytes) or grey a7 &gt;&gt; 5 (4096 when negative). Scale: the frame's x/y scaled by
    /// <paramref name="a4"/> through GPF (z from IR3 = 4096), or a4 for x and y (4096 when negative). The sprite MATRIX is
    /// Rz/Rx/Ry of <paramref name="a3"/> (null = zero; [3] != 0 = premultiplied by the camera RT) with columns scaled,
    /// translation = camera RT * <paramref name="a2"/> + TR; blend <paramref name="a6"/> (255 = opaque), x/y origin
    /// <paramref name="a8"/>/<paramref name="a9"/>. Packets come from context dword 9 (advanced by 40 bytes per cell).
    /// </summary>
    public static long EmitSpriteCellFrame(ushort* a1, long* a2, short* a3, int a4, int a5, int a6, int a7, int a8, int a9)
    {
        Recompiler.Runtime.Tracer.Enter("EmitSpriteCellFrame(a1, a2, a3, a4, a5, a6, a7, a8, a9)", (nint)a1, (nint)a2, (nint)a3, a4, a5, a6, a7, a8, a9);
        long ctx = (long)unk_180576A10;
        byte* bytes = (byte*)a1;
        short* rotation = a3;
        long zero = 0;
        if (a3 == null)
        {
            rotation = (short*)&zero;
        }

        int frame = a5;
        int frameCount = *a1 & 0x7FFF;
        if (a5 >= frameCount)
        {
            frame = frameCount - 1;
        }

        // Parameter block of EmitSpriteQuadList (dword 7 is never written by the original).
        Span<uint> block = stackalloc uint[8];
        uint* blockAddress = (uint*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(block));
        new Span<uint>(blockAddress, 8).Clear();
        block[4] = a1[1];
        long header = a1[frame + 3];
        int flags = *(ushort*)(bytes + header);
        int cursor = (int)header + 2;
        if ((flags & 0x8000) == 0)
        {
            int level = a7 < 0 ? 4096 : a7;
            int grey = (byte)(level >> 5);
            block[0] = (uint)(grey | ((grey | (grey << 8)) << 8));
        }
        else
        {
            int colourOffset = cursor;
            cursor += 4;
            int colour = *(ushort*)(bytes + colourOffset) | (bytes[colourOffset + 2] << 16);
            block[0] = (uint)colour;
            if (a7 >= 0)
            {
                byte* ir0 = (byte*)Unsafe.AsPointer(ref dword_180211FE0);
                ir0[0] = (byte)((a7 * (byte)colour) >> 12);
                ir0[1] = (byte)((a7 * (byte)(colour >> 8)) >> 12);
                ir0[3] = (byte)(colour >> 24);
                ir0[2] = (byte)((a7 * (byte)(colour >> 16)) >> 12);
                block[0] = (uint)dword_180211FE0;
            }
        }

        int scaleX;
        int scaleY;
        int scaleZ = 4096;
        if ((flags & 0x4000) != 0)
        {
            scaleX = *(ushort*)(bytes + cursor);
            scaleY = *(ushort*)(bytes + cursor + 2);
            if (a4 >= 0)
            {
                int* ir23 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
                dword_180211FE0 = a4;
                dword_180211FE4 = scaleX;
                ir23[0] = scaleY;
                ir23[1] = 4096;
                GteGpf();
                scaleX = (short)dword_180211FE4;
                scaleY = (short)ir23[0];
                scaleZ = (short)ir23[1];
                dword_180211FE4 = (short)dword_180211FE4;
                ir23[0] = (short)ir23[0];
                ir23[1] = (short)ir23[1];
            }

            cursor += 4;
        }
        else
        {
            scaleX = a4 < 0 ? 4096 : a4;
            scaleY = scaleX;
        }

        uint* context = (uint*)off_180066C68;
        int cellCount = flags & 0x3FFF;
        block[5] = (uint)cellCount;
        ulong cells = (ulong)(bytes + cursor);

        // Camera matrix (context dword 5) into RT/TR, V0 = position.
        byte* camera = (byte*)PsxAddrToPtr(ctx, context[5]);
        System.Buffer.MemoryCopy(camera, xmmword_180211F40, 16, 16);
        dword_180211F50 = ((Camera6*)camera)->field_16;
        camera = (byte*)PsxAddrToPtr(ctx, context[5]);
        *(uint*)Unsafe.AsPointer(ref qword_180211F54) = ((Camera6*)camera)->field_20;
        *(uint*)unk_180211F58 = ((Camera6*)camera)->field_24;
        dword_180211F5C = ((Camera6*)camera)->field_28;
        qword_180211FC0 = *a2;

        uint packetAddress = context[9];
        byte* packets = (byte*)PsxAddrToPtr(ctx, packetAddress);
        context[9] = (uint)(packetAddress + 40 * cellCount);
        GteRtV0Tr();

        // Sprite MATRIX: identity rotated by Rz, Rx, Ry, columns scaled by (x, y, z); translation = MAC1..3.
        Stack32_EmitSpriteAnimFrame_m m = default;
        new Span<short>(((short*)&m), 16).Clear();
        short rz = rotation[2];
        m.field_6 = 0x10000000;
        m.field_0 = 4096;
        m.field_4 = 0;
        m.union_10_8 = 0x1000000000000000UL;
        short* rotated = (short*)RotMatrixZ(rz, ((short*)&m));
        rotated = (short*)RotMatrixX(rotation[0], rotated);
        RotMatrixY(rotation[1], rotated);

        int ty = dword_180212028;
        int tz = dword_18021202C;
        for (int row = 0; row < 3; row++)
        {
            ((short*)&m)[3 * row] = (short)((scaleX * ((short*)&m)[3 * row]) >> 12);
            ((short*)&m)[3 * row + 1] = (short)((scaleY * ((short*)&m)[3 * row + 1]) >> 12);
            ((short*)&m)[3 * row + 2] = (short)((scaleZ * ((short*)&m)[3 * row + 2]) >> 12);
        }

        int tx = dword_180212024;
        m.field_20 = tx;
        m.field_24 = ty;
        m.field_28 = tz;
        if (rotation[3] != 0)
        {
            // m.rotation = RT(camera) * m.rotation, from a copy of the GTE RT/TR registers.
            Stack32_EmitSpriteAnimFrame_rt rt = default;
            System.Buffer.MemoryCopy(xmmword_180211F40, ((short*)&rt), 16, 16);
            rt.field_16 = *(long*)Unsafe.AsPointer(ref dword_180211F50);
            rt.field_24 = *(uint*)unk_180211F58;
            rt.field_28 = dword_180211F5C;
            MulMatrix0(((short*)&rt), ((short*)&m), (ushort*)((short*)&m));
        }

        *(int*)Unsafe.AsPointer(ref qword_180211F54) = tx;
        dword_180211F50 = (ushort)m.union_16_2;
        *(int*)unk_180211F58 = ty;
        System.Buffer.MemoryCopy(((short*)&m), xmmword_180211F40, 16, 16);
        dword_180211F5C = tz;
        int blend = a6 == 255 ? 0 : 2;
        block[3] = (uint)(32 * (a6 & 3));
        block[0] |= (uint)((blend | 0x2C) << 24);
        block[1] = (uint)a8;
        block[2] = (uint)a9;
        block[6] = context[10];
        byte* ot = (byte*)PsxAddrToPtr(ctx, context[8]);
        return EmitSpriteQuadList(cells, (ulong)blockAddress, (ulong)packets, (ulong)ot);
    }
}
