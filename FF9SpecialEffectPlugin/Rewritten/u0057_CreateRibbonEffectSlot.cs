using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Takes the first free 108-byte slot (u16 at +48 == 0) of the pool <paramref name="a2"/> (+0 int count, +8 int
    /// high-water index, +16 slot array), clears it and fills it with PSX addresses: +0 the effect data
    /// <paramref name="a3"/>, +20 its 8160-byte work area in unk_180587520, +4 the optional block at +56 of the header
    /// (header at a3 + 16 when the first data byte is 0xFF, else a3 + 40; +96 when present), +24 the 40-byte part table
    /// (header + 20; part offsets at +28/+32 up to 0x27FF are relocated against a3 unless the part type byte is 9 or the
    /// second byte is 0xFF), +36 the block after the table (its +60 cleared when it starts with -1), +40/+44
    /// <paramref name="a4"/>. +48 = 1 (8 bytes), +14 = 0 (4 bytes), +34 = 0, +58 = 255. Returns the slot, or 0 when the
    /// pool is full. <paramref name="a1"/> is unused. Count compares are signed (claim in the ledger).
    /// </summary>
    public static long CreateRibbonEffectSlot(long a1, long a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("CreateRibbonEffectSlot(a1, a2, a3, a4)", a1, a2, a3, a4);
        int count = *(int*)a2;
        byte* slot = *(byte**)(a2 + 16);
        if (count <= 0)
        {
            return 0;
        }

        int index = 0;
        while (*(ushort*)(slot + 48) != 0)
        {
            index++;
            slot += 108;
            if (index >= count)
            {
                return 0;
            }
        }

        if (*(int*)(a2 + 8) < index)
        {
            *(int*)(a2 + 8) = index;
        }

        for (int offset = 0; offset < 108; offset += 4)
        {
            *(uint*)(slot + offset) = 0;
        }

        long mapper = (long)unk_180576A10;
        *(ulong*)(slot + 48) = 1;
        *(uint*)(slot + 14) = 0;
        *(uint*)(slot + 20) = (uint)PsxAddrMapperRegister(mapper, (ulong)(unk_180587520 + (long)(8160 * index)));
        *(ushort*)(slot + 34) = 0;
        *(ushort*)(slot + 58) = 255;

        uint data = (uint)PsxAddrMapperRegister(mapper, a3);
        *(uint*)slot = data;
        ulong header = *ResolveGlobalPsxAddress(data) == 0xFF ? a3 + 16 : a3 + 40;
        if (ResolveGlobalPsxAddress(data)[1] != 0)
        {
            *(uint*)(slot + 4) = (uint)PsxAddrMapperRegister(mapper, header + 56);
            header += 96;
        }
        else
        {
            *(uint*)(slot + 4) = 0;
        }

        ulong parts = header + 20;
        *(uint*)(slot + 24) = (uint)PsxAddrMapperRegister(mapper, parts);
        ulong after = parts + (ulong)(40L * *(short*)(header + 4));
        uint* part = (uint*)(parts + 28);
        for (int i = 0; i < *(short*)(header + 4); i++, part += 10)
        {
            if (*((byte*)part - 28) != 9)
            {
                uint relative = part[0];
                if (relative <= 0x27FF)
                {
                    part[0] = (uint)PsxAddrMapperRegister(mapper, relative + a3);
                }
            }

            if (*((byte*)part - 27) != 0xFF)
            {
                uint relative = part[1];
                if (relative <= 0x27FF)
                {
                    part[1] = (uint)PsxAddrMapperRegister(mapper, relative + a3);
                }
            }
        }

        uint tail = (uint)PsxAddrMapperRegister(mapper, after);
        *(uint*)(slot + 36) = tail;
        if (*(int*)ResolveGlobalPsxAddress(tail) == -1)
        {
            *(uint*)(ResolveGlobalPsxAddress(tail) + 60) = 0;
        }

        uint target = (uint)PsxAddrMapperRegister(mapper, a4);
        *(uint*)(slot + 44) = target;
        *(uint*)(slot + 40) = target;
        return (long)slot;
    }

    /// <summary>
    /// Twin of DrawFacingRibbon for EmitScreenAlignedQuadStrip: builds the 112-byte stack descriptor (no reference
    /// point; +20 = <paramref name="a3"/>, +22 = <paramref name="a5"/>, +36/+38 = <paramref name="a4"/> and a4 + a6
    /// shifted by 8), loads the camera block of table slot 5 into the GTE RT/TR globals and emits <paramref name="a2"/>
    /// segments. Returns the left-over rax of the camera resolve when a2 is 0 (claim RENDER-022), else the strip's result.
    /// </summary>
    public static long DrawScreenAlignedRibbon(ulong a1, int a2, short a3, short a4, short a5, short a6, int a7, int a8, int a9, byte* a10, byte* a11, int a12)
    {
        Recompiler.Runtime.Tracer.Enter("DrawScreenAlignedRibbon(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, …)", a1, a2, a3, a4, a5, a6, a7, a8, a9, (nint)a10);
        uint* table = (uint*)off_180066C68;
        int scale = a9 < 0 ? 4096 : a9;
        byte* color = (byte*)Unsafe.AsPointer(ref dword_180211FE0);

        color[0] = (byte)((scale * a10[0]) >> 12);
        color[1] = (byte)((scale * a10[1]) >> 12);
        color[2] = (byte)((scale * a10[2]) >> 12);
        color[3] = a10[3];

        uint packetAddress = table[9];
        byte* packet = ResolveGlobalPsxAddress(packetAddress);
        table[9] = unchecked(packetAddress + (uint)(52 * a2));

        // rsp+20h..8Fh of the original frame; +40..+111 is the callee's scratch area.
        Stack112_DrawScreenAlignedRibbon_desc desc = default;
        desc.union_12_4 = dword_180211FE0;

        color[0] = (byte)((scale * a11[0]) >> 12);
        color[1] = (byte)((scale * a11[1]) >> 12);
        color[2] = (byte)((scale * a11[2]) >> 12);
        color[3] = a11[3];

        desc.field_36 = (short)(a4 << 8);
        desc.field_38 = (short)((short)(a4 + a6) * 256);
        desc.field_24 = (a7 | (32 * (a12 & 3))) << 16;
        desc.field_28 = a8 << 16;
        desc.union_15_1 = (byte)((a12 == 255 ? 0 : 2) | 0x3C);
        desc.field_16 = dword_180211FE0;

        byte* camera = ResolveGlobalPsxAddress(table[5]);
        Camera3* camera3 = (Camera3*)camera;
        *(uint*)xmmword_180211F40 = camera3->field_0;
        *(uint*)(xmmword_180211F40 + 4) = camera3->field_4;
        *(uint*)(xmmword_180211F40 + 8) = camera3->field_8;
        *(uint*)(xmmword_180211F40 + 12) = camera3->field_12;
        dword_180211F50 = camera3->field_16;

        uint cameraAddress = table[5];
        long leftOver = DrawFacingRibbonLeftOverRax(cameraAddress, (uint)dword_180211F50);
        byte* translation = ResolveGlobalPsxAddress(cameraAddress);
        Translation2* translation2 = (Translation2*)translation;
        *(uint*)unk_180211F54 = translation2->field_20;
        *(uint*)unk_180211F58 = translation2->field_24;
        *(uint*)unk_180211F5C = translation2->field_28;

        if (a2 == 0)
        {
            return leftOver;
        }

        int step = 4096 / a2;
        desc.field_32 = step;
        desc.field_8 = step;
        desc.field_4 = (int)PsxAddrMapperRegister((long)unk_180576A10, a1);
        desc.field_22 = a5;
        desc.field_20 = a3;

        uint* reloaded = (uint*)off_180066C68;
        desc.field_0 = (int)reloaded[10];
        byte* ot = ResolveGlobalPsxAddress(reloaded[8]);
        return EmitScreenAlignedQuadStrip(a2, (ulong)((byte*)&desc), (ulong)packet, (ulong)ot);
    }

    /// <summary>
    /// Draws one ribbon trail of the effect model <paramref name="a1"/> (+0 u16 frame count, +2 u16 ring capacity,
    /// +4 s16, +6 u16, +8/+9 u8 size + 2, +10 u16 color table offset, 4-byte frames from +12: u8 a, b (passed + 1), width, color
    /// index) into the trail ring <paramref name="a8"/> (+0 count - 1, +1 head, +2 length, 8-byte points from +4).
    /// <paramref name="a4"/> == -1 resets the ring. With a point <paramref name="a2"/> (6 bytes) it is pushed at the head
    /// with the frame width scaled by <paramref name="a5"/>/4096 in bits 17+. The style comes from dword_180367754
    /// (9, 51, 82, 243, 403: depth-cued; 486: facing) or <paramref name="a9"/> (0 screen-aligned, 1 facing, else
    /// depth-cued). Returns the low byte of rax: the callee's result, (byte)a1[0] on reset, see DIV for a null ring.
    /// </summary>
    public static sbyte DrawRibbonTrail(ushort* a1, long a2, short* a3, int a4, int a5, int a6, int a7, byte* a8, int a9)
    {
        Recompiler.Runtime.Tracer.Enter("DrawRibbonTrail(a1, a2, a3, a4, a5, a6, a7, a8, a9)", (nint)a1, a2, (nint)a3, a4, a5, a6, a7, (nint)a8, a9);
        if (a8 == null)
        {
            // The original returns the low byte of its entry rsp here (divergence recorded).
            return 0;
        }

        int capacity = a1[1];
        int field4 = (short)a1[2];
        int frames = a1[0];
        int field6 = a1[3];
        short sizeA = (short)(*((byte*)a1 + 8) - 2);
        short sizeB = (short)(*((byte*)a1 + 9) - 2);
        if (a4 == -1)
        {
            a8[1] = (byte)capacity;
            a8[0] = (byte)capacity;
            a8[2] = 0;
            return (sbyte)frames;
        }

        if (a4 >= frames)
        {
            a4 = frames - 1;
        }

        byte* frameData = (byte*)(a1 + 6);
        byte* frame = frameData + (long)(4 * (a4 + (2 * a1[5])));
        short frameA = (short)(frame[0] + 1);
        int scaledWidth = (a5 * frame[2]) >> 12;
        short frameB = (short)(frame[1] + 1);
        byte* colors = frameData + (long)(8 * frame[3]);
        int segments = capacity;
        if (a2 != 0)
        {
            int length = a8[2];
            int head = a8[1] - 1;
            if (head < 0)
            {
                head = capacity;
            }

            long offset = 8 * head;
            *(uint*)(a8 + offset + 4) = *(uint*)a2;
            *(uint*)(a8 + offset + 8) = (uint)(scaledWidth << 17) | *(ushort*)(a2 + 4);
            a8[1] = (byte)head;
            a8[2] = (byte)(length != capacity ? length + 1 : length);
            segments = length;
        }

        int style = dword_180367754;
        if (style != 9 && style != 51 && style != 82 && style != 243 && style != 403)
        {
            if (style == 486 || a9 == 1)
            {
                return (sbyte)DrawFacingRibbon((ulong)a8, a3, segments, frameA, frameB, sizeA, sizeB, field4, field6, a6, colors, colors + 4, a7);
            }

            if (a9 == 0)
            {
                return (sbyte)DrawScreenAlignedRibbon((ulong)a8, segments, frameA, frameB, sizeA, sizeB, field4, field6, a6, colors, colors + 4, a7);
            }
        }

        return EmitDepthCuedRibbon((ulong)a8, segments, frameA, frameB, sizeA, sizeB, field4, field6, a6, colors, colors + 4, a7);
    }
}
