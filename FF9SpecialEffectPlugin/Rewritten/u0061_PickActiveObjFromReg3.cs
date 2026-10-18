namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>Clears bytes 0 and 2 of the dword at 0x180323192 and sets word_180323170 to -1; returns 0xFFFFFFFF.</summary>
    public static long sub_1800313D0()
    {
        Recompiler.Runtime.Tracer.Enter("sub_1800313D0()");
        byte* flags = (byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref dword_180323192);
        flags[0] = 0;
        flags[2] = 0;
        word_180323170 = -1;
        return 0xFFFFFFFFL;
    }

    /// <summary>
    /// Resolves the PSX address in word 3 of the table at off_180066C68 (an unresolvable address is dereferenced as
    /// null, like the original), reads the host pointer at +80 of that record and makes it qword_180220130 when its
    /// byte +16 is non-zero; returns that pointer either way.
    /// </summary>
    public static long PickActiveObjFromReg3()
    {
        Recompiler.Runtime.Tracer.Enter("PickActiveObjFromReg3()");
        uint address = ((uint*)off_180066C68)[3];
        byte* record = (byte*)PsxAddrToPtr((long)unk_180576A10, address);
        long result = *((long*)record + 10);
        long active = qword_180220130;
        if (*(byte*)(result + 16) != 0)
        {
            active = result;
        }

        qword_180220130 = active;
        return result;
    }
}