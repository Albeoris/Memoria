using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Emits SPRT packets (4 words, length 3) from host buffers it first registers with the PSX mapper:
    /// <paramref name="a1"/> = 3-byte items (s8 dx, s8 dy, byte: low nibble UV index, high nibble colour index),
    /// <paramref name="a2"/> = parameter block (+0 UV table, +4 colour table, +8 size, +20 clut, +24 depth-cue IR0
    /// complement (negative = off), +28 count, +36 code byte), <paramref name="a3"/> = packets,
    /// <paramref name="a4"/> = the single OT entry every packet is linked at. Each item: RTPS of (4*dx, 4*dy, 0),
    /// XY = SXY2 - size/2 per axis. The count is not checked before the first item (0 wraps). Returns what the
    /// original leaves in rax: the end of the mapper's scanned windows, or the link xor when the mapper is empty.
    /// </summary>
    public static ulong* BuildSpritePrims(ulong a1, ulong a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("BuildSpritePrims(a1, a2, a3, a4)", a1, a2, a3, a4);
        long mapper = (long)unk_180576A10;
        uint items = (uint)PsxAddrMapperRegister(mapper, a1);
        uint parameters = (uint)PsxAddrMapperRegister(mapper, a2);
        uint packet = (uint)PsxAddrMapperRegister(mapper, a3);
        uint ot = (uint)PsxAddrMapperRegister(mapper, a4);

        uint colours = BsqRead32(parameters + 4);
        uint uvTable = BsqRead32(parameters);
        uint size = BsqRead32(parameters + 8);
        uint clut = BsqRead32(parameters + 20) << 16;
        int depthCue = (int)BsqRead32(parameters + 24);
        uint code = BsqRead32(parameters + 36) << 24;
        int count = (int)BsqRead32(parameters + 28);

        int* v = (int*)Unsafe.AsPointer(ref qword_180211FC0);
        v[1] = 0;
        qword_180211F94 = 0;
        dword_180211F9C = 0;
        uint half = size >> 1;
        while (true)
        {
            int dx = (sbyte)*ResolveGlobalPsxAddress(items);
            int dy = (sbyte)*ResolveGlobalPsxAddress(items + 1);
            v[0] = (ushort)(4 * dx) | (dy << 18);
            byte selector = *ResolveGlobalPsxAddress(items + 2);
            items += 3;
            GteRtps(0);

            uint uv = *(ushort*)ResolveGlobalPsxAddress(2u * (selector & 0xFu) + uvTable);
            uint colour = colours + 4u * (uint)(selector >> 4);
            uint redGreen = *(ushort*)ResolveGlobalPsxAddress(colour);
            uint blue = *ResolveGlobalPsxAddress(colour + 2);
            BsqWrite32(packet + 12, clut | uv);
            uint colourWord = code | redGreen | (blue << 16);
            int xy = dword_180211FF8;
            BsqWrite32(packet + 8, (((uint)(xy >> 16) - half) << 16) | (ushort)((uint)xy - half));
            if (depthCue >= 0)
            {
                dword_180211FD8 = (int)colourWord;
                dword_180211FE0 = 4096 - depthCue;
                GteDpcs();
                colourWord = (uint)dword_180212018;
            }

            int remaining = count - 1;
            BsqWrite32(packet + 4, colourWord);
            byte* head = ResolveGlobalPsxAddress(packet);
            Head* head2 = (Head*)head;
            uint* entry = (uint*)ResolveGlobalPsxAddress(ot);
            head[3] = 3;
            ulong result = (head2->union_0_4 ^ *entry) & 0xFFFFFF;
            head2->union_0_4 ^= (uint)result;
            int windows = (int)((PsxAddrMapper*)mapper)->count;
            if (windows > 0)
            {
                result = (ulong)(unk_180576A20 + 32L * windows);
            }

            uint address = (uint)PsxPtrToAddr(mapper, (ulong)head);
            *entry &= 0xFF000000;
            --count;
            *entry |= address & 0xFFFFFF;
            packet += 16;
            if (remaining == 0)
            {
                return (ulong*)result;
            }
        }
    }
}
