namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// The first two handlers of the pointer table at 0x18004AD00 (.rdata): on message 1 they copy bytes 0 and 4 of
/// the fifth argument into a byte pair (SFX-043); every call returns 1.
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>a1 == 1: byte_180578E00 = a5[0], byte_180578E01 = a5[4]. Returns 1.</summary>
    public static long StoreBytePairA(int a1, long a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("StoreBytePairA(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        if (a1 == 1)
        {
            byte_180578E00 = a5[0];
            byte_180578E01 = a5[4];
        }
        return 1;
    }

    /// <summary>a1 == 1: byte_180578E02 = a5[0], byte_180578E03 = a5[4]. Returns 1.</summary>
    public static long StoreBytePairB(int a1, long a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("StoreBytePairB(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        if (a1 == 1)
        {
            byte_180578E02 = a5[0];
            byte_180578E03 = a5[4];
        }
        return 1;
    }
}
