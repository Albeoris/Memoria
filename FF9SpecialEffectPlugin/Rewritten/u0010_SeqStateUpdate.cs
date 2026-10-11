using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    private const string SeqStateUpdateCppFile = @"..\..\SpecialEffectCode\psx\source\psx_compatibility.cpp";

    /// <summary>
    /// Per-frame step of the sequence state block at 0x180323170 (started by SeqStateBegin). Counts down
    /// dword_18058751C, delivers the pending object notification and the pending 0x1B message, then by state:
    /// 0 asks the host to upload the sequence table, 1 reads the sequence's flags and starts its PSX task (or switches to
    /// table mode), 2 looks up the table record, 3 waits 24 frames, 4 interprets 3-byte script ops (op, a, b) until one
    /// returns: ops &gt;= 32 start PSX tasks, 0..20 are control ops (0 ends the script, 1 waits, 2 sets a flag,
    /// 4/6/7/8/10/11/12/13 wait for conditions by rewinding the op). Unknown ops assert (line 0x312) and continue.
    /// </summary>
    public static void SeqStateUpdate()
    {
        Recompiler.Runtime.Tracer.Enter("SeqStateUpdate()");
        if (dword_18058751C > 0)
        {
            dword_18058751C--;
        }

        byte* pending = (byte*)Unsafe.AsPointer(ref dword_180323192);
        if (pending[0] != 0)
        {
            pending[0] = 0;
            HostNotifyActiveObjects();
        }

        if (*unk_180323194 != 0)
        {
            *unk_180323194 = 0;
            long context = PsxGetContextQword(16);
            if (*(byte*)(context + 16) != 0)
            {
                if (BattleCallbackAddress != null)
                {
                    SeqStateUpdateHost(*(ushort*)(context + 24) | 0x1B000000u, 0, 0, 0, 0, 0);
                }

                HostResetScaleIfFlag1000(context);
                HostFadeMeshIfFlag400(context);
            }
        }

        switch (word_180323170)
        {
            case 0:
                if (BattleCallbackAddress != null)
                {
                    SeqStateUpdateHost(0x65000000, 0x300, 0x140, 0x40, 0x10, (ulong)unk_1802208D0);
                }

                dword_180578E04 = 0;
                word_180323170 = 1;
                return;
            case 1:
                SeqStateUpdateStart();
                return;
            case 2:
                qword_180323198 = (long)PsxFindTableRecord(word_180323172);
                word_180323170 = 4;
                if (*(byte*)(PsxGetContextQword(16) + 16) != 0 && BattleCallbackAddress != null)
                {
                    SeqStateUpdateHost(0x71000000, 0, 0, 0, 0, 0);
                }

                return;
            case 3:
                if (++dword_180587518 >= 24)
                {
                    word_180323170 = 4;
                    *(byte*)(PsxAddrToPtr((long)unk_180576A10, ((uint*)off_180066C68)[11]) + 92) |= 2;
                }

                return;
            case 4:
                SeqStateUpdateRun();
                return;
            default:
                return;
        }
    }

    /// <summary>State 1: flags of sequence word_180323172 from the uploaded table at word_1802208D0.</summary>
    private static void SeqStateUpdateStart()
    {
        Recompiler.Runtime.Tracer.Enter("SeqStateUpdateStart()");
        int flags = (ushort)((short*)unk_1802208D0)[word_180323172];
        dword_18032318C = flags & unchecked((int)0xFFFFFFFE);
        *(ushort*)(PsxAddrToPtr((long)unk_180576A10, ((uint*)off_180066C68)[3]) + 64) &= 0xFFF7;
        if ((flags & 0x8000) != 0)
        {
            if (BattleCallbackAddress != null)
            {
                SeqStateUpdateHost(0x65000000, 0x300, 0x150, 0x40, 0xB0, (ulong)unk_1802208D0);
            }

            dword_18032318C &= unchecked((int)0xFFFFFFF3);
            word_180323170 = 2;
            return;
        }

        byte_180323191 = 0;
        RecomputeGate323268();
        SfxStateReset();
        if ((*(byte*)(PsxAddrToPtr((long)unk_180576A10, ((uint*)off_180066C68)[3]) + 64) & 1) == 0)
        {
            SfxTriggerReaction(dword_18032318C);
        }

        if ((int)PsxChannelReset(word_180323172) != 0)
        {
            return;
        }

        int task = (dword_18032318C >> 4) & 0xF;
        if (task == 1)
        {
            *(ushort*)(PsxAddrToPtr((long)unk_180576A10, ((uint*)off_180066C68)[3]) + 64) |= 8;
        }

        StartPsxTask7(task);
        word_180323170 = 3;
        dword_180587518 = 0;
        qword_180323198 = (long)unk_180320CD0;
    }

    /// <summary>State 4: the script interpreter (3-byte ops at qword_180323198, wait counter in the low word of dword_180323174).</summary>
    private static void SeqStateUpdateRun()
    {
        Recompiler.Runtime.Tracer.Enter("SeqStateUpdateRun()");
        short* wait = (short*)Unsafe.AsPointer(ref dword_180323174);
        byte* flags = (byte*)Unsafe.AsPointer(ref dword_180323180);
        while (true)
        {
            if (*wait != 0)
            {
                short left = (short)(*wait - 1);
                *wait = left;
                if (left > 0)
                {
                    return;
                }

                *wait = 0;
            }

            byte* op = (byte*)qword_180323198;
            short code = op[0];
            short a = op[1];
            short b = op[2];
            qword_180323198 += 3;
            if (code >= 32)
            {
                int task = code < 128 ? code - 32 : code;
                StartPsxTask(task, word_180323178, a, b);
                continue;
            }

            switch (code)
            {
                case 0:
                    SeqStateUpdateEnd();
                    return;
                case 1:
                    if (a == 0)
                    {
                        *wait = b;
                        return;
                    }

                    if (flags[b] != 0)
                    {
                        continue;
                    }

                    qword_180323198 -= 3;
                    flags[b] = 0;
                    return;
                case 2:
                    flags[a] = (byte)b;
                    continue;
                case 3:
                    if ((int)PsxChannelFlip() != 0)
                    {
                        SeqStateUpdateAssert();
                    }

                    continue;
                case 4:
                    if (dword_18058751C == 0)
                    {
                        continue;
                    }

                    byte_180323190 = 1;
                    qword_180323198 -= 3;
                    return;
                case 5:
                    *(short*)unk_180323176 = a;
                    word_180323178 = (short)FindPairSlot(a);
                    continue;
                case 6:
                    {
                        long obj = PsxAddrToPtr((long)unk_180576A10, ((uint*)off_180066C68)[1]);
                        Obj* obj2 = (Obj*)obj;
                        bool ready = a switch
                        {
                            0 => obj2->field_80 == 2,
                            1 => obj2->field_82 == 2,
                            2 => obj2->field_83 != 0,
                            _ => true,
                        };
                        if (ready)
                        {
                            continue;
                        }

                        SeqStateUpdateBlock();
                        return;
                    }
                case 7:
                    if (a == 1)
                    {
                        SeqStateUpdateUploadPages(b);
                    }
                    else if (a == 2 && (short)*(byte*)(PsxAddrToPtr((long)unk_180576A10, ((uint*)off_180066C68)[1]) + 105) < b)
                    {
                        SeqStateUpdateBlock();
                        return;
                    }

                    continue;
                case 8:
                    if (*(byte*)(PsxGetContextQword(16) + 16) != 0)
                    {
                        continue;
                    }

                    *unk_180323193 = 1;
                    if ((*(byte*)(PsxAddrToPtr((long)unk_180576A10, ((uint*)off_180066C68)[3]) + 64) & 0x10) != 0 || byte_18032317F != 0)
                    {
                        continue;
                    }

                    qword_180323198 -= 3;
                    return;
                case 9:
                    if (BattleCallbackAddress != null)
                    {
                        SeqStateUpdateHost(0x20000000, 3, (uint)(int)b, 0, 0, 0);
                    }

                    continue;
                case 10:
                    {
                        short* toggle = (short*)Unsafe.AsPointer(ref dword_1803231FC);
                        if (*toggle == 0)
                        {
                            *toggle = 0;
                            *(short*)Unsafe.AsPointer(ref dword_180323256) = 257;
                            continue;
                        }

                        *toggle = 1;
                        SeqStateUpdateBlock();
                        return;
                    }
                case 11:
                    if (a == 0)
                    {
                        *(byte*)Unsafe.AsPointer(ref dword_180323192) = 1;
                        return;
                    }

                    if (*(byte*)Unsafe.AsPointer(ref dword_180323192) != 0)
                    {
                        qword_180323198 -= 3;
                    }

                    return;
                case 12:
                    if (*(byte*)(PsxAddrToPtr((long)unk_180576A10, ((uint*)off_180066C68)[1]) + 104) == 2)
                    {
                        continue;
                    }

                    qword_180323198 -= 3;
                    return;
                case 13:
                    if (a == 0)
                    {
                        *unk_180323194 = (byte)(*(byte*)(PsxGetContextQword(b) + 18) + 1);
                        return;
                    }

                    if (*unk_180323194 != 0)
                    {
                        qword_180323198 -= 3;
                    }

                    return;
                case 14:
                    if (dword_180578E04 != 0)
                    {
                        dword_180578E04 = 0;
                        qword_180323198 = qword_180578E08;
                    }

                    continue;
                case 15:
                    if (b != 0)
                    {
                        if (BattleCallbackAddress != null)
                        {
                            SeqStateUpdateHost(0x77000000, 2, 0, 0, 0, 0);
                        }
                    }
                    else
                    {
                        SeqStateUpdateSendTable(a);
                    }

                    continue;
                case 20:
                    dword_18058751C = a;
                    continue;
                default:
                    SeqStateUpdateAssert();
                    continue;
            }
        }
    }

    /// <summary>Op 0: rewinds to itself, polls the host (0x73, then 0x72 when it answers 0) and ends or restarts the sequence.</summary>
    private static void SeqStateUpdateEnd()
    {
        Recompiler.Runtime.Tracer.Enter("SeqStateUpdateEnd()");
        qword_180323198 -= 3;
        if (BattleCallbackAddress != null && (uint)SeqStateUpdateHost(0x73000000, 0, 0, 0, 0, 0) == 0 && BattleCallbackAddress != null)
        {
            SeqStateUpdateHost(0x72000000, 1, 0, 0, 0, 0);
        }

        if (dword_180323270 != 0)
        {
            return;
        }

        if ((dword_18032318C & 1) != 0)
        {
            if (byte_180323191 != 0)
            {
                if ((int)SfxVramPagesUploadOrRestore(1) != 0)
                {
                    dword_18032318C &= ~1;
                }

                return;
            }

            SfxBeginBlock();
            SfxPoolsReset();
            if (BattleCallbackAddress != null)
            {
                SeqStateUpdateHost(0x20000000, 2, 0, 63, 0, 0);
            }

            SeqStateUpdateClearSlots();
            System.Buffer.MemoryCopy(unk_18004F860, unk_1802208D0, 528 * 128, 528 * 128);
            byte_180323191 = 1;
            return;
        }

        word_180323170 = -1;
        ((uint*)off_180066C68)[3] = 0;
        SfxBeginBlock();
        SfxPoolsReset();
        long callback = (long)BattleCallbackAddress;
        if (callback != 0)
        {
            SeqStateUpdateHost(0x20000000, 2, 0, 63, 0, 0);
            callback = (long)BattleCallbackAddress;
        }

        SeqStateUpdateClearSlots();
        if (callback != 0)
        {
            Native.InvokeBattleCallback(callback, HostCommand.Vibrate, 0, 0, 0, 0, 0);
        }
    }

    private static void SeqStateUpdateClearSlots()
    {
        Recompiler.Runtime.Tracer.Enter("SeqStateUpdateClearSlots()");
        dword_1803231A0 = 0;
        word_1803231A4 = 0;
        word_1803231AC = 0;
        word_1803231B4 = 0;
        word_1803231BC = 0;
        word_1803231C4 = 0;
        word_1803231CC = 0;
    }

    /// <summary>Op 7/1: uploads the 0x8000-byte pages below table entry (-1, <paramref name="b"/>) selected by its page mask.</summary>
    private static void SeqStateUpdateUploadPages(short b)
    {
        Recompiler.Runtime.Tracer.Enter("SeqStateUpdateUploadPages(b)", b);
        byte* page = PsxGetTableEntry(-1, b);
        int mask = 1 << *(int*)(page + 32764);
        int bit = 1;
        for (int k = 0; k < 4; k++)
        {
            if ((mask & bit) != 0)
            {
                page -= 0x8000;
                if (BattleCallbackAddress != null)
                {
                    short x = (short)(((k & 0xFFFE) + 28) << 5);
                    short y = (short)((k & 1) << 7);
                    SeqStateUpdateHost(0x64000000, (uint)(int)x, (uint)(int)y, 64, 128, (ulong)page);
                }
            }

            if ((k & 1) != 0)
            {
                bit *= 2;
            }
        }
    }

    /// <summary>Op 15 with b == 0: copies 450 dwords of table entry (word_180323178, a) to unk_180586E10 and hands them to the host.</summary>
    private static void SeqStateUpdateSendTable(short a)
    {
        Recompiler.Runtime.Tracer.Enter("SeqStateUpdateSendTable(a)", a);
        int slot = word_180323178;
        int* source = (int*)PsxGetTableEntry(slot, a);
        int* target = (int*)unk_180586E10;
        for (int i = 0; i < 450; i++)
        {
            target[i] = source[i];
        }

        byte* second = PsxGetTableEntry(slot, 1);
        if ((int)second - (int)PsxGetTableEntry(slot, 0) > 1800)
        {
            SeqStateUpdateAssert();
        }

        if (BattleCallbackAddress != null)
        {
            SeqStateUpdateHost(0x77000000, 1, 0, 0, 0, (ulong)unk_180586E10);
        }
    }

    /// <summary>Sets byte_180323190 and rewinds to the current op (the op is retried next frame).</summary>
    private static void SeqStateUpdateBlock()
    {
        Recompiler.Runtime.Tracer.Enter("SeqStateUpdateBlock()");
        byte_180323190 = 1;
        qword_180323198 -= 3;
    }

    /// <summary>_wassert(L"0", psx_compatibility.cpp, 0x312); the original continues the script after it returns.</summary>
    private static void SeqStateUpdateAssert()
    {
        Recompiler.Runtime.Tracer.Enter("SeqStateUpdateAssert()");
        GeoWassert("0", SeqStateUpdateCppFile, 0x312);
    }

    private static long SeqStateUpdateHost(ulong message, ulong a, ulong b, ulong c, uint d, ulong e)
    {
        Recompiler.Runtime.Tracer.Enter("SeqStateUpdateHost(message, a, b, c, d, e)", message, a, b, c, d, e);
        return Native.InvokeBattleCallback((long)(BattleCallbackFn)BattleCallbackAddress, message, a, b, c, d, e);
    }
}
