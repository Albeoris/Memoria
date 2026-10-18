using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// Host object commands: each resolves the context object of an index (PsxGetContextQword) and calls the host
/// callback (qword_1801C1DE8) with (command &lt;&lt; 24) | object id (u16 at +24), or the bare command for a null object.
/// Plus the PSX-style LCG random helpers and a 3D distance.
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>When dword_1803678E8 == 435: a1[0], a1[2], .., a1[10] = 128..133 and returns 133; otherwise 0.</summary>
    public static long FillIdTable435(ushort* a1)
    {
        Recompiler.Runtime.Tracer.Enter("FillIdTable435(a1)", (nint)a1);
        if (CurrentEffectId == (int)SpecialEffectId.Special_Necron_Death)
        {
            a1[0] = 128;
            a1[2] = 129;
            a1[4] = 130;
            a1[6] = 131;
            a1[8] = 132;
            a1[10] = 133;
            return 133;
        }

        // The original leaves rax untouched here.
        return 0;
    }

    /// <summary>Flag +16 of object a1 (255 checks object 0): when set, host HostCommand.SetMotion | id of object a1 with a2.</summary>
    public static long HostObjCmd0C(long a1, byte a2)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd0C(a1, a2)", a1, a2);
        return HostObjFlaggedCommand(a1, a2);
    }

    /// <summary>Flag +16 of object a1 (255 checks object 0): when set, HostObjCmd0C(a1, a2).</summary>
    public static long HostObjCmd0CIfFlag(long a1, byte a2)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd0CIfFlag(a1, a2)", a1, a2);
        uint index = (uint)a1;
        long result = PsxGetContextQword(index == 255 ? 0 : (int)a1);
        if (*(byte*)(result + 16) != 0)
        {
            return HostObjCmd0C(index, a2);
        }

        return result;
    }

    /// <summary>HostObjCmd0C(a1, 0); then, when a2 != 0, host HostCommand.SetRotate | id (0).</summary>
    public static long HostObjCmd0CReset(long a1, sbyte a2)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd0CReset(a1, a2)", a1, a2);
        uint index = (uint)a1;
        long result = HostObjCmd0C(a1, 0);
        if (a2 != 0)
        {
            result = PsxGetContextQword((int)index);
            if (BattleCallbackAddress != null)
            {
                return HostObjCallback(ObjCommand(result, HostCommand.SetRotate), 0, 0, 0, 0, 0);
            }
        }

        return result;
    }

    /// <summary>HostObjCmd0C with a 32-bit argument.</summary>
    public static long HostObjCmd0CU32(long a1, uint a2)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd0CU32(a1, a2)", a1, a2);
        return HostObjFlaggedCommand(a1, a2);
    }

    /// <summary>Host HostCommand.SetMotionFrame | id (a2); returns the object when no callback is installed.</summary>
    public static long HostObjCmd0B(long a1, uint a2)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd0B(a1, a2)", a1, a2);
        return HostObjSimpleCommand(a1, HostCommand.SetMotionFrame, a2);
    }

    /// <summary>Host HostCommand.GetMotionFrameMax | id; 1 when no callback is installed.</summary>
    public static long HostObjCmd09(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd09(a1)", a1);
        long obj = PsxGetContextQword((int)a1);
        if (BattleCallbackAddress == null)
        {
            return 1;
        }

        return HostObjCallback(ObjCommand(obj, HostCommand.GetMotionFrameMax), 0, 0, 0, 0, 0);
    }

    /// <summary>Host HostCommand.GetMotionFrame | id; 0 when no callback is installed.</summary>
    public static long HostObjCmd0A(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd0A(a1)", a1);
        long obj = PsxGetContextQword((int)a1);
        if (BattleCallbackAddress == null)
        {
            return 0;
        }

        return HostObjCallback(ObjCommand(obj, HostCommand.GetMotionFrame), 0, 0, 0, 0, 0);
    }

    /// <summary>
    /// Object vector query into the s16[4] at a2: host HostCommand.GetSlave | id; when it answers nonzero and flag +16 is clear,
    /// or when there is no callback and the flag is clear, HostCommand.GetMatrix | id (0, byte +26, 0, 0, a2) and a2[1] = 0.
    /// Otherwise (flag set) HostCommand.GetPosition | id (0, 0, 0, 0, a2). Returns the callback result (or the callback pointer).
    /// </summary>
    public static long HostQueryObjPosition(long a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("HostQueryObjPosition(a1, a2)", a1, a2);
        long result = (long)BattleCallbackAddress;
        bool queryE = false;
        if (result != 0)
        {
            ulong command = ObjCommand(a1, HostCommand.GetSlave);
            if ((uint)HostObjCallback(command, 0, 0, 0, 0, 0) != 0 && *(byte*)(a1 + 16) == 0)
            {
                queryE = true;
            }

            result = (long)BattleCallbackAddress;
        }

        if (queryE || *(byte*)(a1 + 16) == 0)
        {
            if (result != 0)
            {
                result = HostObjCallback(*(ushort*)(a1 + 24) | HostCommand.GetMatrix, 0, *(byte*)(a1 + 26), 0, 0, (ulong)a2);
            }

            *(ushort*)(a2 + 2) = 0;
            return result;
        }

        if (result != 0)
        {
            return HostObjCallback(*(ushort*)(a1 + 24) | HostCommand.GetPosition, 0, 0, 0, 0, (ulong)a2);
        }

        return result;
    }

    /// <summary>
    /// Host HostCommand.GetSlave | id of object a1; when it answers 0 or flag +16 is set, HostCommand.SetRotate | id (2, 0, 0, 0, a2).
    /// Returns the last callback result.
    /// </summary>
    public static long HostSetObjRotation(int a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("HostSetObjRotation(a1, a2)", a1, a2);
        long obj = PsxGetContextQword(a1);
        long result = (long)BattleCallbackAddress;
        if (result != 0)
        {
            result = HostObjCallback(ObjCommand(obj, HostCommand.GetSlave), 0, 0, 0, 0, 0);
            if ((uint)result == 0 || *(byte*)(obj + 16) != 0)
            {
                result = (long)BattleCallbackAddress;
                if (result != 0)
                {
                    return HostObjCallback(ObjCommand(obj, HostCommand.SetRotate), 2, 0, 0, 0, (ulong)a2);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// a2 == 0: host HostCommand.SetRotate | id (1); a2 == 2: (0). a2 == 1: queries the object position p, then sets its rotation
    /// to face the origin: yaw = atan2(-p.x, -p.z) * 4096 / 2pi, pitch = -1024 - (int)(-atan2(|xz|, -p.y) * 4096 / 2pi),
    /// with |xz| = (int)sqrt(MAC1 + MAC2) from GTE SQR of IR1 = -p.x, IR2 = -p.z.
    /// </summary>
    public static long HostObjSetFacing(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjSetFacing(a1, a2)", a1, a2);
        uint index = (uint)a1;
        long result = PsxGetContextQword((int)a1);
        ulong mode;
        if (a2 == 0)
        {
            if (BattleCallbackAddress == null)
            {
                return result;
            }

            mode = 1;
        }
        else if (a2 == 1)
        {
            long obj = PsxGetContextQword((int)index);
            Span<ushort> position = stackalloc ushort[4];
            ushort* positionAddress = (ushort*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(position));
            HostQueryObjPosition(obj, (long)positionAddress);
            int y = position[1];
            int dx = -position[0];
            int dz = -position[2];
            dword_180211FE4 = dx;
            *(int*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref qword_180211FE8) = dz;
            GteSqr();

            Span<ushort> rotation = stackalloc ushort[4];
            ushort* rotationAddress = (ushort*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(rotation));
            rotation[1] = (ushort)Cvttsd2si(-Math.Atan2(dx, dz) / 6.283185307179586 * -4096.0);
            rotation[2] = 0;
            double horizontal = Cvttsd2si(Math.Sqrt(unchecked(dword_180212028 + dword_180212024)));
            rotation[0] = (ushort)(-1024 - Cvttsd2si(-Math.Atan2(horizontal, -y) / 6.283185307179586 * 4096.0));
            return HostSetObjRotation((int)index, (long)rotationAddress);
        }
        else if (a2 == 2)
        {
            if (BattleCallbackAddress == null)
            {
                return result;
            }

            mode = 0;
        }
        else
        {
            return result;
        }

        return HostObjCallback(ObjCommand(result, HostCommand.SetRotate), mode, 0, 0, 0, 0);
    }

    /// <summary>Host HostCommand.StopMotion | id (1); 0 when no callback is installed.</summary>
    public static long HostObjCmd0DOn(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd0DOn(a1)", a1);
        long obj = PsxGetContextQword((int)a1);
        if (BattleCallbackAddress == null)
        {
            return 0;
        }

        return HostObjCallback(ObjCommand(obj, HostCommand.StopMotion), 1, 0, 0, 0, 0);
    }

    /// <summary>Host HostCommand.StopMotion | id (a2); returns the object when no callback is installed.</summary>
    public static long HostObjCmd0D(long a1, uint a2)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd0D(a1, a2)", a1, a2);
        return HostObjSimpleCommand(a1, HostCommand.StopMotion, a2);
    }

    /// <summary>Host HostCommand.SetDisappear | id (a2 == 0); returns the object when no callback is installed.</summary>
    public static long HostObjCmd12(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd12(a1, a2)", a1, a2);
        return HostObjSimpleCommand(a1, HostCommand.SetDisappear, a2 == 0 ? 1u : 0u);
    }

    /// <summary>Host HostCommand.ShowWeapon | id (a2); returns the object when no callback is installed.</summary>
    public static long HostObjCmd13(long a1, uint a2)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd13(a1, a2)", a1, a2);
        return HostObjSimpleCommand(a1, HostCommand.ShowWeapon, a2);
    }

    /// <summary>1 when host HostCommand.CheckStatus | id (0, 4355, 512) answers 0 in its low 32 bits, or when there is no callback.</summary>
    public static long HostObjQuery14IsZero(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjQuery14IsZero(a1)", a1);
        long obj = PsxGetContextQword((int)a1);
        if (BattleCallbackAddress == null)
        {
            return 1;
        }

        return (uint)HostObjCallback(ObjCommand(obj, HostCommand.CheckStatus), 0, 4355, 512, 0, 0) == 0 ? 1 : 0;
    }

    /// <summary>(int)sqrt(dx*dx + dy*dy + dz*dz) of two s16 vectors (32-bit wrapping sums), 1 instead of 0.</summary>
    public static long VecDistance3(short* a1, short* a2)
    {
        Recompiler.Runtime.Tracer.Enter("VecDistance3(a1, a2)", (nint)a1, (nint)a2);
        int dx = a2[0] - a1[0];
        int dy = a2[1] - a1[1];
        int dz = a2[2] - a1[2];
        uint result = (uint)Cvttsd2si(Math.Sqrt(unchecked(dx * dx + dy * dy + dz * dz)));
        return result == 0 ? 1 : result;
    }

    /// <summary>a1 when a1 == a2; otherwise one LCG step and a1 + (seed &gt;&gt; 16) % (int)(a2 - a1) (64-bit signed remainder).</summary>
    public static long PsxRandRange(uint a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("PsxRandRange(a1, a2)", a1, a2);
        if (a1 == (uint)a2)
        {
            return a1;
        }

        dword_1803231DC = unchecked(1103515245 * dword_1803231DC + 12345);
        long high = (ushort)((uint)dword_1803231DC >> 16);
        return unchecked((uint)(high % (int)((uint)a2 - a1)) + a1);
    }

    /// <summary>
    /// One LCG step; r = seed &gt;&gt; 16, counter c = (dword_1803231E8 + 1) &amp; 7 (stored back); a1 (s16 x, s16 z, s16 0):
    /// x = (r &amp; 0x3FF) + ((c &amp; 6) &lt;&lt; 9), z = ((r &gt;&gt; 6) &amp; 0x3FF), negated when c is even. Returns r &amp; 0x3FF.
    /// </summary>
    public static long PsxRandScatterVec(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("PsxRandScatterVec(a1)", a1);
        int product = unchecked(1103515245 * dword_1803231DC);
        int counter = (sbyte)dword_1803231E8 + 1;
        *(ushort*)(a1 + 4) = 0;
        dword_1803231DC = unchecked(product + 12345);
        int random = (int)((uint)unchecked(product + 12345) >> 16);
        long result = random & 0x3FF;
        int z = (random >> 6) & 0x3FF;
        dword_1803231E8 = counter & 7;
        if ((counter & 1) == 0)
        {
            z = -z;
        }

        *(uint*)a1 = (uint)((int)result + ((counter & 6) << 9)) | (uint)(z << 16);
        return result;
    }

    /// <summary>Shared body of HostObjCmd0C/HostObjCmd0CU32.</summary>
    private static long HostObjFlaggedCommand(long a1, ulong argument)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjFlaggedCommand(a1, argument)", a1, argument);
        uint index = (uint)a1;
        long result = PsxGetContextQword(index == 255 ? 0 : (int)a1);
        if (*(byte*)(result + 16) != 0)
        {
            result = PsxGetContextQword((int)index);
            if (BattleCallbackAddress != null)
            {
                return HostObjCallback(ObjCommand(result, HostCommand.SetMotion), argument, 0, 0, 0, 0);
            }
        }

        return result;
    }

    /// <summary>Host command | id with one argument; returns the object when no callback is installed.</summary>
    private static long HostObjSimpleCommand(long a1, uint command, uint argument)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjSimpleCommand(a1, command, argument)", a1, command, argument);
        long result = PsxGetContextQword((int)a1);
        if (BattleCallbackAddress != null)
        {
            return HostObjCallback(ObjCommand(result, command), argument, 0, 0, 0, 0);
        }

        return result;
    }

    private static long HostObjCallback(ulong code, ulong a, ulong b, ulong c, uint d, ulong e)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCallback(code, a, b, c, d, e)", code, a, b, c, d, e);
        var callback = (BattleCallbackFn)BattleCallbackAddress;
        return Native.InvokeBattleCallback((long)callback, code, a, b, c, d, e);
    }

    /// <summary>x64 cvttsd2si: truncation toward zero, 0x80000000 for NaN and out-of-range values.</summary>
    private static int Cvttsd2si(double value)
    {
        Recompiler.Runtime.Tracer.Enter("Cvttsd2si(value)", value);
        return value is >= -2147483648.0 and < 2147483648.0 ? (int)value : int.MinValue;
    }
}
