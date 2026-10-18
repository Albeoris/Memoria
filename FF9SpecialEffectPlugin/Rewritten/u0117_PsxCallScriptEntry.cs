namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Resets bank (<paramref name="a2"/> &amp; 1) of the script context <paramref name="a1"/>: clears its 32 GPRs,
    /// registers its 4 KB stack (a1 + 3512 + 4096 * bank) with the context's mapper and sets sp to the PSX address of
    /// stack + 4016, translated through the global mapper unk_180576A10. Returns that address.
    /// </summary>
    public static long PsxResetBankFrame(long a1, sbyte a2)
    {
        Recompiler.Runtime.Tracer.Enter("PsxResetBankFrame(a1, a2)", a1, a2);
        var ctx = (PsxCallContext*)a1;
        long bank = a2 & 1;
        uint* gpr = (uint*)((byte*)((uint*)&ctx->gprBanks) + (bank << 7));
        for (int i = 0; i < 32; i++)
        {
            gpr[i] = 0;
        }

        byte* stack = ((byte*)&ctx->stacks) + (bank << 12);
        PsxAddrMapperRegister((long)ctx->memory, (ulong)stack);
        long result = PsxPtrToAddr((long)unk_180576A10, (ulong)(stack + 4016));
        gpr[29] = (uint)result;
        return result;
    }

    /// <summary>
    /// Sets the o32 arguments of the current bank: a0 = <paramref name="a2"/>, a1..a3 = PSX addresses of the host
    /// pointers <paramref name="a3"/>..<paramref name="a5"/> and the fifth argument (word at sp + 16) = PSX address of
    /// <paramref name="a6"/>; each non-null pointer is registered with the mapper first, null gives 0. Returns the host
    /// pointer of sp.
    /// </summary>
    public static long PsxSetCallArgs5(long a1, int a2, long a3, long a4, long a5, long a6)
    {
        Recompiler.Runtime.Tracer.Enter("PsxSetCallArgs5(a1, a2, a3, a4, a5, a6)", a1, a2, a3, a4, a5, a6);
        var ctx = (PsxCallContext*)a1;
        PsxSetCallArgs5Gpr(ctx, 4) = (uint)a2;
        PsxSetCallArgs5Gpr(ctx, 5) = PsxSetCallArgs5Map(in *ctx, a3);
        PsxSetCallArgs5Gpr(ctx, 6) = PsxSetCallArgs5Map(in *ctx, a4);
        PsxSetCallArgs5Gpr(ctx, 7) = PsxSetCallArgs5Map(in *ctx, a5);
        uint fifth = PsxSetCallArgs5Map(in *ctx, a6);
        long result = PsxAddrToPtr((long)ctx->memory, PsxSetCallArgs5Gpr(ctx, 29));
        *(uint*)(result + 16) = fifth;
        return result;
    }

    /// <summary>GPR <paramref name="index"/> of the current bank.</summary>
    private static ref uint PsxSetCallArgs5Gpr(PsxCallContext* ctx, int index)
    {
        Recompiler.Runtime.Tracer.Enter("PsxSetCallArgs5Gpr(ctx, index)", (nint)ctx, index);
        return ref ((uint*)((byte*)((uint*)&ctx->gprBanks) + ((long)ctx->bank << 7)))[index];
    }

    /// <summary>Registers <paramref name="host"/> with the context's mapper and returns its PSX address; 0 for null.</summary>
    private static uint PsxSetCallArgs5Map(in PsxCallContext ctx, long host)
    {
        Recompiler.Runtime.Tracer.Enter("PsxSetCallArgs5Map(ctx, host)", ctx, host);
        if (host == 0)
        {
            return 0;
        }

        PsxAddrMapperRegister((long)ctx.memory, (ulong)host);
        return (uint)PsxPtrToAddr((long)ctx.memory, (ulong)host);
    }

    /// <summary>
    /// Calls entry <paramref name="a2"/> of the code block of RAM bank (*(short*)(a6 + 8) &amp; 1) of the script context
    /// qword_1803678E0: selects the bank, resets its frame, passes (a3, a4, a5, a6, a7) through PsxSetCallArgs5 and
    /// starts at the entry offset (dword at block + 24 + 4 * a2). Returns 0xFFFFFFFF for a zero entry (the pc is set
    /// anyway), else v0 after the interpreter ran. <paramref name="a1"/> is not read.
    /// </summary>
    public static long PsxCallScriptEntry(long a1, int a2, int a3, long a4, long a5, long a6, long a7)
    {
        Recompiler.Runtime.Tracer.Enter("PsxCallScriptEntry(a1, a2, a3, a4, a5, a6, a7)", a1, a2, a3, a4, a5, a6, a7);
        long ctx = qword_1803678E0;
        var context = (PsxCallContext*)ctx;
        Part106bSelectRamBank(ctx, *(short*)(a6 + 8) & 1);
        PsxResetBankFrame(ctx, (sbyte)context->bank);
        PsxSetCallArgs5(ctx, a3, a4, a5, a6, a7);

        byte* block = (byte*)((ulong*)&context->field_DA8)[context->bank];
        uint entry = *(uint*)(block + 24 + 4L * a2);
        Part106bSetProgramCounter(ctx, entry);
        if (entry == 0)
        {
            return 0xFFFFFFFF;
        }

        PsxInterpreterRun(ctx);
        return *(uint*)((byte*)((uint*)&context->gprBanks) + ((long)context->bank << 7) + 8);
    }
}
