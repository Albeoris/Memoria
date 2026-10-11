namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// Table handlers: trivial returns, the PSX rand LCG on dword_1803231DC (SFX-RAND-001, PSX-RAND-001) and align-up-4.
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>Returns the second argument sign-extended to 64 bits (movsxd rax, edx).</summary>
    public static long ReturnArg2(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("ReturnArg2(a1, a2)", a1, a2);
        return a2;
    }

    /// <summary>Returns 0xFFFFFFFF (or eax, -1; the upper half of rax is zero).</summary>
    public static long ReturnMinusOne()
    {
        Recompiler.Runtime.Tracer.Enter("ReturnMinusOne()");
        return 0xFFFFFFFFL;
    }

    /// <summary>rand(): seed = seed * 1103515245 + 12345, returns seed &gt;&gt; 16 (logical).</summary>
    public static long PsxRand()
    {
        Recompiler.Runtime.Tracer.Enter("PsxRand()");
        dword_1803231DC = unchecked(1103515245 * dword_1803231DC + 12345);
        return (uint)dword_1803231DC >> 16;
    }

    /// <summary>
    /// Steps the LCG and returns (seed &gt;&gt; 16) % (a1 != 0 ? a1 : 1) - (a1 &gt;&gt; 1) (signed idiv, arithmetic
    /// shift), zero-extended from 32 bits.
    /// </summary>
    public static long PsxRandCentered(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("PsxRandCentered(a1)", a1);
        int divisor = a1 != 0 ? a1 : 1;
        dword_1803231DC = unchecked(1103515245 * dword_1803231DC + 12345);
        int value = (int)((uint)dword_1803231DC >> 16);
        return (uint)(value % divisor - (a1 >> 1));
    }

    /// <summary>Rounds a1 up to a multiple of 4 in 32 bits: (a1 &amp; ~3) + (a1 &amp; 3 ? 4 : 0), zero-extended.</summary>
    public static long PsxAlignUp4(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("PsxAlignUp4(a1)", a1);
        uint result = (uint)a1 & 0xFFFFFFFCu;
        if ((a1 & 3) != 0)
            result += 4;
        return result;
    }
}
