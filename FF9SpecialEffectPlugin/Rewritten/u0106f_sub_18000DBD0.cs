namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Runs the interpreter on the current bank (PsxInterpreterRun) and returns v0 (gpr 2) of the bank selected
    /// afterwards, zero-extended.
    /// </summary>
    public static long sub_18000E1E0(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("sub_18000E1E0(a1)", a1);
        PsxInterpreterRun(a1);
        var ctx = (PsxCallContext*)a1;
        return *(uint*)((byte*)((uint*)&ctx->gprBanks) + ((long)ctx->bank << 7) + 8);
    }

    /// <summary>
    /// Native-to-PSX call (the original is variadic): calls the PSX function at address <paramref name="a2"/> of the
    /// script context <paramref name="a1"/> with the arguments described by the signature string
    /// <paramref name="a3"/> ('v' = 32-bit value, 'p' = 64-bit pointer). <paramref name="va"/> is the array of
    /// 8-byte argument slots that follow a3 in the original's home area / stack (what its va_list points at).
    /// The RAM bank is the one whose 0x5000-byte window (0x1E7700.., 0x1EC700..) contains a2's low 28 bits (else
    /// bank 1); ra is parked during the call, the stack lowered by 4 * (count - 1) bytes for more than four
    /// arguments, and the previous bank is selected again afterwards. Returns v0 of the call.
    /// </summary>
    public static long sub_18000DBD0(long a1, int a2, long a3, ulong* va)
    {
        Recompiler.Runtime.Tracer.Enter("sub_18000DBD0(a1, a2, a3, va)", a1, a2, a3, (nint)va);
        var ctx = (PsxCallContext*)a1;
        int savedBank = ctx->bank;
        Part106bSelectRamBank(a1, sub_18000DBD0Bank((uint)a2 & 0xFFFFFFF));

        uint* frame = (uint*)((byte*)((uint*)&ctx->gprBanks) + ((long)ctx->bank << 7));
        uint savedReturn = frame[31];
        frame[31] = 0;

        PsxCallParseArgs(a1, a3, (long)va);

        int count = ctx->argCount;
        int lower = count > 4 ? 4 - 4 * count : 0;
        *sub_18000DBD0Sp(ctx) += (uint)lower;
        for (int i = 0; i < ctx->argCount; i++)
        {
            var arg = (PsxCallArg*)((byte*)&ctx->args) + i;
            if (arg->isPointer != 0)
            {
                PsxSetCallArgPtr(a1, i, (long)arg->pointer);
            }
            else
            {
                PsxSetCallArg(a1, i, (int)arg->value);
            }
        }

        Part106bSetProgramCounter(a1, unchecked((uint)a2 - *(uint*)((ulong*)&ctx->field_DA8)[ctx->bank]));
        uint result = (uint)sub_18000E1E0(a1);

        count = ctx->argCount;
        int raise = count > 4 ? 4 * count - 4 : 0;
        *sub_18000DBD0Sp(ctx) += (uint)raise;
        ((uint*)((byte*)((uint*)&ctx->gprBanks) + ((long)ctx->bank << 7)))[31] = savedReturn;
        Part106bSetProgramCounter(a1, savedReturn);
        Part106bSelectRamBank(a1, savedBank & 1);
        return result;
    }

    /// <summary>
    /// Bank of a code address: windows end at 0x1EC700 and 0x1F1700 (masked to 28 bits, 0x5000 bytes each); the index
    /// of the first window containing <paramref name="address"/>, -1 when none does, then &amp; 1.
    /// </summary>
    private static int sub_18000DBD0Bank(uint address)
    {
        Recompiler.Runtime.Tracer.Enter("sub_18000DBD0Bank(address)", address);
        int end = 0x1EC700;
        int bank = 0;
        while (address < ((uint)(end - 0x5000) & 0xFFFFFFF) || address >= ((uint)end & 0xFFFFFFF))
        {
            end += 0x5000;
            bank++;
            if (end >= 0x1F6700)
            {
                bank = -1;
                break;
            }
        }

        return bank & 1;
    }

    /// <summary>sp (gpr 29) of the current bank.</summary>
    private static uint* sub_18000DBD0Sp(PsxCallContext* ctx)
    {
        Recompiler.Runtime.Tracer.Enter("sub_18000DBD0Sp(ctx)", (nint)ctx);
        return (uint*)((byte*)((uint*)&ctx->gprBanks) + ((long)ctx->bank << 7)) + 29;
    }
}
