using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Textured ring mesh. Registers the descriptor <paramref name="a2"/>, the packet buffer <paramref name="a3"/> and
    /// the ordering table <paramref name="a4"/> as PSX addresses and resolves every field address on its own, like the
    /// original. Descriptor: +0 color (RGB | code), +4 clut | uv0 high word, +8 address of {u16 GPF flag, u16 GPF IR0},
    /// +12 address of the u16 radius array (x4), +16 address of the u16 (x, y) direction list, +20 address of the u16
    /// uv table (row stride 2 * a1 + 4), +24 OTZ bias, +28 segments per ring, +32 uv offset, +36 ring counter
    /// (decremented in place until 0; checked after each ring), +40 tpage | uv1 high word; +44/+48 (raw inner/outer
    /// radius) and +52/+56 (GPF-scaled radii) are written as scratch. Per ring the directions are scaled with GPF by the
    /// inner and outer radius (raw on even, scaled on odd points) and every pair of consecutive points becomes a 40-byte
    /// POLY_FT4 linked into the OT at ((SZ3 &gt;&gt; 2) - bias) &gt;&gt; 2 when below 4096. <paramref name="a1"/> is the uv column
    /// mask (applied when a1 + 1 &lt; segments). Returns what the original leaves in rax after its last resolution of
    /// descriptor + 36.
    /// </summary>
    public static long EmitConcentricRingQuads(int a1, ulong a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("EmitConcentricRingQuads(a1, a2, a3, a4)", a1, a2, a3, a4);
        long mapper = (long)unk_180576A10;
        uint desc = (uint)PsxAddrMapperRegister(mapper, a2);
        uint packet = (uint)PsxAddrMapperRegister(mapper, a3);
        uint ot = (uint)PsxAddrMapperRegister(mapper, a4);

        int clutWord = *(int*)ResolveGlobalPsxAddress(desc + 4);
        int segments = *(int*)ResolveGlobalPsxAddress(desc + 28);
        int otzBias = *(int*)ResolveGlobalPsxAddress(desc + 24);
        int color = *(int*)ResolveGlobalPsxAddress(desc);
        uint gpfParams = *(uint*)ResolveGlobalPsxAddress(desc + 8);
        uint radii = *(uint*)ResolveGlobalPsxAddress(desc + 12);
        int uvOffset = *(int*)ResolveGlobalPsxAddress(desc + 32);
        uint uvRow = *(uint*)ResolveGlobalPsxAddress(desc + 20);

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
                // assumption recorded in the ledger.
                ir[1] = dy;
                dword_180211FE0 = outerRadius;
                direction += 4;
                GteGpf();
                ir[1] = ir16[2];
                int outer = (ushort)ir16[0] | (ir16[2] << 16);
                ir[0] = ir16[0];

                if (point != 0)
                {
                    EmitConcentricRingQuadsPacket(a1, segments, point, desc, packet, ot, previousOuter, outer,
                        previousInner, inner, uvRow, uvOffset, clutWord, color, otzBias);
                    packet += 40;
                }

                previousOuter = outer;
                previousInner = inner;
                point++;
            }
            while (segments >= point);

            uint counterAddress = desc + 36;
            int remaining = *(int*)ResolveGlobalPsxAddress(counterAddress) - 1;
            uvRow += (uint)((2 * a1) + 4);
            *(int*)ResolveGlobalPsxAddress(counterAddress) = remaining;
            if (remaining == 0)
            {
                return EmitConcentricRingQuadsRax(counterAddress, a1);
            }
        }
    }

    /// <summary>
    /// One POLY_FT4 (9 words): V0 = previous outer, V1 = outer, V2 = previous inner projected with RTPS, then V0 = inner
    /// projected alone for the fourth corner; uvs from the uv table row at column (point - 1) (masked by a1).
    /// </summary>
    private static void EmitConcentricRingQuadsPacket(int a1, int segments, int point, uint desc, uint packet, uint ot,
        int previousOuter, int outer, int previousInner, int inner, uint uvRow, int uvOffset, int clutWord, int color,
        int otzBias)
    {
        Recompiler.Runtime.Tracer.Enter("EmitConcentricRingQuadsPacket(a1, segments, point, desc, packet, ot, previousOuter, outer, previousInner, inner, …)", a1, segments, point, desc, packet, ot, previousOuter, outer, previousInner, inner);
        *(int*)Unsafe.AsPointer(ref qword_180211FC0) = previousOuter;
        dword_180211FC8 = outer;
        dword_180211FD0 = previousInner;
        int column = point - 1;
        if (a1 + 1 < segments)
        {
            column &= a1;
        }

        GteRtps(0);
        GteRtps(1);
        GteRtps(2);
        uint uv = uvRow + (2u * (uint)column);
        int uv2 = *(ushort*)ResolveGlobalPsxAddress(uv);
        short uv3 = (short)(uvOffset + *(ushort*)ResolveGlobalPsxAddress(uv + 2));
        *(ushort*)ResolveGlobalPsxAddress(packet + 28) = (ushort)(uvOffset + uv2);
        *(short*)ResolveGlobalPsxAddress(packet + 36) = uv3;
        uint uvNext = uv + (2u * (uint)a1);
        *(int*)ResolveGlobalPsxAddress(packet + 8) = dword_180211FF0;
        *(int*)ResolveGlobalPsxAddress(packet + 16) = dword_180211FF4;
        *(int*)ResolveGlobalPsxAddress(packet + 24) = dword_180211FF8;
        int sz3 = dword_18021200C;
        *(int*)Unsafe.AsPointer(ref qword_180211FC0) = inner;
        int uv0 = *(ushort*)ResolveGlobalPsxAddress(uvNext + 4);
        int uv1 = *(ushort*)ResolveGlobalPsxAddress(uvNext + 6);
        GteRtps(0);
        int tpageWord = *(int*)ResolveGlobalPsxAddress(desc + 40) | (uvOffset + uv1);
        int clutUv = clutWord | (uvOffset + uv0);
        *(int*)ResolveGlobalPsxAddress(packet + 12) = clutUv;
        *(int*)ResolveGlobalPsxAddress(packet + 20) = tpageWord;
        *(int*)ResolveGlobalPsxAddress(packet + 4) = color;
        *(int*)ResolveGlobalPsxAddress(packet + 32) = dword_180211FF8;

        int otz = ((sz3 >> 2) - otzBias) >> 2;
        if ((uint)otz < 0x1000)
        {
            byte* otBase = ResolveGlobalPsxAddress(ot);
            byte* prim = ResolveGlobalPsxAddress(packet);
            Prim* prim2 = (Prim*)prim;
            prim[3] = 9;
            uint* entry = (uint*)(otBase + (4L * otz));
            prim2->union_0_4 ^= (*entry ^ prim2->union_0_4) & 0xFFFFFF;
            uint address = (uint)PsxPtrToAddr((long)unk_180576A10, (ulong)prim);
            *entry &= 0xFF000000;
            *entry |= address & 0xFFFFFF;
        }
    }

    /// <summary>
    /// rax after the original's inlined resolution of <paramref name="address"/> (descriptor + 36): 2 * a1 + 4 for a null
    /// address, address &amp; 0xFFFFFFF in the RAM segment, 32 * segment for a mapped segment, else address &amp; 0xC00000.
    /// </summary>
    private static long EmitConcentricRingQuadsRax(uint address, int a1)
    {
        Recompiler.Runtime.Tracer.Enter("EmitConcentricRingQuadsRax(address, a1)", address, a1);
        if (address == 0)
        {
            return (uint)((2 * a1) + 4);
        }

        if ((address >> 24) == 128)
        {
            return address & 0xFFFFFFF;
        }

        uint segment = address & 0xC00000;
        return segment == 0xC00000 ? 32L * (address >> 24) : segment;
    }
}
