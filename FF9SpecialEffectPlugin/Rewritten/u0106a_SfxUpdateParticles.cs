using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Locals of SfxUpdateParticles that live across particles (the original keeps them in one stack frame and
    /// several are read before the current particle assigns them).
    /// </summary>
    private struct Part106aFrame
    {
        /// <summary>v177 [rbp-48h]: particle position scratch (x, y, z int16 + one unused word).</summary>
        public long Pos;

        /// <summary>v176 [rbp-50h]: direction / angle vector, cleared at the start of every particle.</summary>
        public long Dir;

        /// <summary>v178 [rbp-40h]: distance of the attractor step, later the draw mode passed to the emitters.</summary>
        public int Mode;

        /// <summary>v179 [rbp-3Ch]: fade value passed to the emitters (kept when no curve applies).</summary>
        public int Fade;

        /// <summary>v180 [rbp-38h]: attractor phase, later the size passed to the emitters.</summary>
        public int Size;

        /// <summary>LODWORD(v161) [rsp+60h]: attractor amplitude.</summary>
        public int Amp;

        /// <summary>HIDWORD(v161) [rsp+64h]: attractor range.</summary>
        public int Range;

        /// <summary>v167 [rbp-80h]: attractor curve kind.</summary>
        public int Curve;

        /// <summary>v168/v173/v169: reference position (particle position &gt;&gt; 12) of an emitter kind &gt;= 4.</summary>
        public int RefX;

        public int RefY;

        public int RefZ;

        /// <summary>v172 [rbp-6Ch]: emitter kind byte (descriptor - 44) of the last non-0x80 particle.</summary>
        public int EmitterKind;
    }

    /// <summary>
    /// Updates the particle array of the effect a1 (particles of 56 bytes at *(a1 + 16), indices 0..*(a1 + 8)
    /// inclusive): motion per header type, attractor, sprite emission per frame table entry and sub-emitters.
    /// Stores the last live index in *(a1 + 8) (and dword_1803231D4, high-water mark dword_180323164), counts live
    /// particles into *(a1 + 4) and returns that count. off_180066C68[10] is restored on exit.
    /// </summary>
    public static long SfxUpdateParticles(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxUpdateParticles(a1)", a1);
        long p = *(long*)(a1 + 16);
        int savedTick = ((Table*)off_180066C68)->field_40;
        dword_180622400 = savedTick;
        Part106aFrame frame = default;
        Part106aFrame* f = &frame;
        int last = 0;
        int index = 0;
        *(int*)(a1 + 4) = 0;
        if (*(int*)(a1 + 8) >= 0)
        {
            while (true)
            {
                f->Dir = 0;
                if (*(byte*)(p + 31) != 0)
                {
                    int v6 = *(byte*)(p + 26);
                    int v7 = *(byte*)(p + 27);
                    long v8 = *(long*)(p + 48);
                    if (v6 >= v7)
                    {
                        Part106aKill(p, v8);
                    }
                    else
                    {
                        ++*(int*)(a1 + 4);
                        last = index;
                        Part106aUpdateOne(f, a1, p, v8, v6, v7);
                    }
                }

                ++index;
                p += 56;
                if (index > *(int*)(a1 + 8))
                {
                    savedTick = dword_180622400;
                    break;
                }
            }
        }

        *(int*)(a1 + 8) = last;
        dword_1803231D4 = last;
        ((Table*)off_180066C68)->field_40 = savedTick;
        if (dword_180323164 < last)
        {
            dword_180323164 = last;
        }

        return *(uint*)(a1 + 4);
    }

    /// <summary>Marks the particle dead and decrements the live counter of its group (+54 when bit 7 of +18).</summary>
    private static void Part106aKill(long p, long v8)
    {
        Recompiler.Runtime.Tracer.Enter("Part106aKill(p, v8)", p, v8);
        bool high = (*(byte*)(p + 18) & 0x80) != 0;
        *(byte*)(p + 31) = 0;
        if (high)
        {
            --*(ushort*)(v8 + 54);
        }
        else
        {
            --*(ushort*)(v8 + 52);
        }
    }

    /// <summary>One live particle: header selection, motion, attractor, emission, sub-emitter, age.</summary>
    private static void Part106aUpdateOne(Part106aFrame* f, long a1, long p, long v8, int v6, int v7)
    {
        Recompiler.Runtime.Tracer.Enter("Part106aUpdateOne(f, a1, p, v8, v6, v7)", (nint)f, a1, p, v8, v6, v7);
        int v10 = (v6 << 12) / v7;
        byte* header;
        int v13;
        long v14;
        int v17;
        if ((*(byte*)(p + 18) & 0x80) != 0)
        {
            uint address = *(uint*)(v8 + 4);
            header = ResolveGlobalPsxAddress(address);
            v13 = *(short*)(v8 + 86);
            v14 = v8 + 80;
            byte* track = ResolveGlobalPsxAddress(address);
            v17 = *(short*)(track - 20);
            if (v17 != 0)
            {
                f->Curve = *(short*)(track - 18);
                f->Range = *(short*)(v8 + 88);
                f->Amp = v17 >= 7 ? *(short*)(track - 14) : *(short*)(v8 + 90);
            }
        }
        else
        {
            header = ResolveGlobalPsxAddress(*(uint*)v8);
            uint trackAddress = *(uint*)(v8 + 36);
            v13 = *(short*)(v8 + 74);
            v14 = v8 + 68;
            v17 = *(short*)(ResolveGlobalPsxAddress(trackAddress) + 32);
            if (v17 != 0)
            {
                f->Curve = *(short*)(ResolveGlobalPsxAddress(trackAddress) + 34);
                f->Range = *(short*)(v8 + 76);
                f->Amp = v17 >= 7 ? *(short*)(ResolveGlobalPsxAddress(trackAddress) + 38) : *(short*)(v8 + 78);
            }

            uint emitterAddress = *(uint*)(v8 + 4);
            if (ResolveGlobalPsxAddress(emitterAddress) != null)
            {
                f->EmitterKind = *(ResolveGlobalPsxAddress(emitterAddress) - 44);
                if (f->EmitterKind >= 4)
                {
                    f->RefX = *(int*)p >> 12;
                    f->RefY = *(int*)(p + 4) >> 12;
                    f->RefZ = *(int*)(p + 8) >> 12;
                }
            }
        }

        if (*header == 0xFF)
        {
            Part106aScriptMotion(f, p, v8, header, v6, v7);
        }
        else
        {
            Part106aMotion(f, p, v8, header, v13, v14, v10);
        }

        int v110 = 0xFFFF;
        if (v17 != 0)
        {
            v110 = Part106aAttractor(f, p, v8, v17);
        }

        ((Table*)off_180066C68)->field_40 = *(short*)(v8 + 34);
        byte slot = *(byte*)(p + 28);
        if (slot != 0xFF)
        {
            Part106aEmit(f, p, v8, slot, v6, v7, v10, v110);
        }

        Part106aSubEmitter(f, a1, p, v8);
        *(byte*)(p + 26) = (byte)(v6 + 1);
    }

    /// <summary>Header type 0xFF: script motion handler at *(header + 8); a non-zero result kills the particle.</summary>
    private static void Part106aScriptMotion(Part106aFrame* f, long p, long v8, byte* header, int v6, int v7)
    {
        Recompiler.Runtime.Tracer.Enter("Part106aScriptMotion(f, p, v8, header, v6, v7)", (nint)f, p, v8, (nint)header, v6, v7);
        uint handler = *(uint*)(header + 8);
        int result;
        if ((handler & 0xFF000000) == 0x80000000 && (handler & 0xFFFFFFF) < 0x200000)
        {
            byte* record = SfxIndexedRecordPtr(p, v8);
            Span<ulong> va = stackalloc ulong[6];
            ulong* vaAddress = (ulong*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(va));
            va[0] = (ulong)p;
            va[1] = (ulong)v8;
            va[2] = (ulong)&f->Pos;
            va[3] = (ulong)record;
            va[4] = (uint)v6;
            va[5] = (uint)v7;
            fixed (byte* signature = "ppppvv"u8)
            {
                result = (int)sub_18000DBD0(qword_1803678E0, (int)handler, (long)signature, vaAddress);
            }
        }
        else
        {
            var native = (delegate* unmanaged<long, long, long*, byte*, int, int, int>)PsxAddrToPtr((long)unk_180576A10, handler);
            byte* record = SfxIndexedRecordPtr(p, v8);
            result = native(p, v8, &f->Pos, record, v6, v7);
        }

        if (result != 0)
        {
            Part106aKill(p, v8);
        }
    }

    /// <summary>Header types 0..5: integrator per type, then the type's own steering.</summary>
    private static void Part106aMotion(Part106aFrame* f, long p, long v8, byte* header, int v13, long v14, int v10)
    {
        Recompiler.Runtime.Tracer.Enter("Part106aMotion(f, p, v8, header, v13, v14, v10)", (nint)f, p, v8, (nint)header, v13, v14, v10);
        short* pos = (short*)&f->Pos;
        short* dir = (short*)&f->Dir;
        switch (*header)
        {
            case 0:
                ParticleStepScaledMotion32((ulong)p, v13, (ulong)v14, (ulong)pos);
                if (*(ushort*)(header + 24) != 0)
                {
                    Part106aJitter(header, p);
                }

                Part106aFloorBounce(p, v8, header, pos[1]);
                break;
            case 1:
                ParticleStepScaledMotion42((ulong)p, v13, (ulong)v14, (ulong)pos);
                *(ushort*)(p + 36) += *(ushort*)(p + 32);
                dir[0] = *(short*)(p + 36);
                *(ushort*)(p + 38) += *(ushort*)(p + 34);
                dir[1] = *(short*)(p + 38);
                v13 = Part106aScale(header, *(short*)(header + 20), *(short*)(p + 40), v10, v13);
                SfxRotTransPointZ(pos, dir, v13, (ushort*)pos);
                break;
            case 2:
                SfxStepMotion((ulong)p, v13, (ulong)v14, (ulong)pos);
                *(ushort*)(p + 12) += *(ushort*)(p + 14);
                *(ushort*)(p + 32) += *(ushort*)(p + 34);
                *(ushort*)(p + 36) += *(ushort*)(p + 38);
                TransformPointByAngles(pos, (ushort*)pos, *(short*)(p + 32) >> 2, (int*)(p + 40), *(short*)(p + 36) >> 2, *(short*)(p + 12));
                break;
            case 3:
                SfxStepMotionVel36((ulong)p, v13, (ulong)v14, (ulong)pos);
                *(ushort*)(p + 12) += *(ushort*)(p + 32);
                *(ushort*)(p + 14) += *(ushort*)(p + 34);
                dir[0] = (short)(*(ushort*)(p + 38) - Recompiler.Runtime.X86.Cvttsd2si(Math.Sin(Part106aAngle(*(short*)(p + 12))) * -4096.8));
                dir[1] = (short)(*(ushort*)(p + 40) - Recompiler.Runtime.X86.Cvttsd2si(Math.Sin(Part106aAngle(*(short*)(p + 14))) * -4096.8));
                v13 = Part106aScale(header, *(short*)(header + 20), *(short*)(p + 16), v10, v13);
                SfxRotTransPointZ(pos, dir, v13, (ushort*)pos);
                break;
            case 4:
                SfxStepMotion((ulong)p, v13, (ulong)v14, (ulong)pos);
                *(int*)(p + 36) += *(short*)(p + 34);
                *(ushort*)(p + 12) += *(ushort*)(p + 14);
                v13 = Part106aScale(header, *(short*)(header + 14), *(short*)(p + 32), v10, v13);
                TransformPointByAngles(pos, (ushort*)pos, *(int*)(p + 36) >> 6, (int*)(p + 40), v13, *(short*)(p + 12));
                break;
            case 5:
            {
                ParticleStepScaledMotion32((ulong)p, v13, (ulong)v14, (ulong)pos);
                if (*(ushort*)(header + 24) != 0)
                {
                    Part106aJitter(header, p);
                }

                Part106aIntpl(v10, *(short*)(header + 18), *(short*)(header + 20));
                int depth = dword_180212024;
                short* target = (short*)PsxAddrToPtr((long)unk_180576A10, *(uint*)(v8 + 44));
                LookAnglesBetween(pos, target, (ushort*)dir);
                dir[0] += *(short*)(header + 32);
                dir[1] += *(short*)(header + 34);
                SfxRotTransPointZ((short*)(p + 12), dir, depth, (ushort*)(p + 12));
                break;
            }
        }
    }

    /// <summary>(double)value / 4096.8 * 2.0 * pi, evaluated left to right as the original.</summary>
    private static double Part106aAngle(int value)
    {
        Recompiler.Runtime.Tracer.Enter("Part106aAngle(value)", value);
        return (double)value / 4096.8 * 2.0 * Math.PI;
    }

    /// <summary>IR0 = a1, IR1 = a2, RFC = a3, then GTE INTPL (MAC1 = dword_180212024).</summary>
    private static void Part106aIntpl(int ir0, int ir1, int rfc)
    {
        Recompiler.Runtime.Tracer.Enter("Part106aIntpl(ir0, ir1, rfc)", ir0, ir1, rfc);
        dword_180211FE0 = ir0;
        dword_180211FE4 = ir1;
        *(int*)Unsafe.AsPointer(ref qword_180211F94) = rfc;
        GteIntpl();
    }

    /// <summary>
    /// Scale curve of header types 1, 3, 4: 0 = value * sin(INTPL(header[14], header[15], t)), 1 = value * t,
    /// 2 = value - value * t (4.12); any other kind keeps <paramref name="current"/>.
    /// </summary>
    private static int Part106aScale(byte* header, short kind, int value, int v10, int current)
    {
        Recompiler.Runtime.Tracer.Enter("Part106aScale(header, kind, value, v10, current)", (nint)header, kind, value, v10, current);
        switch (kind)
        {
            case 0:
                Part106aIntpl(v10, *(short*)(header + 28), *(short*)(header + 30));
                double s = Math.Sin(Part106aAngle(dword_180212024));
                return (Recompiler.Runtime.X86.Cvttsd2si(s * 4096.8) * value) >> 12;
            case 1:
                return (value * v10) >> 12;
            case 2:
                return value - ((value * v10) >> 12);
            default:
                return current;
        }
    }

    /// <summary>
    /// Random spread of header types 0 and 5: three LCG steps of dword_1803231DC (1103515245, 12345); words
    /// p+12/14/16 += (rand &gt;&gt; 16) % range - range / 2 with the ranges header[13..15] (0 counts as 1).
    /// </summary>
    private static void Part106aJitter(byte* header, long p)
    {
        Recompiler.Runtime.Tracer.Enter("Part106aJitter(header, p)", (nint)header, p);
        uint r1 = 1103515245u * (uint)dword_1803231DC + 12345u;
        int range1 = *(short*)(header + 26);
        int r2Seed = (int)(1103515245u * r1 + 12345u);
        int rem1 = (int)(r1 >> 16) % (range1 != 0 ? range1 : 1);
        *(ushort*)(p + 12) += (ushort)(rem1 - (range1 >> 1));
        int range2 = *(short*)(header + 28);
        int rem2 = (int)((uint)r2Seed >> 16) % (range2 != 0 ? range2 : 1);
        uint r3 = 1103515245u * (uint)r2Seed + 12345u;
        dword_1803231DC = (int)r3;
        *(ushort*)(p + 14) += (ushort)(rem2 - (range2 >> 1));
        int range3 = *(short*)(header + 30);
        int rem3 = (int)(r3 >> 16) % (range3 != 0 ? range3 : 1);
        *(ushort*)(p + 16) += (ushort)(rem3 - (range3 >> 1));
    }

    /// <summary>
    /// Header type 0 floor handling (kind header[9]): 1 reflects a downward vy (p+14) scaled by header[11] / 256 once y
    /// passes floor (v8+98) - header[10]; 2+ damps it, adds a gravity kick and stops it at the floor.
    /// </summary>
    private static void Part106aFloorBounce(long p, long v8, byte* header, short y)
    {
        Recompiler.Runtime.Tracer.Enter("Part106aFloorBounce(p, v8, header, y)", p, v8, (nint)header, y);
        ushort kind = *(ushort*)(header + 18);
        if (kind == 0)
        {
            return;
        }

        if (kind == 1)
        {
            if (y > *(short*)(v8 + 98) - *(short*)(header + 20))
            {
                short vy = *(short*)(p + 14);
                if (vy > 0)
                {
                    short negated = (short)-vy;
                    *(short*)(p + 14) = negated;
                    *(short*)(p + 14) = (short)((negated * *(short*)(header + 22)) >> 8);
                }
            }

            return;
        }

        int margin = *(short*)(header + 20);
        int floor = *(short*)(v8 + 98);
        if (y <= floor - margin)
        {
            return;
        }

        short speed = *(short*)(p + 14);
        if (speed > 0)
        {
            int divisor = (short)margin != 0 ? margin : 1;
            *(short*)(p + 14) = (short)(speed * (floor - y) / divisor);
        }

        if (y > *(short*)(v8 + 98) - (*(short*)(header + 20) >> 1))
        {
            *(ushort*)(p + 14) += (ushort)(-64 - (short)((*(short*)(v8 + 74) * *(sbyte*)(p + 30)) >> 6));
        }

        if (y > *(short*)(v8 + 98))
        {
            *(short*)(p + 14) = 0;
        }
    }

    /// <summary>
    /// Attractor of track kind v17 (1..9) towards the anchor points at PSX addresses v8+100 / v8+104 (static
    /// fallbacks dword_1806223F8 / dword_180622408 are registered when unmapped). Moves the particle along Dir by
    /// the curved amplitude (kinds &lt; 7, GTE GPF, &lt;&lt; 6) or returns the amplitude as fade offset (kinds &gt;= 7);
    /// out of range: 0xFFFF (kinds &lt; 7) or 0.
    /// </summary>
    private static int Part106aAttractor(Part106aFrame* f, long p, long v8, int v17)
    {
        Recompiler.Runtime.Tracer.Enter("Part106aAttractor(f, p, v8, v17)", (nint)f, p, v8, v17);
        short* a = (short*)ResolveGlobalPsxAddress(*(uint*)(v8 + 100));
        short* b = (short*)ResolveGlobalPsxAddress(*(uint*)(v8 + 104));
        if (a == null)
        {
            dword_1806223F8 = 0;
            word_1806223FC = 0;
            a = (short*)Unsafe.AsPointer(ref dword_1806223F8);
            *(uint*)(v8 + 100) = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)a);
        }

        if (b == null)
        {
            dword_180622408 = 0;
            word_18062240C = 0;
            b = (short*)Unsafe.AsPointer(ref dword_180622408);
            *(uint*)(v8 + 104) = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)b);
        }

        short* pos = (short*)&f->Pos;
        short* dir = (short*)&f->Dir;
        int failed = v17 >= 7 ? 0 : 0xFFFF;
        int distance;
        switch (v17)
        {
            case 1:
            case 2:
            {
                distance = Part106aDistance(a, pos[0], pos[1], pos[2]);
                f->Mode = distance;
                if (distance > f->Range)
                {
                    return failed;
                }

                int dx = v17 == 1 ? pos[0] - a[0] : a[0] - pos[0];
                int dy = v17 == 1 ? pos[1] - a[1] : a[1] - pos[1];
                int dz = v17 == 1 ? pos[2] - a[2] : a[2] - pos[2];
                double length = Recompiler.Runtime.X86.Cvttsd2si(Math.Sqrt(dz * dz + dy * dy + dx * dx));
                dir[0] = (short)Recompiler.Runtime.X86.Cvttsd2si(dx / length * 4096.8);
                dir[1] = (short)Recompiler.Runtime.X86.Cvttsd2si(dy / length * 4096.8);
                dir[2] = (short)Recompiler.Runtime.X86.Cvttsd2si(dz / length * 4096.8);
                break;
            }

            case 3:
            case 4:
                f->Dir = f->Pos;
                dir[1] = a[1];
                distance = Part106aDistance(a, dir[0], dir[1], dir[2]);
                *(int*)dir = 0;
                dir[1] = unchecked((short)0xF000);
                f->Mode = distance;
                break;
            case 5:
                ComputeLocalYawDistance((ushort*)pos, (ushort*)a, (ushort*)b, (uint*)&f->Fade, (uint*)&f->Mode, (uint*)&f->Size);
                if (f->Mode > f->Range)
                {
                    return failed;
                }

                RotateDepthVector(b, 4096, (ushort*)dir);
                distance = f->Mode;
                break;
            case 6:
                ComputeLocalYawDistance((ushort*)pos, (ushort*)a, (ushort*)b, (uint*)&f->Fade, (uint*)&f->Mode, (uint*)&f->Size);
                if (f->Mode > f->Range)
                {
                    return failed;
                }

                f->Dir = 0;
                TransformPointByAngles(dir, (ushort*)dir, 0, (int*)b, 4096, (short)f->Size);
                distance = f->Mode;
                break;
            case 7:
                distance = Part106aDistance(a, pos[0], pos[1], pos[2]);
                f->Mode = distance;
                break;
            case 8:
                f->Dir = f->Pos;
                dir[1] = a[1];
                distance = Part106aDistance(a, dir[0], dir[1], dir[2]);
                f->Mode = distance;
                break;
            case 9:
                ComputeLocalYawDistance((ushort*)pos, (ushort*)a, (ushort*)b, (uint*)&f->Fade, (uint*)&f->Mode, (uint*)&f->Size);
                distance = f->Mode;
                break;
            default:
                distance = f->Mode;
                break;
        }

        int range = f->Range;
        if (distance > range)
        {
            return failed;
        }

        int d0 = dir[0];
        int d1 = dir[1];
        int d2 = dir[2];
        if (range == 0)
        {
            range = 1;
        }

        f->Range = range;
        int phase = (distance << 10) / range;
        f->Size = phase;
        int amp;
        switch (f->Curve)
        {
            case 1:
                amp = (Recompiler.Runtime.X86.Cvttsd2si(Math.Cos(Part106aAngle(phase)) * 4096.8) * f->Amp) >> 12;
                break;
            case 2:
                amp = (Recompiler.Runtime.X86.Cvttsd2si(Math.Sin(Part106aAngle(phase)) * 4096.8) * f->Amp) >> 12;
                break;
            case 3:
                amp = ((4096 - Recompiler.Runtime.X86.Cvttsd2si(Math.Sin(Part106aAngle(phase)) * 4096.8)) * f->Amp) >> 12;
                break;
            case 4:
                amp = ((4096 - Recompiler.Runtime.X86.Cvttsd2si(Math.Cos(Part106aAngle(phase)) * 4096.8)) * f->Amp) >> 12;
                break;
            case 5:
                amp = (Recompiler.Runtime.X86.Cvttsd2si(Math.Sin(Part106aAngle(phase * 2)) * 4096.8) * f->Amp) >> 12;
                break;
            default:
                amp = f->Amp;
                break;
        }

        f->Amp = amp;
        if (v17 >= 7)
        {
            return amp;
        }

        int* ir23 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
        dword_180211FE0 = amp;
        dword_180211FE4 = d0;
        ir23[0] = d1;
        ir23[1] = d2;
        GteGpf();
        int g1 = (short)dword_180211FE4;
        int g2 = *(short*)&ir23[0];
        int g3 = *(short*)&ir23[1];
        dword_180211FE4 = g1;
        ir23[0] = g2;
        ir23[1] = g3;
        *(int*)p += g1 << 6;
        *(int*)(p + 4) += g2 << 6;
        *(int*)(p + 8) += g3 << 6;
        return 0xFFFF;
    }

    /// <summary>(int)sqrt((a.z - z)^2 + (a.y - y)^2 + (a.x - x)^2) in 32-bit arithmetic; 0 becomes 1.</summary>
    private static int Part106aDistance(short* a, int x, int y, int z)
    {
        Recompiler.Runtime.Tracer.Enter("Part106aDistance(a, x, y, z)", (nint)a, x, y, z);
        int dx = a[0] - x;
        int dy = a[1] - y;
        int dz = a[2] - z;
        int distance = Recompiler.Runtime.X86.Cvttsd2si(Math.Sqrt(dz * dz + dy * dy + dx * dx));
        return distance == 0 ? 1 : distance;
    }

    /// <summary>
    /// Emission through the 40-byte frame table entry <paramref name="slot"/> at PSX v8+24: offset vector (p+20),
    /// fade (v8+32 by curve), size (v8+28, INTPL of entry[26..27]), mode (entry[2]); then up to two emitters
    /// (entry[0]/[28] and entry[1]/[32]) and the frame advance (entry[4], wrap at entry[8]).
    /// </summary>
    private static void Part106aEmit(Part106aFrame* f, long p, long v8, byte slot, int v6, int v7, int v10, int v110)
    {
        Recompiler.Runtime.Tracer.Enter("Part106aEmit(f, p, v8, slot, v6, v7, v10, v110)", (nint)f, p, v8, slot, v6, v7, v10, v110);
        byte* entry = ResolveGlobalPsxAddress(*(uint*)(v8 + 24)) + 40 * slot;
        byte flags = entry[3];
        short* offset = null;
        if ((flags & 1) == 0)
        {
            offset = (short*)&f->Dir;
            *(int*)offset = *(int*)(p + 20);
            offset[2] = *(short*)(p + 24);
            ushort x = (ushort)(offset[0] + *(ushort*)(entry + 10));
            ushort y = (ushort)(offset[1] + *(ushort*)(entry + 12));
            ushort z = (ushort)(offset[2] + *(ushort*)(entry + 14));
            ushort shift = (ushort)(*(sbyte*)(p + 19) << 2);
            if ((flags & 0x10) != 0)
            {
                x += shift;
            }

            if ((flags & 0x20) != 0)
            {
                y += shift;
            }

            if ((flags & 0x40) != 0)
            {
                z += shift;
            }

            offset[0] = (short)x;
            offset[1] = (short)y;
            offset[2] = (short)z;
            offset[3] = (short)((flags & 4) == 0 ? 1 : 0);
            *(int*)(p + 20) = *(int*)offset;
            *(short*)(p + 24) = offset[2];
        }

        if ((flags & 8) != 0)
        {
            f->Fade = *(short*)(v8 + 32);
        }
        else
        {
            int fade = *(short*)(v8 + 32);
            switch (entry[6])
            {
                case 1:
                    f->Fade = (Recompiler.Runtime.X86.Cvttsd2si(Math.Cos(Part106aAngle(v10 >> 2)) * 4096.8) * fade) >> 12;
                    break;
                case 2:
                    f->Fade = (Recompiler.Runtime.X86.Cvttsd2si(Math.Sin(Part106aAngle(v10 >> 2)) * 4096.8) * fade) >> 12;
                    break;
                case 3:
                    f->Fade = (Recompiler.Runtime.X86.Cvttsd2si(Math.Sin(Part106aAngle(v10 >> 1)) * 4096.8) * fade) >> 12;
                    break;
            }
        }

        // 4096 (fully opaque) is encoded as -1; an attractor offset is clamped to 0..4096 first.
        if (v110 != 0xFFFF)
        {
            f->Fade += v110;
            if (f->Fade < 0)
            {
                f->Fade = 0;
            }
            else if (f->Fade > 4096)
            {
                f->Fade = 4096;
            }
        }

        if (f->Fade == 4096)
        {
            f->Fade = -1;
        }

        if ((flags & 2) != 0)
        {
            f->Size = *(int*)(v8 + 28);
        }
        else
        {
            int size = *(int*)(v8 + 28);
            Part106aIntpl(v10, entry[26] << 7, entry[27] << 7);
            f->Size = (dword_180212024 * size) >> 12;
        }

        int mode = entry[2];
        if ((mode == 255 || mode == 0) && (uint)f->Fade <= 0x800)
        {
            mode = 1;
        }

        f->Mode = mode;
        for (int pass = 0; ;)
        {
            uint source;
            int kind;
            if (pass == 0)
            {
                source = *(uint*)(entry + 28);
                kind = entry[0];
            }
            else
            {
                source = *(uint*)(entry + 32);
                kind = entry[1];
            }

            offset = Part106aEmitOne(f, p, v8, entry, ResolveGlobalPsxAddress(source), kind, offset, v6, v7);
            if (entry[1] == 0xFF || ++pass >= 2)
            {
                break;
            }
        }

        short advance = *(short*)(entry + 4);
        if (advance == 1)
        {
            ++*(byte*)(p + 29);
        }
        else if (advance == 2)
        {
            int frameIndex = *(byte*)(p + 29) + 1;
            if (frameIndex >= *(short*)(entry + 8))
            {
                frameIndex = 0;
            }

            *(byte*)(p + 29) = (byte)frameIndex;
        }
    }

    /// <summary>One emitter of a frame table entry (kinds 0..9); returns the offset vector pointer (kind 9 may set it).</summary>
    private static short* Part106aEmitOne(Part106aFrame* f, long p, long v8, byte* entry, byte* source, int kind, short* offset, int v6, int v7)
    {
        Recompiler.Runtime.Tracer.Enter("Part106aEmitOne(f, p, v8, entry, source, kind, offset, v6, v7)", (nint)f, p, v8, (nint)entry, (nint)source, kind, (nint)offset, v6, v7);
        long* pos = &f->Pos;
        switch (kind)
        {
            case 0:
                EmitSpriteCellFrame((ushort*)source, pos, offset, f->Size, *(byte*)(p + 29), f->Mode, f->Fade, 0, 0);
                break;
            case 1:
                EmitWorldSpriteList((ushort*)source, (int*)pos, *(byte*)(p + 29), f->Mode, f->Fade);
                break;
            case 2:
                EmitSpriteAnimFrame((long)source, pos, offset, f->Size, *(byte*)(p + 29), f->Mode, f->Fade);
                break;
            case 3:
                EmitWorldSpriteFrame((long)source, pos, offset, f->Size, *(byte*)(p + 29), f->Mode, (uint)f->Fade);
                break;
            case 4:
            {
                byte* record = SfxIndexedRecordPtr(p, v8);
                short* target = (short*)ResolveGlobalPsxAddress(*(uint*)(v8 + 40));
                DrawRibbonTrail((ushort*)source, (long)pos, target, *(byte*)(p + 29), f->Size, f->Fade, f->Mode, record, 1);
                break;
            }

            case 5:
            {
                byte* record = SfxIndexedRecordPtr(p, v8);
                DrawRibbonTrail((ushort*)source, (long)pos, null, *(byte*)(p + 29), f->Size, f->Fade, f->Mode, record, 2);
                break;
            }

            case 6:
                DrawRingMeshEffect(source, pos, offset, f->Size, *(byte*)(p + 29), f->Mode, f->Fade);
                break;
            case 7:
                EmitScaledSpriteFrame((ushort*)source, pos, offset, f->Size, *(byte*)(p + 29), f->Mode, f->Fade);
                break;
            case 8:
            {
                byte* record = SfxIndexedRecordPtr(p, v8);
                DrawRibbonTrail((ushort*)source, (long)pos, null, *(byte*)(p + 29), f->Size, f->Fade, f->Mode, record, 0);
                break;
            }

            case 9:
                if (offset == null)
                {
                    f->Dir = 0;
                    offset = (short*)&f->Dir;
                }

                Part106aScriptEmit(f, p, v8, *(uint*)(entry + 36), offset, v6, v7);
                break;
        }

        return offset;
    }

    /// <summary>Emitter kind 9: script handler at entry+36 ("pppppppvv") or a native one, called with the frame state.</summary>
    private static void Part106aScriptEmit(Part106aFrame* f, long p, long v8, uint handler, short* offset, int v6, int v7)
    {
        Recompiler.Runtime.Tracer.Enter("Part106aScriptEmit(f, p, v8, handler, offset, v6, v7)", (nint)f, p, v8, handler, (nint)offset, v6, v7);
        if ((handler & 0xFF000000) == 0x80000000 && (handler & 0xFFFFFFF) < 0x200000)
        {
            Span<ulong> va = stackalloc ulong[9];
            ulong* vaAddress = (ulong*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(va));
            va[0] = (ulong)p;
            va[1] = (ulong)v8;
            va[2] = (ulong)&f->Pos;
            va[3] = (ulong)offset;
            va[4] = (ulong)&f->Size;
            va[5] = (ulong)&f->Fade;
            va[6] = (ulong)&f->Mode;
            va[7] = (uint)v6;
            va[8] = (uint)v7;
            fixed (byte* signature = "pppppppvv"u8)
            {
                sub_18000DBD0(qword_1803678E0, (int)handler, (long)signature, vaAddress);
            }
        }
        else
        {
            var native = (delegate* unmanaged<long, long, long*, short*, int*, int*, int*, int, int, void>)PsxAddrToPtr((long)unk_180576A10, handler);
            native(p, v8, &f->Pos, offset, &f->Size, &f->Fade, &f->Mode, v6, v7);
        }
    }

    /// <summary>
    /// Sub-emitter at PSX v8+4 when p+18 &amp; 0x7F: for emitter kinds &gt;= 4 Dir becomes the yaw/pitch from the
    /// reference position (GTE SQR, atan2) before SfxEmitParticles(a1, p, Pos, Dir).
    /// </summary>
    private static void Part106aSubEmitter(Part106aFrame* f, long a1, long p, long v8)
    {
        Recompiler.Runtime.Tracer.Enter("Part106aSubEmitter(f, a1, p, v8)", (nint)f, a1, p, v8);
        if (ResolveGlobalPsxAddress(*(uint*)(v8 + 4)) == null || (*(byte*)(p + 18) & 0x7F) == 0)
        {
            return;
        }

        short* pos = (short*)&f->Pos;
        short* dir = (short*)&f->Dir;
        if (f->EmitterKind >= 4)
        {
            int dx = pos[0] - (short)f->RefX;
            int dz = pos[2] - (short)f->RefZ;
            int y = pos[1];
            int refY = (short)f->RefY;
            dword_180211FE4 = dx;
            *(int*)Unsafe.AsPointer(ref qword_180211FE8) = dz;
            GteSqr();
            double yaw = Math.Atan2(dx, dz);
            int squared = dword_180212024 + dword_180212028;
            dir[2] = 0;
            dir[1] = (short)Recompiler.Runtime.X86.Cvttsd2si(-yaw / (2.0 * Math.PI) * -4096.0);
            double horizontal = Recompiler.Runtime.X86.Cvttsd2si(Math.Sqrt(squared));
            double pitch = Math.Atan2(horizontal, y - refY);
            dir[0] = (short)(-1024 - Recompiler.Runtime.X86.Cvttsd2si(-pitch / (2.0 * Math.PI) * 4096.0));
        }

        SfxEmitParticles(a1, p, pos, (int*)dir);
    }
}
