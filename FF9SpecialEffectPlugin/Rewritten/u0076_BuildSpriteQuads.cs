using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Emits textured sprite quads (POLY_FT4, 10 words) from host buffers it first registers with the PSX mapper:
    /// <paramref name="a1"/> = 3-byte items (s8 dx, s8 dy, byte: low nibble UV index, high nibble colour index),
    /// <paramref name="a2"/> = parameter block (+0 UV table, +4 colour table, +8 half width, +12 half height,
    /// +16 tpage, +20 clut, +24 depth-cue IR0 complement (negative = off), +28 count, +32 depth bias, +36 code byte),
    /// <paramref name="a3"/> = packets, <paramref name="a4"/> = OT. Every access goes through PSX addresses, as in the
    /// original. Returns what the original leaves in rax.
    /// </summary>
    public static long BuildSpriteQuads(ulong a1, ulong a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("BuildSpriteQuads(a1, a2, a3, a4)", a1, a2, a3, a4);
        long mapper = (long)unk_180576A10;
        uint items = (uint)PsxAddrMapperRegister(mapper, a1);
        uint parameters = (uint)PsxAddrMapperRegister(mapper, a2);
        uint packet = (uint)PsxAddrMapperRegister(mapper, a3);
        uint ot = (uint)PsxAddrMapperRegister(mapper, a4);

        uint halfWidth = BsqRead32(parameters + 8);
        uint halfHeight = BsqRead32(parameters + 12);
        uint colours = BsqRead32(parameters + 4);
        uint uvTable = BsqRead32(parameters);
        uint tpage = BsqRead32(parameters + 16) << 16;
        uint clut = BsqRead32(parameters + 20) << 16;
        int depthCue = (int)BsqRead32(parameters + 24);
        int count = (int)BsqRead32(parameters + 28);
        int depthBias = (int)BsqRead32(parameters + 32);
        uint code = BsqRead32(parameters + 36);

        int* v = (int*)Unsafe.AsPointer(ref qword_180211FC0);
        v[1] = 0;
        dword_180211FCC = 0;
        long result = (uint)(code << 24);
        dword_180211FD4 = 0;
        qword_180211F94 = 0;
        dword_180211F9C = 0;
        uint codeBits = (uint)result;
        if (count == 0)
        {
            return result;
        }

        uint halfU = halfWidth >> 1;
        uint halfV = halfHeight >> 1;
        while (true)
        {
            int dx = (sbyte)*ResolveGlobalPsxAddress(items);
            int dy = 4 * (sbyte)*ResolveGlobalPsxAddress(items + 1);
            ushort left = (ushort)(4 * dx - halfWidth);
            ushort right = (ushort)(halfWidth + 4 * dx);
            uint bottom = (halfHeight + (uint)dy) << 16;
            uint top = (uint)(dy - (int)halfHeight) << 16;
            uint fourth = bottom | right;
            dword_180211FC8 = (int)(right | top);
            v[0] = (int)(left | top);
            dword_180211FD0 = (int)(left | bottom);
            GteRtps(0);
            GteRtps(1);
            GteRtps(2);
            byte selector = *ResolveGlobalPsxAddress(items + 2);
            items += 3;

            uint uv = *(ushort*)ResolveGlobalPsxAddress(2u * (selector & 0xFu) + uvTable);
            uint u = (byte)uv;
            uint vv = uv >> 8;
            uint vHigh = (vv + halfV) << 8;
            uint vLow = vv << 8;
            uint uHigh = u + halfU;
            BsqWrite32(packet + 12, vLow | u | clut);
            BsqWrite32(packet + 20, uHigh | vLow | tpage);
            BsqWrite32(packet + 28, vHigh | u);
            BsqWrite32(packet + 8, (uint)dword_180211FF0);
            BsqWrite32(packet + 16, (uint)dword_180211FF4);
            BsqWrite32(packet + 24, (uint)dword_180211FF8);
            BsqWrite32(packet + 36, vHigh | uHigh);
            int depth = dword_18021200C;
            v[0] = (int)fourth;
            uint colour = colours + 4u * (uint)(selector >> 4);
            GteRtps(0);
            uint blue = *ResolveGlobalPsxAddress(colour + 2);
            uint redGreen = *(ushort*)ResolveGlobalPsxAddress(colour);
            uint rgb = (blue << 16) | redGreen;
            result = (uint)dword_180211FF8;
            uint colourWord = codeBits | rgb;
            BsqWrite32(packet + 32, (uint)dword_180211FF8);
            if (depthCue >= 0)
            {
                dword_180211FD8 = (int)colourWord;
                dword_180211FE0 = 4096 - depthCue;
                result = GteDpcs();
                colourWord = (uint)dword_180212018;
            }

            int remaining = --count;
            *(uint*)ResolveTracked(packet + 4, ref result) = colourWord;
            int slot = ((depth >> 2) - depthBias) >> 2;
            if ((uint)slot < 0x1000)
            {
                byte* otBase = ResolveGlobalPsxAddress(ot);
                byte* head = ResolveGlobalPsxAddress(packet);
                Head2* head2 = (Head2*)head;
                head[3] = 9;
                uint* entry = (uint*)(otBase + 4 * slot);
                head2->union_0_4 ^= (*entry ^ head2->union_0_4) & 0xFFFFFF;
                result = (*entry ^ (uint)PsxPtrToAddr(mapper, (ulong)head)) & 0xFFFFFF;
                *entry ^= (uint)result;
            }

            packet += 40;
            if (remaining == 0)
            {
                return result;
            }
        }
    }

    private static void BsqWrite32(uint address, uint value)
    {
        Recompiler.Runtime.Tracer.Enter("BsqWrite32(address, value)", address, value);
        *(uint*)ResolveGlobalPsxAddress(address) = value;
    }
}
