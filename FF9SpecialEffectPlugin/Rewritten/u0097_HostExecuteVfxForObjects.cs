namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>Sends host message HostCommand.LbossFlagEnable with zero arguments; returns its result, or 0 without a callback.</summary>
    public static long HostEnableLastBossFlag()
    {
        Recompiler.Runtime.Tracer.Enter("HostEnableLastBossFlag()");
        long result = (long)BattleCallbackAddress;
        if (BattleCallbackAddress != null)
        {
            return HostCall((long)BattleCallbackAddress, HostCommand.LbossFlagEnable, 0, 0, 0);
        }

        return result;
    }

    /// <summary>
    /// <paramref name="a1"/> == 0: for object <paramref name="a2"/> asks the host 0x14 (1, 256); a non-zero answer
    /// reports 0x0C with 4; otherwise asks 0x15 and reports 0x0C with 9 (non-zero) or 0xFFFFFFFF (zero); returns 0.
    /// <paramref name="a1"/> != 0: a non-zero 0x14 (1, 256) answer or state byte +17 == 4 runs the a1 == 0 path for
    /// object 16 and returns 1; otherwise, when object 16 is active, sends 0x0C (28) for it; returns 0.
    /// </summary>
    public static long HostObjectCheckAction(int a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjectCheckAction(a1, a2)", a1, a2);
        long obj = PsxGetContextQword(a2);
        if (a1 == 0)
        {
            if (BattleCallbackAddress == null)
            {
                return 0;
            }

            bool zero = (uint)HostCall((long)BattleCallbackAddress, ObjCommand(obj, HostCommand.CheckStatus), 1, 256, 0) == 0;
            long report = (long)BattleCallbackAddress;
            ulong code;
            if (!zero)
            {
                if (BattleCallbackAddress == null)
                {
                    return 0;
                }

                code = 4;
            }
            else
            {
                if (BattleCallbackAddress == null)
                {
                    return 0;
                }

                zero = (uint)HostCall((long)BattleCallbackAddress, ObjCommand(obj, HostCommand.GetCmdIdle), 0, 0, 0) == 0;
                report = (long)BattleCallbackAddress;
                if (BattleCallbackAddress == null)
                {
                    return 0;
                }

                code = zero ? 0xFFFFFFFFUL : 9UL;
            }

            // The original passes only (message, code, 0); the remaining arguments are leftovers (DIV-022 pattern).
            HostCall(report, ObjCommand(obj, HostCommand.SetMotion), code, 0, 0);
            return 0;
        }

        if ((BattleCallbackAddress != null
                && (uint)HostCall((long)BattleCallbackAddress, ObjCommand(obj, HostCommand.CheckStatus), 1, 256, 0) != 0)
            || *(byte*)(obj + 17) == 4)
        {
            HostObjectCheckAction(0, 16);
            return 1;
        }

        if (*(byte*)(PsxGetContextQword(16) + 16) == 0)
        {
            return 0;
        }

        if (*(byte*)(PsxGetContextQword(16) + 16) == 0)
        {
            return 0;
        }

        long active = PsxGetContextQword(16);
        if (BattleCallbackAddress == null)
        {
            return 0;
        }

        HostCall((long)BattleCallbackAddress, ObjCommand(active, HostCommand.SetMotion), 28, 0, 0);
        return 0;
    }

    /// <summary>1 when (object <paramref name="a1"/> active ? its byte +32 : 19) equals byte <paramref name="a2"/> of the table at 0x18004B680.</summary>
    public static long IsObjectStateEqual(int a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("IsObjectStateEqual(a1, a2)", a1, a2);
        long obj = PsxGetContextQword(a1);
        int state = *(byte*)(obj + 16) != 0 ? *(byte*)(obj + 32) : 19;
        return state == unk_18004B680[a2] ? 1 : 0;
    }

    /// <summary>
    /// Looks up, in the list at qword_1800694F0 (next pointer at +0, u16 id at +24), the node whose id is byte
    /// +37 + <paramref name="a1"/> of the record at PSX address off_180066C68[3] and stores it (0 when absent) at +184
    /// of that record. Returns what the original leaves in rax: the address bits it tested (RAM: addr &amp; 0xFFFFFFF;
    /// segment: the record pointer; scratchpad: addr &amp; 0xC00000).
    /// </summary>
    public static long SelectReg3ObjectFromList(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("SelectReg3ObjectFromList(a1)", a1);
        uint address = ((uint*)off_180066C68)[3];
        byte* record = ResolveGlobalPsxAddress(address);
        long node = BattleObjectList;
        if (node != 0)
        {
            int id = record[a1 + 37];
            while (*(ushort*)(node + 24) != id)
            {
                node = *(long*)node;
                if (node == 0)
                {
                    break;
                }
            }
        }

        byte* target = ResolveGlobalPsxAddress(address);
        *(long*)(target + 184) = node;
        if (address >> 24 == 128)
        {
            return address & 0xFFFFFFF;
        }

        if ((address & 0xC00000) == 0xC00000)
        {
            return (long)target;
        }

        return address & 0xC00000;
    }

    /// <summary>Host message HostCommand.EyeBlink | id of object <paramref name="objectIndex"/> with (a2, a3); returns its result, or the object pointer without a callback.</summary>
    public static long HostControlEyeTextureAnimation(int objectIndex, uint operation, uint textureIndex)
    {
        Recompiler.Runtime.Tracer.Enter("HostControlEyeTextureAnimation(objectIndex, operation, textureIndex)", objectIndex, operation, textureIndex);
        long result = PsxGetContextQword(objectIndex);
        if (BattleCallbackAddress != null)
        {
            return HostCall((long)BattleCallbackAddress, ObjCommand(result, HostCommand.EyeBlink), operation, textureIndex, 0);
        }

        return result;
    }

    /// <summary>
    /// 0 when object 16 is active or byte +37 of the record at off_180066C68[3] is 0; otherwise 1 when bit 15 of the
    /// dword +28 of the record at off_180066C68[0] is set.
    /// </summary>
    public static long IsReg0Flag8000Set()
    {
        Recompiler.Runtime.Tracer.Enter("IsReg0Flag8000Set()");
        if (*(byte*)(PsxGetContextQword(16) + 16) != 0)
        {
            return 0;
        }

        uint* table = (uint*)off_180066C68;
        if (ResolveGlobalPsxAddress(table[3])[37] == 0)
        {
            return 0;
        }

        return (*(uint*)(ResolveGlobalPsxAddress(table[0]) + 28) & 0x8000) != 0 ? 1 : 0;
    }

    /// <summary>
    /// Unless <see cref="IsReg0Flag8000Set"/>: <paramref name="objectIndex"/> == -1 sends host message 0x17 for each of the
    /// (signed byte +36) object pointers at +88, +96, ... of the record at off_180066C68[3] and returns that PSX
    /// address; otherwise sends 0x17 for object a1 and returns the callback result (the object pointer without one).
    /// </summary>
    public static long HostExecuteVfxForObjects(int objectIndex)
    {
        Recompiler.Runtime.Tracer.Enter("HostExecuteVfxForObjects(objectIndex)", objectIndex);
        long result = IsReg0Flag8000Set();
        if ((uint)result != 0)
        {
            return result;
        }

        if (objectIndex == -1)
        {
            long callback = (long)BattleCallbackAddress;
            long offset = 88;
            for (int i = 0; ; i++)
            {
                uint address = ((uint*)off_180066C68)[3];
                result = address;
                if (i >= (sbyte)ResolveGlobalPsxAddress(address)[36])
                {
                    return result;
                }

                long obj = *(long*)(ResolveGlobalPsxAddress(address) + offset);
                if (callback != 0)
                {
                    HostCall(callback, ObjCommand(obj, HostCommand.ExecVfx), 0, 0, 0);
                    callback = (long)BattleCallbackAddress;
                }

                offset += 8;
            }
        }

        result = PsxGetContextQword(objectIndex);
        if (BattleCallbackAddress != null)
        {
            return HostCall((long)BattleCallbackAddress, ObjCommand(result, HostCommand.ExecVfx), 0, 0, 0);
        }

        return result;
    }

    /// <summary>Calls the host callback <paramref name="callback"/> with (message, a, b, c, 0, 0).</summary>
    private static long HostCall(long callback, ulong message, ulong a, ulong b, ulong c)
    {
        Recompiler.Runtime.Tracer.Enter("HostCall(callback, message, a, b, c)", callback, message, a, b, c);
        var target = (BattleCallbackFn)callback;
        return Native.InvokeBattleCallback((long)target, message, a, b, c, 0, 0);
    }
}
