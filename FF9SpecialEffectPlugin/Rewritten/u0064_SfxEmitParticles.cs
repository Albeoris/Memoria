using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Emits particles of emitter <paramref name="a2"/> into the 56-byte slot pool <paramref name="a1"/>.
    /// The emitter state (*(a2 + 48)) points at a descriptor (PSX address at +4) whose fields sit at negative
    /// offsets (-56..0) and whose kind byte (+0) selects the per-kind initialisation. The emission count comes from
    /// GTE INTPL of the descriptor rates with the 4.12 ratio a2[26] / a2[27], carried in a2[18] (fraction) and
    /// dword_1806223E4 (accumulator). Each particle gets a weighted sub-emitter pick, lifetime, scatter offset
    /// rotated by random angles (RotateDepthVector) around <paramref name="a3"/>, angle jitter (a4 for kinds 4/5)
    /// and kind-specific motion fields; the LCG state lives in dword_1803231DC. Returns the incidental AX of the
    /// original at the early exits; callers discard it.
    /// </summary>
    public static short SfxEmitParticles(long a1, long a2, short* a3, int* a4)
    {
        Recompiler.Runtime.Tracer.Enter("SfxEmitParticles(a1, a2, a3, a4)", a1, a2, (nint)a3, (nint)a4);
        byte* emitter = (byte*)a2;
        byte* state = *(byte**)(emitter + 48);
        ushort pendingPlusOne = unchecked((ushort)(*(ushort*)(state + 48) + 1));
        if (pendingPlusOne <= 1)
        {
            return unchecked((short)pendingPlusOne);
        }

        // Every inlined PSX ladder of the original resolves the same field state+4, which nothing here writes.
        byte* desc = ResolveGlobalPsxAddress(*(uint*)(state + 4));
        Desc* desc2 = (Desc*)desc;
        int rate = emitter[26];
        int ratio = (rate << 12) / emitter[27];

        if ((emitter[18] & 0x80) != 0)
        {
            byte* limitBlock = ResolveGlobalPsxAddress(*(uint*)(state + 36));
            short limit = *(short*)(limitBlock + 6);
            if (*(short*)(state + 56) >= limit)
            {
                return limit;
            }
        }

        int threshold = desc[-39] - 1;
        if (rate < threshold)
        {
            return unchecked((short)threshold);
        }

        int* ir2 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
        int* ir3 = (int*)unk_180211FEC;
        int* intplLo = (int*)Unsafe.AsPointer(ref qword_180211F94);

        dword_180211FE0 = ratio;
        dword_180211FE4 = *(short*)(desc - 56);
        *intplLo = *(short*)(desc - 54);
        GteIntpl();

        int mac1 = dword_180212024;
        int whole = mac1 >> 8;
        int fraction = ((mac1 >> 1) & 0x7F) + (emitter[18] & 0x7F);
        if (fraction >= 128)
        {
            fraction -= 128;
            if (fraction == 0)
            {
                fraction = 1;
            }

            ++whole;
        }

        emitter[18] = unchecked((byte)(fraction | (emitter[18] & 0x80)));
        int accumulated = (whole << 8) + dword_1806223E4;
        dword_1806223E4 = (byte)accumulated;
        int count = accumulated >> 8;
        if (count <= 0)
        {
            return (byte)accumulated;
        }

        SfxAttachmentList* pool = (SfxAttachmentList*)a1;
        uint seed = unchecked((uint)dword_1803231DC);
        int* angles = stackalloc int[2]; // v197 (x | y << 16), v198 (z in the low word)
        short* ang = (short*)angles;
        Span<short> offset = stackalloc short[4]; // v202..v204, written by RotateDepthVector
        byte* subEntry = null; // v205 keeps the last picked sub-emitter entry across iterations
        int yaw = 0; // v200
        int distance = 0; // v201
        int depth = 0; // v208

        for (int emitted = 0; emitted < count; emitted++)
        {
            int cap = desc[-40];
            if (*(short*)(state + 54) >= cap)
            {
                return (short)cap;
            }

            int slots = pool->Count;
            SfxAttachment* slot = pool->Entries;
            int index = 0;
            if (slots > 0)
            {
                while (index < slots && slot->Active != 0)
                {
                    index++;
                    slot++;
                }

                if (index < slots && pool->HighestUsedIndex < index)
                {
                    pool->HighestUsedIndex = index;
                }
            }

            if (index >= slots || slot == null)
            {
                return unchecked((short)index);
            }

            byte* particle = (byte*)slot;
            SfxAttachment* sfxAttachment = (SfxAttachment*)particle;
            ++*(short*)(state + 54);

            // Weighted pick of a sub-emitter entry (weights at desc-48, entry ids at desc-52).
            int choices = desc[-43];
            uint r0 = SfxEmitParticlesNext(seed); // v30
            int pick = 0;
            int weight = 0;
            if (choices > 0)
            {
                do
                {
                    weight += desc[pick - 48];
                    if ((int)((r0 >> 16) & 0x7F) < weight)
                    {
                        break;
                    }

                    ++pick;
                }
                while (pick < choices);
            }

            yaw = pick;
            int sub = pick < choices ? desc[pick - 52] : 255;
            distance = sub;
            particle[28] = (byte)sub;
            particle[29] = 0;

            uint r; // v46
            int life; // v48 (only the low byte is stored)
            if (sub == 255)
            {
                r = SfxEmitParticlesNext(r0);
                life = SfxEmitParticlesLife(desc, ratio, r);
            }
            else
            {
                byte* table = ResolveGlobalPsxAddress(*(uint*)(state + 24));
                byte* entry = table + 40 * sub;
                subEntry = entry;
                if (*(ushort*)(entry + 4) == 1)
                {
                    life = *(short*)(entry + 8);
                }
                else
                {
                    r0 = SfxEmitParticlesNext(r0);
                    life = SfxEmitParticlesLife(desc, ratio, r0);
                }

                // Sub-emitter offset: GPF (sf = 0) of the 8x entry offsets by a2[26].
                offset[0] = (short)((8 * (sbyte)entry[20]) & 0xFFF);
                short offsetY = (short)((8 * (sbyte)entry[21]) & 0xFFF);
                short offsetZ = (short)((8 * (sbyte)entry[22]) & 0xFFF);
                dword_180211FE4 = offset[0];
                *ir2 = offsetY;
                offset[1] = offsetY;
                offset[2] = offsetZ;
                dword_180211FE0 = rate;
                *ir3 = offsetZ;
                GteGpfSf0();

                uint r1 = SfxEmitParticlesNext(r0);
                int spreadX = 16 * entry[23];
                int spreadY = 16 * entry[24];
                uint r2 = SfxEmitParticlesNext(r1);
                int jitterX = (16 * (sbyte)entry[17] + SfxEmitParticlesJitter(r1, spreadX)) & 0xFFF;
                int jitterY = (16 * (sbyte)entry[18] + SfxEmitParticlesJitter(r2, spreadY)) & 0xFFF;
                int spreadZ = 16 * entry[25];
                uint r3 = SfxEmitParticlesNext(r2);
                dword_180211FE0 = 1;
                dword_180211FE4 = jitterX;
                *ir2 = jitterY;
                *ir3 = (16 * (sbyte)entry[19] + SfxEmitParticlesJitter(r3, spreadZ)) & 0xFFF;
                GteGplSf0();

                r = SfxEmitParticlesNext(r3);
                particle[19] = unchecked((byte)SfxEmitParticlesJitter(r, entry[16]));
                dword_180211FE4 = (short)dword_180211FE4;
                *(short*)(particle + 20) = (short)dword_180211FE4;
                *ir2 = (short)*ir2;
                *(short*)(particle + 22) = (short)*ir2;
                *ir3 = (short)*ir3;
                *(short*)(particle + 24) = (short)*ir3;
            }

            particle[18] = desc[1] != 0 ? (byte)0xFF : (byte)0x80;
            int counter = *(short*)(state + 16);
            particle[27] = unchecked((byte)life);
            particle[26] = 0;
            sfxAttachment->Owner = (long)state;
            particle[31] = unchecked((byte)(counter % 255 + 1));
            uint r4 = SfxEmitParticlesNext(r); // v91: kept in r11 across GteGpf
            particle[30] = unchecked((byte)(SfxEmitParticlesJitter(r4, *(short*)(desc - 38)) + 64));
            ++*(short*)(state + 16);
            if (*(uint*)(state + 20) != 0)
            {
                short wrap = *(short*)(state + 18);
                if (wrap != 0)
                {
                    *(short*)(state + 16) = (short)(*(short*)(state + 16) % (2 * wrap));
                }
            }

            dword_180211FE0 = 4096 - ratio;
            dword_180211FE4 = *(short*)(desc - 8);
            *ir2 = *(short*)(desc - 6);
            *ir3 = *(short*)(desc - 4);
            GteGpf();

            ang[2] = 0;
            seed = SfxEmitParticlesNext(r4);
            dword_1803231DC = unchecked((int)seed);
            int pitch = (int)(seed >> 22);
            int phase = (dword_1803231E8 + 1) & 7;
            dword_1803231E8 = phase;
            int heading = (int)((seed >> 16) & 0x3FF) + ((phase & 6) << 9);
            if ((phase & 1) == 0)
            {
                pitch = -pitch;
            }

            dword_180211FE0 = ratio;
            dword_180211FE4 = *(short*)(desc - 4);
            *ir2 = *(short*)(desc - 2);
            angles[0] = heading | (pitch << 16);
            *ir3 = desc2->union_0_2;
            GteGpl();

            int low = (short)dword_180211FE4;
            int high = (short)*ir2;
            *ir2 = high;
            dword_180211FE4 = low;
            *ir3 = (short)*ir3;
            int radius = low;
            if (low != high)
            {
                seed = SfxEmitParticlesNext(seed);
                dword_1803231DC = unchecked((int)seed);
                radius = (int)(seed >> 16) % (high - low) + low;
            }

            RotateDepthVector(ang, radius, (ushort*)((short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(offset))));
            sfxAttachment->X = (offset[0] + a3[0]) << 12;
            sfxAttachment->Y = (offset[1] + a3[1]) << 12;
            sfxAttachment->Z = (offset[2] + a3[2]) << 12;

            switch (desc[-44])
            {
                case 0:
                    SfxEmitParticlesJitterAngles(desc, ang, ref seed, false);
                    break;
                case 1:
                    SfxEmitParticlesJitterAngles(desc, ang, ref seed, true);
                    break;
                case 2:
                {
                    seed = SfxEmitParticlesNext(seed);
                    ang[2] = 0;
                    dword_1803231DC = unchecked((int)seed);
                    int nextPhase = (phase + 1) & 7;
                    dword_1803231E8 = nextPhase;
                    int nextPitch = (int)(seed >> 22);
                    if ((nextPhase & 1) == 0)
                    {
                        nextPitch = -nextPitch;
                    }

                    angles[0] = ((int)((seed >> 16) & 0x3FF) + ((nextPhase & 6) << 9)) | (nextPitch << 16);
                    break;
                }

                case 3:
                    angles[0] = *(int*)(state + 92);
                    angles[1] = *(int*)(state + 96);
                    SfxEmitParticlesJitterAngles(desc, ang, ref seed, false);
                    break;
                case 4:
                    angles[0] = a4[0];
                    angles[1] = a4[1];
                    SfxEmitParticlesJitterAngles(desc, ang, ref seed, false);
                    break;
                case 5:
                    angles[0] = a4[0];
                    angles[1] = a4[1];
                    SfxEmitParticlesJitterAngles(desc, ang, ref seed, true);
                    break;
            }

            short angleX = ang[0]; // v107
            short angleY = ang[1]; // v108
            if (subEntry != null && subEntry[7] != 0)
            {
                *(short*)(particle + 20) += angleX;
                *(short*)(particle + 22) += angleY;
            }

            short* rec = (short*)desc;
            switch (desc[0])
            {
                case 0:
                case 5:
                {
                    dword_180211FE4 = rec[4];
                    *intplLo = rec[5];
                    dword_180211FE0 = ratio;
                    GteIntpl();
                    uint q1 = SfxEmitParticlesNext(seed);
                    RotateDepthVector(ang, dword_180212024 + SfxEmitParticlesJitter(q1, rec[6]), (ushort*)(particle + 12));
                    seed = SfxEmitParticlesNext(q1);
                    dword_1803231DC = unchecked((int)seed);
                    int speed = rec[7] + SfxEmitParticlesJitter(seed, rec[8]);
                    *(short*)(particle + 32) = (short)(speed < 0 ? 0 : speed);
                    break;
                }

                case 1:
                {
                    dword_180211FE4 = rec[16];
                    *intplLo = rec[17];
                    dword_180211FE0 = ratio;
                    GteIntpl();
                    uint q1 = SfxEmitParticlesNext(seed);
                    RotateDepthVector(ang, dword_180212024 + SfxEmitParticlesJitter(q1, rec[18]), (ushort*)(particle + 12));
                    uint q2 = SfxEmitParticlesNext(q1);
                    int speed = rec[8] + SfxEmitParticlesJitter(q2, rec[9]);
                    *(int*)(particle + 36) = angles[0];
                    *(short*)(particle + 42) = (short)(speed < 0 ? 0 : speed);
                    uint q3 = SfxEmitParticlesNext(q2);
                    uint q4 = SfxEmitParticlesNext(q3);
                    *(short*)(particle + 32) = unchecked((short)(rec[4] + SfxEmitParticlesJitter(q3, rec[6])));
                    *(short*)(particle + 34) = unchecked((short)(rec[5] + SfxEmitParticlesJitter(q4, rec[7])));
                    dword_180211FE4 = rec[12];
                    *intplLo = rec[13];
                    dword_180211FE0 = ratio;
                    GteIntpl();
                    seed = SfxEmitParticlesNext(q4);
                    dword_1803231DC = unchecked((int)seed);
                    *(short*)(particle + 40) = unchecked((short)(dword_180212024 + SfxEmitParticlesJitter(seed, rec[11])));
                    break;
                }

                case 2:
                {
                    *(int*)(particle + 40) = angles[0];
                    SfxEmitParticlesIntplVector(rec, ratio);
                    short mac1Low = (short)dword_180212024;
                    uint q1 = SfxEmitParticlesNext(seed);
                    uint q2 = SfxEmitParticlesNext(q1);
                    *(short*)(particle + 14) = unchecked((short)(dword_180212028 + SfxEmitParticlesJitter(q1, rec[7])));
                    *(short*)(particle + 38) = unchecked((short)(mac1Low + SfxEmitParticlesJitter(q2, rec[12])));
                    dword_180211FE4 = rec[4];
                    *intplLo = rec[5];
                    dword_180211FE0 = ratio;
                    GteIntpl();
                    seed = SfxEmitParticlesNext(q2);
                    dword_1803231DC = unchecked((int)seed);
                    *(short*)(particle + 34) = unchecked((short)(dword_180212024 + SfxEmitParticlesJitter(seed, rec[6])));

                    long origin = 0; // v214, zeroed right before the call
                    ComputeLocalYawDistance((ushort*)((short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(offset))), (ushort*)&origin, (ushort*)ang, (uint*)&depth, (uint*)&distance, (uint*)&yaw);
                    uint* position = (uint*)a2;
                    *(short*)(particle + 12) = unchecked((short)yaw);
                    *(short*)(particle + 32) = unchecked((short)depth);
                    *(short*)(particle + 36) = unchecked((short)distance);
                    sfxAttachment->X = (int)position[0];
                    sfxAttachment->Y = (int)position[1];
                    *(short*)(particle + 16) = 0;
                    sfxAttachment->Z = (int)position[2];
                    break;
                }

                case 3:
                {
                    *(int*)(particle + 12) = angles[0];
                    uint q1 = SfxEmitParticlesNext(seed);
                    uint q2 = SfxEmitParticlesNext(q1);
                    *(short*)(particle + 32) = unchecked((short)(rec[4] + SfxEmitParticlesJitter(q1, rec[6])));
                    *(short*)(particle + 34) = unchecked((short)(rec[5] + SfxEmitParticlesJitter(q2, rec[7])));
                    dword_180211FE4 = rec[12];
                    *intplLo = rec[13];
                    dword_180211FE0 = ratio;
                    GteIntpl();
                    uint q3 = SfxEmitParticlesNext(q2);
                    uint q4 = SfxEmitParticlesNext(q3);
                    *(short*)(particle + 16) = unchecked((short)(dword_180212024 + SfxEmitParticlesJitter(q3, rec[11])));
                    seed = SfxEmitParticlesNext(q4);
                    dword_1803231DC = unchecked((int)seed);
                    *(short*)(particle + 38) = unchecked((short)(angleX + SfxEmitParticlesJitter(q4, rec[8])));
                    *(short*)(particle + 36) = 0;
                    *(short*)(particle + 40) = unchecked((short)(angleY + SfxEmitParticlesJitter(seed, rec[9])));
                    break;
                }

                case 4:
                {
                    *(int*)(particle + 40) = angles[0];
                    SfxEmitParticlesIntplVector(rec, ratio);
                    short mac1Low = (short)dword_180212024;
                    uint q1 = SfxEmitParticlesNext(seed);
                    uint q2 = SfxEmitParticlesNext(q1);
                    *(short*)(particle + 14) = unchecked((short)(dword_180212028 + SfxEmitParticlesJitter(q1, rec[13])));
                    *(short*)(particle + 32) = unchecked((short)(mac1Low + SfxEmitParticlesJitter(q2, rec[12])));
                    dword_180211FE4 = rec[4];
                    *intplLo = rec[5];
                    dword_180211FE0 = ratio;
                    GteIntpl();
                    uint q3 = SfxEmitParticlesNext(q2);
                    seed = SfxEmitParticlesNext(q3);
                    dword_1803231DC = unchecked((int)seed);
                    int radial = dword_180212024 + SfxEmitParticlesJitter(q3, rec[6]);
                    *(short*)(particle + 12) = unchecked((short)(seed >> 16));
                    *(int*)(particle + 36) = 0;
                    *(short*)(particle + 16) = 0;
                    *(short*)(particle + 34) = unchecked((short)radial);
                    break;
                }
            }

            if (subEntry != null && (SfxEmitParticlesCopiesColour(subEntry[0]) || SfxEmitParticlesCopiesColour(subEntry[1])))
            {
                byte* record = SfxIndexedRecordPtr((long)particle, (long)state);
                byte* source = ResolveGlobalPsxAddress(*(uint*)(subEntry + 28));
                if (record != null)
                {
                    ushort value = *(ushort*)(source + 2);
                    record[2] = 0;
                    record[1] = (byte)value;
                    record[0] = (byte)value;
                }
            }
        }

        // The original returns whatever AX holds after the last iteration; both callers discard it.
        return 0;
    }

    /// <summary>One step of the PSX rand LCG (seed * 1103515245 + 12345).</summary>
    private static uint SfxEmitParticlesNext(uint seed)
    {
        Recompiler.Runtime.Tracer.Enter("SfxEmitParticlesNext(seed)", seed);
        return unchecked(seed * 1103515245u + 12345u);
    }

    /// <summary>(r &gt;&gt; 16) % (spread or 1) - spread / 2 (arithmetic shift), as the original's jitter terms.</summary>
    private static int SfxEmitParticlesJitter(uint r, int spread)
    {
        Recompiler.Runtime.Tracer.Enter("SfxEmitParticlesJitter(r, spread)", r, spread);
        return (int)(r >> 16) % (spread != 0 ? spread : 1) - (spread >> 1);
    }

    /// <summary>
    /// Lifetime: GTE INTPL of the descriptor's (+2, +4) pair with the ratio, then MAC1 + jitter(r, desc+6), at
    /// least 1.
    /// </summary>
    private static int SfxEmitParticlesLife(byte* desc, int ratio, uint r)
    {
        Recompiler.Runtime.Tracer.Enter("SfxEmitParticlesLife(desc, ratio, r)", (nint)desc, ratio, r);
        Desc* desc2 = (Desc*)desc;
        dword_180211FE0 = ratio;
        dword_180211FE4 = desc2->field_2;
        *(int*)Unsafe.AsPointer(ref qword_180211F94) = desc2->field_4;
        GteIntpl();
        int life = SfxEmitParticlesJitter(r, desc2->field_6) + dword_180212024;
        return life < 1 ? 1 : life;
    }

    /// <summary>
    /// Angle jitter of the spawn-angle switch: x and y each get jitter(desc-42) from two LCG steps (stored to
    /// dword_1803231DC); <paramref name="wrapX"/> subtracts 2048 from x and keeps 12 bits.
    /// </summary>
    private static void SfxEmitParticlesJitterAngles(byte* desc, short* ang, ref uint seed, bool wrapX)
    {
        Recompiler.Runtime.Tracer.Enter("SfxEmitParticlesJitterAngles(desc, ang, seed, wrapX)", (nint)desc, (nint)ang, seed, wrapX);
        int spread = *(short*)(desc - 42);
        uint t = SfxEmitParticlesNext(seed);
        seed = SfxEmitParticlesNext(t);
        dword_1803231DC = unchecked((int)seed);
        int x = SfxEmitParticlesJitter(t, spread) + ang[0];
        ang[0] = unchecked((short)(wrapX ? (x - 2048) & 0xFFF : x));
        ang[1] = unchecked((short)(SfxEmitParticlesJitter(seed, spread) + ang[1]));
    }

    /// <summary>GTE INTPL with IR = rec[8..10] and the far colour/vector words rec[10..12] (kinds 2 and 4).</summary>
    private static void SfxEmitParticlesIntplVector(short* rec, int ratio)
    {
        Recompiler.Runtime.Tracer.Enter("SfxEmitParticlesIntplVector(rec, ratio)", (nint)rec, ratio);
        dword_180211FE0 = ratio;
        dword_180211FE4 = rec[8];
        *(int*)Unsafe.AsPointer(ref qword_180211FE8) = rec[9];
        *(int*)unk_180211FEC = rec[10];
        *(int*)Unsafe.AsPointer(ref qword_180211F94) = rec[10];
        *(int*)unk_180211F98 = rec[11];
        dword_180211F9C = rec[12];
        GteIntpl();
    }

    /// <summary>Sub-emitter mode byte that copies the colour word into the indexed record: 4, 5 or 8.</summary>
    private static bool SfxEmitParticlesCopiesColour(int mode)
    {
        Recompiler.Runtime.Tracer.Enter("SfxEmitParticlesCopiesColour(mode)", mode);
        return ((mode - 4) & 0xFA) == 0 && mode != 9;
    }
}
