using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// The original reads s16 values from the never-written upper half of its rcx home slot ([rsp+0x94]) for IR3 and
    /// FC3; the port uses 0 (see the divergence).
    /// </summary>
    private const short EmitInterpolatedSpriteTrailStackGarbage = 0;

    /// <summary>Scale of the 4096-per-turn angles and the cos/sin results (qword_18004B6C8).</summary>
    private const double EmitInterpolatedSpriteTrailScale = 4096.8;

    /// <summary>
    /// Projects V0 = *a2 with the camera from PSX table slot 5, then picks the trail end: with a1[2] == 0 the GTE
    /// interpolation of IR (a1[4..6]) towards FC (a1[6..8]) by (a3 &lt;&lt; 12) / a1[0]; otherwise the projected point
    /// offset by a radius/angle pair eased by SfxCurveValue. For each of the a1[1] 12-byte entries from a1 + 0x18
    /// (s32 sprite-list offset, s16 +6 weight, u16 +8 start, u16 +10 duration) the point between the projection and
    /// the end (IR0 = 2 * weight) emits that sprite list with an alpha ramped from 0 to a4 once a3 reaches start.
    /// Returns the low 16 bits of the last value in eax (a1[1] after the loop).
    /// </summary>
    public static short EmitInterpolatedSpriteTrail(ushort* a1, long* a2, int a3, int a4)
    {
        Recompiler.Runtime.Tracer.Enter("EmitInterpolatedSpriteTrail(a1, a2, a3, a4)", (nint)a1, (nint)a2, a3, a4);
        // Camera RT/TR from the PSX address in table slot 5 (inlined PsxAddrToPtr, resolved twice as in the original).
        uint* table = (uint*)off_180066C68;
        byte* camera = (byte*)PsxAddrToPtr((long)unk_180576A10, table[5]);
        uint* rt = (uint*)xmmword_180211F40;
        rt[0] = ((Camera4*)camera)->field_0;
        rt[1] = ((Camera4*)camera)->field_4;
        rt[2] = ((Camera4*)camera)->field_8;
        rt[3] = ((Camera4*)camera)->field_12;
        dword_180211F50 = ((Camera4*)camera)->field_16;
        camera = (byte*)PsxAddrToPtr((long)unk_180576A10, table[5]);
        *(uint*)Unsafe.AsPointer(ref qword_180211F54) = ((Camera4*)camera)->field_20;
        *(uint*)unk_180211F58 = ((Camera4*)camera)->field_24;
        dword_180211F5C = ((Camera4*)camera)->field_28;

        uint* v0 = (uint*)Unsafe.AsPointer(ref qword_180211FC0);
        v0[0] = ((uint*)a2)[0];
        v0[1] = ((uint*)a2)[1];
        GteRtps(0);

        int frames = a1[0];
        int sxy = dword_180211FF8;
        short sx = (short)sxy;
        short sy = (short)(sxy >> 16);
        // The division happens before the branch in the original (it faults on a1[0] == 0 on both paths).
        int ir0 = (a3 << 12) / frames;

        short garbage = EmitInterpolatedSpriteTrailStackGarbage;
        int endX;
        int endY;
        int endZ;
        long last;
        if (a1[2] == 0)
        {
            dword_180211FE0 = ir0;
            EmitInterpolatedSpriteTrailSetIr((short)a1[4], (short)a1[5], (short)a1[6]);
            SfxUpdateKeyframeTracksFc((short)a1[6], (short)a1[7], (short)a1[8]);
            last = GteIntpl();
            if (a1[1] == 0)
            {
                return (short)last;
            }

            endX = (short)dword_180212024;
            endY = (short)dword_180212028;
            endZ = (short)dword_18021202C;
        }
        else
        {
            int angleStart = (short)a1[8];
            int angleUnits = angleStart + ((int)SfxCurveValue(a3, frames, *((byte*)a1 + 6)) * ((short)a1[9] - angleStart) >> 12);
            int radiusStart = (short)a1[10];
            int radius = radiusStart + ((int)SfxCurveValue(a3, frames, *((byte*)a1 + 7)) * ((short)a1[11] - radiusStart) >> 12);

            double radians = (double)angleUnits / EmitInterpolatedSpriteTrailScale * 2.0 * Math.PI;
            short x = (short)(sx + ((Recompiler.Runtime.X86.Cvttsd2si(Math.Cos(radians) * EmitInterpolatedSpriteTrailScale) * radius) >> 12));
            short y = (short)(sy + ((Recompiler.Runtime.X86.Cvttsd2si(Math.Sin(radians) * EmitInterpolatedSpriteTrailScale) * radius) >> 12));
            if (a1[1] == 0)
            {
                return y;
            }

            endX = x;
            endY = y;
            endZ = garbage;
        }

        byte* entry = (byte*)(a1 + 12);
        int count;
        int k = 0;
        do
        {
            EmitInterpolatedSpriteTrailSetIr(sx, sy, garbage);
            SfxUpdateKeyframeTracksFc(endX, endY, endZ);
            dword_180211FE0 = 2 * *(short*)(entry + 6);
            GteIntpl();

            int start = *(ushort*)(entry + 8);
            if (a3 >= start)
            {
                int elapsed = a3 - start;
                int duration = *(ushort*)(entry + 10);
                int alpha = duration <= elapsed ? a4 : elapsed * a4 / duration;
                EmitScreenSpriteList(8, (ushort*)((byte*)a1 + *(int*)entry), (short)dword_180212024, (short)dword_180212028, a3, 1, alpha);
            }

            count = a1[1];
            ++k;
            entry += 12;
        }
        while (k < count);

        return (short)count;
    }

    /// <summary>IR1..IR3 (full dwords at 0x180211FE4..EC).</summary>
    private static void EmitInterpolatedSpriteTrailSetIr(int ir1, int ir2, int ir3)
    {
        Recompiler.Runtime.Tracer.Enter("EmitInterpolatedSpriteTrailSetIr(ir1, ir2, ir3)", ir1, ir2, ir3);
        dword_180211FE4 = ir1;
        *(int*)Unsafe.AsPointer(ref qword_180211FE8) = ir2;
        *(int*)unk_180211FEC = ir3;
    }

    /// <summary>Registers a1 with the PSX address mapper and stores the PSX address in table slot 3.</summary>
    public static long SfxRegisterTableSlot3(ulong a1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxRegisterTableSlot3(a1)", a1);
        long result = PsxAddrMapperRegister((long)unk_180576A10, a1);
        ((uint*)off_180066C68)[3] = (uint)result;
        return result;
    }
}
