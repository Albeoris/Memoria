using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Position of PSX object <paramref name="a1"/> as the host sees it. With <paramref name="a2"/> != 0 the host
    /// callback (message HostCommand.GetMatrix | object id, sub-id 2, object byte +34) fills a 32-byte PSX MATRIX that becomes
    /// the GTE rotation/translation, the object's position (+4/+6 of the record at +8; bit 3 of its first byte
    /// selects whether +4 is X or Y) is transformed by RTV0TR and IR1..IR3 are stored in a3[0..2]. With a2 == 0 the
    /// callback writes a3 itself (sub-id 0). Either way the GTE rotation/translation is then reloaded from the camera
    /// matrix at off_180066C68[5]; returns the last translation word.
    /// </summary>
    public static long GetObjectHostPosition(int a1, int a2, ushort* a3)
    {
        Recompiler.Runtime.Tracer.Enter("GetObjectHostPosition(a1, a2, a3)", a1, a2, (nint)a3);
        long objectRecord = PsxGetContextQword(a1);
        uint objectByte = *(byte*)(objectRecord + 34);
        long idSource = PsxGetContextQword(a1);
        var callback = (BattleCallbackFn)BattleCallbackAddress;
        ulong message = idSource != 0 ? *(ushort*)(idSource + 24) | HostCommand.GetMatrix : HostCommand.GetMatrix;
        if (a2 != 0)
        {
            Stack32_GetObjectHostPosition_matrix matrix = default;
            Unsafe.InitBlock(((byte*)&matrix), 0, 32);
            if (callback != null)
            {
                Native.InvokeBattleCallback((long)callback, message, 2, objectByte, 0, 0, (ulong)(byte*)&matrix);
            }

            byte* gte = xmmword_180211F40;
            dword_180211F50 = matrix.field_16;
            *(UInt128*)gte = *(UInt128*)((byte*)&matrix);
            qword_180211F54 = matrix.field_20;
            dword_180211F5C = matrix.field_28;

            long position = *(long*)(objectRecord + 8);
            ulong vector;
            if ((*(byte*)position & 8) != 0)
            {
                vector = *(ushort*)(position + 4);
            }
            else
            {
                vector = (ulong)*(ushort*)(position + 4) << 16;
            }

            vector |= (ulong)*(ushort*)(position + 6) << 32;
            qword_180211FC0 = (long)vector;
            GteRtV0Tr();
            long ir12 = *(long*)Unsafe.AsPointer(ref dword_180212024);
            int ir3 = dword_18021202C;
            a3[0] = (ushort)ir12;
            a3[1] = (ushort)(ir12 >> 32);
            a3[2] = (ushort)ir3;
        }
        else if (callback != null)
        {
            Native.InvokeBattleCallback((long)callback, message, 0, objectByte, 0, 0, (ulong)a3);
        }

        uint* table = (uint*)off_180066C68;
        byte* camera = ResolveGlobalPsxAddress(table[5]);
        Camera7* camera7 = (Camera7*)camera;
        *(UInt128*)xmmword_180211F40 = *(UInt128*)camera;
        dword_180211F50 = camera7->field_16;
        byte* translation = ResolveGlobalPsxAddress(table[5]);
        Camera7* camera72 = (Camera7*)translation;
        qword_180211F54 = camera72->field_20;
        uint last = camera72->field_28;
        dword_180211F5C = (int)last;
        return last;
    }

    /// <summary>
    /// Host message sequence for object <paramref name="obj"/> (id at +24, 0 without an object): 0x16 query first
    /// (non-zero ends the call unless the force byte +16 is set), then 0x11; for <paramref name="fade"/> &gt;= 8 a
    /// non-zero 0x11 answer sends 0x14 (0x4000) and, if that is zero, 0x12; for fade &lt; 8 a zero 0x11 answer sends
    /// 0x12 (1). A zero 0x11 answer (fade &gt;= 8) or the fade &lt; 8 path ends with ShowMesh (4, fade). Returns what the original
    /// leaves in rax: the callback pointer or the last callback result.
    /// </summary>
    public static long HostSetObjectMeshFade(long obj, int fade)
    {
        Recompiler.Runtime.Tracer.Enter("HostSetObjectMeshFade(obj, fade)", obj, fade);
        long result = (long)BattleCallbackAddress;
        if (BattleCallbackAddress != null)
        {
            result = HostObjectMessage(obj, HostCommand.GetSlave, 0, 0);
            if ((uint)result != 0 && *(byte*)(obj + 16) == 0)
            {
                return result;
            }

            result = (long)BattleCallbackAddress;
        }

        if (fade >= 8)
        {
            if (result == 0)
            {
                return result;
            }

            bool zero = (uint)HostObjectMessage(obj, HostCommand.GetDisappear, 0, 0, result) == 0;
            result = (long)BattleCallbackAddress;
            if (zero)
            {
                return HostApplySevereMeshFade(obj, fade, result);
            }

            if (BattleCallbackAddress != null)
            {
                result = HostObjectMessage(obj, HostCommand.CheckStatus, 0, 0x4000);
                if ((uint)result == 0)
                {
                    result = (long)BattleCallbackAddress;
                    if (BattleCallbackAddress != null)
                    {
                        return HostObjectMessage(obj, HostCommand.SetDisappear, 0, 0);
                    }
                }
            }
        }
        else if (result != 0)
        {
            result = HostObjectMessage(obj, HostCommand.GetDisappear, 0, 0, result);
            if ((uint)result == 0)
            {
                result = (long)BattleCallbackAddress;
                if (BattleCallbackAddress != null)
                {
                    HostObjectMessage(obj, HostCommand.SetDisappear, 1, 0);
                    return HostApplySevereMeshFade(obj, fade, (long)BattleCallbackAddress);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Object <paramref name="objectIndex"/> from the PSX context: <paramref name="visibility"/> &gt;= 0 runs HostSetObjectMeshFade,
    /// -1 sends 0x19 sub-id 0, any other negative value 0x19 sub-id 2. Returns the object pointer when no callback is
    /// installed, otherwise the callback result.
    /// </summary>
    public static ushort* SetObjectMeshVisibilityById(int objectIndex, int visibility)
    {
        Recompiler.Runtime.Tracer.Enter("SetObjectMeshVisibilityById(objectIndex, visibility)", objectIndex, visibility);
        var result = (ushort*)PsxGetContextQword(objectIndex);
        if (visibility >= 0)
        {
            return (ushort*)HostSetObjectMeshFade((long)result, visibility);
        }

        if (BattleCallbackAddress == null)
        {
            return result;
        }

        ulong subId = visibility == -1 ? 0UL : 2UL;
        return (ushort*)HostObjectMessage((long)result, HostCommand.ShowMesh, subId, 0);
    }

    /// <summary>Calls the host callback (or <paramref name="callback"/>) with message <paramref name="code"/> | object id and zero trailing arguments.</summary>
    private static long HostObjectMessage(long obj, uint code, ulong subId, ulong arg3, long callback = 0)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjectMessage(obj, code, subId, arg3, callback)", obj, code, subId, arg3, callback);
        var target = (BattleCallbackFn)(callback != 0 ? callback : (long)BattleCallbackAddress);
        ulong message = obj != 0 ? *(ushort*)(obj + 24) | code : code;
        return Native.InvokeBattleCallback((long)target, message, subId, 0, arg3, 0, 0);
    }

    /// <summary>
    /// Final 0x19 (4, a2) message of HostSetObjectMeshFade through <paramref name="callback"/>; the original passes only
    /// three arguments (the other three are left in registers/stack), the port passes zeros. 0 callback returns 0.
    /// </summary>
    private static long HostApplySevereMeshFade(long obj, int a2, long callback)
    {
        Recompiler.Runtime.Tracer.Enter("HostApplySevereMeshFade(obj, a2, callback)", obj, a2, callback);
        if (callback == 0)
        {
            return callback;
        }

        var target = (BattleCallbackFn)callback;
        ulong message = obj != 0 ? *(ushort*)(obj + 24) | HostCommand.ShowMesh : HostCommand.ShowMesh;
        return Native.InvokeBattleCallback((long)target, message, 4, (uint)a2, 0, 0, 0);
    }
}
