using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Sprite batch. Registers the item stream <paramref name="a1"/>, the parameter block <paramref name="a2"/>, the
    /// packet buffer <paramref name="a3"/> and the OT entry <paramref name="a4"/> with the module mapper and works on
    /// their PSX addresses. Parameters (uint each): +0 uv table (u16[16]), +4 colour table (4-byte entries), +8 width,
    /// +12 height, +20 clut, +24 depth-cue factor (negative: none), +28 count, +36 code byte. Each 3-byte item
    /// (signed x, signed y, nibbles uv | colour &lt;&lt; 4) is projected as (4x, 4y, 0) with RTPS and becomes a 20-byte
    /// sprite packet centered on SXY2, linked at the head of the OT entry. The item loop runs at least once (count 0
    /// wraps). Returns what the original leaves in rax: the end of the mapper entries it scanned
    /// (0x180576A20 + 32 * count) or, with no entries, the 24-bit link it merged into the last packet.
    /// </summary>
    public static ulong* EmitSpriteBatch(ulong a1, ulong a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("EmitSpriteBatch(a1, a2, a3, a4)", a1, a2, a3, a4);
        long context = (long)unk_180576A10;
        uint items = (uint)PsxAddrMapperRegister(context, a1);
        uint parameters = (uint)PsxAddrMapperRegister(context, a2);
        uint packets = (uint)PsxAddrMapperRegister(context, a3);
        uint otEntry = (uint)PsxAddrMapperRegister(context, a4);

        uint width = BsqRead32(parameters + 8);
        uint height = BsqRead32(parameters + 12);
        uint colorTable = BsqRead32(parameters + 4);
        uint uvTable = BsqRead32(parameters);
        uint clut = BsqRead32(parameters + 20) << 16;
        int depthCue = (int)BsqRead32(parameters + 24);
        uint code = BsqRead32(parameters + 36) << 24;
        uint count = BsqRead32(parameters + 28);

        *(int*)unk_180211FC4 = 0;
        qword_180211F94 = 0;
        dword_180211F9C = 0;
        uint halfHeight = height >> 1;
        uint halfWidth = width >> 1;
        uint* vector0 = (uint*)Unsafe.AsPointer(ref qword_180211FC0);

        ulong* result;
        do
        {
            int x = (sbyte)*ResolveGlobalPsxAddress(items);
            int y = (sbyte)*ResolveGlobalPsxAddress(items + 1);
            *vector0 = (ushort)(4 * x) | (uint)(y << 18);
            *(ushort*)ResolveGlobalPsxAddress(packets + 16) = (ushort)width;
            *(ushort*)ResolveGlobalPsxAddress(packets + 18) = (ushort)height;
            GteRtps(0);

            byte select = *ResolveGlobalPsxAddress(items + 2);
            items += 3;
            uint uv = *(ushort*)ResolveGlobalPsxAddress(uvTable + (2u * (select & 0xFu)));
            uint colorEntry = colorTable + (4u * (uint)(select >> 4));
            uint rg = *(ushort*)ResolveGlobalPsxAddress(colorEntry);
            uint b = *ResolveGlobalPsxAddress(colorEntry + 2);
            *(uint*)ResolveGlobalPsxAddress(packets + 12) = clut | uv;
            uint color = code | rg | (b << 16);
            uint top = (uint)(dword_180211FF8 >> 16) - halfHeight;
            *(ushort*)ResolveGlobalPsxAddress(packets + 8) = (ushort)((uint)dword_180211FF8 - halfWidth);
            *(ushort*)ResolveGlobalPsxAddress(packets + 10) = (ushort)top;
            if (depthCue >= 0)
            {
                dword_180211FD8 = (int)color;
                dword_180211FE0 = 4096 - depthCue;
                GteDpcs();
                color = (uint)dword_180212018;
            }

            *(uint*)ResolveGlobalPsxAddress(packets + 4) = color;
            byte* packet = ResolveGlobalPsxAddress(packets);
            Packet3* packet3 = (Packet3*)packet;
            var head = (uint*)ResolveGlobalPsxAddress(otEntry);
            packet[3] = 4;
            uint link = (packet3->union_0_4 ^ *head) & 0xFFFFFF;
            packet3->union_0_4 ^= link;
            int entries = (int)qword_1805789D0;
            result = entries <= 0 ? (ulong*)link : (ulong*)(unk_180576A20 + (32L * entries));
            uint address = (uint)PsxPtrToAddr(context, (ulong)packet);
            *head &= 0xFF000000;
            *head |= address & 0xFFFFFF;
            packets += 20;
        }
        while (--count != 0);

        return result;
    }
}
