using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Builds the 32-byte GTE bone matrices (RT + TR) of the PsgModelHandle <paramref name="a1"/> for animation frame
    /// <paramref name="a2"/> into <paramref name="a3"/> (also stored at handle+56) and returns the end of the array.
    /// <para>Without an animation (handle+16 == 0) only the root is written: a copy of bone ParentBoneIndex of the parent
    /// handle at +48 with its TR moved by RT * V0 (V0 = 8 bytes at handle+160), or, without a parent, the handle's
    /// own matrix at +64 with every RT element outside the first column (RT11, RT21, RT31) negated; returns a3 + 32 bytes.</para>
    /// <para>With an animation: root TR from the header (+4/+6/+8: a constant when flag bit 0/1/2 of +10 is set,
    /// otherwise the offset of a per-frame s16 track); per bone the rotation angles come from the 8-byte keys at the
    /// PSX address +12 (each angle a constant or a per-frame byte track, times 16), refined by the 4-bit tracks at +16
    /// (nibble per frame) when present, and RT = Ry * Rx * Rz applied to identity (X, then Y, then Z). The root is then
    /// replaced by the parent's bone (TR moved as above) or premultiplied by the handle's matrix at +64; every other
    /// bone is premultiplied by its parent bone (skeleton entries at PsgModel.Addr12 + 4: s16 length, parent index in
    /// byte +3) with TR = parent.RT * (0, 0, length) + parent.TR, the vector taken from the scratch at 0x180212048.</para>
    /// </summary>
    public static short* PsgBuildBoneMatrices(long a1, ushort a2, short* a3)
    {
        Recompiler.Runtime.Tracer.Enter("PsgBuildBoneMatrices(a1, a2, a3)", a1, a2, (nint)a3);
        var handle = (PsgModelHandle*)a1;
        byte* anim = (byte*)handle->field_16;
        handle->field_56 = (ulong)a3;
        if (anim == null)
        {
            if (handle->field_48 != 0)
            {
                PsgBuildBoneMatricesAttach(handle, a3);
                return a3 + 16;
            }

            var own = (ushort*)(a1 + 64);
            var root = (ushort*)handle->field_56;
            root[0] = own[0];
            root[1] = (ushort)-own[1];
            root[2] = (ushort)-own[2];
            root[3] = own[3];
            root[4] = (ushort)-own[4];
            root[5] = (ushort)-own[5];
            root[6] = own[6];
            root[7] = (ushort)-own[7];
            root[8] = (ushort)-own[8];
            *(int*)((byte*)root + 20) = *(int*)(a1 + 84);
            *(int*)((byte*)root + 24) = *(int*)(a1 + 88);
            *(int*)((byte*)root + 28) = *(int*)(a1 + 92);
            return a3 + 16;
        }

        long mapper = (long)unk_180576A10;
        var model = (byte*)PsxAddrToPtr(mapper, handle->ModelAddr);
        int boneCount = model[2];
        qword_180212048 = 0;
        var keys = (int*)PsxAddrToPtr(mapper, *(uint*)(anim + 12));
        var fine = (int*)PsxAddrToPtr(mapper, *(uint*)(anim + 16));
        byte flags = anim[10];
        int* rootTr = (int*)a3 + 5;
        rootTr[0] = (flags & 1) != 0 ? *(short*)(anim + 4) : *(short*)(anim + *(ushort*)(anim + 4) + 2L * a2);
        rootTr[1] = (flags & 2) != 0 ? *(short*)(anim + 6) : *(short*)(anim + *(ushort*)(anim + 6) + 2L * a2);
        rootTr[2] = (flags & 4) != 0 ? *(short*)(anim + 8) : *(short*)(anim + *(ushort*)(anim + 8) + 2L * a2);

        short* end = a3 + 16 * boneCount;
        short* bone = a3;
        do
        {
            int key0 = keys[0];
            int key1 = keys[1];
            int rx = (key1 & 0x10000) != 0 ? (short)key0 : anim[(ushort)key0 + (long)a2];
            ushort rx16 = (ushort)(16 * rx);
            uint ry = (key1 & 0x20000) != 0 ? (uint)(key0 >> 16) : anim[(key0 >> 16) + (long)a2];
            int ry20 = (int)(ry << 20);
            int rz = (key1 & 0x40000) != 0 ? (ushort)key1 : anim[(ushort)key1 + (long)a2];
            int rz16 = 16 * rz;
            int packedXy = ry20 | rx16;
            dword_180212044 = rz16;
            dword_180212040 = packedXy;
            if (fine != null)
            {
                int fine0 = fine[0];
                int fine1 = fine[1];
                long frame = a2 >> 1;
                int fx = (fine1 & 0x10000) != 0 ? fine0 : PsgBuildBoneMatricesNibble(in *(anim + (ushort)fine0 + frame), a2);
                int fy = (fine1 & 0x20000) != 0 ? fine0 >> 16 : PsgBuildBoneMatricesNibble(in *(anim + (fine0 >> 16) + frame), a2);
                int fz = (fine1 & 0x40000) != 0 ? fine1 : PsgBuildBoneMatricesNibble(in *(anim + (ushort)fine1 + frame), a2);
                dword_180212044 = (fz & 0xF) | rz16;
                fine += 2;
                dword_180212040 = (fx & 0xF) | ((fy & 0xF) << 16) | packedXy;
            }

            *(uint*)(bone + 7) = 0x10000000;
            *(uint*)(bone + 3) = 0x10000000;
            *(uint*)bone = 4096;
            bone[2] = 0;
            *(uint*)(bone + 5) = 0;
            keys += 2;
            RotMatrixX((short)dword_180212040, bone);
            RotMatrixY((short)(dword_180212040 >> 16), bone);
            RotMatrixZ((short)dword_180212044, bone);
            bone += 16;
        }
        while (bone < end);

        byte* gte = xmmword_180211F40;
        if (handle->field_48 != 0)
        {
            PsgBuildBoneMatricesAttach(handle, a3);
        }
        else
        {
            *(int*)(gte + 20) = *(int*)(a1 + 84);
            *(long*)(gte + 24) = *(long*)(a1 + 88);
            GteMulMatrix0(a1 + 64, a3, (ushort*)a3);
            short* v0 = (short*)Unsafe.AsPointer(ref qword_180211FC0);
            v0[0] = a3[10];
            v0[1] = a3[12];
            v0[2] = a3[14];
            v0[3] = a3[16];
            GteRtV0Tr();
            *((int*)a3 + 5) = dword_180212024;
            *(long*)((int*)a3 + 6) = *(long*)Unsafe.AsPointer(ref dword_180212028);
        }

        model = (byte*)PsxAddrToPtr(mapper, handle->ModelAddr);
        var skeleton = (byte*)PsxAddrToPtr(mapper, ((PsgModel*)model)->Addr12);
        short* entry = (short*)(skeleton + 4);
        short* child = a3 + 16;
        short* parent = a3 + 16 * skeleton[7];
        short* scratch = (short*)Unsafe.AsPointer(ref qword_180212048);
        do
        {
            GslLoadMatrix((byte*)parent);
            dword_180212050 = *entry;
            GteMulMatrix0((long)parent, child, (ushort*)child);
            entry += 2;
            short* v0 = (short*)Unsafe.AsPointer(ref qword_180211FC0);
            v0[0] = scratch[0];
            v0[1] = scratch[2];
            v0[2] = (short)dword_180212050;
            v0[3] = word_180212054;
            GteRtV0Tr();
            *((int*)child + 5) = dword_180212024;
            *(long*)((int*)child + 6) = *(long*)Unsafe.AsPointer(ref dword_180212028);
            parent = a3 + 16 * ((byte*)entry)[3];
            child += 16;
        }
        while (child < end);

        return end;
    }

    /// <summary>
    /// Copies bone ParentBoneIndex of the parent handle (+48) into the root matrix <paramref name="root"/>, loads its RT
    /// into the GTE and moves its TR by the s16 IR1..3 of RT * V0 (V0 = 8 bytes at handle+160).
    /// </summary>
    private static void PsgBuildBoneMatricesAttach(PsgModelHandle* handle, short* root)
    {
        Recompiler.Runtime.Tracer.Enter("PsgBuildBoneMatricesAttach(handle, root)", (nint)handle, (nint)root);
        var parent = (PsgModelHandle*)handle->field_48;
        var source = (byte*)(parent->field_56 + 32UL * handle->ParentBoneIndex);
        UInt128 rotation = *(UInt128*)source;
        *(UInt128*)root = rotation;
        source = (byte*)(((PsgModelHandle*)handle->field_48)->field_56 + 32UL * handle->ParentBoneIndex);
        UInt128 rest = *(UInt128*)(source + 16);
        *(UInt128*)(root + 8) = rest;

        *(UInt128*)xmmword_180211F40 = *(UInt128*)root;
        dword_180211F50 = (ushort)root[8];
        qword_180211FC0 = *(long*)((byte*)handle + 160);
        GteRtV0();
        int* ir23 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
        dword_180211FE4 = (short)dword_180211FE4;
        ir23[0] = (short)ir23[0];
        ir23[1] = (short)ir23[1];
        int* tr = (int*)root + 5;
        tr[0] += (short)dword_180211FE4;
        tr[1] += ir23[0];
        tr[2] += ir23[1];
    }

    /// <summary>Byte of a 4-bit animation track: the low nibble on even frames, the high nibble on odd frames.</summary>
    private static int PsgBuildBoneMatricesNibble(in byte track, ushort frame)
    {
        Recompiler.Runtime.Tracer.Enter("PsgBuildBoneMatricesNibble(track, frame)", track, frame);
        int value = track;
        if ((frame & 1) != 0)
        {
            value >>= 4;
        }

        return value;
    }
}
