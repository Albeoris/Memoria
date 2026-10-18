using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>Host query: object vector (HostCommand.GetPosition | object id), result written through the 6th argument.</summary>
    private const ulong HostCmdQueryVector = HostCommand.GetPosition;

    /// <summary>Host query: one axis of an object (HostCommand.GetMatrix | object id), axis byte as 3rd argument.</summary>
    private const ulong HostCmdQueryAxis = HostCommand.GetMatrix;

    /// <summary>Steps the PSX-style rand LCG kept in dword_1803231DC (shared with the forwarded exports).</summary>
    private static uint PsxRandStep()
    {
        Recompiler.Runtime.Tracer.Enter("PsxRandStep()");
        dword_1803231DC = unchecked(1103515245 * dword_1803231DC + 12345);
        return (uint)dword_1803231DC;
    }

    /// <summary>The host callback (qword_1801C1DE8) as a six-argument query; read from the module data on every use.</summary>
    private static BattleCallbackFn HostCallback6 =>
        (BattleCallbackFn)BattleCallbackAddress;

    /// <summary>
    /// Asks the host for the vector of object a1 and fills a2[0..4]: a2[0..2] from the vector's second component
    /// and the size at +60 when the flag at +16 is set, otherwise one host query per axis (axis ids at +45..+47).
    /// </summary>
    public static long HostQueryObjectVector(long a1, ushort* a2)
    {
        Recompiler.Runtime.Tracer.Enter("HostQueryObjectVector(a1, a2)", a1, (nint)a2);
        if (a1 == 0)
        {
            PsxCompatibilityAssert();
        }

        // v10, v11, v12 of the original: a stack vector the host fills (uninitialized there, zero here).
        Span<short> vector = stackalloc short[8];
        short* vectorAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(vector));
        new Span<short>(vectorAddress, 8).Clear();
        var query = (BattleCallbackFn)BattleCallbackAddress;
        if (query != null)
        {
            Native.InvokeBattleCallback((long)query, *(ushort*)(a1 + 24) | HostCmdQueryVector, 0, 0, 0, 0, (ulong)vectorAddress);
        }

        if (*(byte*)(a1 + 16) != 0)
        {
            int size = *(int*)(a1 + 60);
            short y = vector[1];
            a2[0] = (ushort)(y - (size >> 2));
            a2[2] = (ushort)(y + (size >> 2) - size);
            a2[1] = (ushort)(y - (size >> 1));
        }
        else
        {
            for (long i = 0; i < 3; ++i)
            {
                var axisQuery = (BattleCallbackFn)BattleCallbackAddress;
                short value = axisQuery != null
                    ? (short)Native.InvokeBattleCallback((long)axisQuery, *(ushort*)(a1 + 24) | HostCmdQueryAxis, 1, *(byte*)(a1 + i + 45), 0, 0, 0)
                    : (short)0;
                a2[i] = (ushort)value;
            }
        }

        a2[3] = (ushort)vector[0];
        ushort last = (ushort)vector[2];
        a2[4] = last;
        return last;
    }

    /// <summary>
    /// Resets the effect state block: clears 0x68 bytes at qword_18021FF00, copies 0xC0 bytes from the PSX address in
    /// table entry 3, draws a random bit, then queries the vectors of the tracked host objects.
    /// </summary>
    public static long SfxStateReset()
    {
        Recompiler.Runtime.Tracer.Enter("SfxStateReset()");
        var mapper = (long)unk_180576A10;

        dword_180211E4C = 0;
        new Span<byte>(Unsafe.AsPointer(ref qword_18021FF00), 0x68).Clear();

        uint* table = (uint*)off_180066C68;
        byte* source = (byte*)PsxAddrToPtr(mapper, table[3]);
        System.Buffer.MemoryCopy(source, unk_18021FFA0, 0xC0, 0xC0);
        byte_1802200BC = 0;
        byte_1802200B3 = (byte)((PsxRandStep() >> 16) & 1);

        byte* header = (byte*)PsxAddrToPtr(mapper, ((uint*)off_180066C68)[0]);
        if ((header[28] & 4) != 0)
        {
            HostQueryObjectVector(qword_180220058, (ushort*)unk_18021FF54);
        }

        HostQueryObjectVector(qword_18021FFF0, (ushort*)unk_18021FF4A);
        return HostQueryObjectVector(qword_18021FFF8, (ushort*)unk_18021FF5E);
    }

    /// <summary>
    /// Parses an effect block header and selects one of its (up to four) variants; the selector word lives in variant 3.
    /// Stores the chosen variant pointer in qword_18021FF00 and returns its index.
    /// </summary>
    public static long SfxSelectBlockVariant(short* a1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxSelectBlockVariant(a1)", (nint)a1);
        dword_18021FF08 = 1;
        int header = *a1;
        ushort* cursor = (ushort*)(a1 + 1);
        byte** variants = stackalloc byte*[4];
        int flags = header;
        for (int k = 0; k < 4; k++)
        {
            variants[k] = null;
            if ((flags & 1) != 0)
            {
                variants[k] = (byte*)a1 + *cursor++;
            }

            flags >>= 1;
        }

        byte* records = (byte*)a1 + *cursor;
        byte* table = unk_180220060;
        for (int k = 0; k < 4; k++)
        {
            byte* slot = table + 8 * k;
            if ((flags & 1) != 0)
            {
                System.Buffer.MemoryCopy(records, slot, 6, 6);
                records += 6;
            }
            else
            {
                *(uint*)slot = 0;
                *(ushort*)(slot + 4) = 0;
            }

            *(uint*)(slot + 48) = 0;
            *(ushort*)(slot + 52) = 0;
            flags >>= 1;
        }

        uint selected = SelectVariantIndex((ushort*)variants[3], variants);

        qword_18021FF00 = selected < 4 ? (long)variants[selected] : 0;
        if ((header & 0x200) != 0)
        {
            byte_180220118 = (byte)((header >> (int)(2 * selected + 10)) & 3);
        }

        return selected;
    }

    /// <summary>The selector of SfxSelectBlockVariant; every decided path stores its result in dword_180220108.</summary>
    private static uint SelectVariantIndex(ushort* selector, byte** variants)
    {
        Recompiler.Runtime.Tracer.Enter("SelectVariantIndex(selector, variants)", (nint)selector, (nint)variants);
        if (selector == null)
        {
            return (uint)dword_180220108;
        }

        uint word = *selector;
        uint result;
        switch (word >> 14)
        {
            case 0:
                result = (word >> 12) & 3;
                break;

            case 1:
            {
                int roll = (int)(PsxRandStep() >> 16) % 9;
                int threshold = (int)((word >> 10) & 3);
                if ((word & 0x3000) == 0)
                {
                    result = roll >= threshold + 5 ? 1u : 0u;
                }
                else if (roll >= threshold + 3)
                {
                    result = (roll >= threshold + 6 ? 1u : 0u) + 1;
                }
                else
                {
                    result = 0;
                }

                break;
            }

            case 3:
                result = byte_180220118 & 3u;
                if (result == 2)
                {
                    result = 0;
                }
                else if (result == 3)
                {
                    result = 2;
                }

                break;

            default:
                result = SelectByHost((word >> 12) & 3, variants);
                break;
        }

        dword_180220108 = (int)result;
        return result;
    }

    /// <summary>Mode 2 of the selector: host queries, the tracked object's state or the effect owner's flags.</summary>
    private static uint SelectByHost(uint submode, byte** variants)
    {
        Recompiler.Runtime.Tracer.Enter("SelectByHost(submode, variants)", submode, (nint)variants);
        switch (submode)
        {
            case 0:
                if (HostCallback6 != null && (uint)Native.InvokeBattleCallback((long)HostCallback6, HostCommand.GetStartType, 0, 0, 0, 0, 0) == 2)
                {
                    dword_18021FF1C = 2;
                    return 2;
                }

                dword_18021FF1C = 0;
                PsxRandStep();
                return 0;

            case 1:
                return HostCallback6 == null ? 0u : (uint)Native.InvokeBattleCallback((long)HostCallback6, HostCommand.GetCameraNumber, 0, 0, 0, 0, 0);

            case 2:
            {
                long tracked = qword_18021FFF8;
                if (tracked != 0)
                {
                    int state = *(int*)(tracked + 56);
                    int value = *(int*)(tracked + 60);
                    if (state == 1239 || state >= 3000)
                    {
                        return 2;
                    }

                    if (state > 100)
                    {
                        return value > 1000 ? 2u : (value >= 345 ? 1u : 0u);
                    }
                }

                return 0;
            }

            default:
                return SelectByOwner(variants);
        }
    }

    /// <summary>Submode 3: owner object qword_180220130, host flag mask and two host votes, with PSX rand fallbacks.</summary>
    private static uint SelectByOwner(byte** variants)
    {
        Recompiler.Runtime.Tracer.Enter("SelectByOwner(variants)", (nint)variants);
        long owner = qword_180220130;
        if (owner == 0)
        {
            return 0;
        }

        int mask = 0;
        if (HostCallback6 != null)
        {
            ulong code = *(ushort*)(owner + 24) | HostCommand.CheckStatus;
            if ((uint)Native.InvokeBattleCallback((long)HostCallback6, code, 0, 256, 0x4000, 0, 0) != 0)
            {
                return 0;
            }

            if (HostCallback6 != null)
            {
                mask = (int)Native.InvokeBattleCallback((long)HostCallback6, HostCommand.GetBonus, 0, 0, 0, 0, 0);
            }

            owner = qword_180220130;
        }

        if (((mask >> (*(byte*)(owner + 18) & 31)) & 1) == 0)
        {
            return 0;
        }

        var vote = (BattleCallbackFn)BattleCallbackAddress;
        if (vote == null)
        {
            PsxRandStep();
            return 1;
        }

        if (Native.InvokeBattleCallback((long)vote, HostCommand.GetStartType, 0, 0, 0, 0, 0) == 2)
        {
            return 2;
        }

        if (HostCallback6 != null && (int)Native.InvokeBattleCallback((long)HostCallback6, HostCommand.GetAliveCount, 0, 0, 0, 0, 0) >= 3)
        {
            int roll = (int)(PsxRandStep() >> 16) % 5;
            return roll == 4 ? 1u : 0u;
        }

        PsxRandStep();
        return 1;
    }

    /// <summary>Starts the current effect Block: selects its variant unless suppressed and marks the block started.</summary>
    public static void* SfxBeginBlock()
    {
        Recompiler.Runtime.Tracer.Enter("SfxBeginBlock()");
        void* result = Block;
        dword_180211E4C = 0;
        if (Block != null)
        {
            byte* body = (byte*)Block + *((short*)Block + 1);
            if (body != null)
            {
                if (byte_18021FF49 == 0)
                {
                    dword_180220108 = dword_18021FF1C;
                    SfxSelectBlockVariant((short*)body);
                    ushort high = (ushort)((uint)dword_1802200BE >> 16);
                    result = (void*)(nuint)high;
                    dword_1802200BE = (int)(((uint)dword_1802200BE & 0xFFFF0000u) | high);
                }

                byte_1802200BC = 1;
                byte_18021FF49 = 0;
            }
        }

        return result;
    }

    /// <summary>Resets the 1024-entry (56-byte) and 77-entry (108-byte) record pools.</summary>
    public static long SfxPoolsReset()
    {
        Recompiler.Runtime.Tracer.Enter("SfxPoolsReset()");
        qword_180211E78 = (long)unk_180578E10;
        qword_180211E68 = 1024;
        dword_180211E70 = 0;
        byte* flag = unk_180578E2F;
        for (int i = 0; i < 1024; i++)
        {
            *flag = 0;
            flag += 56;
        }

        dword_180323164 = 0;
        dword_1803231D8 = 0;
        qword_1803210E0 = (long)unk_1803210E8;
        qword_1803210D0 = 77;
        dword_1803210D8 = 0;
        uint count = 0;
        ushort* word = (ushort*)unk_180321118;
        do
        {
            *word = 0;
            count++;
            word += 54;
        }
        while ((int)count < (int)qword_1803210D0);

        return count;
    }

    /// <summary>Index of a1 among the two int16 slots at 0x18032321C; asserts and returns 2 when it is absent.</summary>
    public static long FindPairSlot(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("FindPairSlot(a1)", a1);
        short* slots = (short*)Unsafe.AsPointer(ref dword_18032321C);
        int index = 0;
        do
        {
            if (a1 == slots[index])
            {
                break;
            }

            ++index;
        }
        while (index < 2);

        if (index >= 2)
        {
            PsxCompatibilityAssert();
        }

        return (uint)index;
    }

    /// <summary>
    /// a1 != 0: uploads the four 16 KB pages at 0x1802208D0 to VRAM through the host (rects 64x128 at x 768/832,
    /// y 256/384) and returns 1; a1 == 0: restores the 67584-byte buffer from its .data template and returns 0.
    /// </summary>
    public static long SfxVramPagesUploadOrRestore(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxVramPagesUploadOrRestore(a1)", a1);
        byte* pages = unk_1802208D0;
        if (a1 != 0)
        {
            for (int i = 0; i < 4; i++)
            {
                var upload = (BattleCallbackFn)BattleCallbackAddress;
                if (upload != null)
                {
                    uint x = (uint)(short)(32 * ((i & 0xFFFE) + 24));
                    uint y = (uint)(short)(((i & 1) + 2) << 7);
                    Native.InvokeBattleCallback((long)upload, HostCommand.LoadImage, x, y, 64, 128, (ulong)pages);
                }

                pages += 16384;
            }

            return 1;
        }

        System.Buffer.MemoryCopy(unk_18004F860, pages, 67584, 67584);
        return 0;
    }
}
