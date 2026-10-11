namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Hi_DrawSliceEffModel: effect slot <paramref name="a4"/> (unused slot or no model: message into Buffer, hang).
    /// Sets the model's local matrix from <paramref name="a1"/>/<paramref name="a2"/>/<paramref name="a3"/>, builds the
    /// bone matrices into the packet area at context word 9 (advancing it), clears the first-packet pointer, sets the
    /// slice limit to the u16 at a2 + 4 minus the slot's u16 at +40, then transforms and emits every part of the
    /// geoslice mesh, advancing the packet cursor after each. Returns the last mapped cursor, or for a mesh without
    /// parts the register value left by the inlined resolution of the model address.
    /// </summary>
    public static long DrawSliceEffModel(long a1, long a2, long a3, int a4)
    {
        Recompiler.Runtime.Tracer.Enter("DrawSliceEffModel(a1, a2, a3, a4)", a1, a2, a3, a4);
        EffModelSlot* slot = EffModelSlotOrHang(a4, "Hi_DrawSliceEffModel()\nid:{0} cannot use!");
        ObjSetLocalMatrix((long)slot->model, (short*)a1, (uint*)a2, (short*)a3);
        DrawEffModelBuildBones(in *slot);

        var model = (PsgModelHandle*)slot->model;
        model->field_24 = 0;
        model = (PsgModelHandle*)slot->model;
        model->SliceLimit = (short)(*(ushort*)(a2 + 4) - slot->field_40);

        uint meshAddress = ((PsgModelHandle*)slot->model)->ModelAddr;
        long result = DrawEffModelMeshRax(meshAddress, (long)slot->model);
        int parts = ResolveGlobalPsxAddress(meshAddress)[3];
        for (int part = 0; part < parts; part++)
        {
            GeoSliceTransformPart((long)slot->model, (byte)part);
            byte* packet = ResolveGlobalPsxAddress(((uint*)off_180066C68)[9]);
            long next = GeoSliceEmitPrims((long)slot->model, (byte)part, (long)packet);
            result = PsxAddrMapperRegister((long)unk_180576A10, (ulong)next);
            ((uint*)off_180066C68)[9] = (uint)result;
        }

        return result;
    }

    /// <summary>
    /// Hi_DrawEffModelByBone: effect slot <paramref name="a2"/> attached to bone <paramref name="a4"/> of summon
    /// <paramref name="a3"/> (either unusable: message into Buffer, hang; the summon is checked after the effect's bone
    /// matrices are built). Copies the summon bone's 32-byte matrix into the effect's root matrix, scales it by
    /// diag(a1[0], a1[2], a1[4]) when <paramref name="a1"/> is given, clears the first-packet pointer and draws every
    /// part: the slot's u16 at +38 non-zero renders through GeoSfxRenderPart with it, else the u16 at +36 picks the
    /// mesh drawer (0, 2), GeoSfxRenderPart with 0 (1) or nothing. Returns the last mapped packet cursor or
    /// GeoTransformPart's result for a skipped part; without parts the register value of the model address resolution.
    /// </summary>
    public static long DrawEffModelByBone(short* a1, int a2, int a3, int a4)
    {
        Recompiler.Runtime.Tracer.Enter("DrawEffModelByBone(a1, a2, a3, a4)", (nint)a1, a2, a3, a4);
        EffModelSlot* slot = EffModelSlotOrHang(a2, "Hi_DrawEffModelByBone()\nid:{0} cannot use!");
        DrawEffModelBuildBones(in *slot);

        long summon = SummonModelOrHang(a3, "Hi_GetSummonBoneMatrix () \n id:{0} can not use!");
        byte* bone = (byte*)((PsgModelHandle*)summon)->field_56 + (32L * a4);
        var model = (PsgModelHandle*)slot->model;
        *(UInt128*)((short*)&model->RootMatrix) = *(UInt128*)bone;
        *(UInt128*)(((short*)&model->RootMatrix) + 8) = *(UInt128*)(bone + 16);

        if (a1 != null)
        {
            Stack32_DrawEffModelByBone_scale scale = default;
            *(UInt128*)((short*)&scale) = *(UInt128*)xmmword_18004B700;
            scale.field_0 = a1[0];
            scale.field_8 = a1[2];
            scale.union_16_8 = 4096;
            scale.union_16_2 = a1[4];
            scale.field_24 = 0;
            short* root = ((short*)&((PsgModelHandle*)slot->model)->RootMatrix);
            GteMulMatrix0((long)root, ((short*)&scale), (ushort*)root);
        }

        ((PsgModelHandle*)slot->model)->field_24 = 0;
        uint meshAddress = ((PsgModelHandle*)slot->model)->ModelAddr;
        long result = DrawEffModelMeshRax(meshAddress, (long)slot->model);
        int parts = ResolveGlobalPsxAddress(meshAddress)[3];
        for (int part = 0; part < parts; part++)
        {
            result = GeoTransformPart((long)slot->model, (byte)part);
            ushort renderArg = *(ushort*)((byte*)slot + 38);
            long next;
            if (renderArg != 0)
            {
                uint* packet = (uint*)ResolveGlobalPsxAddress(((uint*)off_180066C68)[9]);
                next = (long)GeoSfxRenderPart((long)slot->model, (byte)part, packet, (short)renderArg);
            }
            else
            {
                switch ((ushort)slot->field_36)
                {
                    case 0:
                    case 2:
                        next = PsgDrawMeshPart((long)slot->model, (byte)part, (long)ResolveGlobalPsxAddress(((uint*)off_180066C68)[9]));
                        break;
                    case 1:
                        next = (long)GeoSfxRenderPart((long)slot->model, (byte)part, (uint*)ResolveGlobalPsxAddress(((uint*)off_180066C68)[9]), 0);
                        break;
                    default:
                        continue;
                }
            }

            result = PsxAddrMapperRegister((long)unk_180576A10, (ulong)next);
            ((uint*)off_180066C68)[9] = (uint)result;
        }

        return result;
    }

    /// <summary>
    /// Builds the bone matrices of the slot's model into the packet area at context word 9 and stores the mapped end
    /// address back into word 9.
    /// </summary>
    private static void DrawEffModelBuildBones(in EffModelSlot slot)
    {
        Recompiler.Runtime.Tracer.Enter("DrawEffModelBuildBones(slot)", slot);
        short* packet = (short*)ResolveGlobalPsxAddress(((uint*)off_180066C68)[9]);
        short* end = PsgBuildBoneMatrices((long)slot.model, 0, packet);
        ((uint*)off_180066C68)[9] = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)end);
    }

    /// <summary>
    /// rax after the inlined resolution of the model address (returned when the mesh has no parts): the model pointer
    /// for 0 (the original then faults reading the part count), address &amp; 0xFFFFFFF in the RAM segment, 32 * segment
    /// for a mapped segment, else address &amp; 0xC00000.
    /// </summary>
    private static long DrawEffModelMeshRax(uint address, long model)
    {
        Recompiler.Runtime.Tracer.Enter("DrawEffModelMeshRax(address, model)", address, model);
        if (address == 0)
        {
            return model;
        }

        uint segment = address >> 24;
        if (segment == 128)
        {
            return address & 0xFFFFFFF;
        }

        return (address & 0xC00000) == 0xC00000 ? 32L * segment : address & 0xC00000;
    }
}
