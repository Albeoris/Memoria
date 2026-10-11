namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Handler of PSX task slots with kind &gt;= 128 (installed by StartPsxTask): calls script entry
    /// <c>slot-&gt;param</c> (kind - 128) of the slot <paramref name="a4"/>'s bank with the arguments
    /// ((int)a1, a2, a3, a4, a5). <paramref name="a2"/> is forwarded as a full 64-bit pointer (r9 = rdx).
    /// </summary>
    public static long PsxTaskScriptHandler(long a1, long a2, long a3, long a4, long a5)
    {
        Recompiler.Runtime.Tracer.Enter("PsxTaskScriptHandler(a1, a2, a3, a4, a5)", a1, a2, a3, a4, a5);
        return PsxCallScriptEntry(a1, (int)((PsxTaskSlot*)a4)->param, (int)a1, a2, a3, a4, a5);
    }
}
