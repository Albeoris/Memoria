namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Hi_DrawEffModel: draws the model of effect slot a4 (hangs when the slot is unused): sets its local matrix from
    /// a1..a3, builds the bone matrices into the PSX work buffer of table slot 9, then transforms and draws each part
    /// (count = byte +3 of the model header at the PSX address model+8). A non-zero u16 +38 of the slot renders every
    /// part through GeoSfxRenderPart with that value; otherwise the u16 +36 selects mesh drawing (0, 2) or
    /// GeoSfxRenderPart (1); other values only transform. Every draw advances slot 9 to the returned buffer end.
    /// Returns the last value left in rax (see DrawEffModelHeaderRax when no part is drawn).
    /// </summary>
    public static long DrawEffModel(long a1, long a2, long a3, int a4)
    {
        Recompiler.Runtime.Tracer.Enter("DrawEffModel(a1, a2, a3, a4)", a1, a2, a3, a4);
        EffModelSlot* slot = EffModelSlotOrHang(a4, "Hi_DrawEffModel()\nid:{0} cannot use!");
        ObjSetLocalMatrix((long)slot->model, (short*)a1, (uint*)a2, (short*)a3);

        uint* table = (uint*)off_180066C68;
        long mapper = (long)unk_180576A10;
        short* bones = PsgBuildBoneMatrices((long)slot->model, 0, (short*)PsxAddrToPtr(mapper, table[9]));
        ((uint*)off_180066C68)[9] = (uint)PsxAddrMapperRegister(mapper, (ulong)bones);

        *(ulong*)(slot->model + 24) = 0;
        uint headerAddress = *(uint*)(slot->model + 8);
        long result = DrawEffModelHeaderRax(headerAddress, (long)slot->model);
        byte* header = (byte*)PsxAddrToPtr(mapper, headerAddress);
        int parts = header[3];
        for (int i = 0; i < parts; i++)
        {
            byte part = (byte)i;
            result = GeoTransformPart((long)slot->model, part);
            short offset = *(short*)((byte*)slot + 38);
            long end;
            if (offset != 0)
            {
                end = (long)GeoSfxRenderPart((long)slot->model, part, DrawEffModelWorkBuffer(), offset);
            }
            else
            {
                ushort kind = (ushort)slot->field_36;
                if (kind == 1)
                {
                    end = (long)GeoSfxRenderPart((long)slot->model, part, DrawEffModelWorkBuffer(), 0);
                }
                else if (kind == 0 || kind == 2)
                {
                    end = PsgDrawMeshPart((long)slot->model, part, (long)DrawEffModelWorkBuffer());
                }
                else
                {
                    continue;
                }
            }

            result = PsxAddrMapperRegister(mapper, (ulong)end);
            ((uint*)off_180066C68)[9] = (uint)result;
        }

        return result;
    }

    /// <summary>Host pointer of the PSX work buffer in table slot 9 (re-read for every part).</summary>
    private static uint* DrawEffModelWorkBuffer()
    {
        Recompiler.Runtime.Tracer.Enter("DrawEffModelWorkBuffer()");
        return (uint*)PsxAddrToPtr((long)unk_180576A10, ((uint*)off_180066C68)[9]);
    }

    /// <summary>
    /// rax after the inlined address resolution of the model header in the original (returned when the model has
    /// no parts): the model pointer for address 0, the masked address for RAM / non-segment addresses, or
    /// 32 * segment for a 0xC0xxxx segment address.
    /// </summary>
    private static long DrawEffModelHeaderRax(uint address, long model)
    {
        Recompiler.Runtime.Tracer.Enter("DrawEffModelHeaderRax(address, model)", address, model);
        if (address == 0)
        {
            return model;
        }

        uint segment = address >> 24;
        if (segment == 128)
        {
            return address & 0xFFFFFFF;
        }

        uint masked = address & 0xC00000;
        return masked == 0xC00000 ? 32L * segment : masked;
    }
}
