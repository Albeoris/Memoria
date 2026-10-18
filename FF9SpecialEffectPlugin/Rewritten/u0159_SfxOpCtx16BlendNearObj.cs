using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// Script op handlers (protocol SFX-OPS-001) that move or turn objects towards an object, a preset vector or the
/// anchor record of the script context (a2 + 12 = PSX address of a record with s16 x/y/z at +56/+58/+60).
/// Host-filled stack vectors are zeroed (DIV-051); sub_180018D30's unused 5th argument gets 0 (SFX-OPS-003).
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>
    /// Step: target = {0, 0, -256} (mode a1[1] == 0), the host 0x01 query of a1[0] (1), the anchor record of the
    /// context a2 (2) or zero (other). Duration +4 == 1 on frame 0 places at once, else LerpObjPosStep from +8.
    /// </summary>
    public static long SfxOpMoveObjToAnchorStep(byte* a1, long a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpMoveObjToAnchorStep(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        State3* state3 = (State3*)a1;
        int frame = *a3;
        Span<short> target = stackalloc short[4];
        short* targetAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(target));
        switch (a1[1])
        {
            case 0:
                target[2] = unchecked((short)0xFF00);
                break;
            case 1:
                HostGetObjectPositionById(*a1, (long)targetAddress);
                break;
            case 2:
                // Three inlined copies of PsxAddrToPtr(&unk_180576A10, *(a2 + 12)) (SFX-OPS-005).
                SfxOpCtx16BlendNearObjAnchor(a2, target);
                break;
        }

        if (frame == 0 && state3->field_4 == 1)
        {
            HostSetObjectPositionById(*a1, (long)targetAddress);
            byte_180323185 = 1;
            return 1;
        }

        long result = LerpObjPosStep(*a1, targetAddress, frame + 1, (int)state3->field_4, 0, (short*)(a1 + 8));
        if (result != 0)
        {
            byte_180323185 = 1;
            return 1;
        }

        return result;
    }

    /// <summary>Op: 16-byte state {obj, mode, duration = low byte of word_180578E00, start}; init clears byte_180323185. The step gets a3 as its context.</summary>
    public static long SfxOpMoveObjToAnchor(int a1, long a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpMoveObjToAnchor(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        State3* state3 = (State3*)a2;
        if (a1 == 0)
        {
            shape_SfxObjAnimWaitHandler_a5->union_0_4 = 16;
            return 0;
        }

        if (a1 != 1)
        {
            return SfxOpMoveObjToAnchorStep((byte*)a2, a3, (int*)a4);
        }

        byte* state = (byte*)a2;
        *state = *a5;
        state[1] = a5[4];
        state3->field_4 = (byte)word_180578E00;
        byte_180323185 = 0;
        return 0;
    }

    /// <summary>
    /// Step: frame 0 sets the target +20 to the point s16 +4 units from the aim point of a1[1] towards a1[0]
    /// (SfxOpCtx16BlendNearObjAimTarget), then pulls it back towards a1[0]'s position by a1[2] / 10 per axis
    /// (16-bit products). Duration +8 == 1 places at once, else LerpObjPosStep from +12.
    /// </summary>
    public static long SfxOpCtx16BlendNearObjStep(byte* a1, long a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpCtx16BlendNearObjStep(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        int frame = *a3;
        if (frame == 0)
        {
            Span<short> from = stackalloc short[4];
            SfxOpCtx16BlendNearObjAimTarget(a1, from);
            int weight = a1[2];
            short* target = (short*)(a1 + 20);
            target[0] = (short)(from[0] + (short)((target[0] - from[0]) * weight) / 10);
            target[1] = (short)(from[1] + (short)((target[1] - from[1]) * weight) / 10);
            target[2] = (short)(from[2] + (short)((target[2] - from[2]) * weight) / 10);
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
    /// Op: 28-byte state {16, obj, weight = low byte of word_180578E02, distance s16 word_180578E00, duration a5[4],
    /// start, target}. Init sets byte_180323185 = 1 (returns 1) when obj resolves to context 16 or obj (255: 0) has
    /// flag +16; otherwise clears it and returns 0.
    /// </summary>
    public static long SfxOpCtx16BlendNearObj(int a1, long a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpCtx16BlendNearObj(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        if (a1 == 0)
        {
            shape_SfxObjAnimWaitHandler_a5->union_0_4 = 28;
            return 0;
        }

        if (a1 != 1)
        {
            return SfxOpCtx16BlendNearObjStep((byte*)a2, a2, (int*)a4);
        }

        byte* state = (byte*)a2;
        *state = 16;
        byte obj = *a5;
        state[1] = obj;
        *(uint*)(state + 8) = a5[4];
        *(ushort*)(state + 4) = (ushort)word_180578E00;
        state[2] = (byte)word_180578E02;
        if (obj != 255 && PsxGetContextQword(16) == PsxGetContextQword(obj))
        {
            byte_180323185 = 1;
            return 1;
        }

        int flagIndex = state[1] == 255 ? 0 : state[1];
        if (*(byte*)(PsxGetContextQword(flagIndex) + 16) != 0)
        {
            byte_180323185 = 1;
            return 1;
        }

        byte_180323185 = 0;
        return 0;
    }

    /// <summary>Step: as SfxOpCtx16BlendNearObjStep without the pull-back (target = the point s16 +4 units from the aim point).</summary>
    public static long SfxOpCtx16MoveNearObjStep(byte* a1, long a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpCtx16MoveNearObjStep(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        int frame = *a3;
        if (frame == 0)
        {
            Span<short> from = stackalloc short[4];
            SfxOpCtx16BlendNearObjAimTarget(a1, from);
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
    /// Op: 28-byte state {16, obj, distance s16 word_180578E00, duration a5[4], start, target}. Init sets
    /// byte_180323185 = 1 (returns 1) when obj resolves to context 16; else returns 1 when obj (255: 0) has no flag +16
    /// (byte_180323185 untouched), otherwise clears it and returns 0.
    /// </summary>
    public static long SfxOpCtx16MoveNearObj(int a1, long a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpCtx16MoveNearObj(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        if (a1 == 0)
        {
            shape_SfxObjAnimWaitHandler_a5->union_0_4 = 28;
            return 0;
        }

        if (a1 != 1)
        {
            return SfxOpCtx16MoveNearObjStep((byte*)a2, a2, (int*)a4);
        }

        byte* state = (byte*)a2;
        *state = 16;
        byte obj = *a5;
        state[1] = obj;
        *(uint*)(state + 8) = a5[4];
        *(ushort*)(state + 4) = (ushort)word_180578E00;
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
    /// Step: turns object a1[0] towards the target (a1[1] == 255: anchor record of the context a2, else the host 0x01
    /// query of a1[1]). With byte +3 set and a1[1] (255: 0) flagged +16 it sets the host 0x03 rotation's y to 2048 at
    /// once (the target is queried but unused) and sets byte_180323186. Otherwise frame 0 stores the look angles from
    /// a1[0] to the target at +12 (x cleared, y + 2048 when byte +2 == 1); each frame adds angles / duration (+4,
    /// signed division) to the host 0x03 rotation (y also + 2048) and finishes on frame duration - 1.
    /// </summary>
    public static long SfxOpTurnTowardsStep(byte* a1, long a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpTurnTowardsStep(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        Scratch2* scratch2 = (Scratch2*)a1;
        int frame = *a3;
        Span<short> rotation = stackalloc short[4];
        short* rotationAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(rotation));
        Span<short> target = stackalloc short[4];
        short* targetAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(target));
        if (a1[3] != 0)
        {
            int flagIndex = a1[1] == 255 ? 0 : a1[1];
            if (*(byte*)(PsxGetContextQword(flagIndex) + 16) != 0)
            {
                SfxOpCtx16BlendNearObjTurnTarget(a1, a2, target);
                long obj = PsxGetContextQword(*a1);
                if (BattleCallbackAddress != null)
                {
                    HostObjCallback(ObjCommand(obj, HostCommand.GetRotate), 1, 0, 0, 0, (ulong)rotationAddress);
                }

                rotation[1] = 2048;
                HostSetObjRotation(*a1, (long)rotationAddress);
                byte_180323186 = 1;
                return 1;
            }
        }

        if (frame == 0)
        {
            short* from = stackalloc short[4];
            HostQueryObjPosition(PsxGetContextQword(*a1), (long)from);
            SfxOpCtx16BlendNearObjTurnTarget(a1, a2, target);
            LookAnglesBetween(from, targetAddress, (ushort*)(a1 + 12));
            scratch2->field_12 = 0;
            if (a1[2] == 1)
            {
                scratch2->field_14 += 2048;
            }
        }

        // Host 0x03 query plus the +2048 on y: HostQueryObjCmd03ById (SFX-OPS-006).
        HostGetObjectRotationById(*a1, (long)rotationAddress);
        int duration = (int)scratch2->field_4;
        rotation[0] = (short)(rotation[0] + scratch2->field_12 / duration);
        rotation[1] = (short)(rotation[1] + scratch2->field_14 / duration);
        rotation[2] = (short)(rotation[2] + scratch2->field_16 / duration);
        HostSetObjRotation(*a1, (long)rotationAddress);
        if (frame == (int)scratch2->field_4 - 1)
        {
            byte_180323186 = 1;
            return 1;
        }

        return 0;
    }

    /// <summary>
    /// Shared frame-0 block of the near-object steps: from = position of a1[0], aim = ObjAimPointById(a1[0], a1[1])
    /// with y = from.y, angles = LookAnglesBetween(aim, from) (SFX-OPS-007), target +20 = SfxRotTransPointZ(aim,
    /// angles, s16 +4).
    /// </summary>
    private static void SfxOpCtx16BlendNearObjAimTarget(byte* a1, Span<short> from)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpCtx16BlendNearObjAimTarget(a1, from)", (nint)a1, from.Length);
        short* fromAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(from));
        Span<short> aim = stackalloc short[4];
        short* aimAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(aim));
        short* angles = stackalloc short[4];
        HostQueryObjPosition(PsxGetContextQword(*a1), (long)fromAddress);
        ObjAimPointById(*a1, a1[1], aimAddress);
        aim[1] = from[1];
        LookAnglesBetween(aimAddress, fromAddress, (ushort*)angles);
        SfxRotTransPointZ(aimAddress, angles, *(short*)(a1 + 4), (ushort*)(a1 + 20));
    }

    /// <summary>s16 x/y/z at +56/+58/+60 of the record at PSX address *(a2 + 12) (a null record faults, as the original).</summary>
    private static void SfxOpCtx16BlendNearObjAnchor(long a2, Span<short> target)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpCtx16BlendNearObjAnchor(a2, target)", a2, target.Length);
        var record = (short*)PsxAddrToPtr((long)unk_180576A10, *(uint*)(a2 + 12));
        target[0] = record[28];
        target[1] = record[29];
        target[2] = record[30];
    }

    /// <summary>Target of SfxOpTurnTowardsStep: the anchor record for a1[1] == 255, else the host 0x01 query of a1[1].</summary>
    private static void SfxOpCtx16BlendNearObjTurnTarget(byte* a1, long a2, Span<short> target)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpCtx16BlendNearObjTurnTarget(a1, a2, target)", (nint)a1, a2, target.Length);
        short* targetAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(target));
        if (a1[1] == 255)
        {
            SfxOpCtx16BlendNearObjAnchor(a2, target);
        }
        else
        {
            HostGetObjectPositionById(a1[1], (long)targetAddress);
        }
    }
}
