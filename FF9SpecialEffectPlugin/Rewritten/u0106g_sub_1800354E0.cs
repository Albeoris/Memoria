using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Locals of sub_1800354E0 that live across spawns and emitters in the original's frame (stack slots and the
    /// r12 origin register): stale values are reused by modes that do not set them.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    private struct Sub1800354E0Frame
    {
        /// <summary>rbp-58h: direction angles (x, y, z, pad) as 4 words.</summary>
        public long Dir;

        /// <summary>rbp-50h: spawn offset (x, y, z, script flag) as 4 words.</summary>
        public long Offset;

        /// <summary>rbp-48h: scratch vector (ribbon head, zero origin, yaw reference).</summary>
        public long Temp;

        /// <summary>rsp+50h: packed angles for TransformPointByAngles / RotationToLookAngles.</summary>
        public long Angles;

        /// <summary>rsp+5Ch.</summary>
        public int Depth;

        /// <summary>rsp+60h.</summary>
        public uint Aux;

        /// <summary>rsp+64h: lifetime of the emitter definition (D.s16[3]), or the roll of mode 3.</summary>
        public int Life;

        /// <summary>rsp+68h: emitter phase E.s16[28].</summary>
        public int Phase;

        /// <summary>rsp+58h: (Phase &lt;&lt; 12) / Life, the GTE interpolation factor.</summary>
        public int Ratio;

        /// <summary>rsp+7Ch: chosen template index (255 = none).</summary>
        public int Pick;

        /// <summary>esi: register copy of dword_1803231DC.</summary>
        public uint Seed;

        /// <summary>r12: spawn origin (3 words), kept from the previous spawn when a mode does not set it.</summary>
        public short* Origin;

        /// <summary>rbp-70h: template entry of the last weighted pick (40 bytes), kept when the pick is 255.</summary>
        public byte* Entry;
    }

    /// <summary>
    /// Emitter tick of the effect system: for every emitter record (108 bytes from *(a2 + 16), indices 0 ..
    /// *(int*)(a2 + 8) inclusive) with an active flag word at +48 (not 0 / 0xFFFF), accumulates the 8.8 spawn rate
    /// E.s16[25] into the fraction dword_1806223E0 and spawns that many particles into the first free 56-byte slots of
    /// the pool a1 (count +0, high-water mark +8, array +16; a slot is free when byte +31 is 0) while E.s16[26] stays
    /// below the definition limit D.s16[6]. A spawn picks a weighted template entry (byte weights before *(E + 24)),
    /// sets life/fade/colour from the template or the type record *(E + 0), places the particle by the definition mode
    /// *(u32*)D (script / sphere / ring / cone / segment / arc, origin *(E + 40)), jitters the direction, applies the
    /// type's motion (types 0..5) and starts a ribbon trail for template kinds 4, 5 and 8.
    /// </summary>
    public static void sub_1800354E0(long a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("sub_1800354E0(a1, a2)", a1, a2);
        Sub1800354E0Frame frame = default;
        Sub1800354E0Frame* f = &frame;
        long emitter = *(long*)(a2 + 16);
        if (*(int*)(a2 + 8) < 0)
        {
            return;
        }

        int fraction = dword_1806223E0;
        f->Seed = (uint)dword_1803231DC;
        for (int index = 0; index <= *(int*)(a2 + 8); index++, emitter += 108)
        {
            ushort active = *(ushort*)(emitter + 48);
            if (active == 0xFFFF || active == 0)
            {
                continue;
            }

            byte* weights = Sub1800354E0Ptr(*(uint*)(emitter + 24));
            int total = (*(short*)(emitter + 50) << 8) + fraction;
            fraction = (byte)total;
            dword_1806223E0 = (byte)total;
            int count = total >> 8;
            if (count <= 0)
            {
                continue;
            }

            for (int n = 0; n < count; n++)
            {
                if (!Sub1800354E0SpawnOne(f, a1, emitter, weights))
                {
                    break;
                }
            }

            fraction = dword_1806223E0;
        }
    }

    private static byte* Sub1800354E0Ptr(uint address)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0Ptr(address)", address);
        return ResolveGlobalPsxAddress(address);
    }

    private static uint Sub1800354E0Rand(uint x)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0Rand(x)", x);
        return unchecked(1103515245u * x + 12345u);
    }

    private static int Sub1800354E0Div(int value)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0Div(value)", value);
        return value != 0 ? value : 1;
    }

    private static int* Sub1800354E0Ir2 => (int*)Unsafe.AsPointer(ref qword_180211FE8);

    private static int* Sub1800354E0Ir3 => (int*)unk_180211FEC;

    private static int* Sub1800354E0Fc0 => (int*)Unsafe.AsPointer(ref qword_180211F94);

    private static int* Sub1800354E0Fc1 => (int*)unk_180211F98;

    /// <summary>Spawns one particle of the emitter; false when the definition limit is reached or the pool is full.</summary>
    private static bool Sub1800354E0SpawnOne(Sub1800354E0Frame* f, long pool, long e, byte* weights)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0SpawnOne(f, pool, e, weights)", (nint)f, pool, e, (nint)weights);
        byte* def = Sub1800354E0Ptr(*(uint*)(e + 36));
        Def* def2 = (Def*)def;
        // Signed 16-bit compare in the original (cmp word; jge).
        if (*(short*)(e + 52) >= def2->field_12)
        {
            return false;
        }

        int capacity = *(int*)pool;
        long p = *(long*)(pool + 16);
        int slot = 0;
        if (*(int*)pool > 0)
        {
            bool full = false;
            while (*(byte*)(p + 31) != 0)
            {
                ++slot;
                p += 56;
                if (slot >= capacity)
                {
                    full = true;
                    break;
                }
            }

            if (!full && *(int*)(pool + 8) < slot)
            {
                *(int*)(pool + 8) = slot;
            }
        }

        if (slot >= capacity || p == 0)
        {
            return false;
        }

        ++*(ushort*)(e + 52);
        short* d = (short*)Sub1800354E0Ptr(*(uint*)(e + 36));
        int life = d[3];
        int entries = *(int*)(weights - 20);
        f->Phase = *(short*)(e + 56);
        f->Life = life;
        uint r0 = Sub1800354E0Rand(f->Seed);
        int roll = (int)((r0 >> 16) & 0x7F);
        f->Depth = roll;
        f->Ratio = (f->Phase << 12) / life;
        int pick = 0;
        int sum = 0;
        for (; pick < entries; ++pick)
        {
            sum += weights[pick - 4];
            if (roll < sum)
            {
                break;
            }
        }

        *(byte*)(p + 29) = 0;
        if (pick >= entries)
        {
            pick = 255;
        }

        f->Pick = pick;
        *(byte*)(p + 28) = (byte)pick;

        uint r;
        int fade;
        if (pick == 255)
        {
            r = Sub1800354E0LerpFade(f, e, r0, out fade);
        }
        else
        {
            r = Sub1800354E0TemplateColour(f, e, p, pick, r0, out fade);
        }

        short counter = *(short*)(e + 14);
        *(byte*)(p + 27) = (byte)fade;
        f->Seed = Sub1800354E0Rand(r);
        *(byte*)(p + 26) = 0;
        *(long*)(p + 48) = e;
        dword_1803231DC = (int)f->Seed;
        *(byte*)(p + 31) = (byte)(counter % 255 + 1);
        int spread = d[7];
        *(byte*)(p + 30) = (byte)((int)(f->Seed >> 16) % Sub1800354E0Div(spread) - (spread >> 1) + 64);

        byte* type = Sub1800354E0Ptr(*(uint*)e);
        *(byte*)(p + 18) = (byte)(type[1] != 0 ? 0x7F : 0);
        ++*(ushort*)(e + 14);
        if (*(uint*)(e + 20) != 0)
        {
            short period = *(short*)(e + 18);
            if (period != 0)
            {
                *(short*)(e + 14) = (short)(*(short*)(e + 14) % (2 * period));
            }
        }

        Sub1800354E0Place(f, e, d);
        Sub1800354E0Finish(f, e, p, d);
        return true;
    }

    /// <summary>
    /// No template: GTE INTPL of the type record's IR1 = T.s16[1] towards FC0 = T.s16[2] by the ratio, fade =
    /// MAC1 + jitter(T.s16[3]), at least 1. Returns the random state before the shared tail.
    /// </summary>
    private static uint Sub1800354E0LerpFade(Sub1800354E0Frame* f, long e, uint r0, out int fade)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0LerpFade(f, e, r0)", (nint)f, e, r0);
        short* t = (short*)Sub1800354E0Ptr(*(uint*)e);
        dword_180211FE0 = f->Ratio;
        dword_180211FE4 = t[1];
        *Sub1800354E0Fc0 = t[2];
        GteIntpl();
        short* t2 = (short*)Sub1800354E0Ptr(*(uint*)e);
        int jitter = t2[3];
        uint r = Sub1800354E0Rand(r0);
        fade = (int)(r >> 16) % Sub1800354E0Div(jitter) - (jitter >> 1) + dword_180212024;
        if (fade < 1)
        {
            fade = 1;
        }

        return r;
    }

    /// <summary>
    /// Weighted template entry (40 bytes at *(E + 24) + 40 * pick): fade from the entry (+4 == 1 ? +8) or from the
    /// type record, offset angles bytes 20..22 * 8 (GPF sf0 by the phase), colour (GPL sf0 of the jittered
    /// bytes 17..19 * 16 by spreads 23..25 * 16) to particle +20/+22/+24, byte +19 = jitter(entry[16]).
    /// </summary>
    private static uint Sub1800354E0TemplateColour(Sub1800354E0Frame* f, long e, long p, int pick, uint r0, out int fade)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0TemplateColour(f, e, p, pick, r0)", (nint)f, e, p, pick, r0);
        byte* entry = Sub1800354E0Ptr(*(uint*)(e + 24)) + 40 * pick;
        f->Entry = entry;
        uint r;
        if (*(ushort*)(entry + 4) == 1)
        {
            fade = *(short*)(entry + 8);
            r = r0;
        }
        else
        {
            short* t = (short*)Sub1800354E0Ptr(*(uint*)e);
            dword_180211FE0 = f->Ratio;
            dword_180211FE4 = t[1];
            *Sub1800354E0Fc0 = t[2];
            GteIntpl();
            short* t2 = (short*)Sub1800354E0Ptr(*(uint*)e);
            int jitter = t2[3];
            r = Sub1800354E0Rand(r0);
            fade = (int)(r >> 16) % Sub1800354E0Div(jitter) - (jitter >> 1) + dword_180212024;
            if (fade < 1)
            {
                fade = 1;
            }
        }

        short* offset = (short*)&f->Offset;
        offset[0] = (short)((8 * (sbyte)entry[20]) & 0xFFF);
        offset[1] = (short)((8 * (sbyte)entry[21]) & 0xFFF);
        offset[2] = (short)((8 * (sbyte)entry[22]) & 0xFFF);
        dword_180211FE4 = offset[0];
        *Sub1800354E0Ir2 = offset[1];
        dword_180211FE0 = f->Phase;
        *Sub1800354E0Ir3 = offset[2];
        GteGpfSf0();

        uint r1 = Sub1800354E0Rand(r);
        int range1 = 16 * entry[23];
        uint r2 = Sub1800354E0Rand(r1);
        int range2 = 16 * entry[24];
        short* angles = (short*)&f->Angles;
        short ax = (short)((16 * (sbyte)entry[17] + (int)(r1 >> 16) % Sub1800354E0Div(range1) - (range1 >> 1)) & 0xFFF);
        angles[0] = ax;
        int range3 = 16 * entry[25];
        short ay = (short)((16 * (sbyte)entry[18] + (int)(r2 >> 16) % Sub1800354E0Div(range2) - (range2 >> 1)) & 0xFFF);
        angles[1] = ay;
        uint r3 = Sub1800354E0Rand(r2);
        short az = (short)((16 * (sbyte)entry[19] + (int)(r3 >> 16) % Sub1800354E0Div(range3) - (range3 >> 1)) & 0xFFF);
        angles[2] = az;
        dword_180211FE0 = 1;
        dword_180211FE4 = ax;
        *Sub1800354E0Ir2 = ay;
        *Sub1800354E0Ir3 = az;
        GteGplSf0();

        int range4 = entry[16];
        uint r4 = Sub1800354E0Rand(r3);
        *(byte*)(p + 19) = (byte)((int)(r4 >> 16) % Sub1800354E0Div(range4) - (range4 >> 1));
        dword_180211FE4 = (short)dword_180211FE4;
        *(short*)(p + 20) = (short)dword_180211FE4;
        *Sub1800354E0Ir2 = (short)*Sub1800354E0Ir2;
        *(short*)(p + 22) = (short)*Sub1800354E0Ir2;
        *Sub1800354E0Ir3 = (short)*Sub1800354E0Ir3;
        *(short*)(p + 24) = (short)*Sub1800354E0Ir3;
        return r4;
    }

    /// <summary>Random value in [lo, hi) of the emitter words at +loOffset / +hiOffset (lo when equal).</summary>
    private static int Sub1800354E0RandRange(Sub1800354E0Frame* f, long e, int loOffset, int hiOffset)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0RandRange(f, e, loOffset, hiOffset)", (nint)f, e, loOffset, hiOffset);
        int hi = *(short*)(e + hiOffset);
        int lo = *(short*)(e + loOffset);
        if (lo != hi)
        {
            f->Seed = Sub1800354E0Rand(f->Seed);
            dword_1803231DC = (int)f->Seed;
            lo += (int)(f->Seed >> 16) % (hi - lo);
        }

        return lo;
    }

    /// <summary>
    /// Random direction: angle x in one of four quadrant pairs cycled by dword_1803231E8, angle y = +-(seed &gt;&gt; 22),
    /// angle z = 0.
    /// </summary>
    private static void Sub1800354E0RandomDir(Sub1800354E0Frame* f)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0RandomDir(f)", (nint)f);
        short* dir = (short*)&f->Dir;
        dir[2] = 0;
        f->Seed = Sub1800354E0Rand(f->Seed);
        dword_1803231DC = (int)f->Seed;
        int cycle = (dword_1803231E8 + 1) & 7;
        dword_1803231E8 = cycle;
        int pitch = (int)(f->Seed >> 22);
        int yaw = (int)((f->Seed >> 16) & 0x3FF) + ((cycle & 6) << 9);
        if ((cycle & 1) == 0)
        {
            pitch = -pitch;
        }

        *(int*)dir = yaw | (pitch << 16);
    }

    /// <summary>Direction from the definition word: 1 = packed angles as is, 2 = yaw + 2048, else look-at.</summary>
    private static void Sub1800354E0FaceAngles(Sub1800354E0Frame* f, ushort select, short roll)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0FaceAngles(f, select, roll)", (nint)f, select, roll);
        short* dir = (short*)&f->Dir;
        short* angles = (short*)&f->Angles;
        if (select == 1)
        {
            *(int*)dir = *(int*)angles;
        }
        else if (select == 2)
        {
            *(int*)dir = *(int*)angles;
            dir[0] = (short)(angles[0] + 2048);
        }
        else
        {
            RotationToLookAngles((ushort*)dir, (int*)angles, roll);
        }
    }

    /// <summary>Placement by the definition mode *(u32*)D: offset (Offset), direction (Dir) and origin.</summary>
    private static void Sub1800354E0Place(Sub1800354E0Frame* f, long e, short* d)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0Place(f, e, d)", (nint)f, e, (nint)d);
        Def* def2 = (Def*)d;
        byte* def = (byte*)d;
        short* dir = (short*)&f->Dir;
        short* offset = (short*)&f->Offset;
        switch (def2->field_0)
        {
            case 0xFFFFFFFF:
                Sub1800354E0Script(f, e, d);
                break;

            case 0:
            {
                // Sphere shell: random direction (hemisphere by D[71]), depth in [E+60, E+62), scaled per axis by
                // the GPF/GPL blend of D[68..70] and D[72..74].
                Sub1800354E0RandomDir(f);
                int depth = Sub1800354E0RandRange(f, e, 60, 62);
                if (def[71] == 1)
                {
                    if (dir[0] > 2048)
                    {
                        dir[0] = (short)(dir[0] - 2048);
                    }
                }
                else if (def[71] == 2)
                {
                    if (dir[0] < 2048)
                    {
                        dir[0] = (short)(dir[0] + 2048);
                    }
                }

                RotateDepthVector(dir, depth, (ushort*)offset);
                int ratio = f->Ratio;
                dword_180211FE0 = 4096 - ratio;
                dword_180211FE4 = def[68];
                *Sub1800354E0Ir2 = def[69];
                *Sub1800354E0Ir3 = def[70];
                GteGpf();
                dword_180211FE0 = ratio;
                dword_180211FE4 = def[72];
                *Sub1800354E0Ir2 = def[73];
                *Sub1800354E0Ir3 = def[74];
                GteGpl();
                *Sub1800354E0Ir2 = (short)*Sub1800354E0Ir2;
                *Sub1800354E0Ir3 = (short)*Sub1800354E0Ir3;
                dword_180211FE4 = (short)dword_180211FE4;
                offset[0] = (short)((offset[0] * (byte)dword_180211FE4) >> 7);
                offset[1] = (short)((offset[1] * (byte)*Sub1800354E0Ir2) >> 7);
                offset[2] = (short)((offset[2] * (byte)*Sub1800354E0Ir3) >> 7);
                f->Origin = (short*)Sub1800354E0Ptr(*(uint*)(e + 40));
                break;
            }

            case 1:
            {
                // Ring: angle y random, depth in [E+60, E+62), height in [E+64, E+66); direction by D+76.
                dir[0] = 0;
                f->Seed = Sub1800354E0Rand(f->Seed);
                dword_1803231DC = (int)f->Seed;
                dir[1] = (short)(f->Seed >> 16);
                dir[2] = 0;
                int depth = Sub1800354E0RandRange(f, e, 60, 62);
                RotateDepthVector(dir, depth, (ushort*)offset);
                int height = Sub1800354E0RandRange(f, e, 64, 66);
                offset[1] = (short)height;
                ushort select = def2->field_76;
                if (select == 1)
                {
                    *(int*)dir = 1024;
                }
                else if (select == 2)
                {
                    *(int*)dir = 64512;
                }

                f->Origin = (short*)Sub1800354E0Ptr(*(uint*)(e + 40));
                break;
            }

            case 2:
            {
                // Cone: point (depth, height) rotated by the emitter angles E+92 and a random roll.
                int hi = *(short*)(e + 66);
                int lo = *(short*)(e + 64);
                f->Seed = Sub1800354E0Rand(f->Seed);
                ((int*)&f->Angles)[0] = *(int*)(e + 92);
                dword_1803231DC = (int)f->Seed;
                ((int*)&f->Angles)[1] = *(int*)(e + 96);
                short roll = (short)(f->Seed >> 16);
                f->Aux = f->Seed >> 16;
                if (lo != hi)
                {
                    f->Seed = Sub1800354E0Rand(f->Seed);
                    dword_1803231DC = (int)f->Seed;
                    lo += (int)(f->Seed >> 16) % (hi - lo);
                }

                int depth = Sub1800354E0RandRange(f, e, 60, 62);
                f->Depth = depth;
                f->Offset = 0;
                TransformPointByAngles(offset, (ushort*)offset, lo, (int*)&f->Angles, depth, roll);
                Sub1800354E0FaceAngles(f, def2->field_76, roll);
                f->Origin = (short*)Sub1800354E0Ptr(*(uint*)(e + 40));
                break;
            }

            case 3:
            {
                // Segment: point between the origin *(E+40) and the target *(E+44), relative to the origin.
                short* target = (short*)Sub1800354E0Ptr(*(uint*)(e + 44));
                short* source = (short*)Sub1800354E0Ptr(*(uint*)(e + 40));
                LookAnglesBetween(source, target, (ushort*)&f->Angles);
                int hi = *(short*)(e + 66);
                int lo = *(short*)(e + 64);
                f->Seed = Sub1800354E0Rand(f->Seed);
                dword_1803231DC = (int)f->Seed;
                f->Life = (int)(f->Seed >> 16);
                f->Aux = f->Seed >> 16;
                if (lo != hi)
                {
                    f->Seed = Sub1800354E0Rand(f->Seed);
                    dword_1803231DC = (int)f->Seed;
                    lo += (int)(f->Seed >> 16) % (hi - lo);
                }

                int depth = Sub1800354E0RandRange(f, e, 60, 62);
                f->Depth = depth;
                target = (short*)Sub1800354E0Ptr(*(uint*)(e + 44));
                source = (short*)Sub1800354E0Ptr(*(uint*)(e + 40));
                dword_180211FE0 = lo;
                dword_180211FE4 = source[0];
                *Sub1800354E0Ir2 = source[1];
                *Sub1800354E0Ir3 = source[2];
                *Sub1800354E0Fc0 = target[0];
                *Sub1800354E0Fc1 = target[1];
                dword_180211F9C = target[2];
                GteIntpl();
                short roll = (short)f->Life;
                offset[0] = (short)dword_180212024;
                offset[1] = (short)dword_180212028;
                offset[2] = (short)dword_18021202C;
                TransformPointByAngles(offset, (ushort*)offset, 0, (int*)&f->Angles, depth, roll);
                Sub1800354E0FaceAngles(f, def2->field_76, roll);
                f->Origin = (short*)Sub1800354E0Ptr(*(uint*)(e + 40));
                offset[0] = (short)(offset[0] - f->Origin[0]);
                offset[1] = (short)(offset[1] - f->Origin[1]);
                offset[2] = (short)(offset[2] - f->Origin[2]);
                break;
            }

            case 4:
                Sub1800354E0Arc(f, e, def);
                break;

            default:
                // Offset, direction and origin stay as the previous spawn left them (claim in the ledger).
                break;
        }
    }

    /// <summary>
    /// Mode -1: the definition's handler D.u32[15] fills Offset/Dir: a PSX RAM script runs through sub_18000DBD0
    /// ("ppvv": &amp;Offset, &amp;Dir, phase, life), otherwise the native function at the resolved address. Non-zero result
    /// clears D.s16[3]; Offset word 3 set = origin (0, 0, 0) in Temp, else *(E + 40).
    /// </summary>
    private static void Sub1800354E0Script(Sub1800354E0Frame* f, long e, short* d)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0Script(f, e, d)", (nint)f, e, (nint)d);
        short* offset = (short*)&f->Offset;
        offset[3] = 0;
        uint handler = *(uint*)(Sub1800354E0Ptr(*(uint*)(e + 36)) + 60);
        int result;
        if ((handler & 0xFF000000) == 0x80000000 && (handler & 0xFFFFFFF) < 0x200000)
        {
            Span<ulong> va = stackalloc ulong[4];
            ulong* vaAddress = (ulong*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(va));
            va[0] = (ulong)&f->Offset;
            va[1] = (ulong)&f->Dir;
            va[2] = (uint)f->Phase;
            va[3] = (uint)f->Life;
            fixed (byte* signature = "ppvv"u8)
            {
                result = (int)sub_18000DBD0(qword_1803678E0, (int)handler, (long)signature, vaAddress);
            }
        }
        else
        {
            var native = (delegate* unmanaged<long*, long*, int, int, long>)ResolveGlobalPsxAddress(handler);
            result = (int)native(&f->Offset, &f->Dir, f->Phase, f->Life);
        }

        if (result != 0)
        {
            d[3] = 0;
        }

        f->Seed = (uint)dword_1803231DC;
        if (offset[3] != 0)
        {
            f->Temp = 0;
            f->Origin = (short*)&f->Temp;
        }
        else
        {
            f->Origin = (short*)Sub1800354E0Ptr(*(uint*)(e + 40));
        }
    }

    /// <summary>
    /// Mode 4: arc between radii E.s16[30] and E.s16[31] at a random fraction of E.s16[32]; with D+72 not 1/2 the
    /// direction looks from the inner to the outer point (atan2 yaw, pitch over the GTE SQR horizontal length).
    /// </summary>
    private static void Sub1800354E0Arc(Sub1800354E0Frame* f, long e, byte* def)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0Arc(f, e, def)", (nint)f, e, (nint)def);
        Def* def2 = (Def*)def;
        short* dir = (short*)&f->Dir;
        short* offset = (short*)&f->Offset;
        int steps = *(ushort*)(e + 64) != 0 ? *(short*)(e + 64) : 1;
        f->Angles = *(long*)(e + 92);
        int inner = *(short*)(e + 60);
        int outer = *(short*)(e + 62);
        f->Offset = 0;
        uint r1 = Sub1800354E0Rand(f->Seed);
        int step = (int)(r1 >> 16) % steps;
        uint r2 = Sub1800354E0Rand(r1);
        uint scale = (r2 >> 16) & 0xFFF;
        int innerRadius = (int)(scale * (uint)inner) >> 12;
        int outerRadius = (int)(scale * (uint)outer) >> 12;
        f->Seed = Sub1800354E0Rand(r2);
        f->Aux = (uint)innerRadius;
        dword_1803231DC = (int)f->Seed;
        double t = (double)step / (double)steps;
        int depth = Recompiler.Runtime.X86.Cvttsd2si((1.0 - t) * (double)innerRadius + (double)outerRadius * t);
        f->Depth = depth;
        short roll = (short)(f->Seed >> 16);
        TransformPointByAngles(offset, (ushort*)offset, step, (int*)&f->Angles, depth, roll);
        ushort select = def2->union_72_2;
        if (select == 1)
        {
            *(int*)dir = *(int*)&f->Angles;
        }
        else if (select == 2)
        {
            short* angles = (short*)&f->Angles;
            *(int*)dir = *(int*)angles;
            dir[0] = (short)(angles[0] + 2048);
        }
        else
        {
            short* from = (short*)&f->Temp;
            f->Temp = 0;
            TransformPointByAngles(from, (ushort*)from, 0, (int*)&f->Angles, innerRadius, roll);
            f->Dir = 0;
            TransformPointByAngles(dir, (ushort*)dir, *(short*)(e + 64), (int*)&f->Angles, outerRadius, roll);
            int fromY = from[1];
            f->Angles = f->Dir;
            int dx = dir[0] - from[0];
            int dz = dir[2] - from[2];
            dword_180211FE4 = dx;
            *Sub1800354E0Ir2 = dz;
            int toY = dir[1];
            GteSqr();
            dir[1] = (short)Recompiler.Runtime.X86.Cvttsd2si(-Math.Atan2(dx, dz) / 6.283185307179586 * -4096.0);
            dir[2] = 0;
            double horizontal = Recompiler.Runtime.X86.Cvttsd2si(Math.Sqrt(unchecked(dword_180212024 + dword_180212028)));
            double pitch = Math.Atan2(horizontal, toY - fromY);
            dir[0] = (short)(-1024 - Recompiler.Runtime.X86.Cvttsd2si(-pitch / 6.283185307179586 * 4096.0));
        }

        f->Origin = (short*)Sub1800354E0Ptr(*(uint*)(e + 40));
    }

    /// <summary>
    /// Shared tail: position = (Offset + Origin) &lt;&lt; 12, direction jitter by D.s16[25] (spread D.s16[29]), template
    /// rotation add, motion by the type byte *T, ribbon trail for template kinds 4/5/8.
    /// </summary>
    private static void Sub1800354E0Finish(Sub1800354E0Frame* f, long e, long p, short* d)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0Finish(f, e, p, d)", (nint)f, e, p, (nint)d);
        short* dir = (short*)&f->Dir;
        short* offset = (short*)&f->Offset;
        short* origin = f->Origin;
        *(int*)p = (offset[0] + origin[0]) << 12;
        *(int*)(p + 4) = (offset[1] + origin[1]) << 12;
        *(int*)(p + 8) = (offset[2] + origin[2]) << 12;
        switch (d[25])
        {
            case 0:
                Sub1800354E0Jitter(f, d);
                break;

            case 1:
            {
                uint r1 = Sub1800354E0Rand(f->Seed);
                int spread = Sub1800354E0Div(d[29]);
                f->Seed = Sub1800354E0Rand(r1);
                short x = (short)((int)(r1 >> 16) % spread - (d[29] >> 1) + dir[0]);
                dir[0] = x;
                dir[1] = (short)(dir[1] + (int)(f->Seed >> 16) % spread - (d[29] >> 1));
                dir[0] = (short)((x - 2048) & 0xFFF);
                dword_1803231DC = (int)f->Seed;
                break;
            }

            case 2:
                Sub1800354E0RandomDir(f);
                break;

            case 3:
                f->Dir = *(long*)(e + 92);
                Sub1800354E0Jitter(f, d);
                break;
        }

        byte* entry = f->Entry;
        if (entry != null && entry[7] != 0)
        {
            *(short*)(p + 20) = (short)(*(short*)(p + 20) + dir[0]);
            *(short*)(p + 22) = (short)(*(short*)(p + 22) + dir[1]);
        }

        uint typeAddress = *(uint*)e;
        short* t = (short*)Sub1800354E0Ptr(typeAddress);
        Site_Sub1800354E0Ptr* site_Sub1800354E0Ptr = (Site_Sub1800354E0Ptr*)t;
        switch (site_Sub1800354E0Ptr->field_0)
        {
            case 0:
                Sub1800354E0MotionDepth(f, e, p, t);
                break;
            case 1:
                Sub1800354E0MotionDirected(f, e, p, t);
                break;
            case 2:
                Sub1800354E0MotionYaw(f, e, p, t);
                break;
            case 3:
                Sub1800354E0MotionAngles(f, e, p, t);
                break;
            case 4:
                Sub1800354E0MotionLerp(f, e, p, t);
                break;
            case 5:
                Sub1800354E0MotionDepth(f, e, p, (short*)PsxAddrToPtr((long)unk_180576A10, typeAddress));
                break;
        }

        if (entry == null || (!Sub1800354E0IsTrail(entry[0]) && !Sub1800354E0IsTrail(entry[1])))
        {
            return;
        }

        short* head = (short*)&f->Temp;
        head[0] = (short)(*(int*)p >> 12);
        head[1] = (short)(*(int*)(p + 4) >> 12);
        head[2] = (short)(*(int*)(p + 8) >> 12);
        byte* record = SfxIndexedRecordPtr(p, e);
        ushort* trail = (ushort*)Sub1800354E0Ptr(*(uint*)(entry + 28));
        DrawRibbonTrail(trail, (long)head, null, -1, 0, 0, 0, record, 0);
        f->Seed = (uint)dword_1803231DC;
    }

    /// <summary>Template kinds 4, 5 and 8 ((kind - 4) &amp; 0xFA == 0, kind 9 excluded) draw a ribbon trail.</summary>
    private static bool Sub1800354E0IsTrail(byte kind)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0IsTrail(kind)", kind);
        return ((kind - 4) & 0xFA) == 0 && kind != 9;
    }

    /// <summary>Adds the spread D.s16[29] jitter to direction x and y.</summary>
    private static void Sub1800354E0Jitter(Sub1800354E0Frame* f, short* d)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0Jitter(f, d)", (nint)f, (nint)d);
        short* dir = (short*)&f->Dir;
        uint r1 = Sub1800354E0Rand(f->Seed);
        int spread = Sub1800354E0Div(d[29]);
        f->Seed = Sub1800354E0Rand(r1);
        dir[0] = (short)((int)(r1 >> 16) % spread - (d[29] >> 1) + dir[0]);
        dir[1] = (short)(dir[1] + (int)(f->Seed >> 16) % spread - (d[29] >> 1));
        dword_1803231DC = (int)f->Seed;
    }

    /// <summary>Types 0 and 5: velocity +12 = depth E.s16[4] +- T.s16[6] along Dir; speed +32 = T.s16[7] +- T.s16[8], at least 0.</summary>
    private static void Sub1800354E0MotionDepth(Sub1800354E0Frame* f, long e, long p, short* t)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0MotionDepth(f, e, p, t)", (nint)f, e, p, (nint)t);
        short* dir = (short*)&f->Dir;
        int spread = t[6];
        uint r1 = Sub1800354E0Rand(f->Seed);
        RotateDepthVector(dir, *(short*)(e + 8) + (int)(r1 >> 16) % Sub1800354E0Div(spread) - (spread >> 1), (ushort*)(p + 12));
        int range = t[8];
        f->Seed = Sub1800354E0Rand(r1);
        dword_1803231DC = (int)f->Seed;
        int speed = t[7] + (int)(f->Seed >> 16) % Sub1800354E0Div(range) - (range >> 1);
        if (speed < 0)
        {
            speed = 0;
        }

        *(short*)(p + 32) = (short)speed;
    }

    /// <summary>Type 1: velocity by depth E.s16[5] +- T.s16[18]; +42, +36 = Dir, +32, +34, +40 jittered from T.</summary>
    private static void Sub1800354E0MotionDirected(Sub1800354E0Frame* f, long e, long p, short* t)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0MotionDirected(f, e, p, t)", (nint)f, e, p, (nint)t);
        Site_Sub1800354E0Ptr* site_Sub1800354E0Ptr = (Site_Sub1800354E0Ptr*)t;
        short* dir = (short*)&f->Dir;
        uint r1 = Sub1800354E0Rand(f->Seed);
        int spread = t[18];
        RotateDepthVector(dir, *(short*)(e + 10) + (int)(r1 >> 16) % Sub1800354E0Div(spread) - (spread >> 1), (ushort*)(p + 12));
        uint r2 = Sub1800354E0Rand(r1);
        int value = t[8] + (int)(r2 >> 16) % Sub1800354E0Div(t[9]) - (t[9] >> 1);
        *(short*)(p + 42) = value < 0 ? (short)0 : (short)value;
        *(int*)(p + 36) = *(int*)dir;
        uint r3 = Sub1800354E0Rand(r2);
        uint r4 = Sub1800354E0Rand(r3);
        *(short*)(p + 32) = (short)(site_Sub1800354E0Ptr->field_8 + (int)(r3 >> 16) % Sub1800354E0Div(t[6]) - (t[6] >> 1));
        int range7 = t[7];
        f->Seed = Sub1800354E0Rand(r4);
        dword_1803231DC = (int)f->Seed;
        *(short*)(p + 34) = (short)(site_Sub1800354E0Ptr->field_10 + (int)(r4 >> 16) % Sub1800354E0Div(range7) - (range7 >> 1));
        *(short*)(p + 40) = (short)(*(ushort*)(e + 8) + (int)(f->Seed >> 16) % Sub1800354E0Div(t[11]) - (t[11] >> 1));
    }

    /// <summary>Loads IR = T.s16[8..10], FC = T.s16[10..12], IR0 = ratio and runs GTE INTPL (types 2 and 4).</summary>
    private static void Sub1800354E0TypeIntpl(Sub1800354E0Frame* f, long p, short* t)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0TypeIntpl(f, p, t)", (nint)f, p, (nint)t);
        *(int*)(p + 40) = *(int*)&f->Dir;
        dword_180211FE0 = f->Ratio;
        dword_180211FE4 = t[8];
        *Sub1800354E0Ir2 = t[9];
        *Sub1800354E0Ir3 = t[10];
        *Sub1800354E0Fc0 = t[10];
        *Sub1800354E0Fc1 = t[11];
        dword_180211F9C = t[12];
        GteIntpl();
    }

    /// <summary>Type 2: INTPL of T, +14/+38/+34 jittered, then yaw/distance of Offset (ComputeLocalYawDistance); position = origin.</summary>
    private static void Sub1800354E0MotionYaw(Sub1800354E0Frame* f, long e, long p, short* t)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0MotionYaw(f, e, p, t)", (nint)f, e, p, (nint)t);
        Sub1800354E0TypeIntpl(f, p, t);
        int range7 = t[7];
        short mac1 = (short)dword_180212024;
        f->Temp = 0;
        uint r1 = Sub1800354E0Rand(f->Seed);
        uint r2 = Sub1800354E0Rand(r1);
        *(short*)(p + 14) = (short)((short)dword_180212028 + (int)(r1 >> 16) % Sub1800354E0Div(range7) - (range7 >> 1));
        int range12 = t[12];
        f->Seed = Sub1800354E0Rand(r2);
        dword_1803231DC = (int)f->Seed;
        *(short*)(p + 38) = (short)(mac1 + (int)(r2 >> 16) % Sub1800354E0Div(range12) - (range12 >> 1));
        int range6 = t[6];
        *(short*)(p + 34) = (short)(*(ushort*)(e + 8) + (int)(f->Seed >> 16) % Sub1800354E0Div(range6) - (range6 >> 1));
        ComputeLocalYawDistance((ushort*)&f->Offset, (ushort*)&f->Temp, (ushort*)&f->Dir, (uint*)&f->Pick, (uint*)&f->Depth, &f->Aux);
        *(short*)(p + 12) = (short)f->Aux;
        *(short*)(p + 32) = (short)f->Pick;
        *(short*)(p + 36) = (short)f->Depth;
        short* origin = f->Origin;
        *(int*)p = origin[0] << 12;
        *(int*)(p + 4) = origin[1] << 12;
        *(int*)(p + 8) = origin[2] << 12;
        *(short*)(p + 16) = 0;
    }

    /// <summary>Type 3: +12 = Dir, +32/+34/+16/+38/+40 jittered from T and the emitter, +36 = 0.</summary>
    private static void Sub1800354E0MotionAngles(Sub1800354E0Frame* f, long e, long p, short* t)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0MotionAngles(f, e, p, t)", (nint)f, e, p, (nint)t);
        Site_Sub1800354E0Ptr* site_Sub1800354E0Ptr = (Site_Sub1800354E0Ptr*)t;
        short* dir = (short*)&f->Dir;
        *(int*)(p + 12) = *(int*)dir;
        int range6 = t[6];
        uint r1 = Sub1800354E0Rand(f->Seed);
        uint r2 = Sub1800354E0Rand(r1);
        *(short*)(p + 32) = (short)(site_Sub1800354E0Ptr->field_8 + (int)(r1 >> 16) % Sub1800354E0Div(range6) - (range6 >> 1));
        uint r3 = Sub1800354E0Rand(r2);
        *(short*)(p + 34) = (short)(site_Sub1800354E0Ptr->field_10 + (int)(r2 >> 16) % Sub1800354E0Div(t[7]) - (t[7] >> 1));
        uint r4 = Sub1800354E0Rand(r3);
        *(short*)(p + 16) = (short)(*(ushort*)(e + 8) + (int)(r3 >> 16) % Sub1800354E0Div(t[11]) - (t[11] >> 1));
        int range8 = t[8];
        f->Seed = Sub1800354E0Rand(r4);
        dword_1803231DC = (int)f->Seed;
        *(short*)(p + 38) = (short)(dir[0] + (int)(r4 >> 16) % Sub1800354E0Div(range8) - (range8 >> 1));
        short y = (short)(dir[1] + (int)(f->Seed >> 16) % Sub1800354E0Div(t[9]) - (t[9] >> 1));
        *(short*)(p + 36) = 0;
        *(short*)(p + 40) = y;
    }

    /// <summary>Type 4: INTPL of T into Temp, +14/+32/+34 jittered, +36 = 0, +12 = random, +16 = 0.</summary>
    private static void Sub1800354E0MotionLerp(Sub1800354E0Frame* f, long e, long p, short* t)
    {
        Recompiler.Runtime.Tracer.Enter("Sub1800354E0MotionLerp(f, e, p, t)", (nint)f, e, p, (nint)t);
        Sub1800354E0TypeIntpl(f, p, t);
        int range13 = t[13];
        short mac1 = (short)dword_180212024;
        short* temp = (short*)&f->Temp;
        temp[2] = (short)dword_18021202C;
        temp[1] = (short)dword_180212028;
        temp[0] = mac1;
        uint r1 = Sub1800354E0Rand(f->Seed);
        *(short*)(p + 14) = (short)((short)dword_180212028 + (int)(r1 >> 16) % Sub1800354E0Div(range13) - (range13 >> 1));
        uint r2 = Sub1800354E0Rand(r1);
        int range12 = t[12];
        *(short*)(p + 32) = (short)(mac1 + (int)(r2 >> 16) % Sub1800354E0Div(range12) - (range12 >> 1));
        int range6 = t[6];
        uint r3 = Sub1800354E0Rand(r2);
        f->Seed = Sub1800354E0Rand(r3);
        dword_1803231DC = (int)f->Seed;
        short value = (short)(*(ushort*)(e + 8) + (int)(r3 >> 16) % Sub1800354E0Div(range6) - (range6 >> 1));
        *(int*)(p + 36) = 0;
        *(short*)(p + 34) = value;
        *(short*)(p + 12) = (short)(f->Seed >> 16);
        *(short*)(p + 16) = 0;
    }
}
