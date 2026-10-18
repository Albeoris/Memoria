using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Registers a1 (record: 5 x s16 at +0..+8, colour table of 8-byte entries at +12), a2 (header: +0 colour/code,
    /// +4 depth base, +8 depth cue (&gt;= 0: DPCT with IR0 = 4096 - it), +12 tpage bits, +16 z offset, +20 colour index),
    /// a3 (3 POLY_G3 packets of 28 bytes and a DR_MODE) and a4 (OT). Builds three triangles in the y = 0 plane from
    /// 4 * the record coordinates, projects each through GTE RTPS (V0..V2 written as whole dwords), stores SXY0..2,
    /// zeroes rgb1 and links all three at OT index ((SZ3 of the first) &gt;&gt; 2 - base) &gt;&gt; 2 when below 4096, followed by
    /// the DR_MODE (tpage | 0xE1000200, length 1) at the same index. Returns the XOR delta of that OT entry, or
    /// (index &lt; 4096) when nothing was linked.
    /// </summary>
    public static long PsxAddGroundTriFan(ulong a1, ulong a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("PsxAddGroundTriFan(a1, a2, a3, a4)", a1, a2, a3, a4);
        long mapper = (long)unk_180576A10;
        uint record = (uint)PsxAddrMapperRegister(mapper, a1);
        uint header = (uint)PsxAddrMapperRegister(mapper, a2);
        uint packet = (uint)PsxAddrMapperRegister(mapper, a3);
        uint ot = (uint)PsxAddrMapperRegister(mapper, a4);

        uint colorCode = *(uint*)PsxField(header, 0);
        int depthBase = *(int*)PsxField(header, 4);
        int depthCue = *(int*)PsxField(header, 8);
        uint modeBits = *(uint*)PsxField(header, 12);
        int zOffset = *(int*)PsxField(header, 16);
        qword_180211F94 = 0;
        dword_180211F9C = 0;

        int c0 = 4 * *(short*)PsxField(record, 0);
        ushort c1 = (ushort)(4 * *(short*)PsxField(record, 2));
        int c2 = 4 * *(short*)PsxField(record, 4);
        short c3 = (short)(4 * *(short*)PsxField(record, 6));
        int c4 = 4 * *(short*)PsxField(record, 8);
        uint colourEntry = unchecked(8 * *(uint*)PsxField(header, 20) + record + 12);
        int blue0 = *PsxField(colourEntry, 2);
        int rg0 = *(ushort*)PsxField(colourEntry, 0);
        uint rgb0 = colorCode | (uint)(blue0 << 16) | (uint)rg0;
        int blue1 = *PsxField(colourEntry, 6);
        int rg1 = *(ushort*)PsxField(colourEntry, 4);
        uint rgb1 = colorCode | (uint)(blue1 << 16) | (uint)rg1;
        if (depthCue >= 0)
        {
            dword_180211FD8 = (int)colorCode;
            dword_180212010 = (int)rgb0;
            dword_180212014 = (int)rgb1;
            dword_180211FE0 = 4096 - depthCue;
            GteDpct();
            rgb0 = (uint)dword_180212010;
            rgb1 = (uint)dword_180212014;
        }

        int centerZ = -zOffset;
        int z0 = c0 - zOffset;
        int z2 = c2 - zOffset;
        int z4 = c4 - zOffset;
        ushort x3 = (ushort)-c3;
        int depth = rg1;
        int* v0 = (int*)Unsafe.AsPointer(ref qword_180211FC0);

        for (int triangle = 0; ; triangle++, packet += 28)
        {
            if (triangle == 0)
            {
                dword_180211FCC = z2;
                v0[0] = 0;
                dword_180211FD0 = x3;
                v0[1] = centerZ;
                dword_180211FC8 = (ushort)-c1;
                dword_180211FD4 = z4;
                *(uint*)PsxField(packet, 4) = colorCode;
                *(uint*)PsxField(packet, 20) = rgb1;
            }
            else if (triangle == 1)
            {
                v0[1] = z2;
                v0[0] = (ushort)-c1;
                dword_180211FCC = z0;
                dword_180211FC8 = c1;
                *(uint*)PsxField(packet, 4) = colorCode;
                *(uint*)PsxField(packet, 20) = rgb1;
            }
            else
            {
                v0[0] = x3;
                dword_180211FD0 = 0;
                v0[1] = z4;
                dword_180211FD4 = centerZ;
                *(uint*)PsxField(packet, 4) = rgb1;
                *(uint*)PsxField(packet, 20) = rgb0;
            }

            GteRtps(0);
            GteRtps(1);
            GteRtps(2);
            *(uint*)PsxField(packet, 12) = 0;
            if (triangle != 0)
            {
                LinkOtPacketAtDepth(packet, ot, depth, depthBase, 6);
            }

            *(int*)PsxField(packet, 8) = dword_180211FF0;
            *(int*)PsxField(packet, 16) = dword_180211FF4;
            *(int*)PsxField(packet, 24) = dword_180211FF8;
            if (triangle == 0)
            {
                depth = dword_18021200C;
                LinkOtPacketAtDepth(packet, ot, depth, depthBase, 6);
            }

            if (triangle == 2)
            {
                break;
            }
        }

        uint mode = packet + 28;
        *(uint*)PsxField(mode, 4) = modeBits | 0xE1000200;
        uint index = (uint)(((depth >> 2) - depthBase) >> 2);
        long result = index < 0x1000 ? 1 : 0;
        if (index < 0x1000)
        {
            byte* entries = ResolveGlobalPsxAddress(ot);
            byte* modePacket = ResolveGlobalPsxAddress(mode);
            ModePacket* modePacket2 = (ModePacket*)modePacket;
            modePacket[3] = 1;
            var entry = (uint*)(entries + 4 * index);
            modePacket2->union_0_4 ^= (modePacket2->union_0_4 ^ *entry) & 0xFFFFFF;
            result = (*entry ^ (uint)PsxPtrToAddr((long)unk_180576A10, (ulong)modePacket)) & 0xFFFFFF;
            *entry ^= (uint)result;
        }

        return result;
    }

    /// <summary>addPrim of the packet at PSX address <paramref name="packet"/> into OT entry ((z &gt;&gt; 2) - base) &gt;&gt; 2 when below 4096.</summary>
    private static void LinkOtPacketAtDepth(uint packet, uint ot, int z, int depthBase, byte length)
    {
        Recompiler.Runtime.Tracer.Enter("LinkOtPacketAtDepth(packet, ot, z, depthBase, length)", packet, ot, z, depthBase, length);
        uint index = (uint)(((z >> 2) - depthBase) >> 2);
        if (index < 0x1000)
        {
            byte* entries = ResolveGlobalPsxAddress(ot);
            byte* target = ResolveGlobalPsxAddress(packet);
            Target* target2 = (Target*)target;
            target[3] = length;
            var entry = (uint*)(entries + 4 * index);
            target2->union_0_4 ^= (*entry ^ target2->union_0_4) & 0xFFFFFF;
            *entry ^= (*entry ^ (uint)PsxPtrToAddr((long)unk_180576A10, (ulong)target)) & 0xFFFFFF;
        }
    }
}
