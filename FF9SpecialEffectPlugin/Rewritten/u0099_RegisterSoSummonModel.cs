namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>Magic u16 of a psx_compatibility model header ("so").</summary>
    private const ushort RegisterSoSummonModelMagic = 0x6F73;

    /// <summary>
    /// Registers an "so" model header: u16 magic, u16 texture-list flag, u16 data offset, u16 UV-table offset, then
    /// 4-byte entries from +8. Without a texture list the data goes to RegisterGouEffModel; otherwise the first u16 of
    /// each entry gets the texture page bits (a2 &amp; 3) &lt;&lt; 5 and RegisterTexListModel takes data, entries and UV table.
    /// The entry count is (data offset - 8) &gt;&gt; 3, as in the original.
    /// </summary>
    public static long RegisterSoModel(ushort* a1, sbyte a2)
    {
        Recompiler.Runtime.Tracer.Enter("RegisterSoModel(a1, a2)", (nint)a1, a2);
        if (a1[0] != RegisterSoSummonModelMagic)
        {
            PsxCompatibilityAssert();
        }

        ulong data = (ulong)a1 + a1[2];
        if (a1[1] == 0)
        {
            return RegisterGouEffModel(data);
        }

        ushort* entries = a1 + 4;
        int count = (a1[2] - 8) >> 3;
        ushort bits = (ushort)((a2 & 3) << 5);
        for (int i = 0; i < count; i++)
        {
            entries[2 * i] |= bits;
        }

        return RegisterTexListModel(data, (long)entries, (long)a1 + a1[3]);
    }

    /// <summary>
    /// Builds the summon header a4 from the "so" model a1 (a3 PSX words from a2 to +384, entry/UV tables to
    /// +24/+36/+48, model PSX address to +60), points the summon slot at a4 + 144 after clearing it, registers a4 with
    /// the PSX mapper (dword_180323244) and claims the summon slot; dword_180323250 = its result, which is returned.
    /// </summary>
    public static long RegisterSoSummonModel(ushort* a1, int* a2, uint a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("RegisterSoSummonModel(a1, a2, a3, a4)", (nint)a1, (nint)a2, a3, a4);
        if (a1[0] != RegisterSoSummonModelMagic)
        {
            PsxCompatibilityAssert();
        }

        byte* header = (byte*)a4;
        int count = (a1[2] - 8) >> 3;
        *(ushort*)(header + 2) = (ushort)a3;
        *(uint*)(header + 64) = 0;
        *(ushort*)(header + 4) = (ushort)count;
        *(uint*)(header + 60) = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)a1 + a1[2]);

        if ((int)a3 > 0)
        {
            uint* words = (uint*)(header + 384);
            for (ulong i = 0; i < a3; i++)
            {
                words[i] = (uint)a2[i];
            }
        }

        byte* uv = (byte*)a1 + a1[3];
        for (int k = 0; k < count; k++)
        {
            *(ushort*)(header + 24 + 2 * k) = a1[2 * k + 4];
            *(ushort*)(header + 36 + 2 * k) = *(ushort*)((byte*)a1 + 10 + 4 * k);
            *(ushort*)(header + 48 + 2 * k) = *(ushort*)(uv + 2 + 4 * k);
        }

        // The original clears 8 bytes at 0x18022087C (summon slot +76..+83, including inUse at +80), not one byte.
        *(ulong*)unk_18022087C = 0;
        qword_180220838 = 0;
        qword_180220850 = 0;
        qword_180220830 = (long)(a4 + 144);
        qword_180220858 = 0;
        qword_180220840 = 0;
        qword_180220860 = 0;
        qword_180220868 = 0;
        qword_180220848 = 0;
        qword_180220870 = 0;
        dword_180220878 = 0;
        dword_180323244 = (int)PsxAddrMapperRegister((long)unk_180576A10, a4);

        // The third argument is a stale r8 in the original; RegisterSummonModel never reads it.
        long result = RegisterSummonModel((long)a4, 0, 0);
        dword_180323250 = (int)result;
        return result;
    }
}
