using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>Host message sent once when the frame enters the paused state (claim in area frame-loop).</summary>
    private const ulong SfxPauseEnterMessage = 0x74000000;

    /// <summary>
    /// One emulated frame: registers the frame buffer a1 and the GPU state block, clears the pause byte of the root
    /// object, runs the sequencer and the loader, then either (paused) notifies the host once with 0x74000000 and sets
    /// byte +0x5D of object [11], or (running) ticks effect tasks, attachments, particles and keyframe tracks.
    /// Returns the host pointer of the PSX address in slot [9].
    /// </summary>
    public static byte* SfxRunFrame(ulong a1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxRunFrame(a1)", a1);
        dword_18032325C = 0;
        uint frame = (uint)PsxAddrMapperRegister((long)unk_180576A10, a1);
        uint* state = (uint*)off_180066C68;
        state[9] = frame;
        state[10] = 0;
        uint gpuState = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)xmmword_180069730);
        state = (uint*)off_180066C68;
        state[5] = gpuState;

        ResolveGlobalPsxAddress(state[0])[32] = 0;
        SeqStateUpdate();
        SfxLoaderStep();

        state = (uint*)off_180066C68;
        if (ResolveGlobalPsxAddress(state[0])[32] != 0)
        {
            if (dword_180220138 == 0)
            {
                dword_180220138 = 1;
                long callback = (long)BattleCallbackAddress;
                if (callback != 0)
                {
                    Native.InvokeBattleCallback(callback, SfxPauseEnterMessage, 2, 0, 0, 0, 0);
                    state = (uint*)off_180066C68;
                }
            }

            ResolveGlobalPsxAddress(state[11])[93] = 1;
        }
        else
        {
            dword_180220138 = 0;
            SfxTickEffectTasks();
            long attachments = (long)Unsafe.AsPointer(ref qword_180211E68);
            long trails = (long)Unsafe.AsPointer(ref qword_1803210D0);
            sub_1800354E0(attachments, trails);
            SfxUpdateParticles(attachments);
            SfxUpdateKeyframeTracks(attachments, trails);
            state = (uint*)off_180066C68;
        }

        return ResolveGlobalPsxAddress(state[9]);
    }

    /// <summary>
    /// Export SFX_Update: when a game is loaded (word_180323170 != -1) registers the PSX views of the game state,
    /// clears the ordering table, resets the 160x120 draw offset and the draw environment copies, runs one frame and
    /// stores the incremented frame counter in *a1 (returns 1); otherwise stores the current counter and returns 0.
    /// </summary>
    public static sbyte SFX_Update_0(int* frameIndex)
    {
        Recompiler.Runtime.Tracer.Enter("SFX_Update_0(frameIndex)", (nint)frameIndex);
        if (word_180323170 == -1)
        {
            dword_1801C1DC4 = 0;
            *frameIndex = EffectFrameIndex;
            return 0;
        }

        dword_1800697AC = 0;
        dword_1801C1DC4 = 1;
        long context = (long)unk_180576A10;
        ((uint*)off_180066C68)[8] = (uint)PsxAddrMapperRegister(context, (ulong)(off_180066C60 + 116));
        ((uint*)off_180066C68)[6] = (uint)PsxAddrMapperRegister(context, (ulong)(off_180066C60 + 96));
        ((uint*)off_180066C68)[7] = (uint)PsxAddrMapperRegister(context, (ulong)(off_180066C60 + 4));
        ((uint*)off_180066C68)[9] = (uint)PsxAddrMapperRegister(context, (ulong)(off_180066C60 + 16500));
        uint drawEnv = (uint)PsxAddrMapperRegister(context, (ulong)xmmword_1801C1DC8);
        uint* state = (uint*)off_180066C68;
        state[5] = drawEnv;

        PsxClearOTagR((uint*)ResolveGlobalPsxAddress(state[8]));

        dword_180211FA0 = 160;
        dword_180211FA4 = 120;
        uint* env0 = (uint*)xmmword_180069730;
        uint* env1 = (uint*)xmmword_180069740;
        uint e1x = env1[0], e1y = env1[1], e1z = env1[2], e1w = env1[3];
        System.Buffer.MemoryCopy(env0, xmmword_1801C1DC8, 16, 16);
        System.Buffer.MemoryCopy(env0, xmmword_180211F40, 16, 16);
        System.Buffer.MemoryCopy(env1, xmmword_1801C1DD8, 16, 16);
        dword_180211F50 = (ushort)e1x;
        dword_180211FA8 = (ushort)CameraProjectionDistance;
        *(uint*)unk_180211F54 = e1y;
        *(uint*)unk_180211F58 = e1z;
        dword_180211F5C = (int)e1w;

        SfxRunFrame((ulong)ResolveGlobalPsxAddress(((uint*)off_180066C68)[9]));
        int counter = EffectFrameIndex + 1;
        EffectFrameIndex = counter;
        *frameIndex = counter;
        return 1;
    }

    /// <summary>
    /// Export SFX_BeginRender: when the game state pointer qword_1800697A0 is set, maps its +116 block to a PSX
    /// address, finds the mapper window holding its host pointer (closest start, as the mapper does but without
    /// registering), stores window &lt;&lt; 24 in dword_180069728 and 4095 in dword_18006972C, sets qword_180171DB8 to the
    /// host pointer + 16380 and returns 1; returns 0 without a state pointer.
    /// </summary>
    public static sbyte SFX_BeginRender_0()
    {
        Recompiler.Runtime.Tracer.Enter("SFX_BeginRender_0()");
        long gameState = qword_1800697A0;
        if (gameState == 0)
        {
            return 0;
        }

        uint address = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)(gameState + 116));
        ulong host = (ulong)ResolveGlobalPsxAddress(address);

        int window = -1;
        ulong bestDistance = 0x300000;
        int count = (int)qword_1805789D0;
        byte* entry = unk_180576A20;
        for (int i = 0; i < count; i++, entry += 32)
        {
            if (entry[-16] != 0)
            {
                ulong start = *(ulong*)entry;
                if (host >= start && host < *(ulong*)(entry + 8) && bestDistance > host - start)
                {
                    bestDistance = host - start;
                    window = i;
                }
            }
        }

        dword_180069728 = window << 24;
        dword_18006972C = 4095;
        qword_180171DB8 = (long)(ResolveGlobalPsxAddress(address) + 16380);
        return 1;
    }
}
