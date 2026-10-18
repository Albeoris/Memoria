using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Script opcode "move object to point" (phase a1: 0 = state size 48 into *a5, 1 = start, else = step).
    /// State a2: +1 turning phase, +2 object id, +3 run flag, +12 target point, +28 facing data. Start takes the id
    /// from the operand byte at a5 (17 when the script object at the PSX address a3+12 has flag 4 at +64) and, unless
    /// flag 1 is set and when the object is active, asks the host whether it runs (message 0x1000). A step walks
    /// with SfxObjStepToPoint (*a4 = step counter, -1 after arrival, reporting 0x8000000 to the host), then turns
    /// with SfxObjFaceTowards until the counter reaches the turn count; returns 1 when the opcode is finished.
    /// </summary>
    public static long SfxOpMoveObjToPoint(int a1, long a2, long a3, int* a4, uint* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpMoveObjToPoint(a1, a2, a3, a4, a5)", a1, a2, a3, (nint)a4, (nint)a5);
        Shape_SfxObjCallKindScriptHandler_a5* shape_SfxObjCallKindScriptHandler_a5 = (Shape_SfxObjCallKindScriptHandler_a5*)a5;
        Shape_SfxOpMoveObjToPoint_a3* shape_SfxOpMoveObjToPoint_a3 = (Shape_SfxOpMoveObjToPoint_a3*)a3;
        byte* state = (byte*)a2;
        if (a1 == 0)
        {
            *a5 = 48;
            return 0;
        }

        if (a1 == 1)
        {
            state[1] = 0;
            state[2] = shape_SfxObjCallKindScriptHandler_a5->union_0_1;
            byte* script = ResolveGlobalPsxAddress(shape_SfxOpMoveObjToPoint_a3->field_12);
            if ((script[64] & 4) != 0)
            {
                state[2] = 17;
            }

            script = ResolveGlobalPsxAddress(shape_SfxOpMoveObjToPoint_a3->field_12);
            if ((script[64] & 1) == 0 && *(byte*)(PsxGetContextQword(state[2]) + 16) != 0)
            {
                long obj = PsxGetContextQword(state[2]);
                state[3] = HostCallbackSend(obj, 0x1000u);
                return 0;
            }

            return 1;
        }

        int counter = *a4;
        if (state[1] == 0)
        {
            if ((uint)SfxObjStepToPoint(state[2], counter, (short*)(state + 12), state[3]) != 0)
            {
                state[1] = 1;
                counter = -1;
                long obj = PsxGetContextQword(state[2]);
                if (BattleCallbackAddress != null
                    && (uint)HostCall((long)BattleCallbackAddress, ObjCommand(obj, HostCommand.GetAutoPotionCommand), 0, 0, 0) != 0)
                {
                    HostObjCmd0CIfFlag(state[2], 9);
                    return 1;
                }

                if (state[3] == 0)
                {
                    HostObjCmd0CIfFlag(state[2], 0x20);
                }

                HostObjCmd0CIfFlag(state[2], 0x20);
            }

            *a4 = counter;
            return 0;
        }

        SfxObjFaceTowards(state[2], null, (ushort*)(state + 28), counter);
        if (counter < (int)(HostObjCmd09(state[2]) - 1))
        {
            *a4 = counter;
            return 0;
        }

        byte id = state[2];
        HostObjCmd0C(id, 0);
        long target = PsxGetContextQword(id);
        if (BattleCallbackAddress != null)
        {
            HostCall((long)BattleCallbackAddress, ObjCommand(target, HostCommand.SetRotate), 0, 0, 0);
        }

        return 1;
    }

    /// <summary>Script opcode: on start (a1 == 1) stores operand bytes a5[0] and a5[4] at 0x180620B80/81; returns 1.</summary>
    public static long SfxOpStoreArgBytes(int a1, long a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpStoreArgBytes(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        if (a1 == 1)
        {
            byte_180620B80 = a5[0];
            byte_180620B81 = a5[4];
        }

        return 1;
    }

    /// <summary>
    /// Script opcode: on start copies the u16 variable a5[1] &amp; 0x1F of the table at qword_180323260 into the entry
    /// at byte offset dword_1802228D0[a5[0]] of the table 0x1802228D0: field +2 for mode (a5[1] &gt;&gt; 5) &amp; 7
    /// 0..1, field +4 for mode 2..3, nothing otherwise. Returns 1.
    /// </summary>
    public static long SfxOpCopyVarToEntry(int a1, long a2, long a3, long a4, int* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpCopyVarToEntry(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        if (a1 == 1)
        {
            byte* table = (byte*)Unsafe.AsPointer(ref dword_1802228D0);
            long offset = ((int*)table)[a5[0]];
            int variable = a5[1] & 0x1F;
            int mode = (a5[1] >> 5) & 7;
            if (mode <= 1)
            {
                *(ushort*)(table + offset + 2) = ((ushort*)&((Shape_qword_180323260*)qword_180323260)->field_0)[variable];
            }
            else if (mode <= 3)
            {
                *(ushort*)(table + offset + 4) = ((ushort*)&((Shape_qword_180323260*)qword_180323260)->field_0)[variable];
            }
        }

        return 1;
    }
}
