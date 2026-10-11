namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Starts the state block at 0x180323170 when it is idle (word_180323170 == -1, see sub_1800313D0): stores
    /// <paramref name="a1"/>, resets the block, picks the active object and stores the PSX address of unk_180171DC0
    /// in uint slot 12 of the table at off_180066C68. Returns 0, or 0xFFFFFFFF when already running.
    /// </summary>
    public static long SeqStateBegin(short a1)
    {
        Recompiler.Runtime.Tracer.Enter("SeqStateBegin(a1)", a1);
        if (word_180323170 != -1)
        {
            return 0xFFFFFFFFL;
        }

        word_180323172 = a1;
        dword_18032318C = 0x8000;
        word_180323170 = 0;
        byte_18032317F = 0;
        dword_180323192 = 0;
        dword_180323174 = 0;
        dword_180323180 = 0;
        word_180323178 = 0;
        qword_180323198 = 0;
        PickActiveObjFromReg3();
        ((uint*)off_180066C68)[12] = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)unk_180171DC0);
        return 0;
    }
}
