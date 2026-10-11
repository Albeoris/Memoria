namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Effect script handler that spawns up to four trail effects, one per list object. Phase <paramref name="a1"/>:
    /// 0 stores the state size 80 in <paramref name="a5"/>; 1 resets the state <paramref name="a2"/> (+0 index, +4 timer;
    /// when byte +39 of the block at PSX address a3 + 12 is 0: timer 0xFFFF, index 12 if byte +16 of context object 16
    /// is set else 0), starts the idle track, clears byte 0x180323182 and the four slot pointers at +48. Otherwise:
    /// timer 0xFFFF counts the index down (finished: byte 0x180323182 = 1, returns 1); a running timer advances, resets
    /// after 4 frames (or 14 while the index is negative; negative = finished, also sets byte 0x180323195), then copies
    /// the u16 triple at +40 into +92/+94/+96 of every live slot and drops slots whose +48 is 0xFFFF; timer 0 queries
    /// the pose of list object <c>index</c> (into +8 + 8 * index and +40) and either ends the list (index -1) or spawns
    /// the effect at table offset +12 (kind 115, sub-blocks +28 and +24 at +28/+68), sets +40 = 1024, +42 += 2048 and
    /// allocates voices (3, index + 8). *<paramref name="a4"/> is written back unchanged on the paths returning 0.
    /// </summary>
    public static long SfxTrailSequenceHandler(int a1, int* a2, long a3, int* a4, uint* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxTrailSequenceHandler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, (nint)a4, (nint)a5);
        Slots* shape_SfxTrailSequenceHandler_a2 = (Slots*)a2;
        Shape_SfxTrailSequenceHandler_a3* shape_SfxTrailSequenceHandler_a3 = (Shape_SfxTrailSequenceHandler_a3*)a3;
        if (a1 == 0)
        {
            *a5 = 80;
            return 0;
        }

        if (a1 == 1)
        {
            shape_SfxTrailSequenceHandler_a2->union_0_8 = 0;
            byte* block = ResolveGlobalPsxAddress(shape_SfxTrailSequenceHandler_a3->field_12);
            if (block[39] == 0)
            {
                a2[1] = 0xFFFF;
                a2[0] = *(byte*)(PsxGetContextQword(16) + 16) != 0 ? 12 : 0;
            }

            SfxStartIdleTrack();
            *unk_180323182 = 0;
            shape_SfxTrailSequenceHandler_a2->field_48[0] = 0;
            shape_SfxTrailSequenceHandler_a2->field_56 = 0;
            shape_SfxTrailSequenceHandler_a2->field_64 = 0;
            shape_SfxTrailSequenceHandler_a2->field_72 = 0;
            return 0;
        }

        int timer = a2[1];
        int saved = *a4;
        if (timer == 0xFFFF)
        {
            if (--a2[0] < 0)
            {
                *unk_180323182 = 1;
                return 1;
            }
        }
        else if (timer != 0)
        {
            int next = timer + 1;
            a2[1] = next;
            if ((next > 4 && a2[0] >= 0) || (next > 14 && a2[0] < 0))
            {
                a2[1] = 0;
                if (a2[0] < 0)
                {
                    *unk_180323182 = 1;
                    *unk_180323195 = 1;
                    return 1;
                }
            }

            long* slots = (long*)(a2 + 12);
            for (int i = 0; i < 4; i++)
            {
                if (slots[i] != 0)
                {
                    *(ushort*)(slots[i] + 92) = *((ushort*)a2 + 20);
                    *(ushort*)(slots[i] + 94) = *((ushort*)a2 + 21);
                    *(ushort*)(slots[i] + 96) = *((ushort*)a2 + 22);
                    if (*(ushort*)(slots[i] + 48) == 0xFFFF)
                    {
                        slots[i] = 0;
                    }
                }
            }
        }
        else if ((uint)SfxQueryListObjPose(a2[0], (long)(a2 + 2 + (2 * a2[0])), (long)(a2 + 10)) != 0)
        {
            a2[0] = -1;
            a2[1] = 1;
        }
        else
        {
            long table = qword_1803231E0;
            Table2* table2 = (Table2*)table;
            long* slots = (long*)(a2 + 12);
            slots[a2[0]] = CreateTrailEffect(table + table2->field_12, (long)(a2 + (2 * (a2[0] + 1))));
            if (slots[a2[0]] != 0)
            {
                *(ushort*)(slots[a2[0]] + 34) = 115;
            }

            SfxTrailAttachSubBlock(slots, a2[0], 28, 28);
            SfxTrailAttachSubBlock(slots, a2[0], 24, 68);
            int index = a2[0];
            *((ushort*)a2 + 20) = 1024;
            *((ushort*)a2 + 21) += 2048;
            a2[1] = 1;
            SfxAllocVoices(3, (uint)(index + 8));
            a2[0]++;
        }

        *a4 = saved;
        return 0;
    }

    /// <summary>
    /// Spawns trail effect kind <paramref name="a2"/> at the 8-byte position <paramref name="a3"/> (copied to
    /// <paramref name="a4"/> first) with the effect table qword_1803231E0: 0 / 2 use the effect at +20 with sub-block
    /// +28 / +32 (kind 8, voice set 1 when context object 16 has byte +16 set and byte +32 == 10, else 0); 1 / 7 move
    /// a4's y up by 64 and use +4 with sub-blocks +80, +24 and +8 (at +108), target a4 (kind 32, voice set 2); 4 uses
    /// +16 with +28 (kind 128, voice set 0). The sub-block +24 always goes to +68. Voices are allocated for
    /// <paramref name="a1"/>. Returns the slot (0 when the pool is full or for other kinds).
    /// </summary>
    public static long SfxSpawnTrailEffect(uint a1, int a2, uint* a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("SfxSpawnTrailEffect(a1, a2, a3, a4)", a1, a2, (nint)a3, a4);
        *(uint*)a4 = a3[0];
        *(uint*)(a4 + 4) = a3[1];
        long table;
        long slot;
        switch (a2)
        {
            case 0:
            case 2:
                table = qword_1803231E0;
                slot = CreateTrailEffect(table + ((Table2*)table)->field_20, (long)a3);
                if (slot != 0)
                {
                    table = qword_1803231E0;
                    int first = a2 != 0 ? 32 : 28;
                    SfxTrailWriteSubBlocks(slot, table + *(int*)(table + first));
                    *(ushort*)(slot + 34) = 8;
                }

                long context = PsxGetContextQword(16);
                uint voices = *(byte*)(context + 16) != 0 && *(byte*)(context + 32) == 10 ? 1u : 0u;
                SfxAllocVoices(voices, a1);
                return slot;

            case 1:
            case 7:
                table = qword_1803231E0;
                *(ushort*)(a4 + 2) += 0xFFC0;
                slot = CreateTrailEffect(table + ((Table2*)table)->field_4, (long)a3);
                if (slot != 0)
                {
                    table = qword_1803231E0;
                    SfxTrailWriteSubBlocks(slot, table + ((Table2*)table)->field_80);
                    table = qword_1803231E0;
                    uint extra = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)(table + ((Table2*)table)->field_8));
                    *(uint*)(PsxAddrToPtr((long)unk_180576A10, *(uint*)(slot + 24)) + 108) = extra;
                    *(uint*)(slot + 44) = (uint)PsxAddrMapperRegister((long)unk_180576A10, a4);
                    *(ushort*)(slot + 34) = 32;
                }

                SfxAllocVoices(2, a1);
                return slot;

            case 4:
                table = qword_1803231E0;
                slot = CreateTrailEffect(table + ((Table2*)table)->field_16, (long)a3);
                if (slot != 0)
                {
                    table = qword_1803231E0;
                    SfxTrailWriteSubBlocks(slot, table + ((Table2*)table)->field_28);
                    *(ushort*)(slot + 34) = 128;
                }

                SfxAllocVoices(0, a1);
                return slot;

            default:
                return 0;
        }
    }

    /// <summary>
    /// Re-checks slot <paramref name="index"/> and stores the PSX address of the effect table entry at
    /// <paramref name="tableOffset"/> into field <paramref name="field"/> of the slot's part table (+24).
    /// </summary>
    private static void SfxTrailAttachSubBlock(long* slots, int index, int tableOffset, int field)
    {
        Recompiler.Runtime.Tracer.Enter("SfxTrailAttachSubBlock(slots, index, tableOffset, field)", (nint)slots, index, tableOffset, field);
        if (slots[index] == 0)
        {
            return;
        }

        long table = qword_1803231E0;
        uint address = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)(table + *(int*)(table + tableOffset)));
        *(uint*)(PsxAddrToPtr((long)unk_180576A10, *(uint*)(slots[index] + 24)) + field) = address;
    }

    /// <summary>
    /// Stores <paramref name="first"/> at +28 and the effect table entry +24 at +68 of the slot's part table (+24),
    /// mapping each address right before its store, as the original.
    /// </summary>
    private static void SfxTrailWriteSubBlocks(long slot, long first)
    {
        Recompiler.Runtime.Tracer.Enter("SfxTrailWriteSubBlocks(slot, first)", slot, first);
        uint firstAddress = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)first);
        *(uint*)(PsxAddrToPtr((long)unk_180576A10, *(uint*)(slot + 24)) + 28) = firstAddress;
        long table = qword_1803231E0;
        Table2* table2 = (Table2*)table;
        uint second = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)(table + table2->field_24));
        *(uint*)(PsxAddrToPtr((long)unk_180576A10, *(uint*)(slot + 24)) + 68) = second;
    }
}
