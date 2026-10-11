namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Resolves the object at PSX address *a3 (0xFFFFFFFF = the default object qword_1800694F0; 0 returns
    /// 0xFFFFFFFE without touching *a3), queries its anchor position into a2, then asks the host: message
    /// HostCommand.CheckStatus|id (sub 1, 0x100) must be zero or the object must be the context object of a1, and message
    /// HostCommand.IsTarget|id must be nonzero. On success *a4 = byte +16 and the result is 1 when the object is the context
    /// object, else 0; otherwise the result is 0xFFFFFFFF. Both paths set *a3 to the PSX address of the host pointer
    /// at +0 (0 when null). The object is dereferenced without a null check, as in the original.
    /// </summary>
    public static long HostObjQueryAccess(int a1, long a2, uint* a3, uint* a4)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjQueryAccess(a1, a2, a3, a4)", a1, a2, (nint)a3, (nint)a4);
        uint address = *a3;
        uint mapped = 0;
        byte* obj;
        if (address == 0xFFFFFFFF)
        {
            obj = (byte*)BattleObjectList;
        }
        else if (address == 0)
        {
            return 0xFFFFFFFE;
        }
        else
        {
            obj = ResolveGlobalPsxAddress(address);
        }

        HostQueryObjAnchorPos((long)obj, 0, a2);
        long result;
        if (BattleCallbackAddress != null
            && ((uint)HostObjCallback(ObjCommand((long)obj, HostCommand.CheckStatus), 1, 0x100, 0, 0, 0) == 0
                || (long)obj == PsxGetContextQword(a1))
            && BattleCallbackAddress != null
            && (uint)HostObjCallback(ObjCommand((long)obj, HostCommand.IsTarget), 0, 0, 0, 0, 0) != 0)
        {
            long context = PsxGetContextQword(a1);
            *a4 = obj[16];
            result = (long)obj == context ? 1 : 0;
            ulong host = *(ulong*)obj;
            if (host != 0)
            {
                mapped = (uint)PsxAddrMapperRegister((long)unk_180576A10, host);
            }
        }
        else
        {
            ulong host = *(ulong*)obj;
            if (host != 0)
            {
                mapped = (uint)PsxAddrMapperRegister((long)unk_180576A10, host);
            }

            result = 0xFFFFFFFF;
        }

        *a3 = mapped;
        return result;
    }
}
