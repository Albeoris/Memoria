namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Registers host pointer a3 with the call context's PSX address mapper (a3 == 0 stores 0) and writes its PSX address
    /// as o32 argument a2 of the current register bank: a0..a3 for 0..3, otherwise the word at sp + 4 * a2.
    /// Returns the bank byte offset (bank &lt;&lt; 7) for 0..3, or the host pointer of sp otherwise.
    /// </summary>
    public static long PsxSetCallArgPtr(long a1, int a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("PsxSetCallArgPtr(a1, a2, a3)", a1, a2, a3);
        var context = (PsxCallContext*)a1;
        uint address = 0;
        if (a3 != 0)
        {
            PsxAddrMapperRegister((long)context->memory, (ulong)a3);
            address = (uint)PsxPtrToAddr((long)context->memory, (ulong)a3);
        }

        long result = (long)context->bank << 7;
        uint* gpr = CurrentGprBank(context);
        if ((uint)a2 <= 3)
        {
            gpr[RegA0 + a2] = address;
            return result;
        }

        result = PsxAddrToPtr((long)context->memory, gpr[RegSp]);
        *(uint*)(result + (uint)(4 * a2)) = address;
        return result;
    }
}
