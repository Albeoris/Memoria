using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Gouraud ring mesh (the untextured sibling of <see cref="EmitConcentricRingQuads"/>). Registers the descriptor
    /// <paramref name="a2"/>, the packet buffer <paramref name="a3"/> and the ordering table <paramref name="a4"/> as PSX
    /// addresses and resolves every field address on its own, like the original. Descriptor: +0 code bits (ORed into
    /// rgb0), +4 fade (DPCS IR0 = 4096 - fade with the far color cleared), +8 address of {u16 GPF flag, u16 GPF IR0},
    /// +12 address of the u16 radius array (x4), +16 address of the u16 (x, y) direction list, +20 / +32 addresses of the
    /// two RGB tables (each advances 4 * a1 per ring; the outer edge reads + 4 * a1), +24 OTZ bias, +28 segments per
    /// ring, +36 ring counter (decremented in place until 0), +40 tpage bits; +44..+56 radii scratch as in the textured
    /// variant. Every pair of consecutive points becomes a 44-byte POLY_G4 (8 words) plus a DR_MODE (E1 | 0x200 | tpage)
    /// at +36, both linked into the OT at ((SZ3 &gt;&gt; 2) - bias) &gt;&gt; 2 when below 4096. Returns what the original
    /// leaves in rax after its last resolution of descriptor + 36.
    /// </summary>
    public static long EmitShadedRingQuads(int a1, ulong a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("EmitShadedRingQuads(a1, a2, a3, a4)", a1, a2, a3, a4);
        long mapper = (long)unk_180576A10;
        uint desc = (uint)PsxAddrMapperRegister(mapper, a2);
        uint packet = (uint)PsxAddrMapperRegister(mapper, a3);
        uint ot = (uint)PsxAddrMapperRegister(mapper, a4);

        int fade = *(int*)ResolveGlobalPsxAddress(desc + 4);
        int segments = *(int*)ResolveGlobalPsxAddress(desc + 28);
        int otzBias = *(int*)ResolveGlobalPsxAddress(desc + 24);
        int code = *(int*)ResolveGlobalPsxAddress(desc);
        uint gpfParams = *(uint*)ResolveGlobalPsxAddress(desc + 8);
        uint radii = *(uint*)ResolveGlobalPsxAddress(desc + 12);
        uint colorsA = *(uint*)ResolveGlobalPsxAddress(desc + 20);
        uint colorsB = *(uint*)ResolveGlobalPsxAddress(desc + 32);
        qword_180211F94 = 0;
        dword_180211F9C = 0;

        int* ir = (int*)Unsafe.AsPointer(ref dword_180211FE4);
        short* ir16 = (short*)ir;
        int* v0 = (int*)Unsafe.AsPointer(ref qword_180211FC0);

        // Previous point of the ring (outer / inner); only read once the first point of a ring has been computed.
        int previousOuter = 0;
        int previousInner = 0;
        while (true)
        {
            int gpfFlag = *(ushort*)ResolveGlobalPsxAddress(gpfParams);
            int gpfIr0 = *(ushort*)ResolveGlobalPsxAddress(gpfParams + 2);
            int innerRaw = 4 * *(ushort*)ResolveGlobalPsxAddress(radii);
            int innerScaled;
            if (gpfFlag != 0)
            {
                dword_180211FE0 = gpfIr0;
                ir[0] = innerRaw;
                GteGpf();
                innerScaled = ir16[0];
                ir[0] = ir16[0];
            }
            else
            {
                innerScaled = innerRaw;
            }

            *(int*)ResolveGlobalPsxAddress(desc + 44) = innerRaw;
            *(int*)ResolveGlobalPsxAddress(desc + 52) = innerScaled;
            radii += 2;
            uint direction = *(uint*)ResolveGlobalPsxAddress(desc + 16);
            int outerRaw = 4 * *(ushort*)ResolveGlobalPsxAddress(radii);
            int outerScaled;
            if (gpfFlag != 0)
            {
                ir[0] = outerRaw;
                GteGpf();
                outerScaled = ir16[0];
                ir[0] = ir16[0];
            }
            else
            {
                outerScaled = outerRaw;
            }

            *(int*)ResolveGlobalPsxAddress(desc + 48) = outerRaw;
            *(int*)ResolveGlobalPsxAddress(desc + 56) = outerScaled;

            int point = 0;
            do
            {
                bool odd = (point & 1) != 0;
                int innerRadius = *(int*)ResolveGlobalPsxAddress(desc + (odd ? 52u : 44u));
                int dx = *(ushort*)ResolveGlobalPsxAddress(direction);
                int dy = *(ushort*)ResolveGlobalPsxAddress(direction + 2);
                ir[0] = dx;
                dword_180211FE0 = innerRadius;
                ir[1] = dy;
                GteGpf();

                int outerRadius = *(int*)ResolveGlobalPsxAddress(desc + (odd ? 56u : 48u));
                ushort innerX = (ushort)ir16[0];
                ir[0] = dx;
                int inner = innerX | (ir16[2] << 16);

                // The pseudocode restores IR2 from an uninitialised register; the IR2 input of the first GPF (dy) is the
                // assumption recorded in the ledger (same shape as EmitConcentricRingQuads).
                ir[1] = dy;
                dword_180211FE0 = outerRadius;
                direction += 4;
                GteGpf();
                ir[1] = ir16[2];
                int outer = (ushort)ir16[0] | (ir16[2] << 16);
                ir[0] = ir16[0];

                if (point != 0)
                {
                    v0[0] = previousOuter;
                    dword_180211FC8 = outer;
                    dword_180211FD0 = previousInner;
                    EmitShadedRingQuadsPacket(a1, odd, desc, packet, ot, inner, colorsA, colorsB, code, fade, otzBias);
                    packet += 44;
                }

                previousOuter = outer;
                previousInner = inner;
                point++;
            }
            while (segments >= point);

            uint counterAddress = desc + 36;
            int remaining = *(int*)ResolveGlobalPsxAddress(counterAddress) - 1;
            uint rowStride = (uint)(4 * a1);
            colorsB += rowStride;
            colorsA += rowStride;
            *(int*)ResolveGlobalPsxAddress(counterAddress) = remaining;
            if (remaining == 0)
            {
                return EmitShadedRingQuadsRax(counterAddress, rowStride);
            }
        }
    }

    /// <summary>
    /// One POLY_G4 + DR_MODE: V0..V2 (set by the caller) projected with RTPS, then V0 = inner projected alone for the
    /// fourth corner; colors are DPCS(RGB from the two tables picked by point parity, IR0 = 4096 - fade).
    /// </summary>
    private static void EmitShadedRingQuadsPacket(int a1, bool odd, uint desc, uint packet, uint ot, int inner,
        uint colorsA, uint colorsB, int code, int fade, int otzBias)
    {
        Recompiler.Runtime.Tracer.Enter("EmitShadedRingQuadsPacket(a1, odd, desc, packet, ot, inner, colorsA, colorsB, code, fade, …)", a1, odd, desc, packet, ot, inner, colorsA, colorsB, code, fade);
        int tpage = *(int*)ResolveGlobalPsxAddress(desc + 40);
        GteRtps(0);
        GteRtps(1);
        GteRtps(2);
        *(uint*)ResolveGlobalPsxAddress(packet + 40) = (uint)tpage | 0xE1000200;
        *(int*)ResolveGlobalPsxAddress(packet + 8) = dword_180211FF0;
        *(int*)ResolveGlobalPsxAddress(packet + 16) = dword_180211FF4;
        *(int*)ResolveGlobalPsxAddress(packet + 24) = dword_180211FF8;
        int sz3 = dword_18021200C;
        *(int*)Unsafe.AsPointer(ref qword_180211FC0) = inner;
        GteRtps(0);

        int otz = ((sz3 >> 2) - otzBias) >> 2;
        if ((uint)otz < 0x1000)
        {
            EmitShadedRingQuadsLink(packet, ot, otz, 8);
            EmitShadedRingQuadsLink(packet + 36, ot, otz, 1);
        }

        *(int*)ResolveGlobalPsxAddress(packet + 32) = dword_180211FF8;
        uint rowStride = (uint)(4 * a1);
        EmitShadedRingQuadsFade(odd ? colorsB : colorsA, fade);
        *(int*)ResolveGlobalPsxAddress(packet + 20) = dword_180212018;
        EmitShadedRingQuadsFade(odd ? colorsA : colorsB, fade);
        *(int*)ResolveGlobalPsxAddress(packet + 28) = dword_180212018;
        EmitShadedRingQuadsFade(rowStride + (odd ? colorsB : colorsA), fade);
        *(int*)ResolveGlobalPsxAddress(packet + 4) = code | dword_180212018;
        EmitShadedRingQuadsFade(rowStride + (odd ? colorsA : colorsB), fade);
        *(int*)ResolveGlobalPsxAddress(packet + 12) = dword_180212018;
    }

    /// <summary>DPCS of the RGB at PSX address <paramref name="color"/> with IR0 = 4096 - fade.</summary>
    private static void EmitShadedRingQuadsFade(uint color, int fade)
    {
        Recompiler.Runtime.Tracer.Enter("EmitShadedRingQuadsFade(color, fade)", color, fade);
        int rgb = *(int*)ResolveGlobalPsxAddress(color);
        dword_180211FE0 = 4096 - fade;
        dword_180211FD8 = rgb;
        GteDpcs();
    }

    /// <summary>addPrim of the packet at PSX address <paramref name="packet"/> (length words) into OT entry otz.</summary>
    private static void EmitShadedRingQuadsLink(uint packet, uint ot, int otz, byte length)
    {
        Recompiler.Runtime.Tracer.Enter("EmitShadedRingQuadsLink(packet, ot, otz, length)", packet, ot, otz, length);
        byte* otBase = ResolveGlobalPsxAddress(ot);
        byte* prim = ResolveGlobalPsxAddress(packet);
        Prim5* prim5 = (Prim5*)prim;
        prim[3] = length;
        uint* entry = (uint*)(otBase + (4L * otz));
        prim5->union_0_4 ^= (*entry ^ prim5->union_0_4) & 0xFFFFFF;
        uint address = (uint)PsxPtrToAddr((long)unk_180576A10, (ulong)prim);
        *entry &= 0xFF000000;
        *entry |= address & 0xFFFFFF;
    }

    /// <summary>
    /// rax after the original's inlined resolution of <paramref name="address"/> (descriptor + 36): 4 * a1 for a null
    /// address, address &amp; 0xFFFFFFF in the RAM segment, 32 * segment for a mapped segment, else address &amp; 0xC00000.
    /// </summary>
    private static long EmitShadedRingQuadsRax(uint address, uint rowStride)
    {
        Recompiler.Runtime.Tracer.Enter("EmitShadedRingQuadsRax(address, rowStride)", address, rowStride);
        if (address == 0)
        {
            return rowStride;
        }

        if ((address >> 24) == 128)
        {
            return address & 0xFFFFFFF;
        }

        uint segment = address & 0xC00000;
        return segment == 0xC00000 ? 32L * (address >> 24) : segment;
    }
}
