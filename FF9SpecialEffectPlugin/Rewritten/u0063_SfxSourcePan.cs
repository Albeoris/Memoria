using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>Six 8-byte voice slots at word_1803231A4: +0 u16 voice bit (0 = free), +2 s16 age, +4 source.</summary>
    private static short* SfxSourcePanVoices => (short*)Unsafe.AsPointer(ref word_1803231A4);

    /// <summary>Remembers the current table record (qword_180578E08 = qword_180323198, dword_180578E04 = 1) and selects PsxFindTableRecord(a1).</summary>
    public static byte* PsxSelectTableRecord(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("PsxSelectTableRecord(a1)", a1);
        dword_180578E04 = 1;
        qword_180578E08 = qword_180323198;
        byte* result = PsxFindTableRecord(a1);
        qword_180323198 = (long)result;
        return result;
    }

    /// <summary>
    /// Stops the voice whose bit is <paramref name="a1"/>: host message HostCommand.SfxPlay sub-id 2 with the bit, then frees
    /// the slot. Returns the host result (0 without callback), the last bit compared when no slot matches, and 0 for
    /// a1 == 0 (the original leaves rax unset there).
    /// </summary>
    public static long SfxVoiceStop(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxVoiceStop(a1)", a1);
        if (a1 == 0)
        {
            return 0;
        }

        short* slot = SfxSourcePanVoices;
        for (int i = 0; ; i++, slot += 4)
        {
            long id = (ushort)*slot;
            if ((uint)id == (uint)a1)
            {
                break;
            }

            if (i + 1 >= 6)
            {
                return id;
            }
        }

        long result = 0;
        var callback = HostCallback;
        if (callback != null)
        {
            result = Native.InvokeBattleCallback((long)callback, HostCommand.SfxPlay, 2, 0, unchecked((ulong)(a1 & 0xFFFFFF)), 0, 0);
        }

        *slot = 0;
        return result;
    }

    /// <summary>Sets the source of the voice whose bit is <paramref name="a1"/> to <paramref name="a2"/>.</summary>
    public static void SfxVoiceSetSource(int a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("SfxVoiceSetSource(a1, a2)", a1, a2);
        if (a1 == 0)
        {
            return;
        }

        short* slot = SfxSourcePanVoices;
        for (int i = 0; (ushort)*slot != a1; slot += 4)
        {
            if (++i >= 6)
            {
                return;
            }
        }

        *(int*)(slot + 2) = a2;
    }

    /// <summary>Sets the source of every active voice to <paramref name="a1"/>; returns the sixth slot's source.</summary>
    public static long SfxVoiceSetAllSources(uint a1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxVoiceSetAllSources(a1)", a1);
        short* slot = SfxSourcePanVoices;
        for (int i = 0; i < 6; i++, slot += 4)
        {
            if (*slot != 0)
            {
                *(uint*)(slot + 2) = a1;
            }
        }

        return (uint)dword_1803231D0;
    }

    /// <summary>
    /// Takes the first free voice slot, or the oldest (largest age) when all six are busy, and sets bit 1 &lt;&lt;
    /// slot, age 0 and source <paramref name="a1"/>. Returns the bit (0 when no slot qualified).
    /// </summary>
    public static long SfxVoiceAlloc(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxVoiceAlloc(a1)", a1);
        short* slot = SfxSourcePanVoices;
        uint bit = 1;
        short* oldest = null;
        uint oldestBit = 0;
        int maxAge = -1;
        int i = 0;
        while (*slot != 0)
        {
            if (maxAge < slot[1])
            {
                maxAge = slot[1];
                oldest = slot;
                oldestBit = bit;
            }

            i++;
            slot += 4;
            bit *= 2;
            if (i >= 6)
            {
                bit = oldestBit;
                slot = oldest;
                break;
            }
        }

        if (slot != null)
        {
            *slot = (short)bit;
            *(int*)(slot + 2) = a1;
            slot[1] = 0;
        }

        return bit;
    }

    /// <summary>
    /// Stereo pan 65..191 (128 centre) of a sound source: -1 → 128; a value with bit 31 or 28 set is a pointer
    /// (truncated to 32 bits, as in the original) to the position, otherwise the context object index whose host
    /// position is queried. Projects the position through the current matrix (RTPS) and maps SX - 160, clamped to
    /// ±170, to ±63.
    /// </summary>
    public static long SfxSourcePan(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxSourcePan(a1)", a1);
        if ((uint)a1 == 0xFFFFFFFF)
        {
            return 128;
        }

        long position;
        long* source;
        if ((a1 & 0x90000000) != 0)
        {
            source = (long*)(ulong)(uint)a1;
        }
        else
        {
            source = &position;
            long obj = PsxGetContextQword((int)a1);
            HostQueryObjPosition(obj, (long)source);
        }

        GslLoadMatrix(ResolveGlobalPsxAddress(ContextAddress(5)));
        qword_180211FC0 = *source;
        GteRtps(0);
        int x = (short)dword_180211FF8 - 160;
        int sign = x < 0 ? -1 : 1;
        int distance = Math.Abs(x);
        if (distance > 170)
        {
            distance = 170;
        }

        return (uint)((sign * (63 * distance / 170)) + 128);
    }

    /// <summary>
    /// Allocates a voice for source <paramref name="a3"/>, counts the distinct "AKAO" entries among
    /// PsxGetTableEntry(a2, 0..a1), refreshes the GTE projection of the source (SfxSourcePan, result unused) and
    /// sends host message HostCommand.SfxPlay sub-id 0 (count - 1, voice bit, table a2). Returns the voice bit or 0.
    /// </summary>
    public static long SfxVoicePlay(int a1, uint a2, uint a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxVoicePlay(a1, a2, a3)", a1, a2, a3);
        long result = SfxVoiceAlloc((int)a3);
        uint bit = (uint)result;
        if (bit == 0)
        {
            return result;
        }

        const int Capacity = 67;
        ulong* seen = stackalloc ulong[Capacity];
        int distinct = 0;
        long stored = 0;
        for (int index = 0; index <= a1; index++)
        {
            byte* entry = PsxGetTableEntry((int)a2, index);
            if (entry[0] == 'A' && entry[1] == 'K' && entry[2] == 'A' && entry[3] == 'O')
            {
                int position = 0;
                for (long i = 0; i < stored; i++, position++)
                {
                    if ((byte*)seen[i] == entry)
                    {
                        break;
                    }
                }

                if (position == distinct)
                {
                    distinct++;
                    if (stored < Capacity)
                    {
                        seen[stored] = (ulong)entry;
                    }

                    stored++;
                }
            }
        }

        SfxSourcePan(a3);
        var callback = HostCallback;
        if (callback != null)
        {
            Native.InvokeBattleCallback((long)callback, HostCommand.SfxPlay, 0, (uint)distinct - 1, bit & 0xFFFFFF, a2, 0);
        }

        return bit;
    }

    /// <summary>Stops voice <paramref name="a4"/> and plays a new one (SfxVoicePlay(a1, a2, a3)).</summary>
    public static long SfxVoiceRestart(int a1, uint a2, uint a3, int a4)
    {
        Recompiler.Runtime.Tracer.Enter("SfxVoiceRestart(a1, a2, a3, a4)", a1, a2, a3, a4);
        SfxVoiceStop(a4);
        return SfxVoicePlay(a1, a2, a3);
    }

    /// <summary>
    /// Initialises pool header <paramref name="a3"/> over <paramref name="a2"/>: records of 56 bytes, count a1 / 56,
    /// high-water and +8 cleared, in-use byte +31 of every record cleared; clears dword_1803231D8 and
    /// dword_180323164. Returns the pointer past the last cleared byte (0 or 1 without records, as the original).
    /// </summary>
    public static byte* InitPool56(int a1, long a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("InitPool56(a1, a2, a3)", a1, a2, a3);
        *(long*)(a3 + 16) = a2;
        *(long*)(a3 + 4) = 0;
        int quotient = (int)(((ulong)(2454267027L * a1)) >> 32) >> 5;
        byte* result = (byte*)(nuint)((uint)quotient >> 31);
        int count = (int)(nuint)result + quotient;
        *(int*)a3 = count;
        if (count > 0)
        {
            result = (byte*)(a2 + 31);
            int i = 0;
            do
            {
                *result = 0;
                i++;
                result += 56;
            }
            while (i < *(int*)a3);
        }

        dword_1803231D8 = 0;
        dword_180323164 = 0;
        return result;
    }

    /// <summary>
    /// Initialises the pool at <paramref name="a2"/> (header 24 bytes, records of 108 bytes from +24, count
    /// (a1 - 24) / 108), clearing u16 +48 of every record. Returns the pointer past the last cleared word (a2 + 24
    /// without records).
    /// </summary>
    public static ushort* InitPool108(int a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("InitPool108(a1, a2)", a1, a2);
        *(long*)(a2 + 4) = 0;
        uint quotient = (uint)((int)((ulong)(1272582903L * (a1 - 24)) >> 32) >> 5);
        int count = (int)((quotient >> 31) + quotient);
        ushort* result = (ushort*)(a2 + 24);
        *(long*)(a2 + 16) = a2 + 24;
        *(int*)a2 = count;
        if (count > 0)
        {
            result = (ushort*)(a2 + 72);
            int i = 0;
            do
            {
                *result = 0;
                i++;
                result += 54;
            }
            while (i < *(int*)a2);
        }

        return result;
    }

    /// <summary>
    /// For every in-use record (+31) of pool <paramref name="a1"/> owned by <paramref name="a2"/> (+48), copies byte
    /// +27 to +26; then marks a2 released (u16 +48 = 0xFFFF). Returns 0xFFFFFFFF.
    /// </summary>
    public static long Pool56ReleaseOwner(long a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("Pool56ReleaseOwner(a1, a2)", a1, a2);
        int i = 0;
        if (*(int*)a1 > 0)
        {
            byte* record = (byte*)(*(long*)(a1 + 16));
            do
            {
                if (record[31] != 0 && *(long*)(record + 48) == a2)
                {
                    record[26] = record[27];
                }

                i++;
                record += 56;
            }
            while (i < *(int*)a1);
        }

        *(ushort*)(a2 + 48) = 0xFFFF;
        return 0xFFFFFFFFL;
    }

    /// <summary>First free record (+31 == 0) of pool <paramref name="a1"/>, raising the high-water index at +8; 0 when full.</summary>
    public static long Pool56FindFree(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("Pool56FindFree(a1)", a1);
        int count = *(int*)a1;
        long record = *(long*)(a1 + 16);
        int index = 0;
        if (count > 0)
        {
            while (*(byte*)(record + 31) != 0)
            {
                index++;
                record += 56;
                if (index >= count)
                {
                    return 0;
                }
            }

            if (*(uint*)(a1 + 8) < (uint)index)
            {
                *(uint*)(a1 + 8) = (uint)index;
            }
        }

        return index >= count ? 0 : record;
    }
}
