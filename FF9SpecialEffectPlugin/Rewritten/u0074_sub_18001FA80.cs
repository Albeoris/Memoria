namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Registers the host buffers a1 (SpriteRecord[], 10 bytes each), a2 (SpriteListHeader), a3 (SPRT packets, 20 bytes
    /// each) and a4 (OT entry) with the PSX address mapper, then for each of header.count records writes one SPRT packet
    /// (colour/code, x/y = header base + 4 * record dx/dy, uv | clut &lt;&lt; 16, w/h) and links it into the OT entry
    /// (length 4). Whenever the record tpage differs from the current one, a DR_MODE packet for the previous tpage
    /// (header.modeBits | tpage | 0xE1000200, length 1) is appended at header.modeCursor and linked first; the last group's
    /// DR_MODE follows the loop. Every field is resolved through its own PSX address, like the original.
    /// Returns the host pointer of the advanced mode cursor.
    /// </summary>
    public static byte* PsxAddSpriteList(ulong a1, ulong a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("PsxAddSpriteList(a1, a2, a3, a4)", a1, a2, a3, a4);
        long mapper = (long)unk_180576A10;
        uint record = (uint)PsxAddrMapperRegister(mapper, a1);
        uint header = (uint)PsxAddrMapperRegister(mapper, a2);
        uint packet = (uint)PsxAddrMapperRegister(mapper, a3);
        uint otEntry = (uint)PsxAddrMapperRegister(mapper, a4);

        uint clut = *(uint*)PsxField(header, 16);
        int count = *(int*)PsxField(header, 20);
        uint modeCursor = *(uint*)PsxField(header, 24);
        uint colorCode = *(uint*)PsxField(header, 0);
        uint modeBits = *(uint*)PsxField(header, 12);
        int x = *(int*)PsxField(header, 4);
        int y = *(int*)PsxField(header, 8);
        uint clutBits = clut << 16;

        uint tpage = 0;
        if (count != 0)
        {
            do
            {
                *(uint*)PsxField(packet, 4) = colorCode;
                short dx = *(sbyte*)PsxField(record, 0);
                short top = (short)(y + 4 * *(sbyte*)PsxField(record, 1));
                *(ushort*)PsxField(packet, 8) = (ushort)(x + 4 * dx);
                *(ushort*)PsxField(packet, 10) = (ushort)top;
                ushort width = *PsxField(record, 2);
                ushort height = *PsxField(record, 3);
                *(ushort*)PsxField(packet, 16) = width;
                *(ushort*)PsxField(packet, 18) = height;
                uint uv = *(ushort*)PsxField(record, 4);
                uint recordTpage = *(ushort*)PsxField(record, 8);
                *(uint*)PsxField(packet, 12) = clutBits | uv;
                record += 10;

                if (recordTpage != tpage)
                {
                    if (tpage != 0)
                    {
                        AppendSpriteDrawMode(ref modeCursor, modeBits | tpage, otEntry);
                    }

                    tpage = recordTpage;
                }

                LinkSpritePacket(ResolveGlobalPsxAddress(packet), ResolveGlobalPsxAddress(otEntry), 4);
                packet += 20;
            }
            while (--count != 0);
        }

        if (tpage != 0)
        {
            AppendSpriteDrawMode(ref modeCursor, modeBits | tpage, otEntry);
        }

        return ResolveGlobalPsxAddress(modeCursor);
    }

    /// <summary>DR_MODE word at cursor + 4, length 1, linked into the OT entry; the cursor advances by 8.</summary>
    private static void AppendSpriteDrawMode(ref uint cursor, uint bits, uint otEntry)
    {
        Recompiler.Runtime.Tracer.Enter("AppendSpriteDrawMode(cursor, bits, otEntry)", cursor, bits, otEntry);
        *(uint*)PsxField(cursor, 4) = bits | 0xE1000200;
        LinkSpritePacket(ResolveGlobalPsxAddress(cursor), ResolveGlobalPsxAddress(otEntry), 1);
        cursor += 8;
    }

    /// <summary>addPrim: packet length byte, packet link = entry link, entry link = PSX address of the packet.</summary>
    private static void LinkSpritePacket(byte* packet, byte* entry, byte length)
    {
        Recompiler.Runtime.Tracer.Enter("LinkSpritePacket(packet, entry, length)", (nint)packet, (nint)entry, length);
        packet[3] = length;
        *(uint*)packet ^= (*(uint*)entry ^ *(uint*)packet) & 0xFFFFFF;
        *(uint*)entry ^= (*(uint*)entry ^ (uint)PsxPtrToAddr((long)unk_180576A10, (ulong)packet)) & 0xFFFFFF;
    }
}
