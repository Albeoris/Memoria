using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiler.Runtime;

/// <summary>
/// Native entries for managed window procedures. NativeAOT cannot create callbacks at run time, so a fixed pool of
/// <c>[UnmanagedCallersOnly]</c> <c>__stdcall</c> slots (the platform convention on x64) forwards to the managed procedure
/// registered in the slot. The rewritten procedure stays an ordinary static method other rewritten code can call directly;
/// <c>Entry</c> is what goes into <c>lpfnWndProc</c>. A procedure registered twice gets the same entry.
/// </summary>
public static unsafe class WindowProcedures
{
    /// <summary>Distinct window procedures one program can register (classes, subclassing).</summary>
    public const int Capacity = 16;

    private static readonly nint[] Managed = new nint[Capacity];
    private static readonly object Gate = new();
    private static int s_count;

    /// <summary>The native entry for <paramref name="procedure"/>; throws when <see cref="Capacity"/> procedures are registered already.</summary>
    public static nint Entry(delegate*<nint, uint, nint, nint, nint> procedure)
    {
        if (procedure is null)
        {
            throw new ArgumentNullException(nameof(procedure));
        }

        lock (Gate)
        {
            for (int slot = 0; slot < s_count; slot++)
            {
                if (Managed[slot] == (nint)procedure)
                {
                    return EntryOf(slot);
                }
            }

            if (s_count == Capacity)
            {
                throw new InvalidOperationException($"More than {Capacity} window procedures registered; raise WindowProcedures.Capacity.");
            }

            Managed[s_count] = (nint)procedure;
            return EntryOf(s_count++);
        }
    }

    /// <summary>
    /// The entry for a procedure that takes <c>WPARAM</c> as <c>nuint</c>: the same slot pool (<c>nint</c> and <c>nuint</c> share
    /// one ABI, so the slot calls it through the <c>nint</c> shape).
    /// </summary>
    public static nint Entry(delegate*<nint, uint, nuint, nint, nint> procedure) => Entry((delegate*<nint, uint, nint, nint, nint>)procedure);

    /// <summary>The managed procedure behind a native entry from <c>Entry</c>, or <c>null</c> for any other address.</summary>
    public static delegate*<nint, uint, nint, nint, nint> ManagedOf(nint entry)
    {
        lock (Gate)
        {
            for (int slot = 0; slot < s_count; slot++)
            {
                if (EntryOf(slot) == entry)
                {
                    return (delegate*<nint, uint, nint, nint, nint>)Managed[slot];
                }
            }
        }

        return null;
    }

    private static nint EntryOf(int slot) => slot switch
    {
        0 => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&Slot0,
        1 => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&Slot1,
        2 => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&Slot2,
        3 => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&Slot3,
        4 => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&Slot4,
        5 => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&Slot5,
        6 => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&Slot6,
        7 => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&Slot7,
        8 => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&Slot8,
        9 => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&Slot9,
        10 => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&Slot10,
        11 => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&Slot11,
        12 => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&Slot12,
        13 => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&Slot13,
        14 => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&Slot14,
        15 => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&Slot15,
        _ => throw new ArgumentOutOfRangeException(nameof(slot)),
    };

    private static nint Call(int slot, nint window, uint message, nint wParam, nint lParam) =>
        ((delegate*<nint, uint, nint, nint, nint>)Volatile.Read(ref Managed[slot]))(window, message, wParam, lParam);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint Slot0(nint window, uint message, nint wParam, nint lParam) => Call(0, window, message, wParam, lParam);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint Slot1(nint window, uint message, nint wParam, nint lParam) => Call(1, window, message, wParam, lParam);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint Slot2(nint window, uint message, nint wParam, nint lParam) => Call(2, window, message, wParam, lParam);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint Slot3(nint window, uint message, nint wParam, nint lParam) => Call(3, window, message, wParam, lParam);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint Slot4(nint window, uint message, nint wParam, nint lParam) => Call(4, window, message, wParam, lParam);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint Slot5(nint window, uint message, nint wParam, nint lParam) => Call(5, window, message, wParam, lParam);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint Slot6(nint window, uint message, nint wParam, nint lParam) => Call(6, window, message, wParam, lParam);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint Slot7(nint window, uint message, nint wParam, nint lParam) => Call(7, window, message, wParam, lParam);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint Slot8(nint window, uint message, nint wParam, nint lParam) => Call(8, window, message, wParam, lParam);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint Slot9(nint window, uint message, nint wParam, nint lParam) => Call(9, window, message, wParam, lParam);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint Slot10(nint window, uint message, nint wParam, nint lParam) => Call(10, window, message, wParam, lParam);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint Slot11(nint window, uint message, nint wParam, nint lParam) => Call(11, window, message, wParam, lParam);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint Slot12(nint window, uint message, nint wParam, nint lParam) => Call(12, window, message, wParam, lParam);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint Slot13(nint window, uint message, nint wParam, nint lParam) => Call(13, window, message, wParam, lParam);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint Slot14(nint window, uint message, nint wParam, nint lParam) => Call(14, window, message, wParam, lParam);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint Slot15(nint window, uint message, nint wParam, nint lParam) => Call(15, window, message, wParam, lParam);
}
