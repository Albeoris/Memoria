using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>GEO_MAX_VERTICES of geoslice.cpp (0x1B58): size of the projected-vertex tables.</summary>
    private const uint GeoSliceMaxVertices = 0x1B58;

    /// <summary>Depth sum limits: quads are dropped from 0x40000000 (4 depths), triangles from 0x30000000 (3 depths).</summary>
    private const int GeoQuadDepthLimit = 0x40000000;
    private const int GeoTriDepthLimit = 0x30000000;

    /// <summary>
    /// geoslice.cpp: emits the PSX GPU primitives of part <paramref name="a2"/> of the mesh <paramref name="a1"/> into the
    /// packet buffer <paramref name="a3"/> and links each into the ordering table addressed by dword 8 of off_180066C68.
    /// The mesh (+0 bit 0x10 = hidden, +8 PSX address of a header whose dword +16 addresses the
    /// <see cref="GeoSlicePart"/> array, +24 first packet, +40 texture table: clut &lt;&lt; 16 | tpage per texture index) is
    /// rendered in the order FT4, FT3, GT4, GT3, G4, G3, F4, F3 from one record stream. Vertices come from the projected
    /// screen XY table 0x1802191A0 and the depth table 0x180212440; faces whose depth sum is too large or that fail
    /// NCLIP (unless double-sided, flag bit 2) are skipped. Returns the address past the terminating 0xFF code byte.
    /// </summary>
    public static long GeoSliceEmitPrims(long a1, byte a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("GeoSliceEmitPrims(a1, a2, a3)", a1, a2, a3);
        Shape_PsgBuildBoneMatrices_a1* shape_PsgBuildBoneMatrices_a1 = (Shape_PsgBuildBoneMatrices_a1*)a1;
        if ((shape_PsgBuildBoneMatrices_a1->field_0 & 0x10) != 0)
        {
            return a3;
        }

        byte* mesh = ResolveGlobalPsxAddress(shape_PsgBuildBoneMatrices_a1->field_8);
        byte* parts = ResolveGlobalPsxAddress(*((uint*)mesh + 4));
        GeoSlicePart* part = (GeoSlicePart*)(parts + 40 * a2);
        GeoSliceTables t;
        t.Uv = ResolveGlobalPsxAddress(part->uvTableAddr);
        byte* rec = ResolveGlobalPsxAddress(part->primsAddr);
        t.Colors = ResolveGlobalPsxAddress(part->colorTableAddr);
        if ((long)shape_PsgBuildBoneMatrices_a1->field_24 == 0)
        {
            shape_PsgBuildBoneMatrices_a1->field_24 = (ulong)a3;
        }

        t.Tex = (byte*)shape_PsgBuildBoneMatrices_a1->field_40;
        t.Ot = ResolveGlobalPsxAddress(((uint*)off_180066C68)[8]);
        byte* dst = (byte*)a3;

        for (int n = part->countFt4; n != 0; --n, rec += 24)
        {
            dst = GeoEmitFt4(rec, dst, &t);
        }

        for (int n = part->countFt3; n != 0; --n, rec += 20)
        {
            dst = GeoEmitFt3(rec, dst, &t);
        }

        for (int n = part->countGt4; n != 0; --n, rec += 32)
        {
            dst = GeoEmitGt4(rec, dst, &t);
        }

        for (int n = part->countGt3; n != 0; --n, rec += 24)
        {
            dst = GeoEmitGt3(rec, dst, &t);
        }

        for (int n = part->countG4; n != 0; --n, rec += 24)
        {
            dst = GeoEmitG4(rec, dst, &t);
        }

        for (int n = part->countG3; n != 0; --n, rec += 20)
        {
            dst = GeoEmitG3(rec, dst, &t);
        }

        for (int n = part->countF4; n != 0; --n, rec += 16)
        {
            dst = GeoEmitF4(rec, dst, &t);
        }

        for (int n = part->countF3; n != 0; --n, rec += 12)
        {
            dst = GeoEmitF3(rec, dst, &t);
        }

        dst[7] = 0xFF;
        return (long)(dst + 8);
    }

    /// <summary>Resolved tables shared by the primitive emitters.</summary>
    private struct GeoSliceTables
    {
        /// <summary>UV words (ushort per UV index).</summary>
        public byte* Uv;

        /// <summary>Gouraud colors (uint per color index).</summary>
        public byte* Colors;

        /// <summary>Texture table of the mesh (uint clut &lt;&lt; 16 | tpage per texture index).</summary>
        public byte* Tex;

        /// <summary>Ordering table (4096 uint entries).</summary>
        public byte* Ot;
    }

    /// <summary>FT4 record (24 bytes): +0/+4 vertex indices, +8/+12 UV indices, +16 color, +19 texture, +21 flags.</summary>
    private static byte* GeoEmitFt4(byte* rec, byte* dst, GeoSliceTables* t)
    {
        Recompiler.Runtime.Tracer.Enter("GeoEmitFt4(rec, dst, t)", (nint)rec, (nint)dst, (nint)t);
        GeoQuadIndices(rec, 0x101, out uint i0, out uint i1, out uint i2, out uint i3);
        int* sxy = GeoSxy;
        int s1 = sxy[i1];
        int s0 = sxy[i0];
        int s2 = sxy[i2];
        int z = GeoDepthSum4(i0, i1, i2, i3);
        if (z >= GeoQuadDepthLimit || !GeoSliceFacing(rec[21], s0, s1, s2))
        {
            return dst;
        }

        uint color = *(uint*)(rec + 16);
        *(int*)(dst + 16) = s1;
        *(uint*)(dst + 4) = color;
        *(int*)(dst + 8) = s0;
        dst[7] = 44;
        *(int*)(dst + 24) = s2;
        *(int*)(dst + 32) = sxy[i3];
        uint uv23 = *(uint*)(rec + 12);
        uint tex = GeoTex(t, rec[19]);
        uint uv01 = *(uint*)(rec + 8);
        *(uint*)(dst + 12) = (tex & 0xFFFF0000) | GeoUv(t, uv01 & 0xFFFF);
        *(uint*)(dst + 20) = (tex << 16) | GeoUv(t, uv01 >> 16);
        *(ushort*)(dst + 28) = GeoUv(t, uv23 & 0xFFFF);
        *(ushort*)(dst + 36) = GeoUv(t, uv23 >> 16);
        GeoSemiTrans(dst, rec[21], 46, 22);
        GeoAddPrim(dst, 9, t->Ot, GeoOtzQuad(z));
        return dst + 40;
    }

    /// <summary>FT3 record (20 bytes): +0/+4 vertex indices, +6 texture, +8 color, +12/+16 UV indices, +18 flags.</summary>
    private static byte* GeoEmitFt3(byte* rec, byte* dst, GeoSliceTables* t)
    {
        Recompiler.Runtime.Tracer.Enter("GeoEmitFt3(rec, dst, t)", (nint)rec, (nint)dst, (nint)t);
        GeoTriIndices(rec, 0x158, out uint i0, out uint i1, out uint i2);
        int* sxy = GeoSxy;
        int s1 = sxy[i1];
        int s2 = sxy[i2];
        int z = GeoDepthSum3(i0, i1, i2);
        int s0 = sxy[i0];
        if (z >= GeoTriDepthLimit || !GeoSliceFacing(rec[18], s0, s1, s2))
        {
            return dst;
        }

        uint color = *(uint*)(rec + 8);
        *(int*)(dst + 8) = s0;
        *(uint*)(dst + 4) = color;
        *(int*)(dst + 16) = s1;
        *(int*)(dst + 24) = s2;
        dst[7] = 36;
        uint tex = GeoTex(t, rec[6]);
        uint uv01 = *(uint*)(rec + 12);
        *(uint*)(dst + 12) = (tex & 0xFFFF0000) | GeoUv(t, uv01 & 0xFFFF);
        *(uint*)(dst + 20) = (tex << 16) | GeoUv(t, uv01 >> 16);
        *(ushort*)(dst + 28) = GeoUv(t, *(ushort*)(rec + 16));
        GeoSemiTrans(dst, rec[18], 38, 22);
        GeoAddPrim(dst, 7, t->Ot, GeoOtzTri(z));
        return dst + 32;
    }

    /// <summary>GT4 record (32 bytes): +0/+4 vertex, +8/+12 UV, +16/+20 color indices, +28 flags, +29 texture.</summary>
    private static byte* GeoEmitGt4(byte* rec, byte* dst, GeoSliceTables* t)
    {
        Recompiler.Runtime.Tracer.Enter("GeoEmitGt4(rec, dst, t)", (nint)rec, (nint)dst, (nint)t);
        GeoQuadIndices(rec, 0x1AE, out uint i0, out uint i1, out uint i2, out uint i3);
        int* sxy = GeoSxy;
        int s1 = sxy[i1];
        int s0 = sxy[i0];
        int s2 = sxy[i2];
        int z = GeoDepthSum4(i0, i1, i2, i3);
        if (z >= GeoQuadDepthLimit || !GeoSliceFacing(rec[28], s0, s1, s2))
        {
            return dst;
        }

        *(int*)(dst + 8) = s0;
        *(int*)(dst + 20) = s1;
        *(int*)(dst + 32) = s2;
        *(int*)(dst + 44) = sxy[i3];
        uint c23 = *(uint*)(rec + 20);
        uint tex = GeoTex(t, rec[29]);
        uint c01 = *(uint*)(rec + 16);
        *(uint*)(dst + 4) = GeoColor(t, c01 & 0xFFFF);
        *(uint*)(dst + 16) = GeoColor(t, c01 >> 16);
        *(uint*)(dst + 28) = GeoColor(t, c23 & 0xFFFF);
        uint c3 = GeoColor(t, c23 >> 16);
        dst[7] = 60;
        *(uint*)(dst + 40) = c3;
        uint uv23 = *(uint*)(rec + 12);
        uint uv01 = *(uint*)(rec + 8);
        *(uint*)(dst + 12) = (tex & 0xFFFF0000) | GeoUv(t, uv01 & 0xFFFF);
        *(uint*)(dst + 24) = (tex << 16) | GeoUv(t, uv01 >> 16);
        *(ushort*)(dst + 36) = GeoUv(t, uv23 & 0xFFFF);
        *(ushort*)(dst + 48) = GeoUv(t, uv23 >> 16);
        GeoSemiTrans(dst, rec[28], 62, 26);
        GeoAddPrim(dst, 12, t->Ot, GeoOtzQuad(z));
        return dst + 52;
    }

    /// <summary>GT3 record (24 bytes): +0/+4 vertex, +6 UV0, +8 UV1/UV2, +15 flags, +16/+20 color indices, +22 texture.</summary>
    private static byte* GeoEmitGt3(byte* rec, byte* dst, GeoSliceTables* t)
    {
        Recompiler.Runtime.Tracer.Enter("GeoEmitGt3(rec, dst, t)", (nint)rec, (nint)dst, (nint)t);
        GeoTriIndices(rec, 0x20D, out uint i0, out uint i1, out uint i2);
        int* sxy = GeoSxy;
        int z = GeoDepthSum3(i0, i1, i2);
        int s1 = sxy[i1];
        int s2 = sxy[i2];
        int s0 = sxy[i0];
        if (z >= GeoTriDepthLimit || !GeoSliceFacing(rec[15], s0, s1, s2))
        {
            return dst;
        }

        *(int*)(dst + 8) = s0;
        *(int*)(dst + 20) = s1;
        *(int*)(dst + 32) = s2;
        uint tex = GeoTex(t, rec[22]);
        uint c01 = *(uint*)(rec + 16);
        *(uint*)(dst + 4) = GeoColor(t, c01 & 0xFFFF);
        *(uint*)(dst + 16) = GeoColor(t, c01 >> 16);
        uint c2 = GeoColor(t, *(ushort*)(rec + 20));
        dst[7] = 52;
        *(uint*)(dst + 28) = c2;
        *(uint*)(dst + 12) = (tex & 0xFFFF0000) | GeoUv(t, *(ushort*)(rec + 6));
        uint uv12 = *(uint*)(rec + 8);
        *(uint*)(dst + 24) = (tex << 16) | GeoUv(t, uv12 & 0xFFFF);
        *(ushort*)(dst + 36) = GeoUv(t, uv12 >> 16);
        GeoSemiTrans(dst, rec[15], 54, 26);
        GeoAddPrim(dst, 9, t->Ot, GeoOtzTri(z));
        return dst + 40;
    }

    /// <summary>G4 record (24 bytes): +0/+4 vertex indices, +8/+12 color indices, +20 flags (no semi-transparency code).</summary>
    private static byte* GeoEmitG4(byte* rec, byte* dst, GeoSliceTables* t)
    {
        Recompiler.Runtime.Tracer.Enter("GeoEmitG4(rec, dst, t)", (nint)rec, (nint)dst, (nint)t);
        GeoQuadIndices(rec, 0x267, out uint i0, out uint i1, out uint i2, out uint i3);
        int* sxy = GeoSxy;
        int s1 = sxy[i1];
        int s0 = sxy[i0];
        int s2 = sxy[i2];
        int z = GeoDepthSum4(i0, i1, i2, i3);
        if (z >= GeoQuadDepthLimit || !GeoSliceFacing(rec[20], s0, s1, s2))
        {
            return dst;
        }

        *(int*)(dst + 8) = s0;
        *(int*)(dst + 16) = s1;
        *(int*)(dst + 24) = s2;
        *(int*)(dst + 32) = sxy[i3];
        uint c23 = *(uint*)(rec + 12);
        uint c01 = *(uint*)(rec + 8);
        *(uint*)(dst + 4) = GeoColor(t, c01 & 0xFFFF);
        *(uint*)(dst + 12) = GeoColor(t, c01 >> 16);
        *(uint*)(dst + 20) = GeoColor(t, c23 & 0xFFFF);
        uint c3 = GeoColor(t, c23 >> 16);
        dst[7] = 56;
        *(uint*)(dst + 28) = c3;
        GeoAddPrim(dst, 8, t->Ot, GeoOtzQuad(z));
        return dst + 36;
    }

    /// <summary>G3 record (20 bytes): +0/+4 vertex indices, +8/+12 color indices, +19 flags.</summary>
    private static byte* GeoEmitG3(byte* rec, byte* dst, GeoSliceTables* t)
    {
        Recompiler.Runtime.Tracer.Enter("GeoEmitG3(rec, dst, t)", (nint)rec, (nint)dst, (nint)t);
        GeoTriIndices(rec, 0x2B2, out uint i0, out uint i1, out uint i2);
        int* sxy = GeoSxy;
        int s1 = sxy[i1];
        int s2 = sxy[i2];
        int z = GeoDepthSum3(i0, i1, i2);
        int s0 = sxy[i0];
        if (z >= GeoTriDepthLimit || !GeoSliceFacing(rec[19], s0, s1, s2))
        {
            return dst;
        }

        *(int*)(dst + 8) = s0;
        *(int*)(dst + 16) = s1;
        *(int*)(dst + 24) = s2;
        uint c01 = *(uint*)(rec + 8);
        *(uint*)(dst + 4) = GeoColor(t, c01 & 0xFFFF);
        *(uint*)(dst + 12) = GeoColor(t, c01 >> 16);
        uint c2 = GeoColor(t, *(ushort*)(rec + 12));
        dst[7] = 48;
        *(uint*)(dst + 20) = c2;
        GeoAddPrim(dst, 6, t->Ot, GeoOtzTri(z));
        return dst + 28;
    }

    /// <summary>F4 record (16 bytes): +0/+4 vertex indices, +8 color, +13 flags.</summary>
    private static byte* GeoEmitF4(byte* rec, byte* dst, GeoSliceTables* t)
    {
        Recompiler.Runtime.Tracer.Enter("GeoEmitF4(rec, dst, t)", (nint)rec, (nint)dst, (nint)t);
        GeoQuadIndices(rec, 0x2F8, out uint i0, out uint i1, out uint i2, out uint i3);
        int* sxy = GeoSxy;
        int s1 = sxy[i1];
        int s0 = sxy[i0];
        int s2 = sxy[i2];
        int z = GeoDepthSum4(i0, i1, i2, i3);
        if (z >= GeoQuadDepthLimit || !GeoSliceFacing(rec[13], s0, s1, s2))
        {
            return dst;
        }

        uint color = *(uint*)(rec + 8);
        *(int*)(dst + 8) = s0;
        *(int*)(dst + 12) = s1;
        *(uint*)(dst + 4) = color;
        *(int*)(dst + 16) = s2;
        dst[7] = 40;
        *(int*)(dst + 20) = sxy[i3];
        GeoAddPrim(dst, 5, t->Ot, GeoOtzQuad(z));
        return dst + 24;
    }

    /// <summary>F3 record (12 bytes): +0/+4 vertex indices, +8 color, +11 flags.</summary>
    private static byte* GeoEmitF3(byte* rec, byte* dst, GeoSliceTables* t)
    {
        Recompiler.Runtime.Tracer.Enter("GeoEmitF3(rec, dst, t)", (nint)rec, (nint)dst, (nint)t);
        GeoTriIndices(rec, 0x338, out uint i0, out uint i1, out uint i2);
        int* sxy = GeoSxy;
        int s1 = sxy[i1];
        int s2 = sxy[i2];
        int z = GeoDepthSum3(i0, i1, i2);
        int s0 = sxy[i0];
        if (z >= GeoTriDepthLimit || !GeoSliceFacing(rec[11], s0, s1, s2))
        {
            return dst;
        }

        uint color = *(uint*)(rec + 8);
        *(int*)(dst + 8) = s0;
        *(uint*)(dst + 4) = color;
        *(int*)(dst + 12) = s1;
        dst[7] = 32;
        *(int*)(dst + 16) = s2;
        GeoAddPrim(dst, 4, t->Ot, GeoOtzTri(z));
        return dst + 20;
    }

    /// <summary>Projected screen XY per vertex (x low int16, y high int16), 0x1802191A0.</summary>
    private static int* GeoSxy => (int*)Unsafe.AsPointer(ref dword_1802191A0);

    /// <summary>Depth per vertex, 0x180212440.</summary>
    private static int* GeoSz => (int*)Unsafe.AsPointer(ref dword_180212440);

    private static int GeoDepthSum4(uint i0, uint i1, uint i2, uint i3)
    {
        Recompiler.Runtime.Tracer.Enter("GeoDepthSum4(i0, i1, i2, i3)", i0, i1, i2, i3);
        int* sz = GeoSz;
        return sz[i0] + sz[i1] + sz[i2] + sz[i3];
    }

    private static int GeoDepthSum3(uint i0, uint i1, uint i2)
    {
        Recompiler.Runtime.Tracer.Enter("GeoDepthSum3(i0, i1, i2)", i0, i1, i2);
        int* sz = GeoSz;
        return sz[i0] + sz[i1] + sz[i2];
    }

    private static ushort GeoUv(GeoSliceTables* t, uint index)
    {
        Recompiler.Runtime.Tracer.Enter("GeoUv(t, index)", (nint)t, index);
        return *(ushort*)(t->Uv + 2 * (ulong)index);
    }

    private static uint GeoColor(GeoSliceTables* t, uint index)
    {
        Recompiler.Runtime.Tracer.Enter("GeoColor(t, index)", (nint)t, index);
        return *(uint*)(t->Colors + 4 * (ulong)index);
    }

    private static uint GeoTex(GeoSliceTables* t, byte index)
    {
        Recompiler.Runtime.Tracer.Enter("GeoTex(t, index)", (nint)t, index);
        return *(uint*)(t->Tex + 4 * index);
    }

    /// <summary>Semi-transparent faces (flag bit 1) get code <paramref name="code"/> and the ABR bits (flags &amp; 0xE0) in the tpage word.</summary>
    private static void GeoSemiTrans(byte* dst, byte flags, byte code, int tpageOffset)
    {
        Recompiler.Runtime.Tracer.Enter("GeoSemiTrans(dst, flags, code, tpageOffset)", (nint)dst, flags, code, tpageOffset);
        if ((flags & 1) != 0)
        {
            dst[7] = code;
            *(ushort*)(dst + tpageOffset) |= (ushort)(flags & 0xE0);
        }
    }

    /// <summary>
    /// Double-sided faces (flag bit 2) always pass; otherwise SXY0..2 are loaded into the GTE globals 0x180211FF0..F8
    /// and MAC0 (0x180212020) = NCLIP must be positive.
    /// </summary>
    private static bool GeoSliceFacing(byte flags, int s0, int s1, int s2)
    {
        Recompiler.Runtime.Tracer.Enter("GeoSliceFacing(flags, s0, s1, s2)", flags, s0, s1, s2);
        if ((flags & 2) != 0)
        {
            return true;
        }

        dword_180211FF0 = s0;
        dword_180211FF4 = s1;
        dword_180211FF8 = s2;
        int x0 = (short)s0, y0 = s0 >> 16;
        int x1 = (short)s1, y1 = s1 >> 16;
        int x2 = (short)s2, y2 = s2 >> 16;
        dword_180212020 = x2 * y0 + y1 * x0 + x1 * y2 - x0 * y2 - y0 * x1 - x2 * y1;
        return dword_180212020 > 0;
    }

    private static int GeoOtzQuad(int depthSum)
    {
        Recompiler.Runtime.Tracer.Enter("GeoOtzQuad(depthSum)", depthSum);
        int otz = (depthSum >> 6) & 0x3FFF;
        return otz >= 4096 ? 4095 : otz;
    }

    private static int GeoOtzTri(int depthSum)
    {
        Recompiler.Runtime.Tracer.Enter("GeoOtzTri(depthSum)", depthSum);
        int otz = ((depthSum & 0xFFFFF) / 3) >> 4;
        if (otz < 0)
        {
            return 0;
        }

        return otz >= 4096 ? 4095 : otz;
    }

    /// <summary>addPrim: length byte, then the packet takes the OT entry's link and the entry the packet's PSX address.</summary>
    private static void GeoAddPrim(byte* prim, byte length, byte* ot, int otz)
    {
        Recompiler.Runtime.Tracer.Enter("GeoAddPrim(prim, length, ot, otz)", (nint)prim, length, (nint)ot, otz);
        prim[3] = length;
        uint* entry = (uint*)(ot + 4 * otz);
        *(uint*)prim ^= (*(uint*)prim ^ *entry) & 0xFFFFFF;
        *entry ^= (*entry ^ (uint)PsxPtrToAddr((long)unk_180576A10, (ulong)prim)) & 0xFFFFFF;
    }

    /// <summary>Quad vertex indices (dword 0: i0 | i1 &lt;&lt; 16, dword 1: i2 | i3 &lt;&lt; 16) with the four geoslice.cpp asserts.</summary>
    private static void GeoQuadIndices(byte* rec, int line, out uint i0, out uint i1, out uint i2, out uint i3)
    {
        Recompiler.Runtime.Tracer.Enter("GeoQuadIndices(rec, line)", (nint)rec, line);
        uint d0 = *(uint*)rec;
        uint d1 = *(uint*)(rec + 4);
        i0 = d0 & 0xFFFF;
        i1 = d0 >> 16;
        i2 = d1 & 0xFFFF;
        i3 = d1 >> 16;
        GeoCheckIndex(i0, "(u16)index0 < GEO_MAX_VERTICES", line);
        GeoCheckIndex(i1, "index1 < GEO_MAX_VERTICES", line + 1);
        GeoCheckIndex(i2, "(u16)index2 < GEO_MAX_VERTICES", line + 2);
        GeoCheckIndex(i3, "index3 < GEO_MAX_VERTICES", line + 3);
    }

    /// <summary>Triangle vertex indices (dword 0: i0 | i1 &lt;&lt; 16, ushort +4: i2) with the three geoslice.cpp asserts.</summary>
    private static void GeoTriIndices(byte* rec, int line, out uint i0, out uint i1, out uint i2)
    {
        Recompiler.Runtime.Tracer.Enter("GeoTriIndices(rec, line)", (nint)rec, line);
        uint d0 = *(uint*)rec;
        i2 = *(ushort*)(rec + 4);
        i0 = d0 & 0xFFFF;
        i1 = d0 >> 16;
        GeoCheckIndex(i0, "(u16)index0 < GEO_MAX_VERTICES", line);
        GeoCheckIndex(i1, "index1 < GEO_MAX_VERTICES", line + 1);
        GeoCheckIndex(i2, "(u16)index2 < GEO_MAX_VERTICES", line + 2);
    }

    /// <summary><c>_wassert(expression, L"..\\..\\SpecialEffectCode\\sonoda\\Geo\\geoslice.cpp", line)</c> (see DIV-004).</summary>
    private static void GeoCheckIndex(uint index, string expression, int line)
    {
        Recompiler.Runtime.Tracer.Enter("GeoCheckIndex(index, expression, line)", index, expression, line);
        if (index >= GeoSliceMaxVertices)
        {
            System.Environment.FailFast(
                "Assertion failed: " + expression + ", file ..\\..\\SpecialEffectCode\\sonoda\\Geo\\geoslice.cpp, line "
                + line.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
