namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Fatal error handler ("HIRAISHI ERROR"): the original formats the message held in Buffer into a local
    /// 256-byte array that nobody reads, then spins forever. Only the hang is observable, so only the hang is ported.
    /// </summary>
    public static void HiraishiFatalHang()
    {
        Recompiler.Runtime.Tracer.Enter("HiraishiFatalHang()");
        while (true)
        {
        }
    }
}
