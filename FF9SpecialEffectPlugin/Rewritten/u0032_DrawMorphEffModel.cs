namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Hi_DrawMorphEffModel: like DrawEffModel for effect slot a4, morphing towards the model of slot a5 (both hang
    /// when unused): model +128 = host pointer of the target model's header (PSX address at target +8), and each part
    /// is transformed by GeoMorphTransformPart with weight a6 before the same draw selection (u16 +38, then +36).
    /// Returns the last value left in rax.
    /// </summary>
    public static long DrawMorphEffModel(long a1, long a2, long a3, int a4, int a5, ushort a6)
    {
        Recompiler.Runtime.Tracer.Enter("DrawMorphEffModel(a1, a2, a3, a4, a5, a6)", a1, a2, a3, a4, a5, a6);
        const string format = "Hi_DrawMorphEffModel()\nid:{0} cannot use!";
        EffModelSlot* slot = EffModelSlotOrHang(a4, format);
        EffModelSlot* target = EffModelSlotOrHang(a5, format);

        long mapper = (long)unk_180576A10;
        *(long*)(slot->model + 128) = PsxAddrToPtr(mapper, *(uint*)(target->model + 8));
        ObjSetLocalMatrix((long)slot->model, (short*)a1, (uint*)a2, (short*)a3);

        uint* table = (uint*)off_180066C68;
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
            result = GeoMorphTransformPart((long)slot->model, part, a6);
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
}
