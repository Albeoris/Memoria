using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// Script op handlers reached through the op table in the data image. Protocol (SFX-OPS-001): a1 == 0 stores the
/// state size at *a5; a1 == 1 initialises the state a2 from the argument bytes a5[0]/a5[4]; any other a1 runs one
/// step on the state a2 with the frame counter *(int*)a4 and returns 1 when the op has finished.
/// The step handlers move objects through the host callback (HostQueryObj*/HostObjCmd*) and the GTE interpolators
/// LerpObjPosStep/LerpObj16PosStep. Stack buffers the host fills are zeroed here (DIV, uninitialised without a callback).
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>Step: on frame 0 HostObjCmd21IfFlag(state, obj0, obj1) and duration = HostObjCmd09(obj0) - 1; done when frame &gt;= duration (sets byte_180323184).</summary>
    public static long SfxOpCmd09WaitStep(byte* a1, long a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpCmd09WaitStep(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        State3* state3 = (State3*)a1;
        int frame = *a3;
        if (frame == 0)
        {
            HostObjCmd21IfFlag((long)a1, *a1, a1[1]);
            state3->field_4 = (uint)HostObjCmd09(*a1) - 1;
        }

        if (frame < (int)state3->field_4)
        {
            return 0;
        }

        byte_180323184 = 1;
        return 1;
    }

    /// <summary>Op: 8-byte state {obj0, obj1, duration}; init clears byte_180323184.</summary>
    public static long SfxOpCmd09Wait(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpCmd09Wait(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        if (a1 == 0)
        {
            shape_SfxObjAnimWaitHandler_a5->union_0_4 = 8;
            return 0;
        }

        if (a1 == 1)
        {
            *a2 = *a5;
            a2[1] = a5[4];
            byte_180323184 = 0;
            return 0;
        }

        return SfxOpCmd09WaitStep(a2, (long)a2, (int*)a4);
    }

    /// <summary>Op: 4-byte state {obj}; step sends host 0x0D000000 | id (0) and finishes at once.</summary>
    public static long SfxOpHostCmd0D(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpHostCmd0D(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
        Shape_SfxObjCallKindScriptHandler_a5* shape_SfxObjCallKindScriptHandler_a5 = (Shape_SfxObjCallKindScriptHandler_a5*)a5;
        if (a1 == 0)
        {
            shape_SfxObjCallKindScriptHandler_a5->union_0_4 = 4;
            return 0;
        }

        if (a1 == 1)
        {
            *a2 = *a5;
            return 0;
        }

        HostObjCmd0D(*a2, 0);
        return 1;
    }

    /// <summary>
    /// Step: when object a1[1] (255 checks object 0) has flag +16, places object a1[0] 1000 units in front of a1[1]
    /// (rotation from host 0x03 turned by 2048) at once. Otherwise frame 0 takes the duration from word_18004AF30
    /// by byte +32 of a1[0] and the target = position of a1[1]; duration 1 places at once, else LerpObjPosStep
    /// interpolates from +8 to +16 over the frames.
    /// </summary>
    public static long SfxOpMoveObjToObjStep(byte* a1, long a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpMoveObjToObjStep(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        int frame = *a3;
        int flagIndex = a1[1] == 255 ? 0 : a1[1];
        if (*(byte*)(PsxGetContextQword(flagIndex) + 16) != 0)
        {
            short* position = stackalloc short[4];
            short* rotation = stackalloc short[4];
            short* target = stackalloc short[4];
            HostGetObjectPositionById(a1[1], (long)position);

            // Inlined copy of HostQueryObjCmd03ById (SFX-OPS-002).
            HostGetObjectRotationById(a1[1], (long)rotation);
            SfxRotTransPointZ(position, rotation, 1000, (ushort*)target);
            HostSetObjectPositionById(*a1, (long)target);
            byte_180323185 = 1;
            return 1;
        }

        if (frame == 0)
        {
            long obj = PsxGetContextQword(*a1);
            byte other = a1[1];
            // lea rax,[18004AF30h] at 0x18001A60E loads the table address, not its first word.
            *(uint*)(a1 + 4) = ((ushort*)Unsafe.AsPointer(ref word_18004AF30))[*(byte*)(obj + 32)];
            HostQueryObjPosition(PsxGetContextQword(other), (long)(a1 + 16));
            if (*(uint*)(a1 + 4) == 1)
            {
                HostSetObjectPositionById(*a1, (long)(a1 + 16));
                byte_180323185 = 1;
                return 1;
            }
        }

        // The 5th argument is never read by LerpObjPosStep (SFX-OPS-003).
        long result = LerpObjPosStep(*a1, (short*)(a1 + 16), frame + 1, *(int*)(a1 + 4), 0, (short*)(a1 + 8));
        if (result != 0)
        {
            byte_180323185 = 1;
            return 1;
        }

        return result;
    }

    /// <summary>Op: 24-byte state {obj0, obj1, duration, start, target}; init sets byte_180323185 = 1 when both objects resolve to the same context.</summary>
    public static long SfxOpMoveObjToObj(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpMoveObjToObj(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        if (a1 == 0)
        {
            shape_SfxObjAnimWaitHandler_a5->union_0_4 = 24;
            return 0;
        }

        if (a1 != 1)
        {
            return SfxOpMoveObjToObjStep(a2, (long)a2, (int*)a4);
        }

        byte first = *a5;
        *a2 = first;
        byte second = a5[4];
        a2[1] = second;
        if (first == 255 || second == 255 || PsxGetContextQword(first) != PsxGetContextQword(second))
        {
            byte_180323185 = 0;
            return 0;
        }

        byte_180323185 = 1;
        return 1;
    }

    /// <summary>
    /// Step: target = host 0x01 query of object a1[0]; when a1[1] has flag +16, or on frame 0 the duration from
    /// word_18004AFA2 (byte +32 of a1[0]) is 1, places it at once; else LerpObjPosStep interpolates from +8.
    /// </summary>
    public static long SfxOpMoveObjToQueryPosStep(byte* a1, long a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpMoveObjToQueryPosStep(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        State3* state3 = (State3*)a1;
        int frame = *a3;
        short* target = stackalloc short[4];
        HostGetObjectPositionById(*a1, (long)target);
        int flagIndex = a1[1] == 255 ? 0 : a1[1];
        bool placeNow = *(byte*)(PsxGetContextQword(flagIndex) + 16) != 0;
        if (!placeNow && frame == 0)
        {
            // lea rax,[18004AFA2h] at 0x18001A776 loads the table address, not its first word.
            uint duration = ((ushort*)Unsafe.AsPointer(ref word_18004AFA2))[*(byte*)(PsxGetContextQword(*a1) + 32)];
            state3->field_4 = duration;
            placeNow = duration == 1;
        }

        if (placeNow)
        {
            HostSetObjectPositionById(*a1, (long)target);
            byte_180323185 = 1;
            return 1;
        }

        long result = LerpObjPosStep(*a1, target, frame + 1, (int)state3->field_4, 0, (short*)(a1 + 8));
        if (result != 0)
        {
            byte_180323185 = 1;
            return 1;
        }

        return result;
    }

    /// <summary>Op: 16-byte state {obj0, obj1, duration, start}; init clears byte_180323185.</summary>
    public static long SfxOpMoveObjToQueryPos(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpMoveObjToQueryPos(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        if (a1 == 0)
        {
            shape_SfxObjAnimWaitHandler_a5->union_0_4 = 16;
            return 0;
        }

        if (a1 == 1)
        {
            *a2 = *a5;
            a2[1] = a5[4];
            byte_180323185 = 0;
            return 0;
        }

        return SfxOpMoveObjToQueryPosStep(a2, (long)a2, (int*)a4);
    }

    /// <summary>
    /// Step: frame 0 takes the position of a1[0] and the host 0x01 query of a1[1] with its y replaced by the former
    /// (SFX-OPS-004); the points s16 +4 and s16 +6 units along z from it become the start (placed at once) and the
    /// target +20. Duration +8 == 1 places at the target at once, else LerpObjPosStep interpolates from +12.
    /// </summary>
    public static long SfxOpMoveCtx16ToObjStep(byte* a1, long a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpMoveCtx16ToObjStep(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        int frame = *a3;
        if (frame == 0)
        {
            Span<short> start = stackalloc short[4];
            short* startAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(start));
            Span<short> origin = stackalloc short[4];
            short* originAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(origin));
            short* rotation = stackalloc short[4];
            HostQueryObjPosition(PsxGetContextQword(*a1), (long)startAddress);
            HostGetObjectPositionById(a1[1], (long)originAddress);
            origin[1] = start[1];
            SfxRotTransPointZ(originAddress, rotation, *(short*)(a1 + 4), (ushort*)startAddress);
            SfxRotTransPointZ(originAddress, rotation, *(short*)(a1 + 6), (ushort*)(a1 + 20));
            HostSetObjectPositionById(*a1, (long)startAddress);
            if (*(uint*)(a1 + 8) == 1)
            {
                HostSetObjectPositionById(*a1, (long)(a1 + 20));
                byte_180323185 = 1;
                return 1;
            }
        }

        long result = LerpObjPosStep(*a1, (short*)(a1 + 20), frame + 1, *(int*)(a1 + 8), 0, (short*)(a1 + 12));
        if (result != 0)
        {
            byte_180323185 = 1;
            return 1;
        }

        return result;
    }

    /// <summary>
    /// Op: 28-byte state {16, obj, s16 word_180578E00, s16 word_180578E02, duration, start, target}. Init sets
    /// byte_180323185 = 1 (returns 1) when obj resolves to context 16; else returns 1 when obj (255: 0) has no flag
    /// +16 (byte_180323185 untouched), otherwise clears it and returns 0.
    /// </summary>
    public static long SfxOpMoveCtx16ToObj(int a1, long a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpMoveCtx16ToObj(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        if (a1 == 0)
        {
            shape_SfxObjAnimWaitHandler_a5->union_0_4 = 28;
            return 0;
        }

        if (a1 != 1)
        {
            return SfxOpMoveCtx16ToObjStep((byte*)a2, a2, (int*)a4);
        }

        byte* state = (byte*)a2;
        *state = 16;
        byte obj = *a5;
        state[1] = obj;
        *(uint*)(state + 8) = a5[4];
        *(ushort*)(state + 4) = (ushort)word_180578E00;
        *(ushort*)(state + 6) = (ushort)word_180578E02;
        if (obj != 255 && PsxGetContextQword(16) == PsxGetContextQword(obj))
        {
            byte_180323185 = 1;
            return 1;
        }

        int flagIndex = state[1] == 255 ? 0 : state[1];
        if (*(byte*)(PsxGetContextQword(flagIndex) + 16) == 0)
        {
            return 1;
        }

        byte_180323185 = 0;
        return 0;
    }

    /// <summary>
    /// Step: frame 0 sets the target +20 to the point s16 +4 units in front of object a1[0] (host 0x03 rotation turned
    /// by 2048), HostObjCmd0CIfFlag(a1[0], 29) when byte +3 is set and duration +8 = HostObjCmd09(a1[0]); then
    /// LerpObj16PosStep mode 0 interpolates from +12.
    /// </summary>
    public static long SfxOpCtx16AdvanceStep(byte* a1, long a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpCtx16AdvanceStep(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        int frame = *a3;
        if (frame == 0)
        {
            short* position = stackalloc short[4];
            short* rotation = stackalloc short[4];
            HostQueryObjPosition(PsxGetContextQword(*a1), (long)position);

            // Inlined copy of HostQueryObjCmd03ById (SFX-OPS-002).
            HostGetObjectRotationById(*a1, (long)rotation);
            SfxRotTransPointZ(position, rotation, *(short*)(a1 + 4), (ushort*)(a1 + 20));
            if (a1[3] != 0)
            {
                HostObjCmd0CIfFlag(*a1, 29);
            }

            *(uint*)(a1 + 8) = (uint)HostObjCmd09(*a1);
        }

        if ((int)LerpObj16PosStep(*a1, (short*)(a1 + 20), frame, *(int*)(a1 + 8), (short*)(a1 + 12), 0) == 0)
        {
            return 0;
        }

        byte_180323185 = 1;
        return 1;
    }

    /// <summary>
    /// Op: 28-byte state {16, obj, a5[4], 1, s16 word_180578E00, duration, start, target}. Init sets byte_180323185 = 1
    /// (returns 1) only when a5[4] == 1 and obj (255: 0) has flag +16; otherwise clears it and returns 0.
    /// </summary>
    public static long SfxOpCtx16Advance(int a1, long a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpCtx16Advance(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        if (a1 == 0)
        {
            shape_SfxObjAnimWaitHandler_a5->union_0_4 = 28;
            return 0;
        }

        if (a1 != 1)
        {
            return SfxOpCtx16AdvanceStep((byte*)a2, a2, (int*)a4);
        }

        byte* state = (byte*)a2;
        *state = 16;
        byte obj = *a5;
        state[1] = *a5;
        byte mode = a5[4];
        state[3] = 1;
        state[2] = mode;
        *(ushort*)(state + 4) = (ushort)word_180578E00;
        if (mode == 1)
        {
            int flagIndex = obj == 255 ? 0 : obj;
            if (*(byte*)(PsxGetContextQword(flagIndex) + 16) != 0)
            {
                byte_180323185 = 1;
                return 1;
            }
        }

        byte_180323185 = 0;
        return 0;
    }

    /// <summary>
    /// Step: frame 0 sets the target +16 from the host 0x01 query of a1[0], HostObjCmd0CIfFlag(a1[0], 30) when byte +3
    /// is set and duration +4 = HostObjCmd09(a1[0]); then LerpObj16PosStep mode 1 interpolates from +8.
    /// </summary>
    public static long SfxOpCtx16ToQueryPosStep(byte* a1, long a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpCtx16ToQueryPosStep(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        int frame = *a3;
        if (frame == 0)
        {
            HostGetObjectPositionById(*a1, (long)(a1 + 16));
            if (a1[3] != 0)
            {
                HostObjCmd0CIfFlag(*a1, 30);
            }

            *(uint*)(a1 + 4) = (uint)HostObjCmd09(*a1);
        }

        if ((int)LerpObj16PosStep(*a1, (short*)(a1 + 16), frame, *(int*)(a1 + 4), (short*)(a1 + 8), 1) == 0)
        {
            return 0;
        }

        byte_180323185 = 1;
        return 1;
    }

    /// <summary>Op: 24-byte state {16, obj, a5[4], 1, duration, start, target}; init as SfxOpCtx16Advance without the distance word.</summary>
    public static long SfxOpCtx16ToQueryPos(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpCtx16ToQueryPos(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        if (a1 == 0)
        {
            shape_SfxObjAnimWaitHandler_a5->union_0_4 = 24;
            return 0;
        }

        if (a1 != 1)
        {
            return SfxOpCtx16ToQueryPosStep(a2, (long)a2, (int*)a4);
        }

        *a2 = 16;
        byte obj = *a5;
        a2[1] = *a5;
        byte mode = a5[4];
        a2[3] = 1;
        a2[2] = mode;
        if (mode == 1)
        {
            int flagIndex = obj == 255 ? 0 : obj;
            if (*(byte*)(PsxGetContextQword(flagIndex) + 16) != 0)
            {
                byte_180323185 = 1;
                return 1;
            }
        }

        byte_180323185 = 0;
        return 0;
    }
}
