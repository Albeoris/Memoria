using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Resets the call context qword_1803678E0 points to: for both register banks clears the 32 GPRs, registers the
    /// bank's 4 KB stack (+3512 + 4096 * bank) with the context's mapper and sets sp (r29) to the PSX address of
    /// +7528 + 4096 * bank. Frees the chunk data at +8 when the owned flag at +16 is set, then stores
    /// <paramref name="a2"/> at +0, points +8 at unk_1803678F0 (not owned), indexes the chunks and sets
    /// dword_180068774 to 256. <paramref name="a1"/> is unused.
    /// </summary>
    public static long ResetPsxCallContext(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("ResetPsxCallContext(a1, a2)", a1, a2);
        long context = qword_1803678E0;
        PsxCallContext* psxCallContext = (PsxCallContext*)context;
        for (int bank = 0; bank < 2; bank++)
        {
            long stackOffset = (long)(bank & 1) << 12;
            long bankOffset = (long)(bank & 1) << 7;
            var gprs = (uint*)(context + 3224 + bankOffset);
            for (int r = 0; r < 32; r++)
            {
                gprs[r] = 0;
            }

            PsxAddrMapperRegister((long)((PsxCallContext*)context)->memory, (ulong)(context + 3512 + stackOffset));
            *(uint*)(context + 3340 + bankOffset) =
                (uint)PsxPtrToAddr((long)unk_180576A10, (ulong)(context + 7528 + stackOffset));
        }

        if (psxCallContext->field_10 != 0)
        {
            NativeMemory.Free(*(void**)&psxCallContext->field_8);
        }

        psxCallContext->state = a2;
        psxCallContext->field_8 = unchecked((ulong)((long)unk_1803678F0));
        psxCallContext->field_10 = 0;
        long result = SfxIndexChunkTable(context);
        dword_180068774 = 256;
        return result;
    }

    /// <summary>
    /// Stores <paramref name="a3"/> in dword_1803678EC and the low dword of <paramref name="a1"/> in dword_180367754
    /// and dword_1803678E8, copies the mapper's entry count (0x1805789D0) to the dword after it, resets the call
    /// context with chunk id a1 and copies the 2 KB at unk_1803678F0 to word_1803208D0. Returns unk_1803678F0 + 2048.
    /// <paramref name="a2"/> is unused.
    /// </summary>
    public static void* /* __int128* */ InitSfxScriptContext(long a1, long a2, int a3)
    {
        Recompiler.Runtime.Tracer.Enter("InitSfxScriptContext(a1, a2, a3)", a1, a2, a3);
        dword_1803678EC = a3;
        dword_180367754 = (int)a1;
        CurrentEffectId = (int)a1;
        *(int*)unk_1805789D4 = (int)qword_1805789D0;
        ResetPsxCallContext(a1, (int)a1);
        System.Buffer.MemoryCopy(unk_1803678F0, Unsafe.AsPointer(ref word_1803208D0), 2048, 2048);
        return unk_1803678F0 + 2048;
    }
}
