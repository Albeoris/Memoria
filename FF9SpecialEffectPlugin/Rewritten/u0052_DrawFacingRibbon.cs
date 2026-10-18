using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Builds the ribbon descriptor for EmitFacingRibbonStrip on the stack and emits <paramref name="a3"/> segments.
    /// Scales the near (<paramref name="a11"/>) and far (<paramref name="a12"/>) RGBA by <paramref name="a10"/>/4096
    /// (4096 when negative) through dword_180211FE0, takes the packet buffer from the work table slot 9 (advanced by
    /// 52 bytes per segment), loads the rotation (16 bytes + u16 at +16) and translation (+20..+28) of the camera block
    /// (slot 5) into the GTE RT/TR globals. Without a reference point (<paramref name="a2"/> null) the u16 triple at
    /// +84/+86/+88 of the block in slot 11 is copied to word_180622410. Returns the original's left-over rax when
    /// <paramref name="a3"/> is 0 (claim RENDER-022), else the strip's result.
    /// </summary>
    public static long DrawFacingRibbon(ulong a1, short* a2, int a3, short a4, short a5, short a6, short a7, int a8, int a9, int a10, byte* a11, byte* a12, int a13)
    {
        Recompiler.Runtime.Tracer.Enter("DrawFacingRibbon(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, …)", a1, (nint)a2, a3, a4, a5, a6, a7, a8, a9, a10);
        uint* table = (uint*)off_180066C68;
        int scale = a10 < 0 ? 4096 : a10;
        byte* color = (byte*)Unsafe.AsPointer(ref dword_180211FE0);

        color[0] = (byte)((scale * a11[0]) >> 12);
        color[1] = (byte)((scale * a11[1]) >> 12);
        color[2] = (byte)((scale * a11[2]) >> 12);
        color[3] = a11[3];

        uint packetAddress = table[9];
        byte* packet = ResolveGlobalPsxAddress(packetAddress);
        table[9] = unchecked(packetAddress + (uint)(52 * a3));

        // Descriptor layout as documented on EmitFacingRibbonStrip; +48..+95 is the callee's scratch area.
        Stack96_DrawFacingRibbon_desc desc = default;
        desc.union_12_4 = dword_180211FE0;

        color[0] = (byte)((scale * a12[0]) >> 12);
        color[1] = (byte)((scale * a12[1]) >> 12);
        color[2] = (byte)((scale * a12[2]) >> 12);
        color[3] = a12[3];

        desc.field_36 = (short)(a5 << 8);
        desc.field_38 = (short)((short)(a5 + a7) * 256);
        desc.field_16 = dword_180211FE0;
        desc.field_24 = (a8 | (32 * (a13 & 3))) << 16;
        desc.field_28 = a9 << 16;
        desc.union_15_1 = (byte)((a13 == 255 ? 0 : 2) | 0x3C);

        byte* camera = ResolveGlobalPsxAddress(table[5]);
        Camera2* camera2 = (Camera2*)camera;
        *(uint*)xmmword_180211F40 = camera2->field_0;
        *(uint*)(xmmword_180211F40 + 4) = camera2->field_4;
        *(uint*)(xmmword_180211F40 + 8) = camera2->field_8;
        *(uint*)(xmmword_180211F40 + 12) = camera2->field_12;
        dword_180211F50 = camera2->field_16;

        uint cameraAddress = table[5];
        long leftOver = DrawFacingRibbonLeftOverRax(cameraAddress, (uint)dword_180211F50);
        byte* translation = ResolveGlobalPsxAddress(cameraAddress);
        Translation* translation2 = (Translation*)translation;
        *(uint*)unk_180211F54 = translation2->field_20;
        *(uint*)unk_180211F58 = translation2->field_24;
        *(uint*)unk_180211F5C = translation2->field_28;

        if (a3 == 0)
        {
            return leftOver;
        }

        int step = 4096 / a3;
        desc.field_32 = step;
        desc.field_8 = step;

        if (a2 == null)
        {
            short* fallback = (short*)Unsafe.AsPointer(ref word_180622410);
            fallback[0] = *(short*)(ResolveGlobalPsxAddress(table[11]) + 84);
            fallback[1] = *(short*)(ResolveGlobalPsxAddress(table[11]) + 86);
            fallback[2] = *(short*)(ResolveGlobalPsxAddress(table[11]) + 88);
            a2 = fallback;
        }

        desc.field_40 = *(int*)a2;
        desc.field_44 = *((int*)a2 + 1);
        desc.field_4 = (int)PsxAddrMapperRegister((long)unk_180576A10, a1);
        desc.field_20 = a4;
        desc.field_22 = a6;

        uint* reloaded = (uint*)off_180066C68;
        desc.field_0 = (int)reloaded[10];
        byte* ot = ResolveGlobalPsxAddress(reloaded[8]);
        return EmitFacingRibbonStrip(a3, (ulong)((byte*)&desc), (ulong)packet, (ulong)ot);
    }

    /// <summary>
    /// rax after the inlined PSX address resolve of the camera block (claim RENDER-022): the zero-extended u16 just
    /// stored for address 0, the masked RAM offset, 32 * segment, or address - 0x1F800000 (32-bit) otherwise.
    /// </summary>
    private static long DrawFacingRibbonLeftOverRax(uint address, uint lastWord)
    {
        Recompiler.Runtime.Tracer.Enter("DrawFacingRibbonLeftOverRax(address, lastWord)", address, lastWord);
        if (address == 0)
        {
            return lastWord;
        }

        uint segment = address >> 24;
        if (segment == 0x80)
        {
            if ((address & 0xFFFFFFF) < 0x200000)
            {
                return address & 0xFFFFFFF;
            }
        }
        else if ((address & 0xC00000) == 0xC00000)
        {
            return 32L * segment;
        }

        return unchecked(address - 0x1F800000u);
    }
}
