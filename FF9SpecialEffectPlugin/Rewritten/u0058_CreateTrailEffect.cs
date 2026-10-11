using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Tail call into CreateRibbonEffectSlot with the module's trail pool (qword_1803210D0: count, high-water, slots)
    /// and the unused first argument qword_180211E68; <paramref name="a1"/> is the effect data, <paramref name="a2"/>
    /// the target. Returns the slot or 0.
    /// </summary>
    public static long CreateTrailEffect(long a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("CreateTrailEffect(a1, a2)", a1, a2);
        return CreateRibbonEffectSlot(
            (long)Unsafe.AsPointer(ref qword_180211E68),
            (long)Unsafe.AsPointer(ref qword_1803210D0),
            (ulong)a1,
            (ulong)a2);
    }
}
