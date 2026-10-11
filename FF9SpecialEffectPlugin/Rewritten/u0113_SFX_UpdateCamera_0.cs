using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>Size of one camera track (eye at 0x1802200C8, target at 0x1802200E8).</summary>
    private const int CamScriptTickTrackSize = 32;

    /// <summary>
    /// Runs one frame of the camera script at qword_18021FF00 (frame counter dword_18021FF08). While the 10-bit frame
    /// of the next command matches the counter (at most 10 commands per frame), decodes its 22-bit flag word: 3 bits
    /// per track (eye, target) select key pushes and timing bytes, bits 6..14 read optional operands, bit 15 ends the
    /// block (tracks snapped, new timings from the next command, block variant selected) and returns 1. Otherwise
    /// evaluates both tracks into the eye (0x1802200B4) and a stack target, applies wave/shake, builds the look-at
    /// matrix at 0x180069730 and the yaw at 0x1802200B0, steps the roll/FOV interpolation, advances the frame and
    /// returns 0.
    /// </summary>
    public static long CamScriptTick()
    {
        Recompiler.Runtime.Tracer.Enter("CamScriptTick()");
        uint frame = (uint)dword_18021FF08;
        byte* tracks = unk_1802200C8;
        ushort* pair = (ushort*)Unsafe.AsPointer(ref dword_1802200BE);
        if (frame != 0)
        {
            ushort* p = (ushort*)qword_18021FF00;
            if (p != null)
            {
                int commands = 0;
                while (frame == (uint)(*p & 0x3FF))
                {
                    if ((byte_1802200BC & 1) == 0)
                    {
                        pair[0] = 512;
                        byte_1802200BC |= 1;
                    }

                    uint flags = ((uint)(p[0] >> 10) << 16) | p[1];
                    p += 2;
                    qword_18021FF00 = (long)p;
                    byte* track = tracks;
                    for (int shift = 0; shift < 6; shift += 3, track += CamScriptTickTrackSize)
                    {
                        int kind = (int)(flags >> shift) & 7;
                        if ((uint)(kind - 2) <= 1 && (byte)(track[24] - 2) <= 1 && track[29] != 0)
                        {
                            EvalOrbitTrack(1, (long)track, (ushort*)(track + 12));
                            p = (ushort*)qword_18021FF00;
                        }

                        if (kind != 0 && track[27] == 0)
                        {
                            track[24] = (byte)kind;
                        }

                        if ((uint)(kind - 1) <= 2)
                        {
                            CamPushTrackKey((long)track, (long)p);
                            p = (ushort*)(qword_18021FF00 + 6);
                            qword_18021FF00 = (long)p;
                        }

                        if ((uint)(kind - 2) <= 2)
                        {
                            byte* operand = (byte*)p;
                            if (track[27] == 0)
                            {
                                byte length = operand[0];
                                track[29] = length;
                                track[28] = length;
                                track[26] = operand[2];
                                track[25] = operand[3];
                            }

                            if (kind == 4)
                            {
                                *(short*)(track + 16) = (short)((sbyte)operand[1] << 5);
                            }

                            p += 2;
                            qword_18021FF00 = (long)p;
                        }

                        track[27] = 0;
                    }

                    if ((flags & 0x40) != 0)
                    {
                        dword_18021FF0C = *(byte*)p;
                        p++;
                        qword_18021FF00 = (long)p;
                    }

                    if ((flags & 0x80) != 0)
                    {
                        byte_18021FF48 = 1;
                    }

                    if ((flags & 0x100100) != 0)
                    {
                        byte_1802200B2 = 1;
                    }

                    if ((flags & 0x200) != 0)
                    {
                        dword_18021FF14 = *(byte*)p;
                        p++;
                        qword_18021FF00 = (long)p;
                    }

                    if ((flags & 0x400) != 0)
                    {
                        dword_18022010C = *(byte*)p;
                        p++;
                        qword_18021FF00 = (long)p;
                    }

                    if ((flags & 0x800) != 0)
                    {
                        byte* operand = (byte*)p;
                        dword_18021FF38 = operand[0];
                        dword_18021FF34 = operand[0];
                        dword_18021FF3C = operand[1];
                        dword_18021FF40 = pair[0];
                        ushort end = p[1];
                        dword_18021FF44 = end != 0 ? end : pair[1];
                        p += 2;
                        qword_18021FF00 = (long)p;
                    }

                    if ((flags & 0x1000) != 0)
                    {
                        byte* operand = (byte*)p;
                        dword_18021FF2C = operand[0];
                        dword_18021FF30 = operand[1];
                        dword_18021FF28 = operand[2];
                        dword_18021FF24 = operand[2];
                        p += 2;
                        qword_18021FF00 = (long)p;
                        dword_18021FF20 = operand[3] & 3;
                    }

                    if ((flags & 0x2000) != 0)
                    {
                        byte_1802200B3 = (byte)dword_180220108;
                    }

                    if ((flags & 0x4000) != 0)
                    {
                        if (*(byte*)p == 1)
                        {
                            CameraPhase = 1;
                        }
                        else if (BattleCallbackAddress != null)
                        {
                            HostCall((long)BattleCallbackAddress, 0x6E000000u, 0, 0, 0);
                            p = (ushort*)qword_18021FF00;
                        }

                        p++;
                        qword_18021FF00 = (long)p;
                    }

                    if ((flags & 0x8000) != 0)
                    {
                        return CamScriptTickEndBlock(tracks, pair);
                    }

                    frame = (uint)dword_18021FF08;
                    if ((flags & 0x80000) != 0)
                    {
                        byte_18021FF49 = 1;
                    }

                    if (++commands >= 10)
                    {
                        break;
                    }
                }

                qword_18021FF00 = *p != 0 ? (long)p : 0;
            }

            if (byte_1802200E5 != 0)
            {
                byte_1802200E5--;
            }

            if (byte_180220105 != 0)
            {
                byte_180220105--;
            }
        }

        if (byte_1802200E0 != 0 && byte_180220100 != 0)
        {
            byte_1802200C6 = 0;
            byte_18022011B = 0;
            short* eye = (short*)Unsafe.AsPointer(ref word_1802200B4);
            Span<short> target = stackalloc short[4];
            short* targetAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(target));
            target[0] = 0;
            target[1] = 0;
            target[2] = 0;
            target[3] = 0;
            EvalOrbitTrack(0, (long)tracks, (ushort*)eye);
            EvalOrbitTrack(0, (long)(tracks + CamScriptTickTrackSize), (ushort*)targetAddress);
            if (byte_18021FF48 == 1)
            {
                byte_18021FF48 = 2;
                System.Buffer.MemoryCopy(eye, unk_180220080, 8, 8);
                System.Buffer.MemoryCopy(targetAddress, unk_180220088, 8, 8);
            }

            if (dword_18021FF10 != 0)
            {
                dword_18021FF10--;
            }

            if (dword_180220114 != 0)
            {
                dword_180220114--;
            }

            byte_1802200C5 = byte_18022011B;
            byte_18022011A = 0;
            CamApplyWave(eye, targetAddress);
            if (dword_18021FF28 != 0)
            {
                CamShakeTarget((ushort*)targetAddress);
            }

            CamLookAtMatrix((long)xmmword_180069730, eye, targetAddress);
            double angle = Math.Atan2(word_1802200B8 - target[2], word_1802200B4 - target[0]);
            frame = (uint)dword_18021FF08;
            word_1802200B0 = unchecked((short)(-3072 - Recompiler.Runtime.X86.Cvttsd2si(-angle / 6.283185307179586 * 4096.0)));
        }

        int remaining = dword_18021FF38;
        if (remaining != 0)
        {
            remaining--;
            dword_18021FF38 = remaining;
            double t = (double)remaining / dword_18021FF34;
            int value = Recompiler.Runtime.X86.Cvttsd2si((1.0 - t) * dword_18021FF44 + dword_18021FF40 * t);
            if ((dword_18021FF3C & 1) != 0)
            {
                pair[1] = unchecked((ushort)value);
            }

            if ((dword_18021FF3C & 2) != 0)
            {
                pair[0] = unchecked((ushort)value);
            }
        }

        ushort roll = byte_1802200C6 != 0 ? (ushort)CameraProjectionDistance : pair[0];
        CameraProjectionDistance = unchecked((short)roll);
        dword_180211FA8 = roll;
        if (frame <= 0x3FE && byte_1802200B2 == 0)
        {
            dword_18021FF08 = (int)(frame + 1);
        }

        return 0;
    }

    /// <summary>
    /// Bit 15 of a camera command: snaps both running tracks (state 2/3 with time left) to their end key, restarts
    /// both with the curve modes and length of the command now at qword_18021FF00, and selects the block variant of
    /// the camera block at Block. Returns 1 (the caller runs the next block in the same frame).
    /// </summary>
    private static long CamScriptTickEndBlock(byte* tracks, ushort* pair)
    {
        Recompiler.Runtime.Tracer.Enter("CamScriptTickEndBlock(tracks, pair)", (nint)tracks, (nint)pair);
        byte* track = tracks;
        for (int i = 0; i < 2; i++, track += CamScriptTickTrackSize)
        {
            if (track[29] != 0 && (byte)(track[24] - 2) <= 1)
            {
                EvalOrbitTrack(1, (long)track, (ushort*)(track + 12));
                track[24] = 1;
            }
        }

        byte* command = (byte*)qword_18021FF00;
        byte* block = (byte*)Block;
        Block* block2 = (Block*)block;
        byte_1802200E3 = 1;
        byte_1802200E0 = (byte)(((sbyte)command[0] >> 5) & 3);
        byte_1802200E5 = command[1];
        byte_1802200E4 = command[1];
        byte_1802200E2 = command[2];
        byte_1802200E1 = 0;
        byte_180220103 = 1;
        byte_180220100 = (byte)(((sbyte)command[0] >> 5) & 3);
        byte_180220105 = command[1];
        byte_180220104 = command[1];
        byte_180220101 = 0;
        byte_180220102 = command[2];
        if (block != null)
        {
            short* variant = (short*)(block + block2->field_2);
            if (variant != null)
            {
                dword_180220108 = dword_18021FF1C;
                SfxSelectBlockVariant(variant);
                pair[0] = pair[1];
            }
        }

        return 1;
    }

    /// <summary>Runs the camera script for up to <paramref name="skip"/> frames while a script is loaded.</summary>
    public static void SFX_SkipCameraAnimation(int skip)
    {
        Recompiler.Runtime.Tracer.Enter("SFX_SkipCameraAnimation(skip)", skip);
        while (qword_18021FF00 != 0 && skip != 0)
        {
            if ((uint)CamScriptTick() != 0)
            {
                CamScriptTick();
            }

            skip--;
        }
    }

    /// <summary>
    /// Flips the double-buffer index dword_1801C1DC0 and swaps the two frame buffers 0x1800697B0/0x1800EDAB4 between
    /// off_180066C60 (draw) and qword_1800697A0 (current). Without a scene (word_180323170 == -1) nothing changes.
    /// Memoria declares this operation as void: ref:R001/Assembly-CSharp/Global/SFX/SFX.cs#L731.
    /// </summary>
    public static void SFX_LateUpdate()
    {
        Recompiler.Runtime.Tracer.Enter("SFX_LateUpdate()");
        if (word_180323170 == -1)
        {
            return;
        }

        dword_1801C1DC0 = (dword_1801C1DC0 - 1) & 1;
        if (dword_1801C1DC0 != 0)
        {
            off_180066C60 = (nint)unk_1800EDAB4;
            qword_1800697A0 = (long)unk_1800697B0;
            return;
        }

        off_180066C60 = (nint)unk_1800697B0;
        qword_1800697A0 = (long)unk_1800EDAB4;
    }

    /// <summary>
    /// Kind 1 stores the truncated arg0/arg1/arg2 as the camera target coordinates. The original's search for btl_id in the
    /// 64-byte table at 0x180069510 has no effect and is omitted. Always returns 0.
    /// </summary>
    public static int SFX_SendFloatData(int type, int btl_id, float arg0, float arg1, float arg2)
    {
        Recompiler.Runtime.Tracer.Enter("SFX_SendFloatData(type, btl_id, arg0, arg1, arg2)", type, btl_id, arg0, arg1, arg2);
        if (type == 1)
        {
            CameraTargetX = unchecked((short)Recompiler.Runtime.X86.Cvttss2si(arg0));
            CameraTargetY = unchecked((short)Recompiler.Runtime.X86.Cvttss2si(arg1));
            CameraTargetZ = unchecked((short)Recompiler.Runtime.X86.Cvttss2si(arg2));
        }

        return 0;
    }

    /// <summary>
    /// Runs one camera frame (twice when a block ended), selects the debug/free or scripted camera and returns
    /// 13 floats: rotation[0..8] in PSX units (4096 = 1), translation[9..11], projection distance[12].
    /// Memoria uses the last component as fxNearZ, not roll: ref:R001/Assembly-CSharp/Global/SFX/SFX.cs#L1597-L1604.
    /// </summary>
    public static float* SFX_UpdateCamera_0(int isDebug)
    {
        Recompiler.Runtime.Tracer.Enter("SFX_UpdateCamera_0(isDebug)", isDebug);
        if ((uint)CamScriptTick() != 0)
        {
            CamScriptTick();
        }

        bool host = word_180323170 == -1 ? isDebug != 0 : dword_180211E24 == 1;
        int tx;
        int ty;
        int tz;
        ushort projectionDistance;
        if (host)
        {
            tx = dword_180211E50;
            ty = dword_180211E54;
            tz = dword_180211E58;
            projectionDistance = unchecked((ushort)FreeCameraProjectionDistance);
            System.Buffer.MemoryCopy(xmmword_180211E38, xmmword_180069740, 16, 16);
            System.Buffer.MemoryCopy(xmmword_180211E28, xmmword_180069730, 16, 16);
            *(int*)unk_180069744 = tx;
            *(int*)unk_180069748 = ty;
            *(int*)unk_18006974C = tz;
            CameraProjectionDistance = unchecked((short)projectionDistance);
        }
        else
        {
            projectionDistance = unchecked((ushort)CameraProjectionDistance);
            tz = *(int*)unk_18006974C;
            ty = *(int*)unk_180069748;
            tx = *(int*)unk_180069744;
        }

        short* rotation = (short*)xmmword_180069730;
        dword_180211DF0 = BitConverter.SingleToInt32Bits(rotation[0]);
        dword_180211DF4 = BitConverter.SingleToInt32Bits(rotation[1]);
        dword_180211DF8 = BitConverter.SingleToInt32Bits(rotation[2]);
        dword_180211DFC = BitConverter.SingleToInt32Bits(rotation[3]);
        dword_180211E00 = BitConverter.SingleToInt32Bits(rotation[4]);
        dword_180211E04 = BitConverter.SingleToInt32Bits(rotation[5]);
        dword_180211E08 = BitConverter.SingleToInt32Bits(rotation[6]);
        dword_180211E0C = BitConverter.SingleToInt32Bits(rotation[7]);
        dword_180211E10 = BitConverter.SingleToInt32Bits(*(short*)xmmword_180069740);
        dword_180211E14 = BitConverter.SingleToInt32Bits(tx);
        dword_180211E18 = BitConverter.SingleToInt32Bits(ty);
        dword_180211E1C = BitConverter.SingleToInt32Bits(tz);
        dword_180211E20 = BitConverter.SingleToInt32Bits(projectionDistance);
        return (float*)Unsafe.AsPointer(ref dword_180211DF0);
    }
}
