using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Screen-aligned depth-cued textured ribbon. Registers the descriptor <paramref name="a2"/>, the packet buffer
    /// <paramref name="a3"/> and the ordering table <paramref name="a4"/> as PSX addresses and resolves every field
    /// address on its own, like the original. Descriptor (as EmitDepthCuedQuadStrip up to +38): +0 OTZ bias, +4 point
    /// ring (byte 0 = count - 1, byte 1 = start index, 8-byte points from +4: u32 VXY, u32 VZ, s16 half-width at +6),
    /// +8 initial IR0, +12 RGBC, +16 far color, +20 u16 V base, +22 u16 V scale, +24 tpage | uv1 word, +28 clut | uv0
    /// word, +32 IR0 step, +36/+38 u16 uv halves; scratch: +40..+68 saved GTE rotation/translation, +72/+76 previous
    /// far screen edge, +80..+90 SXY2/SXY1/SXY0, +92 SZ2, +96..+104 MAC1..3. Per segment (<paramref name="a1"/>, at
    /// least one) the current, next and next-but-one points (next for the last segment) are projected, the screen
    /// direction is turned by OP into its perpendicular, normalised with the inverse square-root table unk_1800685C4
    /// and scaled by the point's half-width (GPF); the perpendicular is projected with an identity rotation around the
    /// next point's camera position to give the far edge. One 52-byte POLY_GT4 per segment, near colors = previous DPCS
    /// output (RGBC for the first), far colors = DPCS(RGBC, IR0), linked at ((SZ2 &gt;&gt; 2) - bias) &gt;&gt; 2 when
    /// SZ2 != 0 and below 4096. Returns 0.
    /// </summary>
    public static long EmitScreenAlignedQuadStrip(int a1, ulong a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("EmitScreenAlignedQuadStrip(a1, a2, a3, a4)", a1, a2, a3, a4);
        long mapper = (long)unk_180576A10;
        uint desc = (uint)PsxAddrMapperRegister(mapper, a2);
        uint packet = (uint)PsxAddrMapperRegister(mapper, a3);
        uint ot = (uint)PsxAddrMapperRegister(mapper, a4);

        int otzBias = *(int*)ResolveGlobalPsxAddress(desc);
        uint ring = *(uint*)ResolveGlobalPsxAddress(desc + 4);
        int ir0 = *(int*)ResolveGlobalPsxAddress(desc + 8);
        int rgbc = *(int*)ResolveGlobalPsxAddress(desc + 12);
        uint farColor = *(uint*)ResolveGlobalPsxAddress(desc + 16);
        int vBase = *(ushort*)ResolveGlobalPsxAddress(desc + 20);
        int nearColor = rgbc;
        int v = vBase;

        int* fc = (int*)Unsafe.AsPointer(ref qword_180211F94);
        fc[0] = 16 * (byte)farColor;
        dword_180211F9C = (int)((farColor >> 12) & 0xFF0);
        fc[1] = (int)((farColor >> 4) & 0xFF0);

        int lastIndex = *ResolveGlobalPsxAddress(ring);
        int index = *ResolveGlobalPsxAddress(ring + 1);
        int count = lastIndex + 1;
        uint points = ring + 4;

        // GTE control registers from xmmword_180211F40: R11R12, R13R21, R22R23, R31R32, R33, TRX, TRY, TRZ.
        int* rt = (int*)xmmword_180211F40;
        EmitScreenAlignedQuadStripSaveMatrix(desc, rt);

        int* v0 = (int*)Unsafe.AsPointer(ref qword_180211FC0);
        int* ir = (int*)Unsafe.AsPointer(ref dword_180211FE4);
        int segment = 0;
        do
        {
            uint point = points + (8u * (uint)index);
            int next = index + 1;
            int nextUnwrapped = next;
            if (next == count)
            {
                next = 0;
            }

            int afterNext = next + 1;
            if (afterNext == count)
            {
                afterNext = 0;
            }

            uint nextPoint = points + (8u * (uint)next);
            uint farPoint = segment - a1 + 1 >= 0 ? nextPoint : points + (8u * (uint)afterNext);

            dword_180211FC8 = *(int*)ResolveGlobalPsxAddress(point);
            dword_180211FCC = *(int*)ResolveGlobalPsxAddress(point + 4);
            v0[0] = *(int*)ResolveGlobalPsxAddress(farPoint);
            v0[1] = *(int*)ResolveGlobalPsxAddress(farPoint + 4);
            dword_180211FD0 = *(int*)ResolveGlobalPsxAddress(nextPoint);
            dword_180211FD4 = *(int*)ResolveGlobalPsxAddress(nextPoint + 4);
            GteRtps(0);
            GteRtps(1);
            GteRtps(2);

            *(int*)ResolveGlobalPsxAddress(desc + 80) = dword_180211FF8;
            *(int*)ResolveGlobalPsxAddress(desc + 84) = dword_180211FF4;
            *(int*)ResolveGlobalPsxAddress(desc + 88) = dword_180211FF0;
            *(int*)ResolveGlobalPsxAddress(desc + 96) = dword_180212024;
            *(int*)ResolveGlobalPsxAddress(desc + 100) = dword_180212028;
            *(int*)ResolveGlobalPsxAddress(desc + 104) = dword_18021202C;

            // Screen direction (SXY0 - SXY1) << 7 turned by OP with the diagonal (0, 0, 4096).
            int x1 = *(short*)ResolveGlobalPsxAddress(desc + 84);
            int y1 = *(short*)ResolveGlobalPsxAddress(desc + 86);
            rt[0] = 0;
            rt[2] = 0;
            dword_180211F50 = 4096;
            int x0 = *(short*)ResolveGlobalPsxAddress(desc + 88);
            int y0 = *(short*)ResolveGlobalPsxAddress(desc + 90);
            *(int*)ResolveGlobalPsxAddress(desc + 92) = dword_180212008;
            ir[2] = 0;
            ir[0] = (x0 - x1) << 7;
            ir[1] = (y0 - y1) << 7;
            GteOp();

            int x2 = *(ushort*)ResolveGlobalPsxAddress(desc + 80);
            int y2 = *(ushort*)ResolveGlobalPsxAddress(desc + 82);
            *(ushort*)ResolveGlobalPsxAddress(desc + 84) = (ushort)(x1 - x2);
            *(ushort*)ResolveGlobalPsxAddress(desc + 86) = (ushort)(y1 - y2);

            int shift = EmitScreenAlignedQuadStripNormalise(mapper, ir);

            int halfWidth = *(short*)ResolveGlobalPsxAddress(point + 6);
            GteGpfSf0();
            dword_180211FE0 = halfWidth;
            ir[0] = dword_180212024 >> shift;
            ir[1] = dword_180212028 >> shift;
            GteGpf();

            int appliedIr0 = segment + 1 >= a1 ? 4096 : ir0;
            int px = (short)ir[0];
            int py = (short)ir[1];
            ir[0] = px;
            ir[1] = py;

            // Plain 32-bit ORs: a negative low half spills into the high half, as in the original (claim).
            int edgeMinus = (-65536 * py) | (-px - 1);
            int edgePlus = (py << 16) | (px - 1);

            int uv0 = *(ushort*)ResolveGlobalPsxAddress(desc + 36);
            int clutUv = *(int*)ResolveGlobalPsxAddress(desc + 28);
            *(int*)ResolveGlobalPsxAddress(packet + 12) = clutUv | v | uv0;

            uint vDelta = (uint)(appliedIr0 * *(ushort*)ResolveGlobalPsxAddress(desc + 22));
            int trx = *(int*)ResolveGlobalPsxAddress(desc + 96);
            int try_ = *(int*)ResolveGlobalPsxAddress(desc + 100);
            int trz = *(int*)ResolveGlobalPsxAddress(desc + 104);
            rt[5] = trx;
            rt[7] = trz;
            rt[6] = try_;
            v0[0] = edgePlus;
            v0[1] = 0;
            dword_180211FC8 = edgeMinus;
            rt[0] = 4096;
            rt[1] = 0;
            rt[2] = 4096;
            rt[3] = 0;
            dword_180211F50 = 4096;
            dword_180211FCC = 0;

            int uv1 = *(ushort*)ResolveGlobalPsxAddress(desc + 38);
            int tpageUv = *(int*)ResolveGlobalPsxAddress(desc + 24);
            GteRtps(0);
            GteRtps(1);
            GteRtps(2);
            *(int*)ResolveGlobalPsxAddress(packet + 24) = tpageUv | v | uv1;

            v = (int)(vDelta >> 12) + vBase;
            int uv2 = *(ushort*)ResolveGlobalPsxAddress(desc + 36);
            int uv3 = *(ushort*)ResolveGlobalPsxAddress(desc + 38);
            *(ushort*)ResolveGlobalPsxAddress(packet + 36) = (ushort)(v | uv2);
            *(ushort*)ResolveGlobalPsxAddress(packet + 48) = (ushort)(v | uv3);

            int sxy0;
            int sxy1;
            if (segment != 0)
            {
                int previous0 = *(int*)ResolveGlobalPsxAddress(desc + 72);
                int previous1 = *(int*)ResolveGlobalPsxAddress(desc + 76);
                *(int*)ResolveGlobalPsxAddress(packet + 8) = previous0;
                *(int*)ResolveGlobalPsxAddress(packet + 20) = previous1;
                sxy0 = dword_180211FF0;
                sxy1 = dword_180211FF4;
                dword_180211FF8 = previous0;
            }
            else
            {
                // First segment: the near edge is the projected next point offset by the stored SXY1 - SXY2.
                sxy0 = dword_180211FF0;
                int dx = *(ushort*)ResolveGlobalPsxAddress(desc + 84);
                int dy = *(short*)ResolveGlobalPsxAddress(desc + 86);
                sxy1 = dword_180211FF4;
                *(ushort*)ResolveGlobalPsxAddress(packet + 8) = (ushort)(dx + sxy0);
                *(ushort*)ResolveGlobalPsxAddress(packet + 10) = (ushort)(dy + (sxy0 >> 16));
                *(ushort*)ResolveGlobalPsxAddress(packet + 20) = (ushort)(dx + sxy1);
                *(ushort*)ResolveGlobalPsxAddress(packet + 22) = (ushort)(dy + (int)((uint)sxy1 >> 16));
                dword_180211FF4 = sxy1;
            }

            EmitScreenAlignedQuadStripRestoreMatrix(desc, rt);
            dword_180211FE0 = appliedIr0;
            dword_180211FD8 = rgbc;
            GteDpcs();

            *(int*)ResolveGlobalPsxAddress(packet + 4) = nearColor;
            *(int*)ResolveGlobalPsxAddress(packet + 16) = nearColor;
            int farColorOut = dword_180212018;
            nearColor = farColorOut;
            *(int*)ResolveGlobalPsxAddress(packet + 28) = farColorOut;
            *(int*)ResolveGlobalPsxAddress(packet + 40) = farColorOut;
            *(int*)ResolveGlobalPsxAddress(packet + 32) = sxy0;
            *(int*)ResolveGlobalPsxAddress(packet + 44) = sxy1;
            *(int*)ResolveGlobalPsxAddress(desc + 72) = sxy0;
            *(int*)ResolveGlobalPsxAddress(desc + 76) = sxy1;

            int sz = *(int*)ResolveGlobalPsxAddress(desc + 92);
            index = nextUnwrapped == count ? 0 : nextUnwrapped;
            segment++;
            if (sz != 0)
            {
                int otz = ((sz >> 2) - otzBias) >> 2;
                if ((uint)otz < 0x1000)
                {
                    byte* otBase = ResolveGlobalPsxAddress(ot);
                    byte* prim = ResolveGlobalPsxAddress(packet);
                    Prim4* prim4 = (Prim4*)prim;
                    prim[3] = 12;
                    uint* entry = (uint*)(otBase + (4L * otz));
                    prim4->union_0_4 ^= (*entry ^ prim4->union_0_4) & 0xFFFFFF;
                    *entry ^= (*entry ^ (uint)PsxPtrToAddr(mapper, (ulong)prim)) & 0xFFFFFF;
                }
            }

            ir0 = *(int*)ResolveGlobalPsxAddress(desc + 32) + appliedIr0;
            packet += 52;
        }
        while (segment < a1);

        return 0;
    }

    /// <summary>Copies the GTE rotation (5 words) and translation (3 words) into the descriptor at +40..+68.</summary>
    private static void EmitScreenAlignedQuadStripSaveMatrix(uint desc, int* rt)
    {
        Recompiler.Runtime.Tracer.Enter("EmitScreenAlignedQuadStripSaveMatrix(desc, rt)", desc, (nint)rt);
        for (uint i = 0; i < 8; i++)
        {
            *(int*)ResolveGlobalPsxAddress(desc + 40 + (4 * i)) = rt[i];
        }
    }

    /// <summary>Reloads the GTE rotation and translation saved at descriptor +40..+68.</summary>
    private static void EmitScreenAlignedQuadStripRestoreMatrix(uint desc, int* rt)
    {
        Recompiler.Runtime.Tracer.Enter("EmitScreenAlignedQuadStripRestoreMatrix(desc, rt)", desc, (nint)rt);
        for (uint i = 0; i < 8; i++)
        {
            rt[i] = *(int*)ResolveGlobalPsxAddress(desc + 40 + (4 * i));
        }
    }

    /// <summary>
    /// Inlined SQR + LZC + inverse square root of the OP result: MAC1..3 = IR1², IR2², IR3² (IR3 saturated to
    /// 0..0x7FFF, 16-bit store), LZCS = IR1² + IR2², LZCR = its leading sign-bit count; IR1/IR2 get the OP MAC1/MAC2
    /// back and IR0 the s16 entry of the table unk_1800685C4 (registered as a PSX address) for the length normalised to
    /// bits 24..31, index - 64. Returns the matching right shift (31 - (LZCR &amp; ~1)) / 2.
    /// </summary>
    private static int EmitScreenAlignedQuadStripNormalise(long mapper, int* ir)
    {
        Recompiler.Runtime.Tracer.Enter("EmitScreenAlignedQuadStripNormalise(mapper, ir)", mapper, (nint)ir);
        int mac1 = dword_180212024;
        int mac2 = dword_180212028;
        short* ir3 = word_180211FEC;
        int square3 = *ir3 * *ir3;
        int square1 = (short)ir[0] * (short)ir[0];
        int square2 = (short)ir[1] * (short)ir[1];
        dword_180212024 = square1;
        dword_18021202C = square3;
        dword_180212028 = square2;
        *ir3 = (short)(square3 > 0x7FFF ? 0x7FFF : square3 < 0 ? 0 : square3);

        int lengthSquared = square1 + square2;
        int bits = lengthSquared >= 0 ? ~lengthSquared : lengthSquared;
        int leading = 0;
        while (bits < 0)
        {
            leading++;
            bits <<= 1;
        }

        dword_18021203C = leading;
        int even = leading & 0xFFFE;
        int shift = (31 - even) >> 1;
        dword_180212038 = lengthSquared;
        ir[0] = mac1;
        ir[1] = mac2;
        int normalised = even - 24 >= 0 ? lengthSquared << (even - 24) : lengthSquared >> (24 - even);

        uint table = (uint)PsxAddrMapperRegister(mapper, (ulong)unk_1800685C4);
        dword_180211FE0 = *(short*)ResolveGlobalPsxAddress(unchecked(table + (uint)((2 * normalised) - 128)));
        return shift;
    }
}
