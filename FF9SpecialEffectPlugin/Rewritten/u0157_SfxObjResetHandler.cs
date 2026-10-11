namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// PSX task handlers (PSX-TASK-002 protocol: phase 0 requests scratch bytes in *a5, phase 1 copies the
/// arguments a5[0]/a5[4] into the scratch a2, later phases run) that drive host object commands, plus the
/// flag-gated host commands and frame waits they call. Waits compare the frame counter signed (PSX-TASK-004).
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>Flag +16 of object a1 (255 checks object 0): when set, host 0x1E000000 | id of object a1 with a1.</summary>
    public static long HostObjCmd1EIfFlag(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd1EIfFlag(a1)", a1);
        return HostObjCmd1EIfFlagCommand((uint)a1, HostCommand.SfxPlayerAttack, (uint)a1, 0);
    }

    /// <summary>Task: 4 scratch bytes {object, mode}; phase 1 returns 0 (keep) when the object's flag is set and mode != 1; runs HostObjCmd1EIfFlag.</summary>
    public static long SfxObjCmd1EHandler(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjCmd1EHandler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
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
            if (*(byte*)(PsxGetContextQword(index == 255 ? 0 : index) + 16) != 0 && a2[1] != 1)
            {
                return 0;
            }
        }
        else
        {
            HostObjCmd1EIfFlag(*a2);
        }

        return 1;
    }

    /// <summary>
    /// Flag +16 of object a1 (255 checks object 0): when set, looks up object a2 (result unused) and sends host
    /// 0x1F000000 | id of object a1 with a3 (PSX-TASK-005). Returns the flag object, 0 without callback, or the callback result.
    /// </summary>
    public static byte* HostObjCmd1FIfFlag(long a1, uint a2, uint a3)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd1FIfFlag(a1, a2, a3)", a1, a2, a3);
        uint index = (uint)a1;
        long obj = PsxGetContextQword((int)index);
        byte* result = (byte*)PsxGetContextQword(index == 255 ? 0 : (int)index);
        if (result[16] != 0)
        {
            PsxGetContextQword((int)a2);
            result = (byte*)BattleCallbackAddress;
            if (BattleCallbackAddress != null)
            {
                return (byte*)HostObjCallback(ObjCommand(obj, HostCommand.SfxPlayHit), a3, 0, 0, 0, 0);
            }
        }

        return result;
    }

    /// <summary>Task: 4 scratch bytes {object, a, word_180578E00 (u16)}; phase 1 keeps when the flag is set and the word's high byte != 1.</summary>
    public static long SfxObjCmd1FHandler(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjCmd1FHandler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        Shape_SfxObjCmd1FHandler_a2* shape_SfxObjCmd1FHandler_a2 = (Shape_SfxObjCmd1FHandler_a2*)a2;
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
            shape_SfxObjCmd1FHandler_a2->union_2_2 = (ushort)word_180578E00;
            if (*(byte*)(PsxGetContextQword(index == 255 ? 0 : index) + 16) != 0 && a2[3] != 1)
            {
                return 0;
            }
        }
        else
        {
            HostObjCmd1FIfFlag(*a2, a2[1], a2[2]);
        }

        return 1;
    }

    /// <summary>Host 0x1D000000 | id of object a1[0] with (1110 for a1[1] == 0, 2752 for 1; nothing else), 127, dword_1803678E8; returns 1.</summary>
    public static long HostObjCmd1DSelect(byte* a1)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd1DSelect(a1)", (nint)a1);
        long obj = PsxGetContextQword(*a1);
        ulong value;
        if (a1[1] == 0)
        {
            value = 1110;
        }
        else if (a1[1] == 1)
        {
            value = 2752;
        }
        else
        {
            return 1;
        }

        if (BattleCallbackAddress != null)
        {
            HostObjCallback(ObjCommand(obj, HostCommand.SfxBattle), value, 127, (uint)CurrentEffectId, 0, 0);
        }

        return 1;
    }

    /// <summary>Task: 4 scratch bytes {object, mode}; runs HostObjCmd1DSelect.</summary>
    public static long SfxObjCmd1DHandler(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjCmd1DHandler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
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
            return 0;
        }

        return HostObjCmd1DSelect(a2);
    }

    /// <summary>Task: 4 scratch bytes {object, value}; runs HostObjCmd0CIfFlag(object, value) and returns 1.</summary>
    public static long SfxObjCmd0CHandler(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjCmd0CHandler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
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
            return 0;
        }

        HostObjCmd0CIfFlag(*a2, a2[1]);
        return 1;
    }

    /// <summary>Frame 0: HostObjCmd0CIfFlag(a1[0], a1[1]), duration = HostObjCmd09(a1[0]) - 1; 0 while *a3 &lt; duration, then byte_180323184 = 1 and 1.</summary>
    public static long SfxWaitObjCmd09(byte* a1, long a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxWaitObjCmd09(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        State3* state3 = (State3*)a1;
        int frame = *a3;
        if (frame == 0)
        {
            HostObjCmd0CIfFlag(a1[0], a1[1]);
            state3->field_4 = (uint)((int)HostObjCmd09(*a1) - 1);
        }

        if (frame < (int)state3->field_4)
        {
            return 0;
        }

        byte_180323184 = 1;
        return 1;
    }

    /// <summary>Task: 8 scratch bytes {object, value, -, -, duration}; phase 1 clears byte_180323184; runs SfxWaitObjCmd09.</summary>
    public static long SfxWaitObjCmd09Handler(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxWaitObjCmd09Handler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
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

        return SfxWaitObjCmd09(a2, (long)a2, (int*)a4);
    }

    /// <summary>SfxWaitObjCmd09 with duration HostObjCmd09 - a1[2] - 1; when done also host 0x0D000000 | id with 1.</summary>
    public static long SfxWaitObjCmd09Offset(byte* a1, long a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxWaitObjCmd09Offset(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        int frame = *a3;
        if (frame == 0)
        {
            HostObjCmd0CIfFlag(a1[0], a1[1]);
            *(int*)(a1 + 4) = (int)HostObjCmd09(*a1) - a1[2] - 1;
        }

        if (frame < *(int*)(a1 + 4))
        {
            return 0;
        }

        byte_180323184 = 1;
        HostObjCmd0D(*a1, 1);
        return 1;
    }

    /// <summary>Task: 8 scratch bytes {object, value, low byte of word_180578E00, -, duration}; runs SfxWaitObjCmd09Offset.</summary>
    public static long SfxWaitObjCmd09OffsetHandler(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxWaitObjCmd09OffsetHandler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
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
            a2[2] = (byte)word_180578E00;
            byte_180323184 = 0;
            return 0;
        }

        return SfxWaitObjCmd09Offset(a2, (long)a2, (int*)a4);
    }

    /// <summary>SfxWaitObjCmd09 with the frame-0 command HostObjCmd0CIfFlag(a1[0], 23).</summary>
    public static long SfxWaitObjCmd0C17(byte* a1, long a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxWaitObjCmd0C17(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        State3* state3 = (State3*)a1;
        int frame = *a3;
        if (frame == 0)
        {
            HostObjCmd0CIfFlag(a1[0], 23);
            state3->field_4 = (uint)((int)HostObjCmd09(*a1) - 1);
        }

        if (frame < (int)state3->field_4)
        {
            return 0;
        }

        byte_180323184 = 1;
        return 1;
    }

    /// <summary>Task: 8 scratch bytes; runs SfxWaitObjCmd0C17.</summary>
    public static long SfxWaitObjCmd0C17Handler(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxWaitObjCmd0C17Handler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
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

        return SfxWaitObjCmd0C17(a2, (long)a2, (int*)a4);
    }

    /// <summary>SfxWaitObjCmd0C17 with duration HostObjCmd09 - a1[2] - 1; when done host 0x0D000000 | id with 1, then byte_180323184 = 1.</summary>
    public static long SfxWaitObjCmd0C17Offset(byte* a1, long a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxWaitObjCmd0C17Offset(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        Shape_SfxWaitObjCmd0C17Offset_a1* shape_SfxWaitObjCmd0C17Offset_a1 = (Shape_SfxWaitObjCmd0C17Offset_a1*)a1;
        int frame = *a3;
        if (frame == 0)
        {
            HostObjCmd0CIfFlag(a1[0], 23);
            shape_SfxWaitObjCmd0C17Offset_a1->field_4 = (uint)((int)HostObjCmd09(*a1) - a1[2] - 1);
        }

        if (frame < (int)shape_SfxWaitObjCmd0C17Offset_a1->field_4)
        {
            return 0;
        }

        HostObjCmd0D(*a1, 1);
        byte_180323184 = 1;
        return 1;
    }

    /// <summary>Task: 8 scratch bytes with the low byte of word_180578E00 at +2; runs SfxWaitObjCmd0C17Offset.</summary>
    public static long SfxWaitObjCmd0C17OffsetHandler(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxWaitObjCmd0C17OffsetHandler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
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
            a2[2] = (byte)word_180578E00;
            byte_180323184 = 0;
            return 0;
        }

        return SfxWaitObjCmd0C17Offset(a2, (long)a2, (int*)a4);
    }

    /// <summary>Only while object a1[1]'s flag +16 is clear (255 checks 0): frame 0 HostObjCmd0CIfFlag(a1[0], 25), duration = HostObjCmd09 - 1; 0 while waiting.</summary>
    public static long SfxWaitObjCmd0C19(byte* a1, long a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxWaitObjCmd0C19(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        State3* state3 = (State3*)a1;
        int frame = *a3;
        byte gate = a1[1];
        if (*(byte*)(PsxGetContextQword(gate == 255 ? 0 : gate) + 16) == 0)
        {
            if (frame == 0)
            {
                HostObjCmd0CIfFlag(a1[0], 25);
                state3->field_4 = (uint)((int)HostObjCmd09(*a1) - 1);
            }

            if (frame < (int)state3->field_4)
            {
                return 0;
            }
        }

        byte_180323184 = 1;
        return 1;
    }

    /// <summary>Task: 8 scratch bytes; runs SfxWaitObjCmd0C19.</summary>
    public static long SfxWaitObjCmd0C19Handler(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxWaitObjCmd0C19Handler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
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

        return SfxWaitObjCmd0C19(a2, (long)a2, (int*)a4);
    }

    /// <summary>Task: 8 scratch bytes {object}; runs HostObjCmd0C(object, 0) then host 0x04000000 | id, returns 1.</summary>
    public static long SfxObjResetHandler(int a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjResetHandler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
        Shape_SfxObjCallKindScriptHandler_a5* shape_SfxObjCallKindScriptHandler_a5 = (Shape_SfxObjCallKindScriptHandler_a5*)a5;
        if (a1 == 0)
        {
            shape_SfxObjCallKindScriptHandler_a5->union_0_4 = 8;
            return 0;
        }

        if (a1 == 1)
        {
            *a2 = *a5;
            return 0;
        }

        HostObjCmd0CReset(*a2, 1);
        return 1;
    }

    /// <summary>Flag +16 of object a2 (255 checks object 0): when set, host 0x21000000 | id of object a2 with a3, dword_1803678E8. a1 is unused.</summary>
    public static long HostObjCmd21IfFlag(long a1, uint a2, uint a3)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd21IfFlag(a1, a2, a3)", a1, a2, a3);
        return HostObjCmd1EIfFlagCommand(a2, HostCommand.SetSpmotion, a3, (uint)CurrentEffectId);
    }

    /// <summary>Task: 4 scratch bytes {object, value}; runs HostObjCmd21IfFlag and returns 1.</summary>
    public static long SfxObjCmd21Handler(long a1, byte* a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjCmd21Handler(a1, a2, a3, a4, a5)", a1, (nint)a2, a3, a4, (nint)a5);
        Shape_SfxObjAnimWaitHandler_a5* shape_SfxObjAnimWaitHandler_a5 = (Shape_SfxObjAnimWaitHandler_a5*)a5;
        if ((uint)a1 == 0)
        {
            shape_SfxObjAnimWaitHandler_a5->union_0_4 = 4;
            return 0;
        }

        if ((uint)a1 == 1)
        {
            *a2 = *a5;
            a2[1] = a5[4];
            return 0;
        }

        HostObjCmd21IfFlag(a1, *a2, a2[1]);
        return 1;
    }

    /// <summary>
    /// Shared body of HostObjCmd1EIfFlag/HostObjCmd21IfFlag (the inlined HostObjCmd0C shape): flag +16 of object
    /// index (255 checks 0); when set, command | id of object index with (a, b). Returns the object without callback.
    /// </summary>
    private static long HostObjCmd1EIfFlagCommand(uint index, uint command, ulong a, ulong b)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd1EIfFlagCommand(index, command, a, b)", index, command, a, b);
        long result = PsxGetContextQword(index == 255 ? 0 : (int)index);
        if (*(byte*)(result + 16) != 0)
        {
            result = PsxGetContextQword((int)index);
            if (BattleCallbackAddress != null)
            {
                return HostObjCallback(ObjCommand(result, command), a, b, 0, 0, 0);
            }
        }

        return result;
    }
}
