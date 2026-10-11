using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Depth-cued textured ribbon. Registers the descriptor <paramref name="a2"/>, the packet buffer
    /// <paramref name="a3"/> and the ordering table <paramref name="a4"/> as PSX addresses and resolves every field
    /// address on its own, like the original. Descriptor: +0 OTZ bias, +4 address of the point ring (byte 0 = count - 1,
    /// byte 1 = start index, 8-byte points from +4: u32 VXY, s16 VZ, u16 GPF IR0), +8 initial IR0, +12 RGBC, +16 far
    /// color (0x00BBGGRR), +20 u16 V base, +22 u16 V scale, +24 tpage | uv1 word, +28 clut | uv0 word, +32 IR0 step,
    /// +36 u16 uv0/uv2, +38 u16 uv1/uv3, +40 s16 IR1 / s16 IR2 half-width. Each ring point is widened by GPF (IR0 = the
    /// point's scale, IR1/IR2 = the half-width) into an edge (x ± IR1, z ± IR2); consecutive edges become one 52-byte
    /// POLY_GT4 per segment (<paramref name="a1"/> segments, at least one) with near colors = previous DPCS output (RGBC
    /// for the first), far colors = DPCS(RGBC, IR0) (IR0 accumulating by the step, 4096 for the last), linked into the OT
    /// at ((SZ3 &gt;&gt; 2) - bias) &gt;&gt; 2 when below 4096. Returns 0.
    /// </summary>
    public static long EmitDepthCuedQuadStrip(int a1, ulong a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("EmitDepthCuedQuadStrip(a1, a2, a3, a4)", a1, a2, a3, a4);
        long mapper = (long)unk_180576A10;
        uint desc = (uint)PsxAddrMapperRegister(mapper, a2);
        uint packet = (uint)PsxAddrMapperRegister(mapper, a3);
        uint ot = (uint)PsxAddrMapperRegister(mapper, a4);

        int otzBias = *(int*)ResolveGlobalPsxAddress(desc);
        int halfWidth = *(int*)ResolveGlobalPsxAddress(desc + 40);
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
        int ir1Width = (short)halfWidth;
        int ir2Width = halfWidth >> 16;
        int count = lastIndex + 1;
        uint points = ring + 4;

        int* ir = (int*)Unsafe.AsPointer(ref dword_180211FE4);
        int* v0 = (int*)Unsafe.AsPointer(ref qword_180211FC0);
        int previousXy0 = 0;
        int previousXy1 = 0;
        int segment = 0;
        do
        {
            uint point = points + (8u * (uint)index);
            int scale = *(ushort*)ResolveGlobalPsxAddress(point + 6);
            ir[0] = ir1Width;
            ir[1] = ir2Width;
            dword_180211FE0 = scale;
            GteGpf();

            int next = index + 1;
            int nextUnwrapped = next;
            if (next == count)
            {
                next = 0;
            }

            int farXyPlus;
            int farZPlus;
            int farXyMinus;
            int farZMinus;
            if (segment != 0)
            {
                *(int*)ResolveGlobalPsxAddress(packet + 8) = previousXy0;
                *(int*)ResolveGlobalPsxAddress(packet + 20) = previousXy1;
                EmitDepthCuedQuadStripEdge(points + (8u * (uint)next), ir, out farXyPlus, out farZPlus, out farXyMinus, out farZMinus);
            }
            else
            {
                int xy = *(int*)ResolveGlobalPsxAddress(point);
                int z = *(short*)ResolveGlobalPsxAddress(point + 4);
                int ir1 = (short)ir[0];
                int ir2 = (short)ir[1];
                uint high = (uint)xy & 0xFFFF0000;
                ir[0] = ir1;
                ir[1] = ir2;
                v0[0] = (int)(high | (ushort)(xy + ir1));
                v0[1] = z + ir2;
                dword_180211FCC = z - ir2;
                dword_180211FC8 = (int)(high | (ushort)(xy - ir1));
                GteRtps(0);
                GteRtps(1);
                GteRtps(2);
                dword_180212020 = ((short)dword_180211FF8 * (dword_180211FF0 >> 16))
                                + ((dword_180211FF4 >> 16) * (short)dword_180211FF0)
                                + ((short)dword_180211FF4 * (dword_180211FF8 >> 16))
                                - ((short)dword_180211FF0 * (dword_180211FF8 >> 16))
                                - ((dword_180211FF0 >> 16) * (short)dword_180211FF4)
                                - ((short)dword_180211FF8 * (dword_180211FF4 >> 16));

                uint nextPoint = points + (8u * (uint)next);
                int nextXy = *(int*)ResolveGlobalPsxAddress(nextPoint);
                int nextZ = *(short*)ResolveGlobalPsxAddress(nextPoint + 4);
                uint nextHigh = (uint)nextXy & 0xFFFF0000;
                farXyMinus = (int)(nextHigh | (ushort)(nextXy - ir1));
                farZPlus = ir2 + nextZ;
                farZMinus = nextZ - ir2;
                farXyPlus = (int)(nextHigh | (ushort)(ir1 + nextXy));
                *(int*)ResolveGlobalPsxAddress(packet + 8) = dword_180211FF0;
                *(int*)ResolveGlobalPsxAddress(packet + 20) = dword_180211FF4;
            }

            if (++segment >= a1)
            {
                ir0 = 4096;
            }

            int appliedIr0 = ir0;
            dword_180211FE0 = ir0;
            dword_180211FD8 = rgbc;
            int uv0 = *(ushort*)ResolveGlobalPsxAddress(desc + 36);
            int clutUv = *(int*)ResolveGlobalPsxAddress(desc + 28);
            GteDpcs();

            // The pseudocode reads this u16 through an uninitialised register + 22; desc + 22 (beside the V base at +20)
            // is the assumption recorded in the ledger.
            int vScale = *(ushort*)ResolveGlobalPsxAddress(desc + 22);
            uint vDelta = (uint)(appliedIr0 * vScale);
            *(int*)ResolveGlobalPsxAddress(packet + 4) = nearColor;
            *(int*)ResolveGlobalPsxAddress(packet + 16) = nearColor;
            *(int*)ResolveGlobalPsxAddress(packet + 12) = clutUv | v | uv0;
            int uv1 = *(ushort*)ResolveGlobalPsxAddress(desc + 38);
            int tpageUv = *(int*)ResolveGlobalPsxAddress(desc + 24) | v | uv1;
            *(int*)ResolveGlobalPsxAddress(packet + 24) = tpageUv;

            v = (int)(vDelta >> 12) + vBase;
            int farColorOut = dword_180212018;
            nearColor = farColorOut;
            v0[0] = farXyPlus;
            v0[1] = farZPlus;
            dword_180211FC8 = farXyMinus;
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
            previousXy0 = dword_180211FF0;
            previousXy1 = dword_180211FF4;
            *(int*)ResolveGlobalPsxAddress(packet + 32) = dword_180211FF0;
            *(int*)ResolveGlobalPsxAddress(packet + 44) = previousXy1;

            index = nextUnwrapped == count ? 0 : nextUnwrapped;
            int otz = ((dword_18021200C >> 2) - otzBias) >> 2;
            if ((uint)otz < 0x1000)
            {
                byte* otBase = ResolveGlobalPsxAddress(ot);
                byte* prim = ResolveGlobalPsxAddress(packet);
                Prim2* prim2 = (Prim2*)prim;
                prim[3] = 12;
                uint* entry = (uint*)(otBase + (4L * otz));
                prim2->union_0_4 ^= (*entry ^ prim2->union_0_4) & 0xFFFFFF;
                *entry ^= (*entry ^ (uint)PsxPtrToAddr(mapper, (ulong)prim)) & 0xFFFFFF;
            }

            ir0 = *(int*)ResolveGlobalPsxAddress(desc + 32) + appliedIr0;
            packet += 52;
        }
        while (segment < a1);

        return 0;
    }

    /// <summary>
    /// Edge of the ring point at PSX address <paramref name="point"/> after GPF: IR1/IR2 are written back as their
    /// sign-extended 16-bit views, then (x ± IR1 in the low half of VXY, z ± IR2).
    /// </summary>
    private static void EmitDepthCuedQuadStripEdge(uint point, int* ir, out int xyPlus, out int zPlus, out int xyMinus, out int zMinus)
    {
        Recompiler.Runtime.Tracer.Enter("EmitDepthCuedQuadStripEdge(point, ir)", point, (nint)ir);
        int xy = *(int*)ResolveGlobalPsxAddress(point);
        int z = *(short*)ResolveGlobalPsxAddress(point + 4);
        uint high = (uint)xy & 0xFFFF0000;
        ir[0] = (short)ir[0];
        ir[1] = (short)ir[1];
        int ir1 = (short)ir[0];
        int ir2 = (short)ir[1];
        xyMinus = (int)(high | (ushort)(xy - ir1));
        zMinus = z - ir2;
        zPlus = ir2 + z;
        xyPlus = (int)(high | (ushort)(ir1 + xy));
    }
}
