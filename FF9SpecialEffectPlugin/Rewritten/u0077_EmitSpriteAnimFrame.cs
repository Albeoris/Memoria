using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Emits frame <paramref name="a5"/> (modulo the frame count, low 15 bits of u16 +0) of the sprite animation
    /// <paramref name="a1"/> at position <paramref name="a2"/> (V0) with rotation <paramref name="a3"/> (s16 x/y/z,
    /// [3] != 0 = also apply the camera rotation; null = zero), scale <paramref name="a4"/> (4.12, negative = none),
    /// blend <paramref name="a6"/> (255 = opaque) and depth-cue <paramref name="a7"/>. The frame's items (count byte at
    /// the frame offset) go to BuildSpriteQuads with a parameter block built from the header, packets taken from the
    /// context packet cursor (dword 9 of off_180066C68, advanced by 40 bytes per item), OT from dword 8 and depth bias
    /// from dword 10. The GTE ends with RT = sprite matrix, TR = RT(camera) * V0 + TR(camera). Returns the result of
    /// BuildSpriteQuads, or the depth bias dword when the frame is empty.
    /// </summary>
    public static long EmitSpriteAnimFrame(long a1, long* a2, short* a3, int a4, int a5, int a6, int a7)
    {
        Recompiler.Runtime.Tracer.Enter("EmitSpriteAnimFrame(a1, a2, a3, a4, a5, a6, a7)", a1, (nint)a2, (nint)a3, a4, a5, a6, a7);
        long ctx = (long)unk_180576A10;
        short* rotation = a3;
        long zero = 0;
        if (a3 == null)
        {
            rotation = (short*)&zero;
        }

        int frame = a5;
        int frameCount = *(ushort*)a1 & 0x7FFF;
        if (a5 >= frameCount)
        {
            frame = a5 % frameCount;
        }

        long frameSlot = 2 * frame + 8;
        long tableOffset = 2 * frameCount + 8;

        // Parameter block of BuildSpriteQuads: 7 dwords, item count, depth bias, code.
        Span<uint> block = stackalloc uint[10];
        uint* blockAddress = (uint*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(block));
        int uvCount = *(ushort*)(a1 + tableOffset);
        int uvTable = (int)PsxAddrMapperRegister(ctx, (ulong)(a1 + tableOffset + 2));
        int tpage = *(ushort*)(a1 + 2);
        block[0] = (uint)uvTable;
        block[1] = (uint)(uvTable + 2 * uvCount + 2);
        block[5] = *(ushort*)(a1 + 4);
        int halfWidth = *(byte*)(a1 + 6);
        block[4] = (uint)((32 * (a6 & 3)) | tpage);
        block[3] = (uint)(2 * halfWidth - 2);
        block[2] = (uint)(2 * *(byte*)(a1 + 7) - 2);

        uint* context = (uint*)off_180066C68;
        long frameOffset = *(ushort*)(a1 + frameSlot);
        int itemCount = *(byte*)(a1 + frameOffset);
        block[7] = (uint)itemCount;
        ulong items = (ulong)((int)frameOffset + 1 + a1);

        // Camera matrix (context dword 5) into RT/TR, V0 = position.
        byte* camera = (byte*)PsxAddrToPtr(ctx, context[5]);
        System.Buffer.MemoryCopy(camera, xmmword_180211F40, 16, 16);
        dword_180211F50 = ((Camera5*)camera)->field_16;
        camera = (byte*)PsxAddrToPtr(ctx, context[5]);
        *(uint*)Unsafe.AsPointer(ref qword_180211F54) = ((Camera5*)camera)->field_20;
        *(uint*)unk_180211F58 = ((Camera5*)camera)->field_24;
        dword_180211F5C = ((Camera5*)camera)->field_28;
        qword_180211FC0 = *a2;

        uint packetAddress = context[9];
        byte* packets = (byte*)PsxAddrToPtr(ctx, packetAddress);
        context[9] = (uint)(packetAddress + 40 * itemCount);
        GteRtV0Tr();

        // Sprite MATRIX: identity rotated by Rz, Rx, Ry, optionally scaled; translation = MAC1..3.
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
        if (a4 >= 0)
        {
            for (int i = 0; i < 9; i++)
            {
                ((short*)&m)[i] = (short)((a4 * ((short*)&m)[i]) >> 12);
            }
        }

        int tx = dword_180212024;
        int ty = dword_180212028;
        int tz = dword_18021202C;
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

        dword_180211F50 = (ushort)m.union_16_2;
        *(int*)Unsafe.AsPointer(ref qword_180211F54) = tx;
        System.Buffer.MemoryCopy(((short*)&m), xmmword_180211F40, 16, 16);
        int blend = a6 == 255 ? 0 : 2;
        *(int*)unk_180211F58 = ty;
        dword_180211F5C = tz;
        block[9] = (uint)(blend | 0x2C);
        block[6] = (uint)a7;
        long result = context[10];
        block[8] = context[10];
        if (itemCount != 0)
        {
            byte* ot = (byte*)PsxAddrToPtr(ctx, context[8]);
            return BuildSpriteQuads(items, (ulong)blockAddress, (ulong)packets, (ulong)ot);
        }

        return result;
    }
}
