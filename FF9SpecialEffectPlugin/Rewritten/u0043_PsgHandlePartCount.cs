namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Sends host message HostCommand.GetMeshCount | (u16 id at +24 of the object held in context slot <paramref name="a1"/>), or
    /// plain HostCommand.GetMeshCount when the slot is empty, and returns the callback's result; 0 when no callback is installed.
    /// </summary>
    public static long HostObjCmd1A(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("HostObjCmd1A(a1)", a1);
        long obj = PsxGetContextQword((int)a1);
        var callback = (BattleCallbackFn)BattleCallbackAddress;
        if (callback == null)
        {
            return 0;
        }

        ulong command = obj != 0 ? (*(ushort*)(obj + 24) | HostCommand.GetMeshCount) : HostCommand.GetMeshCount;
        return Native.InvokeBattleCallback((long)callback, command, 0, 0, 0, 0, 0);
    }

    /// <summary>
    /// PartCount (byte +3) of the PsgModel whose PSX address is at +8 of the handle <paramref name="a1"/>; an
    /// unresolvable address reads address 3 and faults, as in the original.
    /// </summary>
    public static long PsgHandlePartCount(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("PsgHandlePartCount(a1)", a1);
        Shape_PsgHandlePartCount_a1* shape_PsgHandlePartCount_a1 = (Shape_PsgHandlePartCount_a1*)a1;
        var model = (PsgModel*)PsxAddrToPtr((long)unk_180576A10, shape_PsgHandlePartCount_a1->field_8);
        return model->PartCount;
    }
}
