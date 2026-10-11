using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// PSX task handlers (PSX-TASK-002 protocol) over object pairs: flag comparisons, frame waits against per-kind
/// animation lengths, a stepped rotation (PSX-TASK-007) and a call into the per-kind script record (PSX-TASK-008).
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>
    /// Task: 20 scratch bytes {object a, object b, word_180578E00 hi, word_180578E02 lo, u32 word lo, u32 0};
    /// phase 1 sets byte_180323186 (and returns 1) when neither is 255 and both resolve to the same context object.
    /// </summary>
    public static long SfxObjPairTaskHandler(int a1, long a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjPairTaskHandler(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        Scratch2* scratch2 = (Scratch2*)a2;
        if (a1 == 0)
        {
            shape_SfxObjAnimWaitHandler_a5->union_0_4 = 20;
            return 0;
        }

        if (a1 != 1)
        {
            return SfxOpTurnTowardsStep((byte*)a2, a3, (int*)a4);
        }

        byte* scratch = (byte*)a2;
        byte first = *a5;
        *scratch = first;
        byte second = a5[4];
        scratch[1] = second;
        ushort word = (ushort)word_180578E00;
        scratch2->field_4 = (byte)word;
        scratch[2] = (byte)(word >> 8);
        scratch2->field_8 = 0;
        scratch[3] = (byte)word_180578E02;
        if (first != 255 && second != 255 && PsxGetContextQword(first) == PsxGetContextQword(second))
        {
            byte_180323186 = 1;
            return 1;
        }

        byte_180323186 = 0;
        return 0;
    }

    /// <summary>
    /// When a1[2] != 0 and object a1[1]'s flag +16 is set: host 0x04000000 | id, done. Else frame 0 stores the host
    /// 0x03000000 rotation (mode 0) into a1+8; every frame reads the current rotation (mode 1), adds a1+8 / count
    /// (signed, y also +2048) and sets it; done (byte_180323186 = 1) when *a3 == count - 1.
    /// </summary>
    public static long SfxObjRotateStep(byte* a1, long a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjRotateStep(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        Scratch* scratch = (Scratch*)a1;
        int frame = *a3;
        long obj = PsxGetContextQword(*a1);
        if (a1[2] != 0)
        {
            byte gate = a1[1];
            if (*(byte*)(PsxGetContextQword(gate == 255 ? 0 : gate) + 16) != 0)
            {
                if (BattleCallbackAddress != null)
                {
                    HostObjCallback(ObjCommand(obj, HostCommand.SetRotate), 0, 0, 0, 0, 0);
                }

                byte_180323186 = 1;
                return 1;
            }
        }

        if (frame == 0 && BattleCallbackAddress != null)
        {
            HostObjCallback(ObjCommand(obj, HostCommand.GetRotate), 0, 0, 0, 0, (ulong)(a1 + 8));
        }

        long current = PsxGetContextQword(*a1);

        // DIV-052: the original leaves this stack vector uninitialised when no host callback fills it.
        Span<short> rotation = stackalloc short[4];
        short* rotationAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(rotation));
        rotation[0] = 0;
        rotation[1] = 0;
        rotation[2] = 0;
        rotation[3] = 0;
        if (BattleCallbackAddress != null)
        {
            HostObjCallback(ObjCommand(current, HostCommand.GetRotate), 1, 0, 0, 0, (ulong)rotationAddress);
        }

        int count = (int)scratch->field_4;
        rotation[0] = (short)(rotation[0] + scratch->field_8 / count);
        rotation[1] = (short)(rotation[1] + 0x800 + scratch->field_10 / count);
        rotation[2] = (short)(rotation[2] + scratch->field_12 / count);
        HostSetObjRotation(*a1, (long)rotationAddress);
        if (frame == (int)scratch->field_4 - 1)
        {
            byte_180323186 = 1;
            return 1;
        }

        return 0;
    }

    /// <summary>
    /// Task: 16 scratch bytes {object, gate, word hi, -, u32 word lo, rotation delta}; phase 1 returns 1 only when the
    /// high byte of word_180578E00 is 2 and the gate object's flag +16 is set (byte_180323186 mirrors it).
    /// </summary>
    public static long SfxObjRotateHandler(int a1, long a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjRotateHandler(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        Scratch* shape_SfxObjRotateStep_a1 = (Scratch*)a2;
        if (a1 == 0)
        {
            shape_SfxObjAnimWaitHandler_a5->union_0_4 = 16;
            return 0;
        }

        if (a1 != 1)
        {
            return SfxObjRotateStep((byte*)a2, a2, (int*)a4);
        }

        byte* scratch = (byte*)a2;
        *scratch = *a5;
        byte gate = a5[4];
        scratch[1] = gate;
        ushort word = (ushort)word_180578E00;
        shape_SfxObjRotateStep_a1->field_4 = (byte)word;
        scratch[2] = (byte)(word >> 8);
        if ((byte)(word >> 8) == 2 && *(byte*)(PsxGetContextQword(gate == 255 ? 0 : gate) + 16) != 0)
        {
            byte_180323186 = 1;
            return 1;
        }

        byte_180323186 = 0;
        return 0;
    }

    /// <summary>Task: 4 scratch bytes {object, -, facing}; runs HostObjSetFacing(object, facing) and returns 1.</summary>
    public static long SfxObjSetFacingHandler(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjSetFacingHandler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        if (a1 == 0)
        {
            shape_SfxObjAnimWaitHandler_a5->union_0_4 = 4;
            return 0;
        }

        if (a1 == 1)
        {
            *a2 = *a5;
            a2[1] = a5[4];
            a2[2] = (byte)word_180578E00;
            return 0;
        }

        HostObjSetFacing(*a2, a2[2]);
        return 1;
    }

    /// <summary>1 when the flag bytes +16 of objects a1 and a2 (255 reads object 0) are equal, else 0.</summary>
    public static long ObjFlagsMatch(long a1, uint a2)
    {
        Recompiler.Runtime.Tracer.Enter("ObjFlagsMatch(a1, a2)", a1, a2);
        uint first = (uint)a1;
        byte flag = *(byte*)(PsxGetContextQword(first == 255 ? 0 : (int)first) + 16);
        return flag == *(byte*)(PsxGetContextQword(a2 == 255 ? 0 : (int)a2) + 16) ? 1 : 0;
    }

    /// <summary>
    /// Task: 4 scratch bytes {object, other, anim}; the length is word_18004AF30[19 * anim + object kind (+32)].
    /// Phase 1 finishes at once for length 0 or (anim 1 and matching flags); later phases when *a4 == length - 1.
    /// </summary>
    public static long SfxObjAnimWaitHandler(int a1, byte* a2, long a3, int* a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjAnimWaitHandler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, (nint)a4, (nint)a5);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        if (a1 == 0)
        {
            shape_SfxObjAnimWaitHandler_a5->union_0_4 = 4;
            return 0;
        }

        if (a1 == 1)
        {
            byte index = *a5;
            *a2 = index;
            a2[1] = a5[4];
            a2[2] = (byte)word_180578E00;
            ulong anim = (byte)word_180578E00;
            if (SfxObjAnimWaitHandlerLength(anim, PsxGetContextQword(index)) != 0
                && (a2[2] != 1 || (uint)ObjFlagsMatch(*a2, a2[1]) != 1))
            {
                byte_180323187 = 0;
                return 0;
            }
        }
        else
        {
            ulong anim = a2[2];
            int frame = *a4;
            if (frame != SfxObjAnimWaitHandlerLength(anim, PsxGetContextQword(*a2)) - 1)
            {
                return 0;
            }
        }

        byte_180323187 = 1;
        return 1;
    }

    /// <summary>Task: 4 scratch bytes {a, b, count}; phase 1 finishes (1) when the flags of a and b differ; later phases when *a4 == count - 1.</summary>
    public static long SfxObjFlagsDifferWaitHandler(int a1, byte* a2, long a3, uint* a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjFlagsDifferWaitHandler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, (nint)a4, (nint)a5);
        return SfxObjFlagsDifferWaitHandlerBody(a1, a2, in *a4, a5, 0);
    }

    /// <summary>Task: 4 scratch bytes {a, b, count}; phase 1 finishes (1) when the flags of a and b match; later phases when *a4 == count - 1.</summary>
    public static long SfxObjFlagsMatchWaitHandler(int a1, byte* a2, long a3, uint* a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjFlagsMatchWaitHandler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, (nint)a4, (nint)a5);
        return SfxObjFlagsDifferWaitHandlerBody(a1, a2, in *a4, a5, 1);
    }

    /// <summary>
    /// Task: 4 scratch bytes {object}; sets dword_180578E04 = 1, saves qword_180323198 in qword_180578E08 and jumps
    /// it to PsxFindTableRecord(word_18004AE80[kind]) with kind = object +33 (>= 85 -> 0), also kept in word_180211E5C.
    /// </summary>
    public static long SfxObjCallKindScriptHandler(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjCallKindScriptHandler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
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

        long obj = PsxGetContextQword(*a2);
        dword_180578E04 = 1;
        int kind = *(byte*)(obj + 33);
        if (kind >= 0x55)
        {
            kind = 0;
        }

        ushort record = ((ushort*)unk_18004AE80)[kind];
        qword_180578E08 = qword_180323198;
        word_180211E5C = (short)record;
        qword_180323198 = (long)PsxFindTableRecord(record);
        return 1;
    }

    /// <summary>Task: 4 scratch bytes {a, b}; phase 1 returns ObjFlagsMatch(a, b); later phases return 1.</summary>
    public static long SfxObjFlagsMatchHandler(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjFlagsMatchHandler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        if (a1 == 0)
        {
            shape_SfxObjAnimWaitHandler_a5->union_0_4 = 4;
            return 0;
        }

        if (a1 != 1)
        {
            return 1;
        }

        byte first = *a5;
        *a2 = first;
        byte second = a5[4];
        a2[1] = second;
        return ObjFlagsMatch(first, second);
    }

    /// <summary>word_18004AF30[19 * anim + kind byte at obj + 32], read like the original (no bounds check).</summary>
    private static int SfxObjAnimWaitHandlerLength(ulong anim, long obj)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjAnimWaitHandlerLength(anim, obj)", anim, obj);
        return ((ushort*)unk_18004AF30)[19 * anim + *(byte*)(obj + 32)];
    }

    /// <summary>Shared body of the two flag waits; matchResult is the phase-1 result when the flags are equal.</summary>
    private static long SfxObjFlagsDifferWaitHandlerBody(int a1, byte* a2, in uint a4, byte* a5, byte matchResult)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjFlagsDifferWaitHandlerBody(a1, a2, a4, a5, matchResult)", a1, (nint)a2, a4, (nint)a5, matchResult);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        if (a1 == 0)
        {
            shape_SfxObjAnimWaitHandler_a5->union_0_4 = 4;
            return 0;
        }

        if (a1 == 1)
        {
            byte first = *a5;
            *a2 = first;
            byte second = a5[4];
            a2[1] = second;
            a2[2] = (byte)word_180578E00;
            byte flag = *(byte*)(PsxGetContextQword(first == 255 ? 0 : first) + 16);
            byte result = flag == *(byte*)(PsxGetContextQword(second == 255 ? 0 : second) + 16)
                ? matchResult
                : (byte)(1 - matchResult);
            byte_180323187 = result;
            return result;
        }

        if (a4 == (uint)(a2[2] - 1))
        {
            byte_180323187 = 1;
            return 1;
        }

        return 0;
    }
}
