using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Shifts the 12-byte camera track key at a1+12 down to a1 and decodes the 6-byte script key a2 into a1+12:
    /// bytes +20/+21 = a2[0]/a2[1]; when the top two bits of the u16 at a2+2 are set and a2[0] &amp; 0x1F selects
    /// object 1 (0x18021FFF0) or 5 (0x18021FFF8) whose byte +16 is set, the four u16 at +18/+12/+14/+16 come from the
    /// 4-byte entry (19 * (bits - 1) + obj[+32]) of the table at 0x18004AC60, else from 7-bit fields of a2+2/a2+4.
    /// Returns the last 16-bit value stored.
    /// </summary>
    public static short CamPushTrackKey(long a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("CamPushTrackKey(a1, a2)", a1, a2);
        Shape_CamPushTrackKey_a2* shape_CamPushTrackKey_a2 = (Shape_CamPushTrackKey_a2*)a2;
        GeoSlicePart* geoSlicePart = (GeoSlicePart*)a1;
        byte* key = (byte*)a1;
        byte* src = (byte*)a2;
        System.Buffer.MemoryCopy(key + 12, key, 12, 12);
        key[20] = src[0];
        key[21] = src[1];
        ushort packed = shape_CamPushTrackKey_a2->field_2;
        if ((packed & 0xC000) != 0)
        {
            int kind = (sbyte)src[0] & 0x1F;
            long obj = 0;
            bool hasObj = true;
            if (kind == 1)
            {
                obj = qword_18021FFF0;
            }
            else if (kind == 5)
            {
                obj = qword_18021FFF8;
            }
            else
            {
                hasObj = false;
            }

            if (hasObj && *(byte*)(obj + 16) != 0)
            {
                int index = 19 * (packed >> 14) - 19 + *(byte*)(obj + 32);
                byte* entry = unk_18004AC60 + 4L * index;
                *(ushort*)(key + 18) = (ushort)(entry[0] << 6);
                geoSlicePart->countG3 = (ushort)(entry[1] << 4);
                geoSlicePart->countF4 = (ushort)(entry[2] << 4);
                short last = (short)(entry[3] << 4);
                geoSlicePart->countF3 = (ushort)last;
                return last;
            }
        }

        geoSlicePart->countG3 = (ushort)((packed & 0x7F) << 5);
        geoSlicePart->countF4 = (ushort)(((shape_CamPushTrackKey_a2->field_2 >> 7) & 0x7F) << 5);
        geoSlicePart->countF3 = (ushort)((shape_CamPushTrackKey_a2->field_4 & 0x7F) << 5);
        short result = (short)(((shape_CamPushTrackKey_a2->field_4 >> 7) & 0x7F) << 7);
        *(short*)(key + 18) = result;
        return result;
    }

    /// <summary>
    /// Camera shake: decrements the counter at 0x18021FF28, range = 2 * (int)((1 - t) * start + end * t) with
    /// t = counter / duration; unless range is 0, jitters a1[1] (flag bit 0 clear) and a1[0], a1[2] (flag bit 1 clear)
    /// by 4 * (rand % range) - range using the PSX LCG seed at 0x1803231DC. Returns the zero-extended eax of the
    /// original: the new counter, the flags, or the last quotient rand / range.
    /// </summary>
    public static long CamShakeTarget(ushort* a1)
    {
        Recompiler.Runtime.Tracer.Enter("CamShakeTarget(a1)", (nint)a1);
        int count = unchecked(dword_18021FF28 - 1);
        dword_18021FF28 = count;
        double t = (double)count / (double)dword_18021FF24;
        int range = unchecked(Recompiler.Runtime.X86.Cvttsd2si((1.0 - t) * (double)dword_18021FF30 + (double)dword_18021FF2C * t) * 2);
        if (range == 0)
        {
            return (uint)count;
        }

        int flags = dword_18021FF20;
        uint seed;
        if ((flags & 1) != 0)
        {
            seed = (uint)dword_1803231DC;
        }
        else
        {
            seed = unchecked((uint)dword_1803231DC * 1103515245u + 12345u);
            dword_1803231DC = (int)seed;
            a1[1] = unchecked((ushort)(a1[1] + 4 * ((int)(seed >> 16) % range) - range));
            flags = dword_18021FF20;
        }

        if ((flags & 2) != 0)
        {
            return (uint)flags;
        }

        uint first = unchecked(seed * 1103515245u + 12345u);
        int firstRemainder = (int)(first >> 16) % range;
        uint second = unchecked(first * 1103515245u + 12345u);
        dword_1803231DC = (int)second;
        int high = (int)(second >> 16);
        a1[0] = unchecked((ushort)(a1[0] + 4 * firstRemainder - range));
        a1[2] = unchecked((ushort)(a1[2] + 4 * (high % range) - range));
        return (uint)(high / range);
    }

    /// <summary>
    /// Vertical camera wave: d = (int)sqrt(dx^2 + dz^2) between a1 and a2; with the wave word at 0x18021FF14 (else
    /// the one at 0x18022010C) non-zero, offset = ((((int)(sin(phase * 64 / 4096.8 * 2 pi) * 4096.8) * d) &gt;&gt; 13)
    /// * (wave &amp; 0xF0)) &gt;&gt;&gt; 11 and phase += wave &amp; 0xF; the offset is added to a1[1] and a2[1].
    /// </summary>
    public static void CamApplyWave(short* a1, short* a2)
    {
        Recompiler.Runtime.Tracer.Enter("CamApplyWave(a1, a2)", (nint)a1, (nint)a2);
        int dx = a1[0] - a2[0];
        int dz = a1[2] - a2[2];
        int distance = Recompiler.Runtime.X86.Cvttsd2si(Math.Sqrt((double)unchecked(dz * dz + dx * dx)));
        int offset = 0;
        int wave = dword_18021FF14;
        if (wave != 0)
        {
            int phase = dword_18021FF18;
            offset = CamLookAtMatrixWaveOffset(wave, phase, distance);
            dword_18021FF18 = unchecked((wave & 0xF) + phase);
        }
        else
        {
            wave = dword_18022010C;
            if (wave != 0)
            {
                int phase = dword_180220110;
                offset = CamLookAtMatrixWaveOffset(wave, phase, distance);
                dword_180220110 = unchecked((wave & 0xF) + phase);
            }
        }

        a1[1] = unchecked((short)(a1[1] + offset));
        a2[1] = unchecked((short)(a2[1] + offset));
    }

    /// <summary>
    /// Look-at camera matrix: forward = normalize(a3 - a2) * 4096.8 (z 0 becomes 1) is row 2; GTE OP of forward with
    /// (0, -4096, 0) normalised gives row 0, OP again gives row 1 (all written to a1 as a PSX MATRIX); translation =
    /// (-MAC1, -MAC2, 0x180211E4C - MAC3) of RT * a2, then the rotation is multiplied by Rz(a2[3]) (Ry(0), Rx(0)) and
    /// the translation recomputed as RT * T with TR cleared. Returns the final MAC3 zero-extended.
    /// </summary>
    public static long CamLookAtMatrix(long a1, short* a2, short* a3)
    {
        Recompiler.Runtime.Tracer.Enter("CamLookAtMatrix(a1, a2, a3)", a1, (nint)a2, (nint)a3);
        byte* matrix = (byte*)a1;
        int* ir2 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
        int* ir3 = (int*)unk_180211FEC;

        double dx = a3[0] - a2[0];
        double dy = a3[1] - a2[1];
        double dz = a3[2] - a2[2];
        double scale = 4096.8 / Math.Sqrt(dx * dx + dy * dy + dz * dz);
        *ir2 = -4096;
        int fz = Recompiler.Runtime.X86.Cvttsd2si(dz * scale);
        int fx = Recompiler.Runtime.X86.Cvttsd2si(dx * scale);
        int fy = Recompiler.Runtime.X86.Cvttsd2si(dy * scale);
        if (fz == 0)
        {
            fz = 1;
        }

        ushort fzWord = (ushort)fz;
        *(int*)xmmword_180211F40 = fx;
        *(int*)unk_180211F48 = fy;
        dword_180211FE4 = 0;
        *ir3 = 0;
        dword_180211F50 = fzWord;
        GteOp();

        double m1 = dword_180212024;
        double m2 = dword_180212028;
        double m3 = dword_18021202C;
        scale = 4096.8 / Math.Sqrt(m1 * m1 + m2 * m2 + m3 * m3);
        *(int*)xmmword_180211F40 = fx;
        *(int*)unk_180211F48 = fy;
        int rz = Recompiler.Runtime.X86.Cvttsd2si(scale * m3);
        int rx = Recompiler.Runtime.X86.Cvttsd2si(scale * m1);
        int ry = Recompiler.Runtime.X86.Cvttsd2si(scale * m2);
        dword_180211F50 = fzWord;
        dword_180211FE4 = rx;
        *ir2 = ry;
        *ir3 = rz;
        GteOp();

        m1 = dword_180212024;
        m2 = dword_180212028;
        m3 = dword_18021202C;
        scale = 4096.8 / Math.Sqrt(m1 * m1 + m2 * m2 + m3 * m3);
        *(ushort*)(matrix + 16) = fzWord;
        int ux = Recompiler.Runtime.X86.Cvttsd2si(scale * m1);
        int uy = Recompiler.Runtime.X86.Cvttsd2si(scale * m2);
        int uz = Recompiler.Runtime.X86.Cvttsd2si(scale * m3);
        *(ushort*)(matrix + 0) = (ushort)rx;
        *(ushort*)(matrix + 2) = (ushort)ry;
        *(ushort*)(matrix + 4) = (ushort)rz;
        *(ushort*)(matrix + 6) = (ushort)ux;
        *(ushort*)(matrix + 8) = (ushort)uy;
        *(ushort*)(matrix + 10) = (ushort)uz;
        *(ushort*)(matrix + 12) = (ushort)fx;
        *(ushort*)(matrix + 14) = (ushort)fy;

        *(int*)xmmword_180211F40 = *(int*)matrix;
        *(int*)unk_180211F44 = *(int*)(matrix + 4);
        *(int*)unk_180211F48 = *(int*)(matrix + 8);
        *(int*)unk_180211F4C = *(int*)(matrix + 12);
        dword_180211F50 = *(ushort*)(matrix + 16);
        *(int*)Unsafe.AsPointer(ref qword_180211FC0) = *(int*)a2;
        *(int*)unk_180211FC4 = *(int*)(a2 + 2);
        GteRtV0();

        int* translation = (int*)(matrix + 20);
        translation[0] = dword_180212024;
        translation[1] = dword_180212028;
        translation[2] = dword_18021202C;
        translation[0] = unchecked(-translation[0]);
        translation[1] = unchecked(-translation[1]);
        translation[2] = unchecked(dword_180211E4C - translation[2]);

        // Stack PSX MATRIX: identity rotation, then Rz(a2[3]), Ry(0), Rx(0) chained through the returned pointer;
        // its translation is zero (the pad short at +18 is never read).
        short* roll = stackalloc short[16];
        roll[0] = 4096;
        roll[1] = 0;
        roll[2] = 0;
        roll[3] = 0;
        roll[4] = 4096;
        roll[5] = 0;
        roll[6] = 0;
        roll[7] = 0;
        roll[8] = 4096;
        short* rotated = (short*)RotMatrixZ(a2[3], roll);
        rotated = (short*)RotMatrixY(0, rotated);
        RotMatrixX(0, rotated);
        int* rollTranslation = (int*)((byte*)roll + 20);
        rollTranslation[0] = 0;
        rollTranslation[1] = 0;
        rollTranslation[2] = 0;
        GteMulMatrix0((long)roll, (short*)matrix, (ushort*)matrix);
        *(int*)Unsafe.AsPointer(ref qword_180211F54) = rollTranslation[0];
        *(int*)unk_180211F58 = rollTranslation[1];
        dword_180211F5C = rollTranslation[2];

        Stack8_CamLookAtMatrix_vector vector = default;
        vector.union_0_2 = *(ushort*)(matrix + 20);
        vector.union_2_2 = *(ushort*)(matrix + 24);
        vector.union_4_2 = *(ushort*)(matrix + 28);
        vector.union_6_2 = *(ushort*)(matrix + 32);
        *(int*)Unsafe.AsPointer(ref qword_180211FC0) = vector.union_0_4;
        *(int*)unk_180211FC4 = vector.union_4_4;
        GteRtV0Tr();

        translation[0] = dword_180212024;
        translation[1] = dword_180212028;
        int mac3 = dword_18021202C;
        translation[2] = mac3;
        return (uint)mac3;
    }

    /// <summary>((((int)(sin(phase * 64 / 4096.8 * 2 pi) * 4096.8) * distance) &gt;&gt; 13) * (wave &amp; 0xF0)) &gt;&gt;&gt; 11.</summary>
    private static int CamLookAtMatrixWaveOffset(int wave, int phase, int distance)
    {
        Recompiler.Runtime.Tracer.Enter("CamLookAtMatrixWaveOffset(wave, phase, distance)", wave, phase, distance);
        int sine = Recompiler.Runtime.X86.Cvttsd2si(Math.Sin((double)unchecked(phase << 6) / 4096.8 * 2.0 * 3.141592653589793) * 4096.8);
        return (int)(unchecked((uint)((sine * distance >> 13) * (wave & 0xF0))) >> 11);
    }
}
