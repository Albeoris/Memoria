using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Registers the point list <paramref name="a1"/>, the parameters <paramref name="a2"/>, the packet buffer
    /// <paramref name="a3"/> and the OT entry <paramref name="a4"/> as PSX addresses and accesses them through those
    /// addresses (each field resolved on its own). Parameters: +4 address of the color table (4-byte entries: ushort
    /// low 16 bits, byte +2 bits 16..23), +24 fade (negative = none), +28 point count, +36 GPU code byte. Point (3 bytes):
    /// sbyte x, sbyte y (both x4, z = 0), color index in the high nibble. Each point is projected with RTPS and emitted as
    /// a 12-byte 2-word packet (code | color, SXY), faded toward black with DPCS (IR0 = 4096 - fade) when fade &gt;= 0, and
    /// linked at the single OT entry. The count is decremented before it is tested, so 0 means 2^32 points.
    /// Returns the value the inlined PsxPtrToAddr left in rax for the last packet.
    /// </summary>
    public static ulong* EmitFadedDots(ulong a1, ulong a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("EmitFadedDots(a1, a2, a3, a4)", a1, a2, a3, a4);
        long mapper = (long)unk_180576A10;
        uint point = (uint)PsxAddrMapperRegister(mapper, a1);
        uint param = (uint)PsxAddrMapperRegister(mapper, a2);
        uint prim = (uint)PsxAddrMapperRegister(mapper, a3);
        uint ot = (uint)PsxAddrMapperRegister(mapper, a4);

        uint colorTable = *(uint*)ResolveGlobalPsxAddress(param + 4);
        int fade = *(int*)ResolveGlobalPsxAddress(param + 24);
        int code = *(int*)ResolveGlobalPsxAddress(param + 36);
        int remaining = *(int*)ResolveGlobalPsxAddress(param + 28);
        int* v0 = (int*)Unsafe.AsPointer(ref qword_180211FC0);
        v0[1] = 0;
        int codeHigh = code << 24;
        qword_180211F94 = 0;
        dword_180211F9C = 0;
        while (true)
        {
            int x = (sbyte)*ResolveGlobalPsxAddress(point);
            int y = (sbyte)*ResolveGlobalPsxAddress(point + 1);
            v0[0] = (ushort)(4 * x) | (y << 18);
            uint colorIndex = *ResolveGlobalPsxAddress(point + 2);
            point += 3;
            GteRtps(0);

            uint entry = colorTable + 4 * (colorIndex >> 4);
            int high = *ResolveGlobalPsxAddress(entry + 2);
            int low = *(ushort*)ResolveGlobalPsxAddress(entry);
            int color = codeHigh | low | (high << 16);
            *(int*)ResolveGlobalPsxAddress(prim + 8) = dword_180211FF8;
            if (fade >= 0)
            {
                dword_180211FD8 = color;
                dword_180211FE0 = 4096 - fade;
                GteDpcs();
                color = dword_180212018;
            }

            --remaining;
            *(int*)ResolveGlobalPsxAddress(prim + 4) = color;
            byte* packet = ResolveGlobalPsxAddress(prim);
            Packet* packet2 = (Packet*)packet;
            uint* otEntry = (uint*)ResolveGlobalPsxAddress(ot);
            packet[3] = 2;
            uint link = (*otEntry ^ packet2->union_0_4) & 0xFFFFFF;
            packet2->union_0_4 ^= link;
            var ctx = (PsxAddrMapper*)unk_180576A10;
            ulong* rax = ctx->count > 0
                ? (ulong*)(unk_180576A20 + 32L * ctx->count)
                : (ulong*)link;
            uint address = (uint)PsxPtrToAddr(mapper, (ulong)packet);
            *otEntry &= 0xFF000000;
            *otEntry |= address & 0xFFFFFF;
            prim += 12;
            if (remaining == 0)
            {
                return rax;
            }
        }
    }
}
