using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>Host callback installed by SFX_InitSystem (qword_1801C1DE8), read from the shared module data.</summary>
    private static BattleCallbackFn HostCallback =>
        (BattleCallbackFn)BattleCallbackAddress;

    /// <summary>uint PSX address <paramref name="index"/> of the int array off_180066C68 points to (0x18021FF68).</summary>
    private static uint ContextAddress(int index)
    {
        Recompiler.Runtime.Tracer.Enter("ContextAddress(index)", index);
        return ((uint*)off_180066C68)[index];
    }

    /// <summary>Entry <paramref name="index"/> of an offset table: table + ((int*)table)[index].</summary>
    private static byte* OffsetTableEntry(byte* table, int index)
    {
        Recompiler.Runtime.Tracer.Enter("OffsetTableEntry(table, index)", (nint)table, index);
        return table + *(int*)(table + 4 * index);
    }

    /// <summary>
    /// Returns entry a2 of one of four self-relative offset tables: a2 &lt; 64 from the table whose PSX address is at
    /// +48 + 4*a1 of the context at off_180066C68[1]; 64..127 the table at 0x18022A8D0; 128..191 the table whose PSX
    /// address is at +80 of the object at dword_180323244; 192+ the table at 0x180171DC0. Unresolvable tables are
    /// dereferenced as null, like the original.
    /// </summary>
    public static byte* PsxGetTableEntry(int a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("PsxGetTableEntry(a1, a2)", a1, a2);
        if (a2 >= 64)
        {
            if (a2 < 128)
            {
                return OffsetTableEntry((byte*)Unsafe.AsPointer(ref dword_18022A8D0), a2 - 64);
            }

            if (a2 >= 192)
            {
                return OffsetTableEntry(unk_180171DC0, a2 - 192);
            }

            byte* owner = ResolveGlobalPsxAddress((uint)dword_180323244);
            Block3* block3 = (Block3*)owner;
            uint tableAddress = block3->field_80;
            return OffsetTableEntry(ResolveGlobalPsxAddress(tableAddress), (sbyte)(a2 + 0x80) & 0x7F);
        }

        byte* context = ResolveGlobalPsxAddress(ContextAddress(1));
        uint address = *(uint*)(context + 4L * a1 + 48);
        return OffsetTableEntry(ResolveGlobalPsxAddress(address), a2);
    }

    /// <summary>
    /// Resets the channel when its state (low word of dword_1803231FC) is 0: stores a1, state 1, clears the
    /// counters and buffers. Returns 0, or 0xFFFFFFFF when the channel is busy.
    /// </summary>
    public static long PsxChannelReset(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("PsxChannelReset(a1)", a1);
        if ((ushort)dword_1803231FC != 0)
        {
            return 0xFFFFFFFFL;
        }

        dword_1803231F4 = a1;
        dword_1803231FC = 1;
        dword_18032321C = -1;
        dword_1803231F8 = 0;
        word_18032320A = 0;
        dword_180323240 = 0;
        dword_180323256 = 0;
        dword_1803231F0 = 0;
        word_180323254 = 0;
        qword_180323228 = 0;
        dword_180323230 = 0;
        return 0;
    }

    /// <summary>
    /// Starts the next frame when the channel state is 0: increments the frame counter, clears the per-frame words,
    /// sets state 3, toggles the buffer index and marks that buffer's slot (ushort at 0x18032321C + 2*index) -1.
    /// Returns 0, or 0xFFFFFFFF when the channel is busy.
    /// </summary>
    public static long PsxChannelFlip()
    {
        Recompiler.Runtime.Tracer.Enter("PsxChannelFlip()");
        if ((ushort)dword_1803231FC != 0)
        {
            return 0xFFFFFFFFL;
        }

        ++word_18032320A;
        *(ushort*)Unsafe.AsPointer(ref dword_180323240) = 0;
        *(ushort*)unk_180323257 = 0;
        word_180323254 = 0;
        dword_1803231FC = 3;
        dword_1803231F8 = ((byte)dword_1803231F8 + 1) & 1;
        ((ushort*)Unsafe.AsPointer(ref dword_18032321C))[dword_1803231F8] = 0xFFFF;
        return 0;
    }

    /// <summary>
    /// Reads a qword of the context resolved from off_180066C68[3]: index 16 at +80, 17 at +184, otherwise at
    /// +88 + 8*index; indices 0..7 must be below the signed byte at +36, indices 8+ minus 8 below the one at +39
    /// (assert otherwise). An unresolvable context is dereferenced as null, like the original.
    /// </summary>
    public static long PsxGetContextQword(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("PsxGetContextQword(a1)", a1);
        long index = a1;
        if (a1 == 16)
        {
            return *(long*)(ResolveGlobalPsxAddress(ContextAddress(3)) + 80);
        }

        if (a1 == 17)
        {
            return *(long*)(ResolveGlobalPsxAddress(ContextAddress(3)) + 184);
        }

        if (a1 >= 8)
        {
            if (a1 - 8 < 0 || a1 - 8 >= *(sbyte*)(ResolveGlobalPsxAddress(ContextAddress(3)) + 39))
            {
                PsxCompatibilityAssert();
            }

            return *(long*)(ResolveGlobalPsxAddress(ContextAddress(3)) + 8 * index + 88);
        }

        if (a1 < 0)
        {
            PsxCompatibilityAssert();
        }

        while (true)
        {
            byte* context = ResolveGlobalPsxAddress(ContextAddress(3));
            Context* context2 = (Context*)context;
            if (a1 < context2->field_36)
            {
                return *(long*)(ResolveGlobalPsxAddress(ContextAddress(3)) + 8 * index + 88);
            }

            PsxCompatibilityAssert();
        }
    }

    /// <summary>
    /// Asks the host (HostCommand.CheckStatus | object id at a1+24, 0, 0, 0x1000) and, when it answers nonzero, sends
    /// HostCommand.SetScale | id (129, 2048, 2048, 2048) and returns that answer. Returns the first answer when it is zero,
    /// and 0 when no callback is installed.
    /// </summary>
    public static long HostResetScaleIfFlag1000(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("HostResetScaleIfFlag1000(a1)", a1);
        return HostQueryThenSend(a1, 4096, HostCommand.SetScale, 129, 2048, 2048, 2048);
    }

    /// <summary>
    /// Asks the host (HostCommand.CheckStatus | object id at a1+24, 0, 0, 0x400) and, when it answers nonzero, sends
    /// HostCommand.ShowMesh | id (1, 0, 0, 0) and returns that answer. Returns the first answer when it is zero, and 0 when
    /// no callback is installed.
    /// </summary>
    public static long HostFadeMeshIfFlag400(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("HostFadeMeshIfFlag400(a1)", a1);
        return HostQueryThenSend(a1, 1024, HostCommand.ShowMesh, 1, 0, 0, 0);
    }

    /// <summary>Shared body of the two query-then-send host helpers.</summary>
    private static long HostQueryThenSend(long a1, ulong queryMask, uint command, ulong p2, ulong p3, ulong p4, uint p5)
    {
        Recompiler.Runtime.Tracer.Enter("HostQueryThenSend(a1, queryMask, command, p2, p3, p4, p5)", a1, queryMask, command, p2, p3, p4, p5);
        var callback = HostCallback;
        if (callback == null)
        {
            return 0;
        }

        ulong query = a1 != 0 ? (*(ushort*)(a1 + 24) | HostCommand.CheckStatus) : HostCommand.CheckStatus;
        long answer = Native.InvokeBattleCallback((long)callback, query, 0, 0, queryMask, 0, 0);
        if ((uint)answer == 0)
        {
            return answer;
        }

        callback = HostCallback;
        if (callback == null)
        {
            return 0;
        }

        ulong code = a1 != 0 ? (*(ushort*)(a1 + 24) | command) : command;
        return Native.InvokeBattleCallback((long)callback, code, p2, p3, p4, p5, 0);
    }
}
