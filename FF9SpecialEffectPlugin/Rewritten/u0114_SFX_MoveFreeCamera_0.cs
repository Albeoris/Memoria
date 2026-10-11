using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// Free camera (MATRIX at 0x180211E28: 3x3 s16 rotation, pad, s32 translation at +20/+24/+28, extra s16 at +32),
/// plunge-camera block loading, battle object slots (8 HostObject records at 0x1800694F8) and actor resolution.
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>
    /// Only while word_180323170 is -1 or dword_180211E24 is 1: Rotate builds R = Rx(y) * Ry(x) * Rz(0) and, through the
    /// GTE, replaces the camera rotation by R * rotation and its translation by TR + R * translation (clamped to s16); mode 1 adds
    /// x/y to the X/Y translation; TranslateZ adds y to the Z translation.
    /// </summary>
    public static void SFX_MoveFreeCamera_0(int type, int x, int y)
    {
        Recompiler.Runtime.Tracer.Enter("SFX_MoveFreeCamera_0(type, x, y)", type, x, y);
        if (word_180323170 != -1 && dword_180211E24 != 1)
        {
            return;
        }

        if (type == (int)FreeCameraOperation.TranslateXY)
        {
            dword_180211E50 += x;
            dword_180211E54 += y;
            return;
        }

        if (type == (int)FreeCameraOperation.TranslateZ)
        {
            dword_180211E58 += y;
            return;
        }

        if (type != (int)FreeCameraOperation.Rotate)
        {
            return;
        }

        // Local MATRIX: rotation identity; the translation is never written by the original (stack garbage there).
        PSXMAT m = default;
        m.r[0] = 4096;
        m.r[4] = 4096;
        m.r[8] = 4096;
        long p = RotMatrixZ(0, (short*)&m);
        p = RotMatrixY((short)x, (short*)p);
        RotMatrixX((short)y, (short*)p);

        // SetRotMatrix(m)
        dword_180211F50 = (ushort)m.r[8];
        System.Buffer.MemoryCopy(&m, xmmword_180211F40, 16, 16);

        // product = RT * camera rotation, one column at a time through IR1..3.
        short* cam = (short*)xmmword_180211E28;
        int* ir23 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
        Span<short> product = stackalloc short[10];
        short* productAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(product));
        new Span<short>(productAddress, 10).Clear();
        for (int col = 0; col < 3; col++)
        {
            dword_180211FE4 = cam[col];
            ir23[0] = cam[col + 3];
            ir23[1] = cam[col + 6];
            GteRtIr();
            product[col] = (short)dword_180211FE4;
            product[col + 3] = (short)ir23[0];
            product[col + 6] = (short)ir23[1];
        }

        dword_180211FE4 = (short)dword_180211FE4;
        ir23[0] = (short)ir23[0];
        ir23[1] = (short)ir23[1];

        // SetTransMatrix(m): the local translation (uninitialized in the original, zero here).
        *(int*)Unsafe.AsPointer(ref qword_180211F54) = m.t[0];
        *(long*)unk_180211F58 = *(long*)&m.t[1];

        // V0 = low halves of the camera translation plus the s16 at +32.
        short* v0 = (short*)Unsafe.AsPointer(ref qword_180211FC0);
        v0[0] = cam[10];
        v0[1] = cam[12];
        v0[2] = cam[14];
        v0[3] = (short)FreeCameraProjectionDistance;
        GteRtV0Tr();

        System.Buffer.MemoryCopy(productAddress, cam, 16, 16);
        dword_180211FE4 = (short)dword_180211FE4;
        ir23[0] = (short)ir23[0];
        ir23[1] = (short)ir23[1];
        *(int*)(xmmword_180211E38 + 4) = dword_180211FE4;
        *(int*)(xmmword_180211E38 + 8) = ir23[0];
        *(int*)(xmmword_180211E38 + 12) = ir23[1];

        // m[2][2] and the pad of the product MATRIX (pad uninitialized in the original, zero here).
        *(ushort*)xmmword_180211E38 = (ushort)product[8];
        *(ushort*)(xmmword_180211E38 + 2) = (ushort)product[9];
    }

    /// <summary>
    /// Stores a2 in dword_180211E4C, clears the block state (bytes 0x180220119/0x1802200B2/0x1802200BC, 0x68 bytes at
    /// 0x18021FF00, 0xC0 bytes at 0x18021FFA0), selects the variant of block a1 and, when the object at
    /// qword_180220130 exists, refreshes its vector into 0x18021FF4A (returning that result instead).
    /// </summary>
    public static long SfxResetAndSelectBlock(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("SfxResetAndSelectBlock(a1, a2)", a1, a2);
        dword_180211E4C = a2;
        byte_180220119 = 0;
        byte_1802200B2 = 0;
        byte_1802200BC = 0;
        new Span<byte>(Unsafe.AsPointer(ref qword_18021FF00), 0x68).Clear();
        new Span<byte>(unk_18021FFA0, 0xC0).Clear();
        long result = SfxSelectBlockVariant((short*)a1);
        if (qword_180220130 != 0)
        {
            return HostQueryObjectVector(qword_180220130, (ushort*)unk_18021FF4A);
        }

        return result;
    }

    /// <summary>
    /// Copies the camera block from btlseq + camOffset through btlseqLen, then starts its first track with projOffset.
    /// A null btlseq reuses the previously loaded block. Evidence: Memoria's SFX_StartPlungeCamera declaration.
    /// </summary>
    public static void SFX_StartPlungeCamera_0(byte* btlseq, int btlseqLen, int camOffset, int projOffset)
    {
        Recompiler.Runtime.Tracer.Enter("SFX_StartPlungeCamera_0(btlseq, btlseqLen, camOffset, projOffset)", (nint)btlseq, btlseqLen, camOffset, projOffset);
        long offset = camOffset;
        short* data;
        if (btlseq != null)
        {
            if (Block != null)
            {
                NativeMemory.Free(Block);
            }

            int size = btlseqLen - (int)offset;
            data = (short*)NativeMemory.Alloc((nuint)(long)size);
            Block = data;
            Recompiler.Runtime.Crt.Memcpy(data, (void*)(btlseq + offset), (nuint)(long)size);
        }
        else
        {
            data = (short*)Block;
        }

        long start = data != null ? (long)((byte*)data + *data) : 0;
        SfxResetAndSelectBlock(start, projOffset);
    }

    /// <summary>
    /// Battle set-up from SFX_INIT_PARAM: fixed-camera bits, battle placement and eight BTL_DATA_INIT records copied
    /// into HostObject slots in BattleObjects (linked through Next, head in BattleObjectList, aux 8-byte entries
    /// at 0x180069760). Enemy slots (bi_player == 0) take the next effect
    /// pointer of Block (offset table from Block[5]). The Next of the last counted slot (HostId != 0) is cleared;
    /// Memoria passes a 300-byte record; the camera matrix, translation and projection tail is unused by this DLL.
    /// </summary>
    public static void SFX_InitBattle_0(SFX_INIT_PARAM* param)
    {
        Recompiler.Runtime.Tracer.Enter("SFX_InitBattle_0(param)", (nint)param);
        short* block = (short*)Block;
        byte* aux = unk_180069760;
        BTL_DATA_INIT* src = &param->btl_data_init0;
        var slot = (HostObject*)BattleObjects;

        dword_180069718 = (dword_180069718 & ~0x800) | ((param->btl_scene_FixedCamera1 & 1) << 11);
        dword_180069718 = (dword_180069718 & ~0x1000) | ((param->btl_scene_FixedCamera2 & 1) << 12);
        word_18006971C = param->btl_scene_patAddr_putX;
        word_18006971E = param->btl_scene_patAddr_putZ;
        BattleObjectList = (long)BattleObjects;

        int count = 0;
        int next = 5;
        for (int k = 0; k < 8; k++, slot++, aux += 8, src++)
        {
            new Span<byte>((byte*)slot + 16, 48).Clear();
            slot->Next = (byte*)slot + sizeof(HostObject);
            slot->Aux = aux;
            *(ulong*)aux = 0;
            slot->HostId = src->btl_id;
            slot->tar_bone = src->tar_bone;
            slot->bi_player = src->bi_player;
            slot->bi_slot_no = src->bi_slot_no;
            slot->bi_line_no = src->bi_line_no;
            slot->geo_radius = (uint)src->geo_radius;
            slot->geo_height = src->geo_height;
            slot->player_serial_no = src->player_serial_no;
            slot->player_equip = src->player_equip;
            slot->player_wep_bone = src->player_wep_bone;
            aux[0] = src->weapon_category;
            *(short*)(slot->Aux + 4) = src->weapon_offset0;
            *(short*)(slot->Aux + 6) = src->weapon_offset1;
            slot->enemy_cam_bone[0] = src->enemy_cam_bone0;
            slot->enemy_cam_bone[1] = src->enemy_cam_bone1;
            slot->enemy_cam_bone[2] = src->enemy_cam_bone2;
            slot->enemy_radius = (uint)src->enemy_radius;
            if (block != null && src->bi_player == 0)
            {
                slot->SfxData = (byte*)block + block[next++];
            }

            if (slot->HostId != 0)
            {
                count++;
            }
        }

        long result = (long)count << 6;

        // With no counted slot this clears the 8 bytes 64 bytes before the table, as the original does.
        *(long*)(BattleObjects + result - 64) = 0;
    }

    /// <summary>
    /// Copies the S_Eff_Req_Org flags, argument, monster bones and target centre into scene globals, then resolves
    /// exe (caster), mexe (magic-sword caster, falling back to exe), trg0..7 and rtrg0..3 to HostObject slots.
    /// Unknown ids resolve to slot 0. Returns the last count used as a loop bound, else the main target pointer.
    /// </summary>
    public static long SfxResolveActors(S_Eff_Req_Org* req)
    {
        Recompiler.Runtime.Tracer.Enter("SfxResolveActors(req)", (nint)req);
        EffectRequestFlags = (short)req->flgs;
        EffectRequestArgument = req->arg0;
        MonsterBone0 = req->monbone0;
        MonsterBone1 = req->monbone1;
        EffectTargetCenter = *(long*)&req->trgcpos_x;
        EffectTargetCount = (byte)req->trgno;
        EffectReflectedTargetCount = (byte)req->rtrgno;

        if (req->exe.btl_id != 0)
        {
            EffectCaster = (long)SfxResolveActorsFindSlot(req->exe.btl_id);
        }

        long result = (long)SfxResolveActorsFindSlot(req->mexe.btl_id != 0 ? req->mexe.btl_id : req->exe.btl_id);
        EffectMagicCaster = result;

        long* targets = (long*)EffectTargets;
        BTL_DATA_REQ* targetRequests = &req->trg0;
        for (int i = 0; i < req->trgno; i++)
        {
            targets[i] = (long)SfxResolveActorsFindSlot(targetRequests[i].btl_id);
            result = (uint)(int)req->trgno;
        }

        long* extra = (long*)ReflectedEffectTargets;
        BTL_DATA_REQ* reflectedRequests = &req->rtrg0;
        for (int i = 0; i < req->rtrgno; i++)
        {
            extra[i] = (long)SfxResolveActorsFindSlot(reflectedRequests[i].btl_id);
            result = (uint)(int)req->rtrgno;
        }

        return result;
    }

    /// <summary>First of the 8 slots at 0x1800694F8 whose HostId is id; slot 0 when none matches.</summary>
    private static HostObject* SfxResolveActorsFindSlot(ushort id)
    {
        Recompiler.Runtime.Tracer.Enter("SfxResolveActorsFindSlot(id)", id);
        var table = (HostObject*)BattleObjects;
        var end = (HostObject*)Unsafe.AsPointer(ref CameraCaster);
        HostObject* p = table;
        while (p->HostId != id)
        {
            p++;
            if (p >= end)
            {
                return table;
            }
        }

        return p;
    }
}
