using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Draws a textured (a6 + 1) x (a7 + 1) vertex grid as 40-byte FT4 quads at the packet pointer off_180066C68[9]
    /// (advanced by 40 * a6 * a7 up front): rotation a10 (Z, X, Y) scaled by a11[0..1] (a11[2] = 4096), translation
    /// (0, 0, dword_180211FA8), screen offset a2/a3; vertex (col * a4 - a4 * a8, row * a5 - a5 * a9) optionally
    /// deformed by the script a14 ("p" = vertex pointer); uv = (a2 + ..., a3 + ...) with a 128 texel tpage shift
    /// when any u &gt;= 256. Quads are linked into OT slot a1 of off_180066C68[8] when a1 &lt; 0x1000.
    /// dword_180211FA0/FA4 are restored; returns the saved dword_180211FA4.
    /// </summary>
    public static long SfxDrawWarpGrid(uint a1, int a2, int a3, int a4, int a5, int a6, int a7, short a8, short a9, short* a10, int* a11, long a12, int a13, int a14)
    {
        Recompiler.Runtime.Tracer.Enter("SfxDrawWarpGrid(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, …)", a1, a2, a3, a4, a5, a6, a7, a8, a9, (nint)a10);
        int savedOffsetX = dword_180211FA0;
        int savedOffsetY = dword_180211FA4;
        int* offsets = (int*)off_180066C68;
        uint packetAddress = (uint)offsets[9];
        byte* prim = ResolveGlobalPsxAddress(packetAddress);
        offsets[9] = (int)(packetAddress + (uint)(40 * (a6 * a7)));

        // MATRIX on the stack: m[3][3] int16, pad, t[3] int32.
        byte* matrix = stackalloc byte[32];
        short* m = (short*)matrix;
        m[0] = 4096;
        m[1] = 0;
        m[2] = 0;
        m[3] = 0;
        m[4] = 4096;
        m[5] = 0;
        m[6] = 0;
        m[7] = 0;
        m[8] = 4096;
        *(byte*)(a12 + 3) = (byte)((a13 == 255 ? 0 : 2) | 0x44);
        int blend = (a13 == 255 ? 0 : a13) & 3;
        dword_180211FA0 = a2;
        dword_180211FA4 = a3;
        int tpageNormal = (ushort)((blend | 8) << 5) << 16;
        int tpageShifted = (ushort)((blend << 5) | 0x102) << 16;
        int originX = a4 * a8;
        int originY = a5 * a9;
        short* rotated = (short*)RotMatrixZ(a10[2], m);
        rotated = (short*)RotMatrixX(a10[0], rotated);
        RotMatrixY(a10[1], rotated);

        int scaleX = a11[0];
        int scaleY = a11[1];
        a11[2] = 4096;
        m[0] = (short)((m[0] * scaleX) >> 12);
        m[1] = (short)((m[1] * scaleY) >> 12);
        m[2] = (short)((m[2] << 12) >> 12);
        m[3] = (short)((m[3] * scaleX) >> 12);
        m[4] = (short)((m[4] * scaleY) >> 12);
        m[5] = (short)((m[5] << 12) >> 12);
        m[6] = (short)((m[6] * scaleX) >> 12);
        *(long*)(matrix + 20) = 0;
        qword_180211F54 = 0;
        *(int*)unk_180211FC4 = 0;
        dword_180211FCC = 0;
        dword_180211FD4 = 0;
        m[7] = (short)((m[7] * scaleY) >> 12);
        int translateZ = dword_180211FA8;
        *(int*)(matrix + 28) = translateZ;
        dword_180211F5C = translateZ;
        System.Buffer.MemoryCopy(matrix, xmmword_180211F40, 16, 16);
        m[8] = (short)((m[8] << 12) >> 12);
        dword_180211F50 = (ushort)m[8];

        if (a7 >= 0)
        {
            // uv[20] then xy[20] as in the original frame; a6 >= 20 overruns uv into xy (and the original's stack).
            int* small = stackalloc int[40];
            int* rowsBuffer = a6 < 20 ? small : (int*)NativeMemory.AllocZeroed((nuint)(a6 + 21), sizeof(int));
            Part106cGridRows(rowsBuffer, prim, a1, a2, a3, a4, a5, a6, a7, originX, originY, a12, a14, tpageNormal, tpageShifted);
            if (rowsBuffer != small)
            {
                NativeMemory.Free(rowsBuffer);
            }
        }

        dword_180211FA0 = savedOffsetX;
        dword_180211FA4 = savedOffsetY;
        return (uint)savedOffsetY;
    }

    /// <summary>Row/column walk of SfxDrawWarpGrid; uv[col] = rowsBuffer[col], xy[col] = rowsBuffer[20 + col].</summary>
    private static void Part106cGridRows(int* rowsBuffer, byte* prim, uint a1, int a2, int a3, int a4, int a5, int a6, int a7, int originX, int originY, long a12, int a14, int tpageNormal, int tpageShifted)
    {
        Recompiler.Runtime.Tracer.Enter("Part106cGridRows(rowsBuffer, prim, a1, a2, a3, a4, a5, a6, a7, originX, …)", (nint)rowsBuffer, (nint)prim, a1, a2, a3, a4, a5, a6, a7, originX);
        int* uv = rowsBuffer;
        int* xy = rowsBuffer + 20;
        int rowY = 0;
        long row = 0;
        ulong rows = (uint)(a7 + 1);
        int leftXy = a4;
        int leftOldXy = a4;
        int leftUv = 0;
        int leftOldUv = 0;
        do
        {
            if (a6 >= 0)
            {
                ushort u0 = (ushort)((ushort)a2 - (ushort)originX);
                short localY = (short)(rowY - originY);
                short v = (short)(rowY - originY + a3);
                int x = 0;
                for (long col = 0; col <= a6; col++)
                {
                    int oldUv = uv[col];
                    int oldXy = xy[col];
                    ushort u = (ushort)(u0 + x);
                    int currentUv = u | (v << 16);
                    int vertex = (ushort)(x - originX) | (localY << 16);
                    uv[col] = currentUv;
                    if (a14 != 0)
                    {
                        ulong slot = (ulong)&vertex;
                        sub_18000DBD0(qword_1803678E0, a14, (long)unk_18004B684, &slot);
                    }

                    int currentXy = vertex;
                    xy[col] = currentXy;
                    if (col != 0 && row != 0)
                    {
                        Part106cEmitQuad(prim, a1, a12, currentXy, oldXy, leftXy, leftOldXy, (short)u, v, oldUv, leftUv, leftOldUv, tpageNormal, tpageShifted);
                        prim += 40;
                    }

                    x += a4;
                    leftXy = currentXy;
                    leftOldXy = oldXy;
                    leftOldUv = oldUv;
                    leftUv = currentUv;
                }
            }

            rowY += a5;
            ++row;
        }
        while (--rows != 0);
    }

    /// <summary>
    /// One FT4 quad: vertices (row, col), (row-1, col), (row, col-1) through RTPS 0..2, then (row-1, col-1) with RTPS 0;
    /// colour word *a12; uv words are (sign-extended v &lt;&lt; 8) | sign-extended u, all u - 128 with the shifted tpage
    /// when any u &gt;= 256.
    /// </summary>
    private static void Part106cEmitQuad(byte* prim, uint a1, long a12, int currentXy, int oldXy, int leftXy, int leftOldXy, short uCurrent, short v, int oldUv, int leftUv, int leftOldUv, int tpageNormal, int tpageShifted)
    {
        Recompiler.Runtime.Tracer.Enter("Part106cEmitQuad(prim, a1, a12, currentXy, oldXy, leftXy, leftOldXy, uCurrent, v, oldUv, …)", (nint)prim, a1, a12, currentXy, oldXy, leftXy, leftOldXy, uCurrent, v, oldUv);
        int* vxy0 = (int*)Unsafe.AsPointer(ref qword_180211FC0);
        *vxy0 = currentXy;
        dword_180211FC8 = oldXy;
        dword_180211FD0 = leftXy;
        *(int*)(prim + 4) = *(int*)a12;
        GteRtps(0);
        GteRtps(1);
        GteRtps(2);
        short uUp = (short)oldUv;
        short uUpLeft = (short)leftOldUv;
        short uLeft = (short)leftUv;
        int tpage;
        if (uUp >= 256 || uCurrent >= 256 || uUpLeft >= 256 || uLeft >= 256)
        {
            uUp -= 128;
            uCurrent -= 128;
            uLeft -= 128;
            uUpLeft -= 128;
            tpage = tpageShifted;
        }
        else
        {
            tpage = tpageNormal;
        }

        *(int*)(prim + 8) = dword_180211FF0;
        *(int*)(prim + 16) = dword_180211FF4;
        *(int*)(prim + 24) = dword_180211FF8;
        *vxy0 = leftOldXy;
        if (a1 < 0x1000)
        {
            uint* ot = (uint*)ResolveGlobalPsxAddress((uint)((int*)off_180066C68)[8]) + (int)a1;
            prim[3] = 9;
            *(uint*)prim ^= (*(uint*)prim ^ *ot) & 0xFFFFFF;
            uint address = (uint)PsxPtrToAddr((long)unk_180576A10, (ulong)prim);
            *ot ^= (address ^ *ot) & 0xFFFFFF;
        }

        GteRtps(0);
        *(int*)(prim + 12) = (v << 8) | uCurrent;
        *(int*)(prim + 20) = (((short)(oldUv >> 16) << 8) | uUp) | tpage;
        *(int*)(prim + 28) = ((short)(leftUv >> 16) << 8) | uLeft;
        *(int*)(prim + 36) = ((short)(leftOldUv >> 16) << 8) | uUpLeft;
        *(int*)(prim + 32) = dword_180211FF8;
    }

    /// <summary>
    /// Ticks the 11 effect tasks (32-byte entries from 0x180323278: tick dword, state word, data PSX address +12,
    /// slot pool +16, native callback +24): callback(2, data, off_180066C68, entry, 0) != 0 ends the task (state 2),
    /// else the tick counts up; an ending task whose pool has no live slot is freed (dword_180323270--) and its data
    /// and pool blocks are filled with -1 (size dword / 4 dwords from the size word on). Returns the active count.
    /// </summary>
    public static long SfxTickEffectTasks()
    {
        Recompiler.Runtime.Tracer.Enter("SfxTickEffectTasks()");
        byte* entry = unk_18032327C;
        uint active = 0;
        for (int remaining = 11; remaining != 0; --remaining, entry += 32)
        {
            if (*(ushort*)entry == 0)
            {
                continue;
            }

            nint offsets = off_180066C68;
            ++active;
            ((int*)offsets)[10] = 0;
            byte* data = ResolveGlobalPsxAddress(*(uint*)(entry + 8));
            var callback = (delegate* unmanaged<long, byte*, nint, byte*, long, int>)*(nint*)(entry + 20);
            if (callback(2, data, offsets, entry - 4, 0) != 0)
            {
                *(ushort*)entry = 2;
            }
            else
            {
                ++*(int*)(entry - 4);
            }

            uint pool = *(uint*)(entry + 12);
            if ((pool == 0 || (uint)SfxTickScriptSlots(pool) == 0) && *(ushort*)entry == 2)
            {
                *(ushort*)entry = 0;
                --dword_180323270;
                uint dataAddress = *(uint*)(entry + 8);
                if (dataAddress != 0)
                {
                    Part106cFillBlock(ResolveGlobalPsxAddress(dataAddress));
                }

                uint poolAddress = *(uint*)(entry + 12);
                if (poolAddress != 0)
                {
                    Part106cFillBlock(ResolveGlobalPsxAddress(poolAddress));
                }
            }
        }

        return active;
    }

    /// <summary>Fills size / 4 dwords with -1 starting at the size dword in front of <paramref name="block"/>.</summary>
    private static void Part106cFillBlock(byte* block)
    {
        Recompiler.Runtime.Tracer.Enter("Part106cFillBlock(block)", (nint)block);
        int* cursor = (int*)(block - 4);
        int count = *cursor / 4;
        for (int i = 0; i < count; i++)
        {
            cursor[i] = -1;
        }
    }
}
