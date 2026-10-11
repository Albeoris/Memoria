using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Per-frame tick of the effect system: emitters (sub_1800354E0), particles, keyframe tracks of the effect at
    /// qword_180211E68 with the track list qword_1803210D0; returns SfxUpdateKeyframeTracks' result (tail call).
    /// </summary>
    public static long SfxTickParticleSystem()
    {
        Recompiler.Runtime.Tracer.Enter("SfxTickParticleSystem()");
        long effect = (long)Unsafe.AsPointer(ref qword_180211E68);
        long tracks = (long)Unsafe.AsPointer(ref qword_1803210D0);
        sub_1800354E0(effect, tracks);
        SfxUpdateParticles(effect);
        return SfxUpdateKeyframeTracks(effect, tracks);
    }

    /// <summary>
    /// Runs the PSX script at a2 in the RAM bank (*(short*)(a6 + 8) &amp; 1) of the script context qword_1803678E0 with
    /// the seven register arguments of PsxSetCallArgs7 (a3, a4, off_180066C68, a6, a7, a8, a9); the bank's stack is
    /// lowered by 24 bytes and its saved return address (+3348) parked during the call; the previous bank is
    /// selected again afterwards. Returns the interpreter's result (sub_18000E1E0). a1 and a5 are not read.
    /// </summary>
    public static long PsxCallScript7(long a1, int a2, int a3, int a4, int a5, long a6, int a7, long a8, int a9)
    {
        Recompiler.Runtime.Tracer.Enter("PsxCallScript7(a1, a2, a3, a4, a5, a6, a7, a8, a9)", a1, a2, a3, a4, a5, a6, a7, a8, a9);
        long ctx = qword_1803678E0;
        PsxCallContext* psxCallContext = (PsxCallContext*)ctx;
        int savedBank = psxCallContext->bank;
        Part106bSelectRamBank(ctx, *(short*)(a6 + 8) & 1);
        long frame = (long)psxCallContext->bank << 7;
        uint savedReturn = *(uint*)(frame + ctx + 3348);
        *(uint*)(frame + ctx + 3348) = 0;
        *(int*)(((long)psxCallContext->bank << 7) + ctx + 3340) -= 24;
        PsxSetCallArgs7(ctx, a3, a4, off_180066C68, a6, a7, a8, a9);
        Part106bSetProgramCounter(ctx, unchecked((uint)a2 - *(uint*)((ulong*)&psxCallContext->field_DA8)[psxCallContext->bank]));
        uint result = (uint)sub_18000E1E0(ctx);
        *(int*)(((long)psxCallContext->bank << 7) + ctx + 3340) += 24;
        *(uint*)(((long)psxCallContext->bank << 7) + ctx + 3348) = savedReturn;
        long thread = (long)psxCallContext->bank << 6;
        *(uint*)(thread + ctx + 3128) = savedReturn - 4;
        *(long*)(thread + ctx + 3136) = *(long*)(thread + ctx + 3120) + (long)(16 * ((ulong)savedReturn >> 2));
        Part106bSelectRamBank(ctx, savedBank & 1);
        return result;
    }

    /// <summary>
    /// Selects RAM bank <paramref name="bank"/> of the script context: *(ctx + 3488) = bank and the memory context
    /// *(ctx + 3480) gets the bank's host base (+8144), PSX base | 0x80000000 (+8136) and the three window pointers.
    /// </summary>
    private static void Part106bSelectRamBank(long ctx, int bank)
    {
        Recompiler.Runtime.Tracer.Enter("Part106bSelectRamBank(ctx, bank)", ctx, bank);
        PsxCallContext* psxCallContext = (PsxCallContext*)ctx;
        long memory = (long)psxCallContext->memory;
        PsxAddrMapper* psxAddrMapper = (PsxAddrMapper*)memory;
        psxCallContext->bank = bank;
        long entry = (long)((ulong*)&psxCallContext->field_DA8)[bank];
        uint psxBase = *(uint*)entry;
        long hostBase = *(long*)(entry + 8);
        psxAddrMapper->ramHostBase = (ulong)hostBase;
        psxAddrMapper->ramPsxAddress = (psxBase & 0xFFFFFFF) | 0x80000000;
        psxAddrMapper->entries[4096] = 1;
        *(long*)(memory + 4104) = hostBase - 0x20000;
        *(long*)(memory + 4112) = (long)psxAddrMapper->ramHostBase;
        *(long*)(memory + 4120) = (long)psxAddrMapper->ramHostBase + 0x200000;
    }

    /// <summary>Program counter of the current bank's thread: +3136 = code base (+3120) + 16 * (offset / 4), +3128 = offset - 4.</summary>
    private static void Part106bSetProgramCounter(long ctx, uint offset)
    {
        Recompiler.Runtime.Tracer.Enter("Part106bSetProgramCounter(ctx, offset)", ctx, offset);
        PsxCallContext* psxCallContext = (PsxCallContext*)ctx;
        long thread = (long)psxCallContext->bank << 6;
        *(long*)(thread + ctx + 3136) = *(long*)(thread + ctx + 3120) + (long)(16 * ((ulong)offset >> 2));
        *(uint*)(thread + ctx + 3128) = offset - 4;
    }

    /// <summary>
    /// Takes the first free slot (word 0 == 0) of the PSX slot pool a1 (header: int16 stride, int16 count, u32 script,
    /// u64 owner; slots from +16), marks it active (dword 1) and starts the pool script in mode 1 with the slot's PSX
    /// address (slot + 4). Returns the slot's payload (slot + 4) or null when the pool is full.
    /// </summary>
    public static ushort* SfxStartScriptSlot(uint a1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxStartScriptSlot(a1)", a1);
        byte* pool = ResolveGlobalPsxAddress(a1);
        Pool2* pool2 = (Pool2*)pool;
        byte* slot = pool + 16;
        uint* found = null;
        int index = 0;
        int count = pool2->field_2;
        long stride = pool2->field_0;
        if (count > 0)
        {
            do
            {
                found = (uint*)slot;
                if (*(ushort*)slot == 0)
                {
                    break;
                }

                ++index;
                slot += stride;
            }
            while (index < count);
        }

        if (index >= count)
        {
            return null;
        }

        *found = 1;
        long owner = pool2->field_8;
        int payload = (int)PsxPtrToAddr((long)unk_180576A10, (ulong)(slot + 4));
        PsxCallScript7((long)unk_180576A10, pool2->field_4, 1, payload, 0, owner, 0, (long)pool, *(int*)(owner + 12));
        return (ushort*)(slot + 4);
    }

    /// <summary>
    /// Runs the pool script in mode 2 for every active slot of the PSX slot pool a1 (off_180066C68[10] = 0 first,
    /// a7 = the slot's tick word); a non-zero result frees the slot (dword 0), else the tick word is incremented.
    /// Returns the number of active slots visited.
    /// </summary>
    public static long SfxTickScriptSlots(uint a1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxTickScriptSlots(a1)", a1);
        byte* pool = ResolveGlobalPsxAddress(a1);
        Pool3* pool3 = (Pool3*)pool;
        byte* slot = pool + 16;
        int active = 0;
        if (pool3->field_2 > 0)
        {
            long stride = pool3->field_0;
            int index = 0;
            do
            {
                if (*(ushort*)slot != 0)
                {
                    ((Table*)off_180066C68)->field_40 = 0;
                    long owner = pool3->field_8;
                    int tick = *(short*)(slot + 2);
                    ++active;
                    int payload = (int)PsxPtrToAddr((long)unk_180576A10, (ulong)(slot + 4));
                    if ((uint)PsxCallScript7((long)unk_180576A10, pool3->field_4, 2, payload, 0, owner, tick, (long)pool, *(int*)(owner + 12)) != 0)
                    {
                        *(uint*)slot = 0;
                    }
                    else
                    {
                        ++*(ushort*)(slot + 2);
                    }
                }

                ++index;
                slot += stride;
            }
            while (index < pool3->field_2);
        }

        return (uint)active;
    }
}
