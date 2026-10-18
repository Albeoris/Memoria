using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>GEO_MAX_VERTICES of geo.cpp: bound of the screen-XY and depth vertex tables.</summary>
    private const int GeoMaxVertices = 0x1B58;

    /// <summary>Last index of the 4096-entry depth range a primitive is sorted into.</summary>
    private const int GeoOtLastIndex = 4095;

    /// <summary>
    /// geo.cpp part renderer. Loads the rotation matrix and translation the 32-bit PSX address in slot 5 of the table at
    /// off_180066C68 points to into 0x180211F40..0x180211F5F, then (unless bit 0x10 of the handle's first byte is set)
    /// emits the primitives of part <paramref name="a2"/> of the PsgModelHandle <paramref name="a1"/> into
    /// <paramref name="a3"/>: eight kinds (textured quads/triangles, gouraud textured, gouraud, flat) built from the
    /// projected vertex XY table at 0x1802191A0 and the depth table at 0x180212440, back faces dropped unless the
    /// primitive's double-sided bit is set, each linked into the ordering table (slot 8, offset by 8 - OtShift) by
    /// average depth. Ends the list with a 0xFF code byte and returns the address after that terminator word.
    /// </summary>
    public static long PsgDrawMeshPart(long a1, byte a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("PsgDrawMeshPart(a1, a2, a3)", a1, a2, a3);
        Shape_PsgBuildBoneMatrices_a1* shape_PsgBuildBoneMatrices_a1 = (Shape_PsgBuildBoneMatrices_a1*)a1;
        uint* table = (uint*)off_180066C68;
        byte* matrix = ResolveGlobalPsxAddress(table[5]);
        Matrix4* matrix4 = (Matrix4*)matrix;
        byte* gte = xmmword_180211F40;
        *(UInt128*)gte = *(UInt128*)matrix;
        dword_180211F50 = matrix4->field_16;
        byte* translation = ResolveGlobalPsxAddress(table[5]);
        Matrix4* matrix42 = (Matrix4*)translation;
        *(uint*)(gte + 0x14) = matrix42->field_20;
        *(ulong*)(gte + 0x18) = matrix42->field_24;

        var handle = (PsgModelHandle*)a1;
        if ((shape_PsgBuildBoneMatrices_a1->field_0 & 0x10) != 0)
        {
            return a3;
        }

        var model = (PsgModel*)ResolveGlobalPsxAddress(handle->ModelAddr);
        var part = (PsgMeshPart*)(ResolveGlobalPsxAddress(model->PartsAddr) + 40 * a2);
        var uvs = (ushort*)ResolveGlobalPsxAddress(part->UvTableAddr);
        byte* r = ResolveGlobalPsxAddress(part->PrimitivesAddr);
        var colors = (uint*)ResolveGlobalPsxAddress(part->Addr36);
        if (handle->field_24 == 0)
        {
            handle->field_24 = (ulong)a3;
        }

        var pages = (uint*)handle->Arg40;
        uint* ot = (uint*)ResolveGlobalPsxAddress(table[8]) + (8L - part->OtShift);
        int* xy = (int*)Unsafe.AsPointer(ref dword_1802191A0);
        int* z = (int*)Unsafe.AsPointer(ref dword_180212440);

        for (int n = part->Quad24Count; n != 0; n--, r += 24)
        {
            a3 = PsgTexturedQuad(r, a3, xy, z, uvs, pages, ot);
        }

        for (int n = part->Tri20Count; n != 0; n--, r += 20)
        {
            a3 = PsgTexturedTri(r, a3, xy, z, uvs, pages, ot);
        }

        for (int n = part->Quad32Count; n != 0; n--, r += 32)
        {
            a3 = PsgGouraudTexturedQuad(r, a3, xy, z, uvs, colors, pages, ot);
        }

        for (int n = part->Tri24Count; n != 0; n--, r += 24)
        {
            a3 = PsgGouraudTexturedTri(r, a3, xy, z, uvs, colors, pages, ot);
        }

        for (int n = part->ExtraQuadCount1; n != 0; n--, r += 24)
        {
            a3 = PsgGouraudQuad(r, a3, xy, z, colors, ot);
        }

        for (int n = part->ExtraTriCount1; n != 0; n--, r += 20)
        {
            a3 = PsgGouraudTri(r, a3, xy, z, colors, ot);
        }

        for (int n = part->ExtraQuadCount2; n != 0; n--, r += 16)
        {
            a3 = PsgFlatQuad(r, a3, xy, z, ot);
        }

        for (int n = part->ExtraTriCount2; n != 0; n--, r += 12)
        {
            a3 = PsgFlatTri(r, a3, xy, z, ot);
        }

        *(byte*)(a3 + 7) = 0xFF;
        return a3 + 8;
    }

    /// <summary>Textured quad record (24 bytes, +21 flags) to a 40-byte code 0x2C packet of 9 words.</summary>
    private static long PsgTexturedQuad(byte* r, long a3, int* xy, int* z, ushort* uvs, uint* pages, uint* ot)
    {
        Recompiler.Runtime.Tracer.Enter("PsgTexturedQuad(r, a3, xy, z, uvs, pages, ot)", (nint)r, a3, (nint)xy, (nint)z, (nint)uvs, (nint)pages, (nint)ot);
        uint w0 = *(uint*)r;
        uint w1 = *(uint*)(r + 4);
        uint i0 = (ushort)w0, i1 = w0 >> 16, i2 = (ushort)w1, i3 = w1 >> 16;
        PsgCheckVertex(i0, 0, 0xE4);
        PsgCheckVertex(i1, 1, 0xE5);
        PsgCheckVertex(i2, 2, 0xE6);
        int xy0 = xy[i0], xy1 = xy[i1], xy2 = xy[i2];
        if ((r[21] & 2) == 0 && !PsgFrontFacing(xy0, xy1, xy2))
        {
            return a3;
        }

        *(uint*)(a3 + 8) = (uint)xy0;
        *(uint*)(a3 + 24) = (uint)xy2;
        *(uint*)(a3 + 4) = *(uint*)(r + 16);
        *(uint*)(a3 + 16) = (uint)xy1;
        *(byte*)(a3 + 7) = 0x2C;
        *(uint*)(a3 + 32) = (uint)xy[i3];
        int depth = z[i0] + z[i1] + z[i2] + z[i3];
        uint page = pages[r[19]];
        uint w2 = *(uint*)(r + 8);
        uint w3 = *(uint*)(r + 12);
        uint t0 = (ushort)w2, t1 = w2 >> 16, t2 = (ushort)w3, t3 = w3 >> 16;
        PsgCheckVertex(t0, 0, 0x116);
        PsgCheckVertex(t1, 1, 0x117);
        PsgCheckVertex(t2, 2, 0x118);
        PsgCheckVertex(t3, 3, 0x119);
        *(uint*)(a3 + 12) = (page & 0xFFFF0000) | uvs[t0];
        *(uint*)(a3 + 20) = (page << 16) | uvs[t1];
        *(ushort*)(a3 + 28) = uvs[t2];
        *(ushort*)(a3 + 36) = uvs[t3];
        PsgApplySemiTransparency(r[21], a3, 22, false);
        *(byte*)(a3 + 3) = 9;
        PsgLinkPrimitive(ot, depth >> 6, a3);
        return a3 + 40;
    }

    /// <summary>Textured triangle record (20 bytes, +18 flags) to a 32-byte code 0x24 packet of 7 words.</summary>
    private static long PsgTexturedTri(byte* r, long a3, int* xy, int* z, ushort* uvs, uint* pages, uint* ot)
    {
        Recompiler.Runtime.Tracer.Enter("PsgTexturedTri(r, a3, xy, z, uvs, pages, ot)", (nint)r, a3, (nint)xy, (nint)z, (nint)uvs, (nint)pages, (nint)ot);
        uint w0 = *(uint*)r;
        uint i0 = (ushort)w0, i1 = w0 >> 16, i2 = *(ushort*)(r + 4);
        PsgCheckVertex(i0, 0, 0x142);
        PsgCheckVertex(i1, 1, 0x143);
        PsgCheckVertex(i2, 2, 0x144);
        int xy0 = xy[i0], xy1 = xy[i1], xy2 = xy[i2];
        if ((r[18] & 2) == 0 && !PsgFrontFacing(xy0, xy1, xy2))
        {
            return a3;
        }

        *(uint*)(a3 + 8) = (uint)xy0;
        *(uint*)(a3 + 16) = (uint)xy1;
        *(uint*)(a3 + 4) = *(uint*)(r + 8);
        *(byte*)(a3 + 7) = 0x24;
        *(uint*)(a3 + 24) = (uint)xy2;
        PsgAverageDepth3(z[i0], z[i1], z[i2]);
        uint page = pages[r[6]];
        uint w3 = *(uint*)(r + 12);
        uint t0 = (ushort)w3, t1 = w3 >> 16, t2 = *(ushort*)(r + 16);
        PsgCheckVertex(t0, 0, 0x174);
        PsgCheckVertex(t1, 1, 0x175);
        PsgCheckVertex(t2, 2, 0x176);
        *(uint*)(a3 + 12) = (page & 0xFFFF0000) | uvs[t0];
        *(uint*)(a3 + 20) = (page << 16) | uvs[t1];
        *(ushort*)(a3 + 28) = uvs[t2];
        int depth = dword_180211FDC;
        PsgApplySemiTransparency(r[18], a3, 22, false);
        *(byte*)(a3 + 3) = 7;
        PsgLinkPrimitive(ot, depth >> 2, a3);
        return a3 + 32;
    }

    /// <summary>Gouraud textured quad record (32 bytes, +28 flags) to a 52-byte code 0x3C packet of 12 words.</summary>
    private static long PsgGouraudTexturedQuad(
        byte* r, long a3, int* xy, int* z, ushort* uvs, uint* colors, uint* pages, uint* ot)
    {
        Recompiler.Runtime.Tracer.Enter("PsgGouraudTexturedQuad(r, a3, xy, z, uvs, colors, pages, ot)", (nint)r, a3, (nint)xy, (nint)z, (nint)uvs, (nint)colors, (nint)pages, (nint)ot);
        uint w0 = *(uint*)r;
        uint w1 = *(uint*)(r + 4);
        uint i0 = (ushort)w0, i1 = w0 >> 16, i2 = (ushort)w1, i3 = w1 >> 16;
        PsgCheckVertex(i0, 0, 0x1A2);
        PsgCheckVertex(i1, 1, 0x1A3);
        PsgCheckVertex(i2, 2, 0x1A4);
        PsgCheckVertex(i3, 3, 0x1A5);
        int xy0 = xy[i0], xy1 = xy[i1], xy2 = xy[i2];
        if ((r[28] & 2) == 0 && !PsgFrontFacing(xy0, xy1, xy2))
        {
            return a3;
        }

        *(uint*)(a3 + 8) = (uint)xy0;
        *(uint*)(a3 + 20) = (uint)xy1;
        *(uint*)(a3 + 32) = (uint)xy2;
        *(uint*)(a3 + 44) = (uint)xy[i3];
        uint c23 = *(uint*)(r + 20);
        int depth = z[i0] + z[i1] + z[i2] + z[i3];
        uint c01 = *(uint*)(r + 16);
        uint page = pages[r[29]];
        *(uint*)(a3 + 4) = colors[(ushort)c01];
        *(uint*)(a3 + 16) = colors[c01 >> 16];
        *(uint*)(a3 + 28) = colors[(ushort)c23];
        uint c3 = colors[c23 >> 16];
        *(byte*)(a3 + 7) = 0x3C;
        *(uint*)(a3 + 40) = c3;
        uint t01 = *(uint*)(r + 8);
        uint t23 = *(uint*)(r + 12);
        *(uint*)(a3 + 12) = (page & 0xFFFF0000) | uvs[(ushort)t01];
        *(uint*)(a3 + 24) = (page << 16) | uvs[t01 >> 16];
        *(ushort*)(a3 + 36) = uvs[(ushort)t23];
        *(ushort*)(a3 + 48) = uvs[t23 >> 16];
        PsgApplySemiTransparency(r[28], a3, 26, true);
        *(byte*)(a3 + 3) = 12;
        PsgLinkPrimitive(ot, depth >> 6, a3);
        return a3 + 52;
    }

    /// <summary>Gouraud textured triangle record (24 bytes, +15 flags) to a 40-byte code 0x34 packet of 9 words.</summary>
    private static long PsgGouraudTexturedTri(
        byte* r, long a3, int* xy, int* z, ushort* uvs, uint* colors, uint* pages, uint* ot)
    {
        Recompiler.Runtime.Tracer.Enter("PsgGouraudTexturedTri(r, a3, xy, z, uvs, colors, pages, ot)", (nint)r, a3, (nint)xy, (nint)z, (nint)uvs, (nint)colors, (nint)pages, (nint)ot);
        uint w0 = *(uint*)r;
        uint i0 = (ushort)w0, i1 = w0 >> 16, i2 = *(ushort*)(r + 4);
        PsgCheckVertex(i0, 0, 0x207);
        PsgCheckVertex(i1, 1, 0x208);
        PsgCheckVertex(i2, 2, 0x209);
        int xy0 = xy[i0], xy1 = xy[i1], xy2 = xy[i2];
        if ((r[15] & 2) == 0 && !PsgFrontFacing(xy0, xy1, xy2))
        {
            return a3;
        }

        *(uint*)(a3 + 8) = (uint)xy0;
        int z0 = z[i0], z1 = z[i1], z2 = z[i2];
        *(uint*)(a3 + 20) = (uint)xy1;
        *(uint*)(a3 + 32) = (uint)xy2;
        PsgAverageDepth3(z0, z1, z2);
        int depth = dword_180211FDC;
        uint c01 = *(uint*)(r + 16);
        uint c2 = *(ushort*)(r + 20);
        uint page = pages[r[22]];
        *(uint*)(a3 + 4) = colors[(ushort)c01];
        *(uint*)(a3 + 16) = colors[c01 >> 16];
        uint color2 = colors[c2];
        *(byte*)(a3 + 7) = 0x34;
        *(uint*)(a3 + 28) = color2;
        uint t12 = *(uint*)(r + 8);
        *(uint*)(a3 + 12) = (page & 0xFFFF0000) | uvs[*(ushort*)(r + 6)];
        *(uint*)(a3 + 24) = (page << 16) | uvs[(ushort)t12];
        *(ushort*)(a3 + 36) = uvs[t12 >> 16];
        PsgApplySemiTransparency(r[15], a3, 26, true);
        *(byte*)(a3 + 3) = 9;
        PsgLinkPrimitive(ot, depth >> 2, a3);
        return a3 + 40;
    }

    /// <summary>Gouraud quad record (24 bytes, +20 flags) to a 36-byte code 0x38 packet of 8 words.</summary>
    private static long PsgGouraudQuad(byte* r, long a3, int* xy, int* z, uint* colors, uint* ot)
    {
        Recompiler.Runtime.Tracer.Enter("PsgGouraudQuad(r, a3, xy, z, colors, ot)", (nint)r, a3, (nint)xy, (nint)z, (nint)colors, (nint)ot);
        uint w0 = *(uint*)r;
        uint w1 = *(uint*)(r + 4);
        uint i0 = (ushort)w0, i1 = w0 >> 16, i2 = (ushort)w1, i3 = w1 >> 16;
        PsgCheckVertex(i0, 0, 0x269);
        PsgCheckVertex(i1, 1, 0x26A);
        PsgCheckVertex(i2, 2, 0x26B);
        PsgCheckVertex(i3, 3, 0x26C);
        int xy0 = xy[i0], xy1 = xy[i1], xy2 = xy[i2];
        if ((r[20] & 2) == 0 && !PsgFrontFacing(xy0, xy1, xy2))
        {
            return a3;
        }

        *(uint*)(a3 + 8) = (uint)xy0;
        *(uint*)(a3 + 16) = (uint)xy1;
        *(uint*)(a3 + 24) = (uint)xy2;
        *(uint*)(a3 + 32) = (uint)xy[i3];
        uint c23 = *(uint*)(r + 12);
        uint c01 = *(uint*)(r + 8);
        int depth = (z[i0] + z[i1] + z[i2] + z[i3]) >> 6;
        *(uint*)(a3 + 4) = colors[(ushort)c01];
        *(uint*)(a3 + 12) = colors[c01 >> 16];
        *(uint*)(a3 + 20) = colors[(ushort)c23];
        uint c3 = colors[c23 >> 16];
        *(byte*)(a3 + 7) = 0x38;
        *(uint*)(a3 + 28) = c3;
        *(byte*)(a3 + 3) = 8;
        PsgLinkPrimitive(ot, depth, a3);
        return a3 + 36;
    }

    /// <summary>Gouraud triangle record (20 bytes, +19 flags) to a 28-byte code 0x30 packet of 6 words.</summary>
    private static long PsgGouraudTri(byte* r, long a3, int* xy, int* z, uint* colors, uint* ot)
    {
        Recompiler.Runtime.Tracer.Enter("PsgGouraudTri(r, a3, xy, z, colors, ot)", (nint)r, a3, (nint)xy, (nint)z, (nint)colors, (nint)ot);
        uint w0 = *(uint*)r;
        uint i0 = (ushort)w0, i1 = w0 >> 16, i2 = *(ushort*)(r + 4);
        PsgCheckVertex(i0, 0, 0x2B6);
        PsgCheckVertex(i1, 1, 0x2B7);
        PsgCheckVertex(i2, 2, 0x2B8);
        int xy0 = xy[i0], xy1 = xy[i1], xy2 = xy[i2];
        if ((r[19] & 2) == 0 && !PsgFrontFacing(xy0, xy1, xy2))
        {
            return a3;
        }

        int z0 = z[i0], z1 = z[i1], z2 = z[i2];
        *(uint*)(a3 + 8) = (uint)xy0;
        *(uint*)(a3 + 16) = (uint)xy1;
        *(uint*)(a3 + 24) = (uint)xy2;
        PsgAverageDepth3(z0, z1, z2);
        uint c01 = *(uint*)(r + 8);
        uint c2 = *(ushort*)(r + 12);
        *(uint*)(a3 + 4) = colors[(ushort)c01];
        *(uint*)(a3 + 12) = colors[c01 >> 16];
        *(uint*)(a3 + 20) = colors[c2];
        int depth = dword_180211FDC;
        *(byte*)(a3 + 7) = 0x30;
        *(byte*)(a3 + 3) = 6;
        PsgLinkPrimitive(ot, depth >> 2, a3);
        return a3 + 28;
    }

    /// <summary>Flat quad record (16 bytes, +13 flags) to a 24-byte code 0x28 packet of 5 words.</summary>
    private static long PsgFlatQuad(byte* r, long a3, int* xy, int* z, uint* ot)
    {
        Recompiler.Runtime.Tracer.Enter("PsgFlatQuad(r, a3, xy, z, ot)", (nint)r, a3, (nint)xy, (nint)z, (nint)ot);
        uint w0 = *(uint*)r;
        uint w1 = *(uint*)(r + 4);
        uint i0 = (ushort)w0, i1 = w0 >> 16, i2 = (ushort)w1, i3 = w1 >> 16;
        PsgCheckVertex(i0, 0, 0x300);
        PsgCheckVertex(i1, 1, 0x301);
        PsgCheckVertex(i2, 2, 0x302);
        PsgCheckVertex(i3, 3, 0x303);
        int xy0 = xy[i0], xy1 = xy[i1], xy2 = xy[i2];
        if ((r[13] & 2) == 0 && !PsgFrontFacing(xy0, xy1, xy2))
        {
            return a3;
        }

        int depth = z[i0] + z[i1] + z[i2] + z[i3];
        *(uint*)(a3 + 4) = *(uint*)(r + 8);
        *(byte*)(a3 + 7) = 0x28;
        *(uint*)(a3 + 8) = (uint)xy0;
        *(uint*)(a3 + 12) = (uint)xy1;
        *(uint*)(a3 + 16) = (uint)xy2;
        *(uint*)(a3 + 20) = (uint)xy[i3];
        *(byte*)(a3 + 3) = 5;
        PsgLinkPrimitive(ot, depth >> 6, a3);
        return a3 + 24;
    }

    /// <summary>Flat triangle record (12 bytes, +11 flags) to a 20-byte code 0x20 packet of 4 words.</summary>
    private static long PsgFlatTri(byte* r, long a3, int* xy, int* z, uint* ot)
    {
        Recompiler.Runtime.Tracer.Enter("PsgFlatTri(r, a3, xy, z, ot)", (nint)r, a3, (nint)xy, (nint)z, (nint)ot);
        uint w0 = *(uint*)r;
        uint i0 = (ushort)w0, i1 = w0 >> 16, i2 = *(ushort*)(r + 4);
        PsgCheckVertex(i0, 0, 0x33F);
        PsgCheckVertex(i1, 1, 0x340);
        PsgCheckVertex(i2, 2, 0x341);
        int xy0 = xy[i0], xy1 = xy[i1], xy2 = xy[i2];
        if ((r[11] & 2) == 0 && !PsgFrontFacing(xy0, xy1, xy2))
        {
            return a3;
        }

        int z0 = z[i0], z1 = z[i1], z2 = z[i2];
        *(uint*)(a3 + 4) = *(uint*)(r + 8);
        PsgAverageDepth3(z0, z1, z2);
        *(uint*)(a3 + 8) = (uint)xy0;
        *(uint*)(a3 + 12) = (uint)xy1;
        *(uint*)(a3 + 16) = (uint)xy2;
        int depth = dword_180211FDC >> 2;
        *(byte*)(a3 + 7) = 0x20;
        *(byte*)(a3 + 3) = 4;
        PsgLinkPrimitive(ot, depth, a3);
        return a3 + 20;
    }

    /// <summary>
    /// GTE-style normal clip: stores the three screen XY words in 0x180211FF0..0x180211FF8 and the 2D cross product
    /// in 0x180212020 (MAC0); true when it is positive.
    /// </summary>
    private static bool PsgFrontFacing(int a, int b, int c)
    {
        Recompiler.Runtime.Tracer.Enter("PsgFrontFacing(a, b, c)", a, b, c);
        dword_180211FF0 = a;
        dword_180211FF4 = b;
        dword_180211FF8 = c;
        int ax = (short)a, ay = a >> 16, bx = (short)b, by = b >> 16, cx = (short)c, cy = c >> 16;
        dword_180212020 = unchecked((cx * ay) + (by * ax) + (bx * cy) - (ax * cy) - (ay * bx) - (cx * by));
        return dword_180212020 > 0;
    }

    /// <summary>
    /// GTE-style AVSZ3: stores the depths in 0x180212000..0x180212008, sets the low word of 0x180211FDC (its high word
    /// is kept) to ((u16 z0 + u16 z1 + u16 z2) shl 12) / 3 shr 14 saturated to 0..0xFFFF, and stores the quotient
    /// in 0x180212020.
    /// </summary>
    private static void PsgAverageDepth3(int z0, int z1, int z2)
    {
        Recompiler.Runtime.Tracer.Enter("PsgAverageDepth3(z0, z1, z2)", z0, z1, z2);
        dword_180212000 = z0;
        dword_180212004 = z1;
        dword_180212008 = z2;
        int quotient = (((ushort)dword_180212008 + (ushort)dword_180212004 + (ushort)dword_180212000) << 12) / 3;
        int average = quotient >> 14;
        ushort otz = average > 0xFFFF ? (ushort)0xFFFF : average < 0 ? (ushort)0 : (ushort)average;
        *(ushort*)Unsafe.AsPointer(ref dword_180211FDC) = otz;
        dword_180212020 = quotient;
    }

    /// <summary>
    /// Flag bit 0 of a primitive record: ORs bits 5..7 into the halfword at <paramref name="offset"/> and sets bit 1 of
    /// the code byte (the gouraud textured kinds store the code with that bit instead of OR-ing it; same result).
    /// </summary>
    private static void PsgApplySemiTransparency(byte flags, long a3, int offset, bool codeFirst)
    {
        Recompiler.Runtime.Tracer.Enter("PsgApplySemiTransparency(flags, a3, offset, codeFirst)", flags, a3, offset, codeFirst);
        if ((flags & 1) == 0)
        {
            return;
        }

        if (codeFirst)
        {
            *(byte*)(a3 + 7) |= 2;
            *(ushort*)(a3 + offset) |= (ushort)(flags & 0xE0);
        }
        else
        {
            *(ushort*)(a3 + offset) |= (ushort)(flags & 0xE0);
            *(byte*)(a3 + 7) |= 2;
        }
    }

    /// <summary>Clamps the depth to 0..4095 and inserts the packet at the head of that ordering-table entry (24-bit links).</summary>
    private static void PsgLinkPrimitive(uint* ot, int depth, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("PsgLinkPrimitive(ot, depth, a3)", (nint)ot, depth, a3);
        int index = depth < 0 ? 0 : depth >= 4096 ? GeoOtLastIndex : depth;
        uint* entry = ot + index;
        *(uint*)a3 ^= (*(uint*)a3 ^ *entry) & 0xFFFFFF;
        *entry ^= (*entry ^ (uint)PsxPtrToAddr((long)unk_180576A10, (ulong)a3)) & 0xFFFFFF;
    }

    /// <summary>
    /// <c>assert(indexN &lt; GEO_MAX_VERTICES)</c> of geo.cpp; a failure terminates like the CRT default (see DIV-004).
    /// </summary>
    private static void PsgCheckVertex(uint index, int which, int line)
    {
        Recompiler.Runtime.Tracer.Enter("PsgCheckVertex(index, which, line)", index, which, line);
        if (index >= GeoMaxVertices)
        {
            System.Environment.FailFast(
                "Assertion failed: index" + which
                + " < GEO_MAX_VERTICES, file ..\\..\\SpecialEffectCode\\sonoda\\Geo\\geo.cpp, line " + line);
        }
    }
}
