namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>PSX context offset of the segment table (32-byte entries, host base at +0 of each entry).</summary>
    private const int PsxCtxSegmentTable = 8;

    /// <summary>PSX context offset of the PSX address of RAM.</summary>
    private const int PsxCtxRamPsxAddress = 8136;

    /// <summary>PSX context offset of the host pointer of RAM.</summary>
    private const int PsxCtxRamHostPointer = 8144;

    /// <summary>PSX context offset of the 1 KB scratchpad.</summary>
    private const int PsxCtxScratchpad = 8152;

    /// <summary>
    /// Sends a 16-bit message pair to the host callback registered by SFX_InitSystem (qword_1801C1DE8 in the shared
    /// module data): message id HostCommand.CheckStatus | *(ushort*)(a1 + 24) (or plain HostCommand.CheckStatus without an object), sub-id 1,
    /// payload lo/hi words of a2. Returns 0 when no callback is installed.
    /// </summary>
    public static byte HostCallbackSend(long obj, uint statusMask)
    {
        Recompiler.Runtime.Tracer.Enter("HostCallbackSend(obj, statusMask)", obj, statusMask);
        HostObject* hostObject = (HostObject*)obj;
        var callback = (BattleCallbackFn)BattleCallbackAddress;
        if (callback == null)
        {
            return 0;
        }

        ulong high = statusMask >> 16;
        ulong low = (ushort)statusMask;
        ulong code = obj != 0 ? (hostObject->HostId | HostCommand.CheckStatus) : HostCommand.CheckStatus;
        return (byte)((uint)Native.InvokeBattleCallback((long)callback, code, 1, low, high, 0, 0) != 0 ? 1 : 0);
    }

    /// <summary>
    /// Resolves a PSX address to a host pointer inside the memory context a1 (the module's context is unk_180576A10):
    /// 0x80xxxxxx below 2 MB is RAM, 0x..C0xxxx is a segment (32-byte table entries at +8), 0x1F8000xx is the 1 KB
    /// scratchpad; otherwise 0.
    /// </summary>
    public static long PsxAddrToPtr(long a1, uint a2)
    {
        Recompiler.Runtime.Tracer.Enter("PsxAddrToPtr(a1, a2)", a1, a2);
        PsxAddrMapper* psxAddrMapper = (PsxAddrMapper*)a1;
        if (a2 == 0)
        {
            return 0;
        }

        long segment = a2 >> 24;
        if ((uint)segment == 128)
        {
            if ((a2 & 0xFFFFFFF) < 0x200000)
            {
                // 32-bit subtraction, then zero-extended, as the original does (claim PSX-ADDR-001).
                return (long)psxAddrMapper->ramHostBase + (uint)(a2 - psxAddrMapper->ramPsxAddress);
            }
        }
        else if ((a2 & 0xC00000) == 0xC00000)
        {
            return *(long*)(32 * segment + a1 + PsxCtxSegmentTable) + (a2 & 0x3FFFFF);
        }

        if (a2 - 0x1F800000u <= 0x3FF)
        {
            return a2 - 0x1F800000u + a1 + PsxCtxScratchpad;
        }

        return 0;
    }
}
