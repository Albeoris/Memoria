using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Hi_DrawMorphModelByBone: effect slot <paramref name="a2"/> morphing towards the mesh of effect slot
    /// <paramref name="a3"/> (either unusable: message into Buffer, hang), attached to bone <paramref name="a6"/> of
    /// summon <paramref name="a5"/>. Stores the host pointer of a3's mesh as the morph target (+128), builds the bone
    /// matrices, copies the summon bone matrix into the root matrix (scaled by diag(a1[0], a1[2], a1[4]) when
    /// <paramref name="a1"/> is given) and draws every part after GeoMorphTransformPart with weight
    /// <paramref name="a4"/>, choosing the drawer like Hi_DrawEffModelByBone. Returns the last mapped packet cursor,
    /// GeoMorphTransformPart's result for a skipped part, or for a mesh without parts the register value of the model
    /// address resolution.
    /// </summary>
    public static long DrawMorphModelByBone(short* a1, int a2, int a3, ushort a4, int a5, int a6)
    {
        Recompiler.Runtime.Tracer.Enter("DrawMorphModelByBone(a1, a2, a3, a4, a5, a6)", (nint)a1, a2, a3, a4, a5, a6);
        EffModelSlot* slot = EffModelSlotOrHang(a2, "Hi_DrawMorphModelByBone()\nid:{0} cannot use!");
        EffModelSlot* target = EffModelSlotOrHang(a3, "Hi_DrawMorphModelByBone()\nid:{0} cannot use!");
        byte* targetMesh = ResolveGlobalPsxAddress(((PsgModelHandle*)target->model)->ModelAddr);
        ((PsgModelHandle*)slot->model)->field_128 = (ulong)targetMesh;
        DrawEffModelBuildBones(in *slot);

        long summon = SummonModelOrHang(a5, "Hi_GetSummonBoneMatrix () \n id:{0} can not use!");
        byte* bone = (byte*)((PsgModelHandle*)summon)->field_56 + (32L * a6);
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
            result = GeoMorphTransformPart((long)slot->model, (byte)part, a4);
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
    /// Hi_DrawSummonModel: summon slot <paramref name="a4"/> (unusable: message into Buffer, hang). Sets the local
    /// matrix, registers the motion block (model +16) and converts its offsets at +12 (&lt; 0x10000) and +16
    /// (&lt; 0x100000) into PSX addresses once. When the frame counter reached the motion's u16 frame count (+2) it wraps
    /// to 0 (bit 0 of <paramref name="a5"/>) or holds the last frame; builds the bone matrices for that frame, advances
    /// the counter and draws every part not hidden by the model's part mask (+32). Returns the frame drawn.
    /// </summary>
    public static long DrawSummonModel(long a1, long a2, long a3, int a4, sbyte a5)
    {
        Recompiler.Runtime.Tracer.Enter("DrawSummonModel(a1, a2, a3, a4, a5)", a1, a2, a3, a4, a5);
        var slot = (SummonSlot*)Unsafe.AsPointer(ref qword_180220830) + a4;
        if (slot->inUse == 0 || slot->model == 0)
        {
            WriteAnsi(Buffer, string.Format(System.Globalization.CultureInfo.InvariantCulture, "Hi_DrawSummonModel()\nid:{0} cannot use!", a4));
            HiraishiFatalHang();
        }

        ObjSetLocalMatrix((long)slot->model, (short*)a1, (uint*)a2, (short*)a3);
        long mapper = (long)unk_180576A10;
        byte* motion = (byte*)((PsgModelHandle*)slot->model)->field_16;
        PsxAddrMapperRegister(mapper, (ulong)motion);
        uint offset = *(uint*)(motion + 12);
        if (offset < 0x10000)
        {
            *(uint*)(motion + 12) = (uint)PsxPtrToAddr(mapper, (ulong)(motion + offset));
            uint secondOffset = *(uint*)(motion + 16);
            if (secondOffset < 0x100000)
            {
                *(uint*)(motion + 16) = (uint)PsxPtrToAddr(mapper, (ulong)(motion + secondOffset));
            }
        }

        var model = (PsgModelHandle*)slot->model;
        int frames = *(ushort*)((byte*)model->field_16 + 2);
        if (frames <= slot->motionFrame)
        {
            slot->motionFrame = (a5 & 1) != 0 ? (ushort)0 : (ushort)(frames - 1);
        }

        ushort frame = slot->motionFrame;
        short* packet = (short*)ResolveGlobalPsxAddress(((uint*)off_180066C68)[9]);
        short* end = PsgBuildBoneMatrices((long)model, frame, packet);
        ((uint*)off_180066C68)[9] = (uint)PsxAddrMapperRegister(mapper, (ulong)end);
        slot->motionFrame++;
        ((PsgModelHandle*)slot->model)->field_24 = 0;

        uint meshAddress = ((PsgModelHandle*)slot->model)->ModelAddr;
        int parts = ResolveGlobalPsxAddress(meshAddress)[3];
        for (int part = 0; part < parts; part++)
        {
            if ((((PsgModelHandle*)slot->model)->field_32 & (1u << part)) != 0)
            {
                continue;
            }

            GeoTransformPart((long)slot->model, (byte)part);
            byte* target = ResolveGlobalPsxAddress(((uint*)off_180066C68)[9]);
            long next = PsgDrawMeshPart((long)slot->model, (byte)part, (long)target);
            ((uint*)off_180066C68)[9] = (uint)PsxAddrMapperRegister(mapper, (ulong)next);
        }

        return frame;
    }
}
