using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>Size of one buffer object in PsxCallContext.buffers (SfxBufferReset / SfxBufferFreeData).</summary>
    private const int PsxCallContextCtorBufferSize = 0x40;

    /// <summary>Size of one PSX stack in PsxCallContext.stacks.</summary>
    private const int PsxCallContextCtorStackSize = 0x1000;

    /// <summary>Offset of the initial stack pointer inside each PSX stack.</summary>
    private const int PsxCallContextCtorStackTop = 4016;

    /// <summary>GPR index of sp (r29) in a 32-register bank.</summary>
    private const int PsxCallContextCtorRegSp = 29;

    /// <summary>
    /// Constructs a recompiled-PSX call context: header fields, the two buffer objects (the original's
    /// eh vector constructor iterator over SfxBufferReset), zeroed GPR banks over the module memory mapper, and for
    /// each bank a 4 KB stack registered with the mapper whose PSX address + 4016 becomes the bank's sp.
    /// Returns a1.
    /// </summary>
    public static long PsxCallContextCtor(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("PsxCallContextCtor(a1)", a1);
        var context = (PsxCallContext*)a1;
        context->state = -1;
        context->field_8 = 0;
        context->field_10 = 0;
        context->field_14 = 0;

        // `eh vector constructor iterator'(buffers, 0x40, 2, SfxBufferReset, SfxBufferFreeData): the constructor
        // cannot throw, so the unwind path (SfxBufferFreeData on constructed elements) is unreachable.
        for (int i = 0; i < 2; i++)
        {
            SfxBufferReset((ulong*)(((byte*)&context->buffers) + i * PsxCallContextCtorBufferSize));
        }

        ulong* banks = (ulong*)((uint*)&context->gprBanks);
        for (int i = 0; i < 32; i++)
        {
            banks[i] = 0;
        }

        context->memory = (PsxAddrMapper*)unk_180576A10;
        context->bank = -1;

        for (int i = 0; i < 2; i++)
        {
            long bank = i & 1;
            uint* gpr = (uint*)((byte*)((uint*)&context->gprBanks) + (bank << 7));
            for (int r = 0; r < 32; r++)
            {
                gpr[r] = 0;
            }

            byte* stack = ((byte*)&context->stacks) + (bank << 12);
            PsxAddrMapperRegister((long)context->memory, (ulong)stack);
            gpr[PsxCallContextCtorRegSp] = (uint)PsxPtrToAddr((long)unk_180576A10, (ulong)(stack + PsxCallContextCtorStackTop));
        }

        for (int i = 0; i < 2; i++)
        {
            ((ulong*)&context->field_DA8)[i] = 0;
            ((byte*)&context->field_2DC8)[i] = 0;
        }

        return a1;
    }

    /// <summary>
    /// Resets the effect system and registers its module globals with the PSX address mapper: dword_18021FF68..7C
    /// (slots 0, 1, 2, 4, 5 of the table off_180066C68 initially points at) receive the PSX addresses of the state
    /// blocks; slot 3 of the current table is cleared.
    /// Returns the result of the last registration.
    /// </summary>
    public static long SfxResetAndRegisterGlobals()
    {
        Recompiler.Runtime.Tracer.Enter("SfxResetAndRegisterGlobals()");
        uint* table = (uint*)off_180066C68;
        table[3] = 0;
        byte* flags = (byte*)Unsafe.AsPointer(ref dword_180323192);
        flags[0] = 0;
        word_180323170 = -1;
        flags[2] = 0;
        ResetSfxWorkArea();
        *(ushort*)Unsafe.AsPointer(ref dword_1803231FC) = 0;
        SfxPoolsReset();
        dword_1803231A0 = 0;
        word_1803231A4 = 0;
        word_1803231AC = 0;
        word_1803231B4 = 0;
        word_1803231BC = 0;
        word_1803231C4 = 0;
        word_1803231CC = 0;

        // 32 records of 48 bytes: u16 index at +0x22 (unk_180220252), byte at +0x20, qword at +0.
        byte* record = unk_180220252;
        for (int i = 0; i < 32; i++, record += 48)
        {
            *(ushort*)record = (ushort)i;
            record[-2] = 0;
            *(ulong*)(record - 34) = 0;
        }

        dword_180220880 = 0;
        qword_180220830 = 0;
        long mapper = (long)unk_180576A10;
        dword_18021FF68 = (int)PsxAddrMapperRegister(mapper, (ulong)Unsafe.AsPointer(ref word_180323170));
        dword_18021FF6C = (int)PsxAddrMapperRegister(mapper, (ulong)Unsafe.AsPointer(ref dword_1803231F0));
        dword_18021FF70 = (int)PsxAddrMapperRegister(mapper, (ulong)Unsafe.AsPointer(ref dword_180323270));
        dword_18021FF78 = (int)PsxAddrMapperRegister(mapper, (ulong)unk_180068250);
        long result = PsxAddrMapperRegister(mapper, (ulong)xmmword_180069730);
        dword_18021FF7C = (int)result;
        return result;
    }
}
