namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Script opcode: on start sends host object message 17 (a5[1] == 0) or 18 (a5[1] == 1) for object a5[0]
    /// (255 = all, 254 = random below the count at +36 of the scene record off_180066C68[3]). Message 18 for object
    /// 16 is skipped in rooms 107/108 unless word_180211E5C is 365. Returns 1.
    /// </summary>
    public static long SfxOpObjMessage(int a1, long a2, long a3, long a4, uint* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpObjMessage(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        if (a1 != 1)
        {
            return 1;
        }

        uint obj = a5[0];
        if (obj == 255)
        {
            obj = 0xFFFFFFFF;
        }
        else if (obj == 254)
        {
            uint* scene = (uint*)off_180066C68;
            dword_1803231DC = unchecked(1103515245 * dword_1803231DC + 12345);
            uint random = (uint)dword_1803231DC >> 16;
            obj = (uint)((int)random % *(sbyte*)(PsxAddrToPtr((long)unk_180576A10, scene[3]) + 36));
        }

        uint message = a5[1];
        if (message == 0)
        {
            HostExecuteVfxForObjects((int)obj);
            return 1;
        }

        if (message == 1
            && (word_180211E5C == 365 || unchecked((ushort)(word_180323172 - 107)) > 1 || obj != 16))
        {
            HostRequestBattleFiguresForObjects((int)obj);
        }

        return 1;
    }

    /// <summary>
    /// Script opcode: on start appends the byte a5[0] to the 3-entry list at 0x18032317A (count byte_18032317D);
    /// 255 clears the dword at 0x18032317A, which also resets the count. Returns 1.
    /// </summary>
    public static long SfxOpPushVariantId(int a1, long a2, long a3, long a4, uint* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpPushVariantId(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        if (a1 == 1)
        {
            uint value = a5[0];
            if (value == 255)
            {
                *(uint*)unk_18032317A = 0;
                return 1;
            }

            byte count = byte_18032317D;
            if (count < 3)
            {
                unk_18032317A[count] = (byte)value;
                byte_18032317D++;
            }
        }

        return 1;
    }

    /// <summary>
    /// Script opcode: on start picks entry a5[0] (a5[1] == 0), the current list id (1), a random list id (2) or the
    /// list id at the scene record's +83 clamped to the count (3) of table (s16 at a4+8) through PsxGetTableEntry,
    /// and starts it as camera block when dword_180323268 is set (null for other a5[1]). Returns 1.
    /// </summary>
    public static long SfxOpStartTrackBlock(int a1, long a2, long a3, long a4, uint* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpStartTrackBlock(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        Shape_SfxOpStartTrackBlock_a4* shape_SfxOpStartTrackBlock_a4 = (Shape_SfxOpStartTrackBlock_a4*)a4;
        if (a1 != 1)
        {
            return 1;
        }

        byte* entry = null;
        byte* list = unk_18032317A;
        int table = shape_SfxOpStartTrackBlock_a4->field_8;
        switch (a5[1])
        {
            case 0:
                entry = PsxGetTableEntry(table, (int)a5[0]);
                break;
            case 1:
                entry = PsxGetTableEntry(table, list[byte_18032317E]);
                break;
            case 2:
                byte count = byte_18032317D;
                if (count != 0)
                {
                    dword_1803231DC = unchecked(1103515245 * dword_1803231DC + 12345);
                    byte_18032317E = (byte)((int)((uint)dword_1803231DC >> 16) % count);
                }

                entry = PsxGetTableEntry(table, list[byte_18032317E]);
                break;
            case 3:
                uint* scene = (uint*)off_180066C68;
                int index = *(sbyte*)(PsxAddrToPtr((long)unk_180576A10, scene[11]) + 83);
                if (byte_18032317D <= index)
                {
                    index = byte_18032317D - 1;
                }

                entry = PsxGetTableEntry(table, list[index]);
                break;
        }

        if (dword_180323268 != 0)
        {
            SfxStartTrackBlock((long)entry, 0);
        }

        return 1;
    }

    /// <summary>
    /// Script opcode: fades the host level (query 0x75000000, set 0x76000000). Phase 0 = state size 8 into *a5;
    /// start stores the current level at a2+4, the delta to the target a5[0] (255 = 128) at a2+6 and the duration
    /// a5+4 at a2, setting the target at once when either is 0; a step sets start + delta * *a4 / duration (below 16
    /// = 0). Returns 1 when finished.
    /// </summary>
    public static long SfxOpHostLevelFade(int a1, long a2, long a3, int* a4, short* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpHostLevelFade(a1, a2, a3, a4, a5)", a1, a2, a3, (nint)a4, (nint)a5);
        Shape_SfxOpHostLevelFade_a5* shape_SfxOpHostLevelFade_a5 = (Shape_SfxOpHostLevelFade_a5*)a5;
        if (a1 == 0)
        {
            shape_SfxOpHostLevelFade_a5->union_0_4 = 8;
            return 0;
        }

        short* state = (short*)a2;
        if (a1 == 1)
        {
            short current = 0;
            if (BattleCallbackAddress != null)
            {
                current = unchecked((short)HostCall((long)BattleCallbackAddress, 0x75000000u, 0, 0, 0));
            }

            state[2] = current;
            ushort target = (ushort)shape_SfxOpHostLevelFade_a5->union_0_2;
            state[3] = unchecked((short)target);
            if (target == 255)
            {
                state[3] = 128;
            }

            state[3] = unchecked((short)(state[3] - current));
            int duration = (int)shape_SfxOpHostLevelFade_a5->field_4;
            *(int*)a2 = duration;
            if (duration != 0 && state[3] != 0)
            {
                return 0;
            }

            if (BattleCallbackAddress != null)
            {
                HostCall((long)BattleCallbackAddress, 0x76000000u, (uint)(state[3] + current), 0, 0);
            }

            return 1;
        }

        int step = *a4;
        int level = state[3] * step / *(int*)a2 + state[2];
        if (level < 16)
        {
            level = 0;
        }

        if (BattleCallbackAddress != null)
        {
            HostCall((long)BattleCallbackAddress, 0x76000000u, (uint)level, 0, 0);
        }

        if (step >= *(int*)a2)
        {
            return 1;
        }

        *a4 = step;
        return 0;
    }

    /// <summary>
    /// Script opcode: fades the motion of object list a5[1] &amp; 0xB from 0 to 128 (flag 4: from 128 to 0) over a5[0]
    /// steps through SetMotionForObjectList. State a2: +0 duration, +4 s16 start, +6 s16 delta, +8 list.
    /// Phase 0 = state size 12 into *a5. Returns 1 when finished.
    /// </summary>
    public static long SfxOpMotionFade(int a1, long a2, long a3, int* a4, int* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpMotionFade(a1, a2, a3, a4, a5)", a1, a2, a3, (nint)a4, (nint)a5);
        Shape_SfxOpMotionFade_a2* shape_SfxOpMotionFade_a2 = (Shape_SfxOpMotionFade_a2*)a2;
        if (a1 == 0)
        {
            *a5 = 12;
            return 0;
        }

        short* range = (short*)(a2 + 4);
        if (a1 == 1)
        {
            shape_SfxOpMotionFade_a2->union_4_4 = (a5[1] & 4) != 0 ? 0xFF800080u : 0x800000u;
            int list = a5[1] & 0xB;
            shape_SfxOpMotionFade_a2->field_8 = (uint)list;
            int duration = a5[0];
            shape_SfxOpMotionFade_a2->field_0 = (uint)duration;
            if (duration == 0)
            {
                SetMeshFadeForObjectList(list, range[1] + range[0]);
                return 1;
            }

            return 0;
        }

        int step = *a4;
        SetMeshFadeForObjectList((int)shape_SfxOpMotionFade_a2->field_8, range[1] * step / (int)shape_SfxOpMotionFade_a2->field_0 + range[0]);
        if (step >= (int)shape_SfxOpMotionFade_a2->field_0)
        {
            return 1;
        }

        *a4 = step;
        return 0;
    }

    /// <summary>Script opcode: on start plays voice a5[0] of bank (s16 at a4+8) with a5[1] (255 = -1). Returns 1.</summary>
    public static long SfxOpVoicePlay(int a1, long a2, long a3, long a4, int* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpVoicePlay(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        if (a1 == 1)
        {
            uint parameter = a5[1] != 255 ? (uint)a5[1] : 0xFFFFFFFF;
            SfxVoicePlay(a5[0], (uint)*(short*)(a4 + 8), parameter);
        }

        return 1;
    }

    /// <summary>
    /// Script opcode: fills the screen tiles of the scene buffer off_180066C68[8] (+4) with level a2[1] through
    /// PsxAddScreenTiles. Start: a5[0] 0 = 255, 1 = 0, 2 = finish at once, else keep; duration a5[1] into a2[0]
    /// (0 = apply once and finish). Each step applies the level and finishes when *a4 reaches the duration.
    /// Phase 0 = state size 12 into *a5.
    /// </summary>
    public static long SfxOpScreenTilesFade(int a1, uint* a2, long a3, int* a4, int* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpScreenTilesFade(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, (nint)a4, (nint)a5);
        if (a1 == 0)
        {
            *a5 = 12;
            return 0;
        }

        uint* scene = (uint*)off_180066C68;
        if (a1 != 1)
        {
            int step = *a4;
            byte* buffer = ResolveGlobalPsxAddress(scene[8]);
            byte level = (byte)a2[1];
            PsxAddScreenTiles((long)(buffer + 4), 255, level, level, level);
            if (step >= (int)a2[0])
            {
                return 1;
            }

            *a4 = step;
            return 0;
        }

        int mode = a5[0];
        if (mode == 2)
        {
            return 1;
        }

        if (mode == 1)
        {
            a2[1] = 0;
        }
        else if (mode == 0)
        {
            a2[1] = 255;
        }

        int duration = a5[1];
        a2[0] = (uint)duration;
        if (duration == 0)
        {
            byte level = (byte)a2[1];
            long buffer = PsxAddrToPtr((long)unk_180576A10, scene[8]);
            PsxAddScreenTiles(buffer + 4, 255, level, level, level);
            return 1;
        }

        return 0;
    }
}
