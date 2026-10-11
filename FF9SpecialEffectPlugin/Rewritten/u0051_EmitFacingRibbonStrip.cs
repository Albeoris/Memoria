using System.Numerics;
using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Depth-cued textured ribbon whose width is oriented towards a reference point. Registers the descriptor
    /// <paramref name="a2"/>, the packet buffer <paramref name="a3"/> and the ordering table <paramref name="a4"/> as PSX
    /// addresses and resolves every field address on its own, like the original. Descriptor: +0 OTZ bias, +4 address of
    /// the point ring (byte 0 = count - 1, byte 1 = start index, 8-byte points from +4: s16 x, y, z, s16 half-width),
    /// +8 initial IR0, +12 RGBC, +16 far color (0x00BBGGRR), +20 u16 V base, +22 u16 V scale, +24 tpage | uv1 word,
    /// +28 clut | uv0 word, +32 IR0 step, +36 u16 uv0/uv2, +38 u16 uv1/uv3, +40 s16 reference point x, y, z; scratch:
    /// +48..+56 saved RT11/RT22/RT33 words, +60/+64 previous SXY0/SXY1, +68..+76 current point, +80..+88 next point.
    /// Per segment the offset is OP(4 * (next - current), reference - current) normalised with the inverse square root
    /// table at 0x1800685C4 (LZCS/LZCR) and scaled by GPF with the point's half-width; the points next ± offset become
    /// the far edge of a 52-byte POLY_GT4 (<paramref name="a1"/> segments, at least one), the near edge is the previous
    /// far edge (current ± offset for the first). Near colors = previous DPCS output (RGBC for the first), far colors =
    /// DPCS(RGBC, IR0) (IR0 accumulating by the step, 4096 for the last); linked into the OT at
    /// ((SZ3 &gt;&gt; 2) - bias) &gt;&gt; 2 when below 4096. Returns 0.
    /// </summary>
    public static long EmitFacingRibbonStrip(int a1, ulong a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("EmitFacingRibbonStrip(a1, a2, a3, a4)", a1, a2, a3, a4);
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

        byte* rt = xmmword_180211F40;
        int rt22 = *(int*)(rt + 8);
        *(int*)ResolveGlobalPsxAddress(desc + 48) = *(int*)rt;
        int rt33 = dword_180211F50;
        *(int*)ResolveGlobalPsxAddress(desc + 52) = rt22;
        *(int*)ResolveGlobalPsxAddress(desc + 56) = rt33;

        int* ir = (int*)Unsafe.AsPointer(ref dword_180211FE4);
        int* v0 = (int*)Unsafe.AsPointer(ref qword_180211FC0);
        int segment = 0;
        do
        {
            uint point = points + (8u * (uint)index);
            int nextUnwrapped = index + 1;
            int next = nextUnwrapped == count ? 0 : nextUnwrapped;
            uint nextPoint = points + (8u * (uint)next);

            EmitFacingRibbonStripOffset(desc, point, nextPoint, rt, ir);

            int ir1 = (short)ir[0];
            int ir2 = (short)ir[1];
            int ir3 = (short)ir[2];
            int nextXy;
            int nextY;
            int nextZ;
            if (segment == 0)
            {
                int x = *(int*)ResolveGlobalPsxAddress(desc + 68);
                int y = *(int*)ResolveGlobalPsxAddress(desc + 72);
                int z = *(int*)ResolveGlobalPsxAddress(desc + 76);
                ir[0] = ir1;
                ir[1] = ir2;
                ir[2] = ir3;
                EmitFacingRibbonStripRestoreRotation(desc, rt);
                v0[0] = ((ir2 + y) << 16) | (ushort)(x + ir1);
                v0[1] = ir3 + z;
                dword_180211FCC = z - ir3;
                dword_180211FC8 = ((y - ir2) << 16) | (ushort)(x - ir1);
                nextXy = *(int*)ResolveGlobalPsxAddress(desc + 80);
                GteRtps(0);
                GteRtps(1);
                GteRtps(2);
                nextY = *(int*)ResolveGlobalPsxAddress(desc + 84);
                nextZ = *(int*)ResolveGlobalPsxAddress(desc + 88);
                *(int*)ResolveGlobalPsxAddress(packet + 8) = dword_180211FF0;
                *(int*)ResolveGlobalPsxAddress(packet + 20) = dword_180211FF4;
            }
            else
            {
                int previousXy0 = *(int*)ResolveGlobalPsxAddress(desc + 60);
                int previousXy1 = *(int*)ResolveGlobalPsxAddress(desc + 64);
                *(int*)ResolveGlobalPsxAddress(packet + 8) = previousXy0;
                *(int*)ResolveGlobalPsxAddress(packet + 20) = previousXy1;
                nextXy = *(int*)ResolveGlobalPsxAddress(desc + 80);
                nextY = *(int*)ResolveGlobalPsxAddress(desc + 84);
                nextZ = *(int*)ResolveGlobalPsxAddress(desc + 88);
                ir[2] = ir3;
                ir[0] = ir1;
                ir[1] = ir2;
            }

            // nextXy holds the next point's sign-extended x (stored at desc + 80), nextY its y (desc + 84).
            int farXyMinus = ((nextY - ir2) << 16) | (ushort)(nextXy - ir1);
            int farZMinus = nextZ - ir3;
            int farZPlus = nextZ + ir3;
            int farXyPlus = ((ir2 + nextY) << 16) | (ushort)(ir1 + nextXy);

            if (++segment >= a1)
            {
                ir0 = 4096;
            }

            int appliedIr0 = ir0;
            dword_180211FD8 = rgbc;
            dword_180211FE0 = ir0;
            int uv0 = *(ushort*)ResolveGlobalPsxAddress(desc + 36);
            int clutUv = *(int*)ResolveGlobalPsxAddress(desc + 28);
            GteDpcs();

            int vScale = *(ushort*)ResolveGlobalPsxAddress(desc + 22);
            uint vDelta = (uint)(appliedIr0 * vScale);
            *(int*)ResolveGlobalPsxAddress(packet + 4) = nearColor;
            *(int*)ResolveGlobalPsxAddress(packet + 16) = nearColor;
            *(int*)ResolveGlobalPsxAddress(packet + 12) = clutUv | v | uv0;
            int uv1 = *(ushort*)ResolveGlobalPsxAddress(desc + 38);
            *(int*)ResolveGlobalPsxAddress(packet + 24) = *(int*)ResolveGlobalPsxAddress(desc + 24) | v | uv1;

            // Logical shift: the product is unsigned in the original (shr r15d, 0Ch).
            v = (int)(vDelta >> 12) + vBase;
            int farColorOut = dword_180212018;
            nearColor = farColorOut;
            EmitFacingRibbonStripRestoreRotation(desc, rt);
            dword_180211FC8 = farXyMinus;
            v0[0] = farXyPlus;
            v0[1] = farZPlus;
            dword_180211FCC = farZMinus;
            dword_180211FD0 = *(int*)ResolveGlobalPsxAddress(point);
            dword_180211FD4 = *(int*)ResolveGlobalPsxAddress(point + 4);
            ushort uv2 = *(ushort*)ResolveGlobalPsxAddress(desc + 36);
            ushort uv3 = *(ushort*)ResolveGlobalPsxAddress(desc + 38);
            GteRtps(0);
            GteRtps(1);
            GteRtps(2);
            *(int*)ResolveGlobalPsxAddress(packet + 28) = farColorOut;
            *(int*)ResolveGlobalPsxAddress(packet + 40) = farColorOut;
            *(ushort*)ResolveGlobalPsxAddress(packet + 36) = (ushort)(v | uv2);
            *(ushort*)ResolveGlobalPsxAddress(packet + 48) = (ushort)(v | uv3);
            int xy0 = dword_180211FF0;
            int xy1 = dword_180211FF4;
            *(int*)ResolveGlobalPsxAddress(packet + 32) = xy0;
            *(int*)ResolveGlobalPsxAddress(packet + 44) = xy1;
            *(int*)ResolveGlobalPsxAddress(desc + 60) = xy0;
            *(int*)ResolveGlobalPsxAddress(desc + 64) = xy1;

            index = next;
            int otz = ((dword_18021200C >> 2) - otzBias) >> 2;
            if ((uint)otz < 0x1000)
            {
                byte* otBase = ResolveGlobalPsxAddress(ot);
                byte* prim = ResolveGlobalPsxAddress(packet);
                Prim3* prim3 = (Prim3*)prim;
                prim[3] = 12;
                uint* entry = (uint*)(otBase + (4L * otz));
                prim3->union_0_4 ^= (*entry ^ prim3->union_0_4) & 0xFFFFFF;
                *entry ^= (*entry ^ (uint)PsxPtrToAddr(mapper, (ulong)prim)) & 0xFFFFFF;
            }

            ir0 = *(int*)ResolveGlobalPsxAddress(desc + 32) + appliedIr0;
            packet += 52;
        }
        while (segment < a1);

        return 0;
    }

    /// <summary>
    /// Width offset of the segment <paramref name="point"/> → <paramref name="nextPoint"/>: stores both points' s16
    /// coordinates at desc + 68..76 / + 80..88, loads RT11/RT22/RT33 with 4 * (next - current) and IR1..3 with
    /// reference - current, runs OP, then SQR-like squares into MAC1..3 with LZCS/LZCR of their sum, restores IR1..3
    /// = OP's MAC1..3, IR0 = s16 table[0x1800685C4 + 2 * (sum normalised to bits 24..) - 128], GPF(sf 0), and finally
    /// IR0 = the point's half-width, IR1..3 = MAC1..3 &gt;&gt; ((31 - (LZCR &amp; ~1)) &gt;&gt; 1) and GPF.
    /// </summary>
    private static void EmitFacingRibbonStripOffset(uint desc, uint point, uint nextPoint, byte* rt, int* ir)
    {
        Recompiler.Runtime.Tracer.Enter("EmitFacingRibbonStripOffset(desc, point, nextPoint, rt, ir)", desc, point, nextPoint, (nint)rt, (nint)ir);
        int nx = *(short*)ResolveGlobalPsxAddress(nextPoint);
        int cx = *(short*)ResolveGlobalPsxAddress(point);
        *(int*)ResolveGlobalPsxAddress(desc + 80) = nx;
        *(int*)rt = 4 * (nx - cx);
        int ny = *(short*)ResolveGlobalPsxAddress(nextPoint + 2);
        int cy = *(short*)ResolveGlobalPsxAddress(point + 2);
        *(int*)ResolveGlobalPsxAddress(desc + 84) = ny;
        *(int*)(rt + 8) = 4 * (ny - cy);
        int nz = *(short*)ResolveGlobalPsxAddress(nextPoint + 4);
        int cz = *(short*)ResolveGlobalPsxAddress(point + 4);
        *(int*)ResolveGlobalPsxAddress(desc + 88) = nz;
        dword_180211F50 = 4 * (nz - cz);
        int refX = *(short*)ResolveGlobalPsxAddress(desc + 40);
        *(int*)ResolveGlobalPsxAddress(desc + 68) = cx;
        ir[0] = refX - cx;
        int refY = *(short*)ResolveGlobalPsxAddress(desc + 42);
        *(int*)ResolveGlobalPsxAddress(desc + 72) = cy;
        ir[1] = refY - cy;
        int refZ = *(short*)ResolveGlobalPsxAddress(desc + 44);
        *(int*)ResolveGlobalPsxAddress(desc + 76) = cz;
        ir[2] = refZ - cz;
        GteOp();

        int mac1 = dword_180212024;
        int mac2 = dword_180212028;
        int mac3 = dword_18021202C;
        int sq1 = (short)ir[0] * (short)ir[0];
        int sq2 = (short)ir[1] * (short)ir[1];
        int sq3 = (short)ir[2] * (short)ir[2];
        int sum = unchecked(sq3 + sq2 + sq1);
        dword_180212024 = sq1;
        dword_18021202C = sq3;
        dword_180212028 = sq2;

        // LZCR: count of leading bits equal to the sign bit (32 for 0 and -1), as the original's shift loop.
        int leading = BitOperations.LeadingZeroCount((uint)(sum < 0 ? ~sum : sum));
        dword_18021203C = leading;
        int even = leading & 0xFFFE;
        dword_180212038 = sum;
        int shift = (31 - even) >> 1;
        ir[0] = mac1;
        ir[1] = mac2;
        ir[2] = mac3;
        int normalised = even - 24 >= 0 ? sum << (even - 24) : sum >> (24 - even);

        uint table = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)unk_1800685C4);
        dword_180211FE0 = *(short*)ResolveGlobalPsxAddress(unchecked((uint)((2 * normalised) - 128) + table));
        int halfWidth = *(short*)ResolveGlobalPsxAddress(point + 6);
        GteGpfSf0();
        dword_180211FE0 = halfWidth;
        ir[2] = dword_18021202C >> shift;
        ir[0] = dword_180212024 >> shift;
        ir[1] = dword_180212028 >> shift;
        GteGpf();
    }

    /// <summary>Reloads RT11/RT12, RT22/RT23 and RT33 from the words saved at desc + 48, + 52 and + 56.</summary>
    private static void EmitFacingRibbonStripRestoreRotation(uint desc, byte* rt)
    {
        Recompiler.Runtime.Tracer.Enter("EmitFacingRibbonStripRestoreRotation(desc, rt)", desc, (nint)rt);
        int rt11 = *(int*)ResolveGlobalPsxAddress(desc + 48);
        int rt22 = *(int*)ResolveGlobalPsxAddress(desc + 52);
        *(int*)rt = rt11;
        int rt33 = *(int*)ResolveGlobalPsxAddress(desc + 56);
        *(int*)(rt + 8) = rt22;
        dword_180211F50 = rt33;
    }
}
