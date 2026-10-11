using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>RVA of the decoder that returns an absolute branch target (the base at +0 of the decoder state is subtracted).</summary>
    private const uint MipsDecodeAllTargetDecoderRva = 0xD140;

    /// <summary>RVA of the decoder that returns a second value through its out parameter.</summary>
    private const uint MipsDecodeAllPairDecoderRva = 0xD180;

    /// <summary>
    /// Builds the rotation matrix Ry(a1[1]) * Rx(a1[0]) * Rz(a1[2]) in <paramref name="a3"/> (a stack scratch matrix
    /// when null), scales column j by a2[j] / 4096 when <paramref name="a2"/> is not null, and when a1[3] is non-zero
    /// premultiplies it by the current GTE rotation (RT * column through GteRtIr). Loads the result into the GTE RT and
    /// returns RT33 zero-extended.
    /// </summary>
    public static long SfxBuildRotScaleMatrix(short* a1, uint* a2, byte* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxBuildRotScaleMatrix(a1, a2, a3)", (nint)a1, (nint)a2, (nint)a3);
        byte* scratch = stackalloc byte[32];
        byte* target = a3 != null ? a3 : scratch;
        *(uint*)(target + 14) = 0x10000000;
        *(uint*)(target + 6) = 0x10000000;
        *(uint*)target = 4096;
        *(ushort*)(target + 4) = 0;
        *(uint*)(target + 10) = 0;
        var m = (short*)RotMatrixZ(a1[2], (short*)target);
        m = (short*)RotMatrixX(a1[0], m);
        RotMatrixY(a1[1], m);

        var matrix = (short*)target;
        if (a2 != null)
        {
            for (int k = 0; k < 9; k++)
            {
                matrix[k] = (short)((a2[k % 3] * (uint)matrix[k]) >> 12);
            }
        }

        if (a1[3] != 0)
        {
            int* ir23 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
            for (int col = 0; col < 3; col++)
            {
                dword_180211FE4 = matrix[col];
                ir23[0] = matrix[col + 3];
                ir23[1] = matrix[col + 6];
                GteRtIr();
                dword_180211FE4 = (short)dword_180211FE4;
                matrix[col] = (short)dword_180211FE4;
                ir23[0] = (short)ir23[0];
                matrix[col + 3] = (short)ir23[0];
                ir23[1] = (short)ir23[1];
                matrix[col + 6] = (short)ir23[1];
            }
        }

        *(UInt128*)xmmword_180211F40 = *(UInt128*)target;
        long result = *(ushort*)(target + 16);
        dword_180211F50 = (int)result;
        return result;
    }

    /// <summary>
    /// Initializes the arena <paramref name="a1"/> of <paramref name="a2"/> bytes (rounded up to 4; nothing below 128):
    /// cursor (+0) = a1 + 16, free (+8) = total (+12) = size - 16, payload filled with 0xFFFFFFFF.
    /// </summary>
    public static void SfxArenaInit(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("SfxArenaInit(a1, a2)", a1, a2);
        if (a2 < 128)
        {
            return;
        }

        int size = a2 & 0x7FFFFFFC;
        if ((a2 & 3) != 0)
        {
            size += 4;
        }

        var payload = (uint*)(a1 + 16);
        int capacity = size - 16;
        *(long*)a1 = a1 + 16;
        *(int*)(a1 + 8) = capacity;
        *(int*)(a1 + 12) = capacity;
        int words = capacity / 4;
        for (int i = 0; i < words; i++)
        {
            payload[i] = 0xFFFFFFFF;
        }
    }

    /// <summary>
    /// 0 when the arena <paramref name="a1"/> still has room for <paramref name="a2"/> bytes (rounded up to 4, unsigned
    /// compare); otherwise rewinds it (free = total, cursor = a1 + 16) and returns 1.
    /// </summary>
    public static long SfxArenaResetIfShort(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("SfxArenaResetIfShort(a1, a2)", a1, a2);
        int size = (int)(a2 & 0xFFFFFFFC);
        if ((a2 & 3) != 0)
        {
            size += 4;
        }

        if (*(uint*)(a1 + 8) >= (uint)size)
        {
            return 0;
        }

        *(uint*)(a1 + 8) = *(uint*)(a1 + 12);
        *(long*)a1 = a1 + 16;
        return 1;
    }

    /// <summary>
    /// Fills size / 4 words with 0xFFFFFFFF starting at the size word at a1 - 4 (which is overwritten too); returns
    /// 0xFFFFFFFF after filling, otherwise the word count.
    /// </summary>
    public static long SfxArenaFillBlock(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxArenaFillBlock(a1)", a1);
        var word = (uint*)(a1 - 4);
        long result = *(uint*)(a1 - 4) / 4;
        if ((int)result > 0)
        {
            long count = (uint)result;
            result = 0xFFFFFFFFL;
            while (count != 0)
            {
                *word++ = 0xFFFFFFFF;
                --count;
            }
        }

        return result;
    }

    /// <summary>Low 32 bits of entry a2 + 1 minus entry a2 of table a1 (the entry's size), zero-extended.</summary>
    public static long PsxTableEntrySize(int a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("PsxTableEntrySize(a1, a2)", a1, a2);
        uint next = (uint)PsxGetTableEntry(a1, a2 + 1);
        return next - (uint)PsxGetTableEntry(a1, a2);
    }

    /// <summary>
    /// Bump allocator over the PSX address at dword_180323248: returns its host pointer, then advances it by
    /// <paramref name="a1"/> and lowers the remaining size at dword_18032324C. No bound check.
    /// </summary>
    public static byte* PsxBumpAlloc(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("PsxBumpAlloc(a1)", a1);
        var block = (byte*)PsxAddrToPtr((long)unk_180576A10, (uint)dword_180323248);
        dword_18032324C -= a1;
        dword_180323248 += a1;
        return block;
    }

    /// <summary>Clears the low half of dword_1803231FC; returns 0.</summary>
    public static long SfxClearWord1FC()
    {
        Recompiler.Runtime.Tracer.Enter("SfxClearWord1FC()");
        *(ushort*)Unsafe.AsPointer(ref dword_1803231FC) = 0;
        return 0;
    }

    /// <summary>
    /// Decodes every instruction word of the MIPS code at the state <paramref name="a1"/> (+0 base address, +4 size,
    /// +8 code, +16 cursor, +24 output, +32 offset, +48 opcode index, +50 opcode byte, +52 operands) into a new
    /// 16-byte record per word (the old output is freed with delete[]): the opcode table at dword_180066C70 (56-byte
    /// entries: mask, value, name, null-terminated decoder pointers at +16, byte at +48, terminated by a zero mask)
    /// gives the first entry with (word &amp; mask) == value; each decoder returns one operand (the target decoder's
    /// minus the base; the pair decoder also stores its out value). Record = index | byte &lt;&lt; 16 and the three
    /// operand words. Returns the size zero-extended, or the new buffer when the size is -4.
    /// </summary>
    public static uint* MipsDecodeAll(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("MipsDecodeAll(a1)", a1);
        *(int*)(a1 + 32) = -4;
        *(long*)(a1 + 16) = *(long*)(a1 + 8);
        void* old = *(void**)(a1 + 24);
        if (old != null)
        {
            NativeMemory.Free(old);
        }

        var result = (uint*)NativeMemory.Alloc((nuint)(((ulong)(*(uint*)(a1 + 4) >> 2) + 1) * 16));
        uint* record = result;
        *(long*)(a1 + 24) = (long)result;
        if (*(uint*)(a1 + 32) == *(uint*)(a1 + 4))
        {
            return result;
        }

        nint targetDecoder = Code_MipsJumpTargetOffset; // RVA MipsDecodeAllTargetDecoderRva; the table holds the same pointer in both image modes
        nint pairDecoder = Code_MipsDecodeImm16Rs; // RVA MipsDecodeAllPairDecoderRva
        do
        {
            *(uint*)(a1 + 32) += 4;
            short index = 0;
            uint word = **(uint**)(a1 + 16);
            var entry = (int*)Unsafe.AsPointer(ref dword_180066C70);
            int mask = *entry;
            if (mask != 0)
            {
                bool matched = true;
                while ((word & (uint)mask) != (uint)entry[1])
                {
                    mask = entry[14];
                    entry += 14;
                    ++index;
                    if (mask == 0)
                    {
                        matched = false;
                        break;
                    }
                }

                if (matched)
                {
                    *(short*)(a1 + 48) = index;
                    *(ushort*)(a1 + 50) = *((byte*)entry + 48);
                    var decoder = (nint*)(entry + 4);
                    if (*decoder != 0)
                    {
                        var operand = (int*)(a1 + 52);
                        do
                        {
                            int extra = 0;
                            nint fn = *decoder;
                            int value = (int)((MipsDecodeAllCallbackFn3)fn)(*(uint*)(a1 + 32), word, &extra);
                            *operand = value;
                            if (fn == targetDecoder)
                            {
                                *operand = value - *(int*)a1;
                            }

                            ++operand;
                            if (fn == pairDecoder)
                            {
                                *operand++ = extra;
                            }

                            ++decoder;
                        }
                        while (*decoder != 0);
                    }
                }
            }

            *(long*)(a1 + 16) += 4;
            if (*entry == 0)
            {
                *(short*)(a1 + 48) = index;
                *(ushort*)(a1 + 50) = *((byte*)entry + 48);
            }

            record[0] = *(uint*)(a1 + 48);
            record[1] = *(uint*)(a1 + 52);
            record[2] = *(uint*)(a1 + 56);
            record[3] = *(uint*)(a1 + 60);
            record += 4;
            result = (uint*)(ulong)*(uint*)(a1 + 4);
        }
        while (*(uint*)(a1 + 32) != (uint)(ulong)result);

        return result;
    }

    /// <summary>
    /// Uploads byte_180323207 texture pages of 64x128 (a1 = RECT, w/h set to 64/128) from consecutive 0x4000-byte
    /// blocks at <paramref name="a2"/>: the position comes from the 8-byte entries at the PSX address dword_180323234
    /// (x, y at +0/+2; a second use of the same entry gets y + 128, then the entry advances; an entry whose +6 is 128
    /// advances at once). Each RECT goes to the host callback with code 0x64000000; a page inside x 768..896,
    /// y 256..512 sets bit 0 of dword_18032318C. Returns the last value of the bounds test chain.
    /// </summary>
    public static int SfxUploadTexturePages(short* a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("SfxUploadTexturePages(a1, a2)", (nint)a1, a2);
        Stack8_CamLookAtMatrix_vector* stack8_CamLookAtMatrix_vector = (Stack8_CamLookAtMatrix_vector*)a1;
        int result = byte_180323207;
        stack8_CamLookAtMatrix_vector->union_4_4 = 0x00800040;
        if (result <= 0)
        {
            return result;
        }

        for (long i = (uint)result; i != 0; --i)
        {
            var entry = (ushort*)PsxAddrToPtr((long)unk_180576A10, (uint)dword_180323234);
            bool first = ++word_180323238 < 2;
            *a1 = (short)entry[0];
            ushort y = entry[1];
            bool advance = true;
            if (first)
            {
                a1[1] = (short)y;
                if (entry[3] != 128)
                {
                    advance = false;
                }
            }
            else
            {
                a1[1] = (short)(y + 128);
            }

            if (advance)
            {
                dword_180323234 += 8;
                word_180323238 = 0;
            }

            short w = a1[2];
            if (w != 0)
            {
                short h = a1[3];
                if (h != 0 && BattleCallbackAddress != null)
                {
                    HostObjCallback(HostCommand.LoadImage, (uint)(int)*a1, (uint)(int)a1[1], (uint)(int)w, (uint)(int)h, (ulong)a2);
                }
            }

            result = (ushort)*a1;
            if ((short)result >= 768)
            {
                int x = (short)result;
                result = a1[2];
                if (result + x <= 896)
                {
                    result = (ushort)a1[1];
                    if ((short)result >= 256)
                    {
                        result = (short)result;
                        if ((short)result + a1[3] <= 512)
                        {
                            dword_18032318C |= 1;
                        }
                    }
                }
            }

            a2 += 0x4000;
        }

        return result;
    }

    /// <summary>
    /// Walks the host object list at qword_1800694F0 (Next at +0) and counts the objects whose byte at +16 is set and
    /// for which the host callback answers 0 to code 0x11000000 | id (u16 at +24); a missing callback counts as 0.
    /// </summary>
    public static long HostCountPendingObjects()
    {
        Recompiler.Runtime.Tracer.Enter("HostCountPendingObjects()");
        long obj = BattleObjectList;
        uint count = 0;
        while (obj != 0)
        {
            int answer = 0;
            if (BattleCallbackAddress != null)
            {
                answer = (int)HostObjCallback(*(ushort*)(obj + 24) | HostCommand.GetDisappear, 0, 0, 0, 0, 0);
            }

            bool flagged = *(byte*)(obj + 16) != 0;
            obj = *(long*)obj;
            if (answer == 0 && flagged)
            {
                count++;
            }
        }

        return count;
    }
}
