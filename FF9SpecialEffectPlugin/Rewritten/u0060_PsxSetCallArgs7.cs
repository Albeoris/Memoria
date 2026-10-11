namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Loads the o32 arguments of a call into recompiled PSX code on the current register bank of context
    /// <paramref name="a1"/>: a0 = a2, a1 = a3, a2/a3 = PSX addresses of the host pointers a4/a5 (registered with the
    /// mapper first; 0 for null), stack words 4, 5, 6 (sp + 16/20/24) = a6, address of a7, a8. Returns the host
    /// address of the PSX stack pointer.
    /// </summary>
    public static long PsxSetCallArgs7(long a1, int a2, int a3, long a4, long a5, int a6, long a7, int a8)
    {
        Recompiler.Runtime.Tracer.Enter("PsxSetCallArgs7(a1, a2, a3, a4, a5, a6, a7, a8)", a1, a2, a3, a4, a5, a6, a7, a8);
        var context = (PsxCallContext*)a1;
        long memory = (long)context->memory;
        CurrentGprBank(context)[RegA0] = (uint)a2;
        CurrentGprBank(context)[RegA0 + 1] = (uint)a3;
        CurrentGprBank(context)[RegA0 + 2] = PsxRegisterHostPointer(memory, a4);
        CurrentGprBank(context)[RegA0 + 3] = PsxRegisterHostPointer(memory, a5);
        *(int*)(PsxAddrToPtr(memory, CurrentGprBank(context)[RegSp]) + 16) = a6;
        uint address7 = PsxRegisterHostPointer(memory, a7);
        *(uint*)(PsxAddrToPtr(memory, CurrentGprBank(context)[RegSp]) + 20) = address7;
        long stack = PsxAddrToPtr(memory, CurrentGprBank(context)[RegSp]);
        Stack* stack2 = (Stack*)stack;
        stack2->field_24 = a8;
        return stack;
    }

    /// <summary>Registers host pointer <paramref name="pointer"/> with the mapper and returns its PSX address; 0 for null.</summary>
    private static uint PsxRegisterHostPointer(long memory, long pointer)
    {
        Recompiler.Runtime.Tracer.Enter("PsxRegisterHostPointer(memory, pointer)", memory, pointer);
        if (pointer == 0)
        {
            return 0;
        }

        PsxAddrMapperRegister(memory, (ulong)pointer);
        return (uint)PsxPtrToAddr(memory, (ulong)pointer);
    }
}
