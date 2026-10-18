namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Effect script handler for an object strike. Phase <paramref name="a1"/> 0 stores the state size 48 in
    /// <paramref name="a5"/>; 1 initialises the state <paramref name="a2"/> (see SfxObjectStrikeSequenceInit); other
    /// phases run the step a2[1] with the frame counter *<paramref name="a4"/>: 0 approach (SfxObjStepToFacingPoint),
    /// 1 wind-up facing the target, then spawn the trail effect, 2 hold / screen fade, 3 recover. A step change resets
    /// the counter to -1. Returns 1 when the sequence has ended, else stores the counter, increments the u16 tick at +6
    /// and returns 0. State: +0 style, +1 step, +2 object id, +3 target id, +4 fade mode, +5 variant, +6 u16 tick,
    /// +8 hold period, +12 position, +20 position copy, +28 facing, +40 effect slot.
    /// </summary>
    public static long SfxObjectStrikeSequence(int a1, byte* a2, uint* a3, int* a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjectStrikeSequence(a1, a2, a3, a4, a5)", a1, (nint)a2, (nint)a3, (nint)a4, (nint)a5);
        Shape_SfxObjectStrikeSequence_a5* shape_SfxObjectStrikeSequence_a5 = (Shape_SfxObjectStrikeSequence_a5*)a5;
        if (a1 == 0)
        {
            shape_SfxObjectStrikeSequence_a5->union_0_4 = 48;
            return 0;
        }

        if (a1 == 1)
        {
            SfxObjectStrikeSequenceInit(a2, a3, a5);
            return 0;
        }

        int frame = *a4;
        switch (a2[1])
        {
            case 0:
                if ((uint)SfxObjStepToFacingPoint(a2[2], a2[3], frame) != 0)
                {
                    if (a2[5] == 3)
                    {
                        HostObjCmd0CIfFlag(a2[2], 0x1F);
                        a2[1] = 3;
                        HostCmd71IfObj16Flag();
                    }
                    else
                    {
                        HostObjCmd0CIfFlag(a2[2], 0x1A);
                        a2[1] = 1;
                    }

                    frame = -1;
                }

                break;

            case 1:
                frame = SfxObjectStrikeSequenceWindUp(a2, frame);
                break;

            case 2:
                if (SfxObjectStrikeSequenceHold(a2, a3, ref frame))
                {
                    return 1;
                }

                break;

            case 3:
                if (SfxObjectStrikeSequenceRecover(a2, a3, frame))
                {
                    return 1;
                }

                break;
        }

        *a4 = frame;
        (*(ushort*)(a2 + 6))++;
        return 0;
    }

    /// <summary>
    /// Phase 1: step 0, tick 0, object a5[4] &amp; 15 + 16, target a5[0] (+8 when the block at a3[3] has byte +39 == 1,
    /// 0xFF when it is another non-zero value), host message 0x72000000 when bit 3 of byte +28 of the block at a3[0] is
    /// set, flags 0x1011 at +2 for bit 2 of the a3[3] block's +64, style (a5 dword +4 &gt;&gt; 4) &amp; 15, fade mode
    /// (dword_18032318C &gt;&gt; 12) &amp; 3, hold period 100; clears the u16 at 0x180323182. Variant: 2 (u16 +64 bit 0 or
    /// style 15) / 3 or 0 (by bit 1 of +64) while the object's byte +16 is set; otherwise 1 with step 2, spawning trail
    /// kind 4 at the object's anchor unless bit 0 / style 15.
    /// </summary>
    private static void SfxObjectStrikeSequenceInit(byte* a2, uint* a3, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjectStrikeSequenceInit(a2, a3, a5)", (nint)a2, (nint)a3, (nint)a5);
        Shape_SfxObjectStrikeSequence_a5* shape_SfxObjectStrikeSequence_a5 = (Shape_SfxObjectStrikeSequence_a5*)a5;
        a2[1] = 0;
        *(ushort*)(a2 + 6) = 0;
        a2[2] = (byte)((a5[4] & 0xF) + 16);
        *(long*)(a2 + 40) = 0;
        a2[3] = a5[0];
        if ((ResolveGlobalPsxAddress(a3[0])[28] & 8) != 0)
        {
            var callback = (BattleCallbackFn)BattleCallbackAddress;
            if (callback != null)
            {
                Native.InvokeBattleCallback((long)callback, HostCommand.SetCursor, 0, 0, 0, 0, 0);
            }
        }

        if (ResolveGlobalPsxAddress(a3[3])[39] != 0 && a2[3] != 0xFF)
        {
            a2[3] = ResolveGlobalPsxAddress(a3[3])[39] == 1 ? (byte)(a2[3] + 8) : (byte)0xFF;
        }

        if ((ResolveGlobalPsxAddress(a3[3])[64] & 4) != 0)
        {
            *(ushort*)(a2 + 2) = 0x1011;
        }

        a2[0] = (byte)((shape_SfxObjectStrikeSequence_a5->union_4_4 >> 4) & 0xF);
        a2[4] = (byte)(((uint)dword_18032318C >> 12) & 3);
        *(ushort*)unk_180323182 = 0;
        int flags = *(ushort*)(ResolveGlobalPsxAddress(a3[3]) + 64);
        bool inactive = *(byte*)(PsxGetContextQword(a2[2]) + 16) == 0;
        *(int*)(a2 + 8) = 100;
        HostObjCmd1C(a2[2], 1, 0);
        bool still = (flags & 1) != 0 || a2[0] == 15;
        if (!inactive)
        {
            if (still)
            {
                a2[5] = 2;
                a2[1] = 2;
            }
            else if ((*(byte*)(PsxAddrToPtr((long)unk_180576A10, a3[3]) + 64) & 2) != 0)
            {
                a2[5] = 3;
                a2[1] = 2;
            }
            else
            {
                a2[5] = 0;
            }

            return;
        }

        a2[5] = 1;
        a2[1] = 2;
        if (still)
        {
            *(long*)(a2 + 40) = 0;
            return;
        }

        HostQueryObjAnchorPos(PsxGetContextQword(a2[2]), 0, (long)(a2 + 12));
        // Inlined copy of SfxSpawnTrailEffect kind 4 (copy +12 to +20, effect +16, kind 128, voices 0); claim EFFECTS-016.
        long slot = SfxSpawnTrailEffect(a2[2], 4, (uint*)(a2 + 12), (ulong)(a2 + 20));
        *(long*)(a2 + 40) = slot;
        *(int*)unk_1805789D4 = (int)qword_1805789D0;
    }

    /// <summary>
    /// Step 1: faces the target (unless 0xFF or word_180323172 == 237); at the last animation frame plays command 0x1B,
    /// switches to step 2, refreshes the position and spawns the trail effect of the style. Returns the new counter.
    /// </summary>
    private static int SfxObjectStrikeSequenceWindUp(byte* a2, int frame)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjectStrikeSequenceWindUp(a2, frame)", (nint)a2, frame);
        if (a2[3] != 0xFF && word_180323172 != 237)
        {
            short* target = stackalloc short[4];
            HostQueryObjPosition(PsxGetContextQword(a2[3]), (long)target);
            SfxObjFaceTowards(a2[2], target, (ushort*)(a2 + 28), frame);
        }

        if (frame < (int)HostObjCmd09(a2[2]) - 1)
        {
            return frame;
        }

        HostObjCmd0CIfFlag(a2[2], 0x1B);
        a2[1] = 2;
        HostQueryObjPosition(PsxGetContextQword(a2[2]), (long)(a2 + 12));
        *(long*)(a2 + 40) = SfxSpawnTrailEffect(a2[2], a2[0], (uint*)(a2 + 12), (ulong)(a2 + 20));
        HostObjCmd1C(a2[2], 0, 0);
        HostCmd71IfObj16Flag();
        return -1;
    }

    /// <summary>
    /// Step 2: drops a finished slot, starts PSX task 10 at frame 0 (period word 0x18004B674[(dword_18032318C &gt;&gt; 8)
    /// &amp; 3] unless 3, 12 for style 1 else 20). Style 1 fades from frame 9 (alpha min(255, 255 * (frame - 9) / 16))
    /// by screen tiles or object motion and ends at a multiple of the hold period from frame 25 once the tick reaches 43
    /// and byte 0x180323182 is set; other styles wait per variant and move to step 3 / 0 or end. Returns true to end.
    /// </summary>
    private static bool SfxObjectStrikeSequenceHold(byte* a2, uint* a3, ref int frame)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjectStrikeSequenceHold(a2, a3, frame)", (nint)a2, (nint)a3, frame);
        long slot = *(long*)(a2 + 40);
        if (slot != 0 && *(ushort*)(slot + 48) == 0xFFFF)
        {
            *(long*)(a2 + 40) = 0;
        }

        if (frame == 0)
        {
            int period = (int)(((uint)dword_18032318C >> 8) & 3);
            if (period != 3)
            {
                int length = a2[0] == 1 ? 12 : 20;
                StartPsxTask(10, -1, ((ushort*)asc_18004B674)[period], length);
            }
        }

        if (a2[0] == 1)
        {
            SfxObjectStrikeSequenceFade(a2, a3, frame);
            if (frame % *(int*)(a2 + 8) == 0 && frame >= 25 && *(ushort*)(a2 + 6) >= 43 && *unk_180323182 != 0)
            {
                DetachSfxObject(*(long*)(a2 + 40));
                *unk_180323183 = 1;
                *unk_180323195 = 1;
                *(byte*)(PsxAddrToPtr((long)unk_180576A10, a3[11]) + 82) = 0;
                HostObjCmd1C(a2[2], 2, 0);
                return true;
            }

            return false;
        }

        switch (a2[5])
        {
            case 0:
                if (frame % (int)HostObjCmd09(a2[2]) != 0 || *(ushort*)(a2 + 6) < 43 || *unk_180323182 == 0)
                {
                    return false;
                }

                HostObjCmd0CIfFlag(a2[2], 0x1C);
                a2[1] = 3;
                break;

            case 1:
                if (*unk_180323182 == 0 || *(ushort*)(a2 + 6) < 27)
                {
                    return false;
                }

                a2[1] = 3;
                break;

            case 3:
                if (*unk_180323182 == 0)
                {
                    return false;
                }

                a2[1] = 0;
                break;

            default:
                if (*unk_180323182 == 0)
                {
                    return false;
                }

                *unk_180323183 = 1;
                MarkNoExtraObjects();
                *(byte*)(PsxAddrToPtr((long)unk_180576A10, a3[11]) + 82) = 0;
                HostObjCmd1C(a2[2], 2, 0);
                HostCmd71IfObj16Flag();
                return true;
        }

        frame = -1;
        return false;
    }

    /// <summary>
    /// Style 1 fade of step 2 from frame 9: at frame 9 the hold period becomes the animation length (and command 0x71
    /// when bit 0 of the a3[3] block's +64 is set); fade mode 0 / 1 adds screen tiles (mode 1 / 2) of the alpha to the
    /// block at a3[8] + 4 and resets every object's motion at frame 25, mode 2 / 3 sets the motion of all / visible
    /// objects to 128 - alpha / 2.
    /// </summary>
    private static void SfxObjectStrikeSequenceFade(byte* a2, uint* a3, int frame)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjectStrikeSequenceFade(a2, a3, frame)", (nint)a2, (nint)a3, frame);
        int step = frame - 9;
        if (step < 0)
        {
            return;
        }

        int alpha = step >= 16 ? 255 : (255 * step) >> 4;
        if (step == 0)
        {
            *(int*)(a2 + 8) = (int)HostObjCmd09(a2[2]);
            if ((*(byte*)(PsxAddrToPtr((long)unk_180576A10, a3[3]) + 64) & 1) != 0)
            {
                HostCmd71IfObj16Flag();
            }
        }

        switch (a2[4])
        {
            case 0:
            case 1:
                long tiles = PsxAddrToPtr((long)unk_180576A10, a3[8]) + 4;
                PsxAddScreenTiles(tiles, a2[4] + 1, (byte)alpha, (byte)alpha, (byte)alpha);
                if (step == 16)
                {
                    for (long node = BattleObjectList; node != 0; node = *(long*)node)
                    {
                        HostSetObjectMeshFade(node, 0);
                    }
                }

                break;

            case 2:
                for (long node = BattleObjectList; node != 0; node = *(long*)node)
                {
                    HostSetObjectMeshFade(node, 128 - (alpha >> 1));
                }

                break;

            case 3:
                for (long node = BattleObjectList; node != 0; node = *(long*)node)
                {
                    if (*(byte*)(node + 16) != 0)
                    {
                        HostSetObjectMeshFade(node, 128 - (alpha >> 1));
                    }
                }

                break;
        }
    }

    /// <summary>
    /// Step 3: at frame 0 (not variant 1) clears byte +82 of the a3[11] block and sends command 0x1C (0, 1). Variant 3
    /// keeps facing the target; others detach the trail and scale it out over frames 0..4 (+32 = 4096 - f * 1024,
    /// +28 = 4096 + f * 2048), then mark the extra objects done. Variant 1 ends after frame 4, others at the last
    /// animation frame (command 9 for variants 0 and 3, 0x1C (2, 0)). Returns true to end.
    /// </summary>
    private static bool SfxObjectStrikeSequenceRecover(byte* a2, uint* a3, int frame)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjectStrikeSequenceRecover(a2, a3, frame)", (nint)a2, (nint)a3, frame);
        if (frame == 0 && a2[5] != 1)
        {
            *(byte*)(PsxAddrToPtr((long)unk_180576A10, a3[11]) + 82) = 0;
            HostObjCmd1C(a2[2], 0, 1);
        }

        if (a2[5] == 3)
        {
            *unk_180323183 = 1;
            if (a2[3] != 0xFF)
            {
                short* target = stackalloc short[4];
                HostQueryObjPosition(PsxGetContextQword(a2[3]), (long)target);
                SfxObjFaceTowards(a2[2], target, (ushort*)(a2 + 28), frame);
            }
        }
        else
        {
            DetachSfxObject(*(long*)(a2 + 40));
            long slot = *(long*)(a2 + 40);
            if (slot != 0 && *(ushort*)(slot + 48) == 0xFFFF)
            {
                *(long*)(a2 + 40) = 0;
            }

            slot = *(long*)(a2 + 40);
            if (slot == 0 || frame > 4)
            {
                *unk_180323183 = 1;
                MarkNoExtraObjects();
            }
            else
            {
                int shrink = (frame << 12) >> 2;
                *(ushort*)(slot + 32) = (ushort)(0x1000 - shrink);
                slot = *(long*)(a2 + 40);
                if (slot != 0)
                {
                    *(int*)(slot + 28) = (2 * shrink) + 0x1000;
                }

                if (frame == 4)
                {
                    DetachSfxObject(*(long*)(a2 + 40));
                    *unk_180323183 = 1;
                    MarkNoExtraObjects();
                }
            }
        }

        if (a2[5] == 1)
        {
            return frame > 4;
        }

        if (frame < (int)HostObjCmd09(a2[2]) - 1)
        {
            return false;
        }

        if (a2[5] == 0 || a2[5] == 3)
        {
            HostObjCmd0CIfFlag(a2[2], 9);
        }

        HostObjCmd1C(a2[2], 2, 0);
        *unk_180323183 = 1;
        MarkNoExtraObjects();
        return true;
    }
}
