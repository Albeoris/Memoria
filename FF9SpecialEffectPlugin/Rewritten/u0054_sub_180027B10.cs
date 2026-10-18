using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Registers the descriptor <paramref name="a2"/>, the packet buffer <paramref name="a3"/> and the ordering table
    /// <paramref name="a4"/> as PSX addresses and accesses all of them through those addresses (each field address is
    /// resolved on its own, like the original). Descriptor: +0 OTZ bias, +4 address of the vertex ring (byte 0 = count - 1,
    /// byte 1 = start index, 8-byte SVECTORs from +4), +8 initial IR0, +12 RGBC, +16 far color (0x00BBGGRR), +32 IR0 step,
    /// +40 tpage bits. Emits <paramref name="a1"/> (at least one) 28-byte packets: LINE_G2 (+0..+19) between consecutive
    /// projected ring vertices, colors depth-cued with DPCS (IR0 accumulating, 4096 for the last segment), followed by an
    /// E1 draw-mode packet (+20, +24), both linked into the OT at ((SZ3 &gt;&gt; 2) - bias) &gt;&gt; 2 when below 4096. Returns 0.
    /// </summary>
    public static sbyte EmitDepthCuedLineStrip(int a1, ulong a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("EmitDepthCuedLineStrip(a1, a2, a3, a4)", a1, a2, a3, a4);
        long mapper = (long)unk_180576A10;
        uint desc = (uint)PsxAddrMapperRegister(mapper, a2);
        uint prim = (uint)PsxAddrMapperRegister(mapper, a3);
        uint ot = (uint)PsxAddrMapperRegister(mapper, a4);

        int otzBias = *(int*)ResolveGlobalPsxAddress(desc);
        int tpage = *(int*)ResolveGlobalPsxAddress(desc + 40);
        uint ring = *(uint*)ResolveGlobalPsxAddress(desc + 4);
        int ir0 = *(int*)ResolveGlobalPsxAddress(desc + 8);
        int rgbc = *(int*)ResolveGlobalPsxAddress(desc + 12);
        uint farColor = *(uint*)ResolveGlobalPsxAddress(desc + 16);
        int color = rgbc;
        int* fc = (int*)Unsafe.AsPointer(ref qword_180211F94);
        fc[0] = 16 * (byte)farColor;
        dword_180211F9C = (int)((farColor >> 12) & 0xFF0);
        fc[1] = (int)((farColor >> 4) & 0xFF0);

        int lastIndex = *ResolveGlobalPsxAddress(ring);
        int index = *ResolveGlobalPsxAddress(ring + 1);
        int count = lastIndex + 1;
        uint vertices = ring + 4;
        LineStripLoadVertex(vertices + 8 * (uint)index);
        GteRtps(0);

        index++;
        if (index == count)
        {
            index = 0;
        }

        uint drawMode = (uint)tpage | 0xE1000200;
        int segment = 0;
        while (true)
        {
            LineStripLoadVertex(vertices + 8 * (uint)index);
            GteRtps(0);

            *(uint*)ResolveGlobalPsxAddress(prim + 24) = drawMode;
            int factor = ir0;
            ++segment;
            if (segment >= a1)
            {
                factor = 4096;
            }

            int appliedFactor = factor;
            *(int*)ResolveGlobalPsxAddress(prim + 4) = color;
            int previousXy = dword_180211FF4;
            *(int*)ResolveGlobalPsxAddress(prim + 8) = dword_180211FF8;
            *(int*)ResolveGlobalPsxAddress(prim + 16) = previousXy;
            int sz3 = dword_180212008;
            dword_180211FD8 = rgbc;
            dword_180211FE0 = factor;
            GteDpcs();

            int otz = ((sz3 >> 2) - otzBias) >> 2;
            if ((uint)otz < 0x1000)
            {
                LineStripAddPrim(ResolveGlobalPsxAddress(prim), 4, ResolveGlobalPsxAddress(ot), otz);
                LineStripAddPrim(ResolveGlobalPsxAddress(prim + 20), 1, ResolveGlobalPsxAddress(ot), otz);
            }

            int* step = (int*)ResolveGlobalPsxAddress(desc + 32);
            color = dword_180212018;
            ir0 = *step + appliedFactor;
            *(int*)ResolveGlobalPsxAddress(prim + 12) = dword_180212018;
            ++index;
            if (index == count)
            {
                index = 0;
            }

            prim += 28;
            if (segment >= a1)
            {
                return 0;
            }
        }
    }

    /// <summary>VXY0 / VZ0 (qword_180211FC0 lo/hi) from the SVECTOR at PSX address <paramref name="address"/>.</summary>
    private static void LineStripLoadVertex(uint address)
    {
        Recompiler.Runtime.Tracer.Enter("LineStripLoadVertex(address)", address);
        int* v0 = (int*)Unsafe.AsPointer(ref qword_180211FC0);
        v0[0] = *(int*)ResolveGlobalPsxAddress(address);
        v0[1] = *(int*)ResolveGlobalPsxAddress(address + 4);
    }

    /// <summary>addPrim of a packet of <paramref name="length"/> words into OT entry <paramref name="otz"/>.</summary>
    private static void LineStripAddPrim(byte* packet, byte length, byte* otBase, int otz)
    {
        Recompiler.Runtime.Tracer.Enter("LineStripAddPrim(packet, length, otBase, otz)", (nint)packet, length, (nint)otBase, otz);
        packet[3] = length;
        uint* entry = (uint*)(otBase + 4L * otz);
        *(uint*)packet ^= (*entry ^ *(uint*)packet) & 0xFFFFFF;
        *entry ^= (*entry ^ (uint)PsxPtrToAddr((long)unk_180576A10, (ulong)packet)) & 0xFFFFFF;
    }
}
