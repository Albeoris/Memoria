using Recompiler.Runtime;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Starts task kind 10 (no arena) with the u16 at index <paramref name="a1"/> of the .rdata table at 0x18004B674
    /// (128 for a negative index; no upper bound check) and <paramref name="a2"/>.
    /// </summary>
    public static long StartPsxTask10(int a1, uint a2)
    {
        Tracer.Enter("StartPsxTask10(a1, a2)", a1, a2);
        // asc_18004B674 has no generated accessor (not an analysis global): read it from the module image.
        int value = a1 >= 0 ? ((ushort*)ModuleData.At(0x4b674))[a1] : 128;
        return StartPsxTask(10, -1, value, (int)a2);
    }

    /// <summary>Starts task kind 14 (no arena) with <paramref name="a1"/> and <paramref name="a2"/>.</summary>
    public static long StartPsxTask14(uint a1, uint a2)
    {
        Tracer.Enter("StartPsxTask14(a1, a2)", a1, a2);
        return StartPsxTask(14, -1, (int)a1, (int)a2);
    }
}
