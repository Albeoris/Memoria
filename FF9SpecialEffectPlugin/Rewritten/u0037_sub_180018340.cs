namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Host message HostCommand.GetMatrix | id (+24) of context qword <paramref name="a1"/> (plain HostCommand.GetMatrix without one), sub-id 0,
    /// payload a2, last argument a3. Returns the host answer, or the context qword when no callback is installed.
    /// </summary>
    public static long HostSendCommand0EMode0(long a1, uint a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("HostSendCommand0EMode0(a1, a2, a3)", a1, a2, a3);
        return HostSendCommand0E(a1, 0, a2, a3);
    }

    /// <summary>
    /// Sub-id 0 message with payload = byte +37 + a2 of the PSX block addressed by dword 3 of off_180066C68; returns the
    /// host answer, or 0 (the null callback pointer) without a callback.
    /// </summary>
    public static long HostSendCommand0EMode0FromBlock(long a1, int a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("HostSendCommand0EMode0FromBlock(a1, a2, a3)", a1, a2, a3);
        return HostSendCommand0EFromBlock(a1, 0, a2, a3);
    }

    /// <summary>As <see cref="HostSendCommand0EMode0"/> with sub-id 2.</summary>
    public static long HostSendCommand0EMode2(long a1, uint a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("HostSendCommand0EMode2(a1, a2, a3)", a1, a2, a3);
        return HostSendCommand0E(a1, 2, a2, a3);
    }

    /// <summary>As <see cref="HostSendCommand0EMode0FromBlock"/> with sub-id 2.</summary>
    public static long HostSendCommand0EMode2FromBlock(long a1, int a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("HostSendCommand0EMode2FromBlock(a1, a2, a3)", a1, a2, a3);
        return HostSendCommand0EFromBlock(a1, 2, a2, a3);
    }

    private static long HostSendCommand0E(long a1, ulong mode, uint payload, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("HostSendCommand0E(a1, mode, payload, a3)", a1, mode, payload, a3);
        long entry = PsxGetContextQword((int)a1);
        if (BattleCallbackAddress != null)
        {
            return Native.InvokeBattleCallback((long)HostCallback0E, HostCommand0E(entry), mode, payload, 0, 0, unchecked((ulong)a3));
        }

        return entry;
    }

    private static long HostSendCommand0EFromBlock(long a1, ulong mode, int a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("HostSendCommand0EFromBlock(a1, mode, a2, a3)", a1, mode, a2, a3);
        long entry = PsxGetContextQword((int)a1);
        byte* block = ResolveGlobalPsxAddress(((uint*)off_180066C68)[3]);
        long result = (long)BattleCallbackAddress;
        ulong payload = block[(long)a2 + 37];
        if (BattleCallbackAddress != null)
        {
            return Native.InvokeBattleCallback((long)HostCallback0E, HostCommand0E(entry), mode, payload, 0, 0, unchecked((ulong)a3));
        }

        return result;
    }

    private static uint HostCommand0E(long entry)
    {
        Recompiler.Runtime.Tracer.Enter("HostCommand0E(entry)", entry);
        return entry != 0 ? *(ushort*)(entry + 24) | HostCommand.GetMatrix : HostCommand.GetMatrix;
    }

    private static BattleCallbackFn HostCallback0E =>
        (BattleCallbackFn)BattleCallbackAddress;
}
