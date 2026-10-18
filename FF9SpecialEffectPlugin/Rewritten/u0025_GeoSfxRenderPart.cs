using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    private const string GsrFile = @"..\..\SpecialEffectCode\sonoda\Geo\geosfxrender.cpp";

    /// <summary>Per-call state of <see cref="GeoSfxRenderPart"/>: vertex tables, part tables, OT base and depth bias.</summary>
    private struct GsrContext
    {
        public int* Sxy;
        public int* Sz;
        public byte* Uv;
        public byte* Colors;
        public long Pages;
        public byte* Ot;
        public short Bias;
    }

    /// <summary>
    /// geosfxrender.cpp part renderer. Loads the GTE matrix the PSX address in slot 5 of off_180066C68 points to into
    /// 0x180211F40..0x180211F5F; unless bit 0x10 of the handle's first byte is set, emits the eight primitive kinds of
    /// part <paramref name="a2"/> of the PsgModelHandle <paramref name="a1"/> into <paramref name="a3"/> (FT4, FT3,
    /// GT4, GT3, TPAGE+G4, TPAGE+G3, F4, F3), dropping back faces unless the double-sided bit is set, and links each
    /// into the OT (slot 8, offset 8 - OtShift) by average depth + <paramref name="a4"/>. Writes the 0xFF terminator
    /// code byte and returns the pointer after the terminator's header.
    /// </summary>
    public static uint* GeoSfxRenderPart(long a1, byte a2, uint* a3, short a4)
    {
        Recompiler.Runtime.Tracer.Enter("GeoSfxRenderPart(a1, a2, a3, a4)", a1, a2, (nint)a3, a4);
        Shape_PsgBuildBoneMatrices_a1* shape_PsgBuildBoneMatrices_a1 = (Shape_PsgBuildBoneMatrices_a1*)a1;
        long mapper = (long)unk_180576A10;
        uint* table = (uint*)off_180066C68;
        byte* matrix = (byte*)PsxAddrToPtr(mapper, table[5]);
        byte* gte = xmmword_180211F40;
        *(UInt128*)gte = *(UInt128*)matrix;
        *(int*)(gte + 16) = ((Matrix3*)matrix)->field_16;
        matrix = (byte*)PsxAddrToPtr(mapper, table[5]);
        *(uint*)(gte + 20) = ((Matrix3*)matrix)->field_20;
        *(ulong*)(gte + 24) = ((Matrix3*)matrix)->field_24;

        if ((shape_PsgBuildBoneMatrices_a1->field_0 & 0x10) != 0)
        {
            return a3;
        }

        var handle = (PsgModelHandle*)a1;
        var model = (PsgModel*)PsxAddrToPtr(mapper, handle->ModelAddr);
        var part = (PsgMeshPart*)((byte*)PsxAddrToPtr(mapper, model->PartsAddr) + 40 * a2);
        GsrContext c;
        c.Uv = (byte*)PsxAddrToPtr(mapper, part->UvTableAddr);
        byte* r = (byte*)PsxAddrToPtr(mapper, part->PrimitivesAddr);
        c.Colors = (byte*)PsxAddrToPtr(mapper, part->Addr36);
        if (handle->field_24 == 0)
        {
            handle->field_24 = (ulong)a3;
        }

        c.Pages = (long)handle->Arg40;
        c.Ot = (byte*)PsxAddrToPtr(mapper, table[8]) + 4 * (8L - part->OtShift);
        c.Sxy = (int*)Unsafe.AsPointer(ref dword_1802191A0);
        c.Sz = (int*)Unsafe.AsPointer(ref dword_180212440);
        c.Bias = a4;

        for (int n = part->Quad24Count; n != 0; n--, r += 24)
        {
            a3 = GsrFt4(&c, r, a3);
        }

        for (int n = part->Tri20Count; n != 0; n--, r += 20)
        {
            a3 = GsrFt3(&c, r, a3);
        }

        for (int n = part->Quad32Count; n != 0; n--, r += 32)
        {
            a3 = GsrGt4(&c, r, a3);
        }

        for (int n = part->Tri24Count; n != 0; n--, r += 24)
        {
            a3 = GsrGt3(&c, r, a3);
        }

        for (int n = part->ExtraQuadCount1; n != 0; n--, r += 24)
        {
            a3 = GsrModeG4(&c, r, a3);
        }

        for (int n = part->ExtraTriCount1; n != 0; n--, r += 20)
        {
            a3 = GsrModeG3(&c, r, a3);
        }

        for (int n = part->ExtraQuadCount2; n != 0; n--, r += 16)
        {
            a3 = GsrF4(&c, r, a3);
        }

        for (int n = part->ExtraTriCount2; n != 0; n--, r += 12)
        {
            a3 = GsrF3(&c, r, a3);
        }

        ((byte*)a3)[7] = 0xFF;
        return a3 + 2;
    }

    /// <summary>
    /// Back-face test: true for double-sided primitives (flag bit 1); otherwise stores the three screen XYs in
    /// dword_180211FF0..FF8 and the signed area in dword_180212020 and returns area &gt; 0.
    /// </summary>
    private static bool GsrVisible(byte flags, int s0, int s1, int s2)
    {
        Recompiler.Runtime.Tracer.Enter("GsrVisible(flags, s0, s1, s2)", flags, s0, s1, s2);
        if ((flags & 2) != 0)
        {
            return true;
        }

        dword_180211FF0 = s0;
        dword_180211FF4 = s1;
        dword_180211FF8 = s2;
        int x0 = (short)s0, y0 = s0 >> 16, x1 = (short)s1, y1 = s1 >> 16, x2 = (short)s2, y2 = s2 >> 16;
        dword_180212020 = unchecked(x2 * y0 + y1 * x0 + x1 * y2 - x0 * y2 - y0 * x1 - x2 * y1);
        return dword_180212020 > 0;
    }

    private static void GsrCheck(int index, string expression, uint line)
    {
        Recompiler.Runtime.Tracer.Enter("GsrCheck(index, expression, line)", index, expression, line);
        if (index >= 0x1B58)
        {
            GeoWassert(expression, GsrFile, line);
        }
    }

    /// <summary>
    /// Average depth of a triangle as the original computes it: the three depths go to dword_180212000..008, their low
    /// halves are summed, (sum &lt;&lt; 12) / 3 lands in dword_180212020 and its &gt;&gt; 14 (clamped to u16) replaces the low
    /// half of dword_180211FDC, whose whole value is returned (its high half is whatever was there).
    /// </summary>
    private static int GsrAverageDepth3(int* sz, int i0, int i1, int i2)
    {
        Recompiler.Runtime.Tracer.Enter("GsrAverageDepth3(sz, i0, i1, i2)", (nint)sz, i0, i1, i2);
        dword_180212000 = sz[i0];
        dword_180212004 = sz[i1];
        dword_180212008 = sz[i2];
        int q = (((ushort)dword_180212008 + (ushort)dword_180212004 + (ushort)dword_180212000) << 12) / 3;
        long v = (long)q >> 14;
        ushort low = (q >> 14) <= 0xFFFF ? ((int)v < 0 ? (ushort)0 : (ushort)v) : (ushort)0xFFFF;
        dword_180212020 = q;
        *(ushort*)Unsafe.AsPointer(ref dword_180211FDC) = low;
        return dword_180211FDC;
    }

    /// <summary>OT slot: bias + depth clamped to 0..4095.</summary>
    private static int GsrOtSlot(short bias, int depth)
    {
        Recompiler.Runtime.Tracer.Enter("GsrOtSlot(bias, depth)", bias, depth);
        int slot = bias + depth;
        if (slot >= 0)
        {
            return slot >= 4096 ? 4095 : slot;
        }

        return 0;
    }

    /// <summary>Sets the length byte of <paramref name="packet"/> and links it at the head of OT slot <paramref name="slot"/>.</summary>
    private static void GsrLink(GsrContext* c, uint* packet, byte length, int slot)
    {
        Recompiler.Runtime.Tracer.Enter("GsrLink(c, packet, length, slot)", (nint)c, (nint)packet, length, slot);
        ((byte*)packet)[3] = length;
        uint* entry = (uint*)(c->Ot + 4 * slot);
        *packet ^= (*entry ^ *packet) & 0xFFFFFF;
        *entry ^= (*entry ^ (uint)PsxPtrToAddr((long)unk_180576A10, (ulong)packet)) & 0xFFFFFF;
    }

    private static ushort GsrUv(GsrContext* c, uint index)
    {
        Recompiler.Runtime.Tracer.Enter("GsrUv(c, index)", (nint)c, index);
        return *(ushort*)(c->Uv + 2 * index);
    }

    private static uint GsrColor(GsrContext* c, uint index)
    {
        Recompiler.Runtime.Tracer.Enter("GsrColor(c, index)", (nint)c, index);
        return *(uint*)(c->Colors + 4 * index);
    }

    private static uint GsrPage(GsrContext* c, byte texture)
    {
        Recompiler.Runtime.Tracer.Enter("GsrPage(c, texture)", (nint)c, texture);
        return *(uint*)(c->Pages + 4L * texture);
    }

    /// <summary>Flag bit 0: semi-transparency mode bits (flags &amp; 0xE0) into the tpage half-word and the ABE code bit.</summary>
    private static void GsrSemiTransparent(uint* p, byte flags)
    {
        Recompiler.Runtime.Tracer.Enter("GsrSemiTransparent(p, flags)", (nint)p, flags);
        if ((flags & 1) != 0)
        {
            ((ushort*)p)[11] |= (ushort)(flags & 0xE0);
            ((byte*)p)[7] |= 2;
        }
    }

    /// <summary>Quad24 record: flat textured quad (POLY_FT4, code 0x2C), 9 words.</summary>
    private static uint* GsrFt4(GsrContext* c, byte* r, uint* p)
    {
        Recompiler.Runtime.Tracer.Enter("GsrFt4(c, r, p)", (nint)c, (nint)r, (nint)p);
        uint w0 = *(uint*)r, w1 = *(uint*)(r + 4);
        int i0 = (ushort)w0, i1 = (int)(w0 >> 16), i2 = (ushort)w1;
        int s0 = c->Sxy[i0], s1 = c->Sxy[i1], s2 = c->Sxy[i2];
        if (!GsrVisible(r[21], s0, s1, s2))
        {
            return p;
        }

        p[1] = *(uint*)(r + 16);
        int i3 = (int)(w1 >> 16);
        ((byte*)p)[7] = 44;
        GsrCheck(i0, "(u16)index0 < GEO_MAX_VERTICES", 0x6F);
        GsrCheck(i1, "index1 < GEO_MAX_VERTICES", 0x70);
        GsrCheck(i2, "(u16)index2 < GEO_MAX_VERTICES", 0x71);
        GsrCheck(i3, "index3 < GEO_MAX_VERTICES", 0x72);
        p[2] = (uint)s0;
        p[4] = (uint)s1;
        p[6] = (uint)s2;
        p[8] = (uint)c->Sxy[i3];
        uint uv1 = *(uint*)(r + 12);
        int depth = c->Sz[i0] + c->Sz[i1] + c->Sz[i2] + c->Sz[i3];
        uint page = GsrPage(c, r[19]);
        uint uv0 = *(uint*)(r + 8);
        p[3] = (page & 0xFFFF0000) | GsrUv(c, (ushort)uv0);
        p[5] = (page << 16) | GsrUv(c, uv0 >> 16);
        ((ushort*)p)[14] = GsrUv(c, (ushort)uv1);
        ((ushort*)p)[18] = GsrUv(c, uv1 >> 16);
        GsrSemiTransparent(p, r[21]);
        GsrLink(c, p, 9, GsrOtSlot(c->Bias, depth >> 6));
        return p + 10;
    }

    /// <summary>Tri20 record: flat textured triangle (POLY_FT3, code 0x24), 7 words.</summary>
    private static uint* GsrFt3(GsrContext* c, byte* r, uint* p)
    {
        Recompiler.Runtime.Tracer.Enter("GsrFt3(c, r, p)", (nint)c, (nint)r, (nint)p);
        uint w0 = *(uint*)r;
        int i2 = *(ushort*)(r + 4), i1 = (int)(w0 >> 16);
        GsrCheck((ushort)w0, "(u16)index0 < GEO_MAX_VERTICES", 0xA9);
        GsrCheck(i1, "index1 < GEO_MAX_VERTICES", 0xAA);
        GsrCheck(i2, "index2 < GEO_MAX_VERTICES", 0xAB);
        int i0 = (ushort)w0;
        int s2 = c->Sxy[i2], s0 = c->Sxy[i0], s1 = c->Sxy[i1];
        if (!GsrVisible(r[18], s0, s1, s2))
        {
            return p;
        }

        uint color = *(uint*)(r + 8);
        p[2] = (uint)s0;
        p[1] = color;
        ((byte*)p)[7] = 36;
        int depth = GsrAverageDepth3(c->Sz, i0, i1, i2);
        p[4] = (uint)s1;
        p[6] = (uint)s2;
        uint page = GsrPage(c, r[6]);
        uint uv0 = *(uint*)(r + 12);
        p[3] = (page & 0xFFFF0000) | GsrUv(c, (ushort)uv0);
        p[5] = (page << 16) | GsrUv(c, uv0 >> 16);
        ((ushort*)p)[14] = GsrUv(c, *(ushort*)(r + 16));
        GsrSemiTransparent(p, r[18]);
        GsrLink(c, p, 7, GsrOtSlot(c->Bias, depth >> 2));
        return p + 8;
    }

    /// <summary>Quad32 record: gouraud textured quad (POLY_GT4, code 0x3C / 0x3E semi-transparent), 12 words.</summary>
    private static uint* GsrGt4(GsrContext* c, byte* r, uint* p)
    {
        Recompiler.Runtime.Tracer.Enter("GsrGt4(c, r, p)", (nint)c, (nint)r, (nint)p);
        uint w0 = *(uint*)r, w1 = *(uint*)(r + 4);
        int i1 = (int)(w0 >> 16), i3 = (int)(w1 >> 16);
        GsrCheck((ushort)w0, "(u16)index0 < GEO_MAX_VERTICES", 0x100);
        GsrCheck(i1, "index1 < GEO_MAX_VERTICES", 0x101);
        GsrCheck((ushort)w1, "(u16)index2 < GEO_MAX_VERTICES", 0x102);
        GsrCheck(i3, "index3 < GEO_MAX_VERTICES", 0x103);
        int i0 = (ushort)w0, i2 = (ushort)w1;
        int s1 = c->Sxy[i1], s0 = c->Sxy[i0], s2 = c->Sxy[i2];
        if (!GsrVisible(r[28], s0, s1, s2))
        {
            return p;
        }

        p[2] = (uint)s0;
        p[5] = (uint)s1;
        p[8] = (uint)s2;
        int depth = c->Sz[i0] + c->Sz[i1] + c->Sz[i2] + c->Sz[i3];
        p[11] = (uint)c->Sxy[i3];
        uint col1 = *(uint*)(r + 20);
        uint page = GsrPage(c, r[29]);
        uint col0 = *(uint*)(r + 16);
        p[1] = GsrColor(c, (ushort)col0);
        p[4] = GsrColor(c, col0 >> 16);
        p[7] = GsrColor(c, (ushort)col1);
        uint last = GsrColor(c, col1 >> 16);
        ((byte*)p)[7] = 60;
        p[10] = last;
        uint uv1 = *(uint*)(r + 12);
        uint uv0 = *(uint*)(r + 8);
        p[3] = (page & 0xFFFF0000) | GsrUv(c, (ushort)uv0);
        p[6] = (page << 16) | GsrUv(c, uv0 >> 16);
        ((ushort*)p)[18] = GsrUv(c, (ushort)uv1);
        ((ushort*)p)[24] = GsrUv(c, uv1 >> 16);
        byte flags = r[28];
        if ((flags & 1) != 0)
        {
            ((byte*)p)[7] = 62;
            ((ushort*)p)[13] |= (ushort)(flags & 0xE0);
        }

        GsrLink(c, p, 12, GsrOtSlot(c->Bias, depth >> 6));
        return p + 13;
    }

    /// <summary>Tri24 record: gouraud textured triangle (POLY_GT3, code 0x34 / 0x36 semi-transparent), 9 words.</summary>
    private static uint* GsrGt3(GsrContext* c, byte* r, uint* p)
    {
        Recompiler.Runtime.Tracer.Enter("GsrGt3(c, r, p)", (nint)c, (nint)r, (nint)p);
        uint w0 = *(uint*)r;
        int i2 = *(ushort*)(r + 4), i1 = (int)(w0 >> 16);
        GsrCheck((ushort)w0, "(u16)index0 < GEO_MAX_VERTICES", 0x15C);
        GsrCheck(i1, "index1 < GEO_MAX_VERTICES", 0x15D);
        GsrCheck(i2, "index2 < GEO_MAX_VERTICES", 0x15E);
        int i0 = (ushort)w0;
        int s2 = c->Sxy[i2], s0 = c->Sxy[i0], s1 = c->Sxy[i1];
        if (!GsrVisible(r[15], s0, s1, s2))
        {
            return p;
        }

        p[2] = (uint)s0;
        int depth = GsrAverageDepth3(c->Sz, i0, i1, i2);
        p[5] = (uint)s1;
        p[8] = (uint)s2;
        uint page = GsrPage(c, r[22]);
        uint col0 = *(uint*)(r + 16);
        p[1] = GsrColor(c, (ushort)col0);
        p[4] = GsrColor(c, col0 >> 16);
        uint col2 = GsrColor(c, *(ushort*)(r + 20));
        ((byte*)p)[7] = 52;
        p[7] = col2;
        p[3] = (page & 0xFFFF0000) | GsrUv(c, *(ushort*)(r + 6));
        uint uv12 = *(uint*)(r + 8);
        p[6] = (page << 16) | GsrUv(c, (ushort)uv12);
        ((ushort*)p)[18] = GsrUv(c, uv12 >> 16);
        byte flags = r[15];
        if ((flags & 1) != 0)
        {
            ((byte*)p)[7] = 54;
            ((ushort*)p)[13] |= (ushort)(flags & 0xE0);
        }

        GsrLink(c, p, 9, GsrOtSlot(c->Bias, depth >> 2));
        return p + 10;
    }

    /// <summary>
    /// Extra quad record (24 bytes): word 0 gets length 10 and word 1 an E1 draw-mode command with the semi-transparency
    /// bits, then a gouraud quad (POLY_G4, code 0x38 / 0x3A) at word 2, which is what gets linked (length 8).
    /// </summary>
    private static uint* GsrModeG4(GsrContext* c, byte* r, uint* p)
    {
        Recompiler.Runtime.Tracer.Enter("GsrModeG4(c, r, p)", (nint)c, (nint)r, (nint)p);
        uint w0 = *(uint*)r, w1 = *(uint*)(r + 4);
        int i0 = (ushort)w0, i1 = (int)(w0 >> 16), i2 = (ushort)w1;
        int s0 = c->Sxy[i0], s1 = c->Sxy[i1], s2 = c->Sxy[i2];
        if (!GsrVisible(r[20], s0, s1, s2))
        {
            return p;
        }

        ((byte*)p)[3] = 10;
        uint* g = p + 2;
        p[1] = (uint)(r[20] & 0xE0) | 0xE1000000;
        p[4] = (uint)s0;
        int i3 = (int)(w1 >> 16);
        g[6] = (uint)s2;
        g[4] = (uint)s1;
        g[8] = (uint)c->Sxy[i3];
        GsrCheck(i0, "(u16)index0 < GEO_MAX_VERTICES", 0x1D7);
        GsrCheck(i1, "index1 < GEO_MAX_VERTICES", 0x1D8);
        GsrCheck(i2, "(u16)index2 < GEO_MAX_VERTICES", 0x1D9);
        GsrCheck(i3, "index3 < GEO_MAX_VERTICES", 0x1DA);
        uint col1 = *(uint*)(r + 12);
        int depth = c->Sz[i0] + c->Sz[i1] + c->Sz[i2] + c->Sz[i3];
        uint col0 = *(uint*)(r + 8);
        g[1] = GsrColor(c, (ushort)col0);
        g[3] = GsrColor(c, col0 >> 16);
        g[5] = GsrColor(c, (ushort)col1);
        g[7] = GsrColor(c, col1 >> 16);
        ((byte*)g)[7] = (r[20] & 1) != 0 ? (byte)58 : (byte)56;
        GsrLink(c, g, 8, GsrOtSlot(c->Bias, depth >> 6));
        return g + 9;
    }

    /// <summary>
    /// Extra triangle record (20 bytes): word 0 length 8, word 1 E1 draw mode, gouraud triangle (POLY_G3, code 0x30 /
    /// 0x32) at word 2, linked with length 6 only when dword_1803678E8 != 184 (the packet is written either way).
    /// </summary>
    private static uint* GsrModeG3(GsrContext* c, byte* r, uint* p)
    {
        Recompiler.Runtime.Tracer.Enter("GsrModeG3(c, r, p)", (nint)c, (nint)r, (nint)p);
        uint w0 = *(uint*)r;
        int i2 = *(ushort*)(r + 4), i1 = (int)(w0 >> 16);
        GsrCheck((ushort)w0, "(u16)index0 < GEO_MAX_VERTICES", 0x20C);
        GsrCheck(i1, "index1 < GEO_MAX_VERTICES", 0x20D);
        GsrCheck(i2, "index2 < GEO_MAX_VERTICES", 0x20E);
        int i0 = (ushort)w0;
        int s2 = c->Sxy[i2], s0 = c->Sxy[i0], s1 = c->Sxy[i1];
        if (!GsrVisible(r[19], s0, s1, s2))
        {
            return p;
        }

        ((byte*)p)[3] = 8;
        uint* g = p + 2;
        p[1] = (uint)(r[19] & 0xE0) | 0xE1000000;
        p[4] = (uint)s0;
        p[6] = (uint)s1;
        p[8] = (uint)s2;
        int depth = GsrAverageDepth3(c->Sz, i0, i1, i2);
        uint col01 = *(uint*)(r + 8);
        p[3] = GsrColor(c, (ushort)col01);
        p[5] = GsrColor(c, col01 >> 16);
        uint col2 = GsrColor(c, *(ushort*)(r + 12));
        ((byte*)p)[15] = 48;
        p[7] = col2;
        ((byte*)p)[15] = (r[19] & 1) != 0 ? (byte)50 : (byte)48;
        int slot = GsrOtSlot(c->Bias, depth >> 2);
        if (CurrentEffectId != (int)SpecialEffectId.Atomos__Full)
        {
            GsrLink(c, g, 6, slot);
        }

        return g + 7;
    }

    /// <summary>Extra quad record (16 bytes): flat quad (POLY_F4, code 0x28) with the colour word at +8, 5 words.</summary>
    private static uint* GsrF4(GsrContext* c, byte* r, uint* p)
    {
        Recompiler.Runtime.Tracer.Enter("GsrF4(c, r, p)", (nint)c, (nint)r, (nint)p);
        uint w0 = *(uint*)r, w1 = *(uint*)(r + 4);
        int i0 = (ushort)w0, i1 = (int)(w0 >> 16), i2 = (ushort)w1;
        int s0 = c->Sxy[i0], s1 = c->Sxy[i1], s2 = c->Sxy[i2];
        if (!GsrVisible(r[13], s0, s1, s2))
        {
            return p;
        }

        p[1] = *(uint*)(r + 8);
        int i3 = (int)(w1 >> 16);
        ((byte*)p)[7] = 40;
        GsrCheck(i0, "(u16)index0 < GEO_MAX_VERTICES", 0x278);
        GsrCheck(i1, "index1 < GEO_MAX_VERTICES", 0x279);
        GsrCheck(i2, "(u16)index2 < GEO_MAX_VERTICES", 0x27A);
        GsrCheck(i3, "index3 < GEO_MAX_VERTICES", 0x27B);
        p[3] = (uint)s1;
        p[4] = (uint)s2;
        p[2] = (uint)s0;
        p[5] = (uint)c->Sxy[i3];
        int depth = c->Sz[i0] + c->Sz[i1] + c->Sz[i2] + c->Sz[i3];
        GsrLink(c, p, 5, GsrOtSlot(c->Bias, depth >> 6));
        return p + 6;
    }

    /// <summary>Extra triangle record (12 bytes): flat triangle (POLY_F3, code 0x20), colour word at +8, 4 words.</summary>
    private static uint* GsrF3(GsrContext* c, byte* r, uint* p)
    {
        Recompiler.Runtime.Tracer.Enter("GsrF3(c, r, p)", (nint)c, (nint)r, (nint)p);
        uint w0 = *(uint*)r;
        int i2 = *(ushort*)(r + 4), i1 = (int)(w0 >> 16);
        GsrCheck((ushort)w0, "(u16)index0 < GEO_MAX_VERTICES", 0x29C);
        GsrCheck(i1, "index1 < GEO_MAX_VERTICES", 0x29D);
        GsrCheck(i2, "index2 < GEO_MAX_VERTICES", 0x29E);
        int i0 = (ushort)w0;
        int s0 = c->Sxy[i0], s1 = c->Sxy[i1], s2 = c->Sxy[i2];
        if (!GsrVisible(r[11], s0, s1, s2))
        {
            return p;
        }

        uint color = *(uint*)(r + 8);
        p[2] = (uint)s0;
        p[3] = (uint)s1;
        p[1] = color;
        ((byte*)p)[7] = 32;
        p[4] = (uint)s2;
        int depth = GsrAverageDepth3(c->Sz, i0, i1, i2);
        GsrLink(c, p, 4, GsrOtSlot(c->Bias, depth >> 2));
        return p + 5;
    }
}
