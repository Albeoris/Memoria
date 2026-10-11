using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Registers the cell list <paramref name="a1"/>, the parameters <paramref name="a2"/>, the packet buffer
    /// <paramref name="a3"/> and the ordering table <paramref name="a4"/> as PSX addresses and accesses them through
    /// those addresses (each field address resolved on its own, like the original). Parameters: +0 color/code, +4 x origin
    /// (x4, int16), +8 y origin (x4), +12 tpage bits, +16 clut, +20 cell count, +24 OTZ bias. Cell (10 bytes): +0/+1 signed
    /// x/y cell (x16), +2/+3 width/height (x4), +4 u0/v0, +6 u1/v1, +8 tpage low bits. For every cell the four corners
    /// (z = 0) go through RTPS and a 40-byte POLY_FT4 (length 9) is written and linked into the OT at
    /// ((SZ &gt;&gt; 2) - bias) &gt;&gt; 2 when below 4096. Returns SXY2 after the last cell; with no cells, the value the
    /// address resolution left in rax.
    /// </summary>
    public static long EmitSpriteQuadList(ulong a1, ulong a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("EmitSpriteQuadList(a1, a2, a3, a4)", a1, a2, a3, a4);
        long mapper = (long)unk_180576A10;
        uint cell = (uint)PsxAddrMapperRegister(mapper, a1);
        uint param = (uint)PsxAddrMapperRegister(mapper, a2);
        uint prim = (uint)PsxAddrMapperRegister(mapper, a3);
        uint ot = (uint)PsxAddrMapperRegister(mapper, a4);

        int clut = *(int*)ResolveGlobalPsxAddress(param + 16);
        int remaining = *(int*)ResolveGlobalPsxAddress(param + 20);
        int otzBias = *(int*)ResolveGlobalPsxAddress(param + 24);
        int color = *(int*)ResolveGlobalPsxAddress(param);
        uint tpage = *(uint*)ResolveGlobalPsxAddress(param + 12);
        long result = tpage;
        int originX = *(int*)SpriteResolveTracked(param + 4, ref result);
        int originY = *(int*)SpriteResolveTracked(param + 8, ref result);
        int clutHigh = clut << 16;
        short x0Base = (short)(4 * originX);
        int y0Base = 4 * originY;

        int* v = (int*)Unsafe.AsPointer(ref qword_180211FC0);
        v[1] = 0;
        dword_180211FCC = 0;
        dword_180211FD4 = 0;
        while (remaining != 0)
        {
            int cellX = (sbyte)*ResolveGlobalPsxAddress(cell);
            int cellY = (sbyte)*ResolveGlobalPsxAddress(cell + 1);
            int left = (ushort)(x0Base + 16 * cellX);
            int width = *ResolveGlobalPsxAddress(cell + 2);
            int height = *ResolveGlobalPsxAddress(cell + 3);
            int top = y0Base + 16 * cellY;
            int bottom = top + 4 * height;
            top <<= 16;
            int right = (ushort)(left + 4 * width);
            v[0] = left | top;
            dword_180211FC8 = top | right;
            int corner3 = right | (bottom << 16);
            dword_180211FD0 = left | (bottom << 16);
            *(int*)ResolveGlobalPsxAddress(prim + 4) = color;
            GteRtps(0);
            GteRtps(1);
            GteRtps(2);

            int tpageLow = *(ushort*)ResolveGlobalPsxAddress(cell + 8);
            ushort uv0 = *(ushort*)ResolveGlobalPsxAddress(cell + 4);
            byte u0 = (byte)uv0;
            ushort uv1 = *(ushort*)ResolveGlobalPsxAddress(cell + 6);
            int v0 = uv0 & 0xFF00;
            int tpageHigh = (int)((tpage | (uint)tpageLow) << 16);
            *(int*)ResolveGlobalPsxAddress(prim + 12) = clutHigh | u0 | v0;
            *(int*)ResolveGlobalPsxAddress(prim + 20) = tpageHigh | v0 | (byte)uv1;
            *(int*)ResolveGlobalPsxAddress(prim + 8) = dword_180211FF0;
            *(int*)ResolveGlobalPsxAddress(prim + 16) = dword_180211FF4;
            *(int*)ResolveGlobalPsxAddress(prim + 24) = dword_180211FF8;
            *(int*)ResolveGlobalPsxAddress(prim + 28) = u0 | (uv1 & 0xFF00);
            int sz = dword_18021200C;
            v[0] = corner3;
            *(uint*)ResolveGlobalPsxAddress(prim + 36) = uv1;
            GteRtps(0);

            int otz = ((sz >> 2) - otzBias) >> 2;
            if ((uint)otz < 0x1000)
            {
                byte* otBase = ResolveGlobalPsxAddress(ot);
                byte* packet = ResolveGlobalPsxAddress(prim);
                Packet4* packet4 = (Packet4*)packet;
                packet[3] = 9;
                uint* entry = (uint*)(otBase + 4L * otz);
                packet4->union_0_4 ^= (*entry ^ packet4->union_0_4) & 0xFFFFFF;
                *entry ^= (*entry ^ (uint)PsxPtrToAddr((long)unk_180576A10, (ulong)packet)) & 0xFFFFFF;
            }

            cell += 10;
            byte* xy3 = ResolveGlobalPsxAddress(prim + 32);
            result = (uint)dword_180211FF8;
            prim += 40;
            remaining--;
            *(int*)xy3 = dword_180211FF8;
        }

        return result;
    }

    /// <summary>
    /// PsxAddrToPtr over the global context that also updates <paramref name="rax"/> the way the original's inlined
    /// resolver leaves it: masked offset for 0x80xxxxxx, the segment bits (then 32 * segment for a segment) otherwise;
    /// unchanged for 0.
    /// </summary>
    private static byte* SpriteResolveTracked(uint address, ref long rax)
    {
        Recompiler.Runtime.Tracer.Enter("SpriteResolveTracked(address, rax)", address, rax);
        if (address == 0)
        {
            return null;
        }

        if (address >> 24 == 128)
        {
            rax = address & 0xFFFFFFF;
        }
        else
        {
            rax = address & 0xC00000;
            if ((uint)rax == 0xC00000)
            {
                rax = 32L * (address >> 24);
            }
        }

        return ResolveGlobalPsxAddress(address);
    }
}
