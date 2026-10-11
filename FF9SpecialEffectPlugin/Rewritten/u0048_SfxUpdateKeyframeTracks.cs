using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>Size of one keyframe track record in the array at *(a2 + 16).</summary>
    private const int SfxUpdateKeyframeTracksStride = 108;

    /// <summary>Size of the per-track PSX address mapper block at unk_180587520.</summary>
    private const int SfxUpdateKeyframeTracksMapperStride = 8160;

    /// <summary>
    /// Steps every keyframe track 0..*(int*)(a2 + 8) (108-byte records at *(a2 + 16)): state +48 (0 idle, -1 reset),
    /// frame +56 against the key's duration (+6); interpolates scale, colours, positions and the extra channels with
    /// the GTE INTPL/GPF/GPL emulation. Writes the active count to a2 + 4, the last active index to a2 + 8 and
    /// dword_180323168 (dword_1803231D8 keeps the maximum); returns the active count.
    /// </summary>
    public static long SfxUpdateKeyframeTracks(long a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("SfxUpdateKeyframeTracks(a1, a2)", a1, a2);
        byte* track = *(byte**)(a2 + 16);
        int blend = 0;
        int lastActive = 0;
        *(uint*)(a2 + 4) = 0;
        if (*(int*)(a2 + 8) >= 0)
        {
            int index = 0;
            do
            {
                short state = *(short*)(track + 48);
                if (state == -1)
                {
                    *(ushort*)(track + 48) = 0;
                }
                else if (state != 0)
                {
                    ++*(uint*)(a2 + 4);
                    lastActive = index;
                    SfxUpdateKeyframeTracksStep(track, index, ref blend);
                }

                ++index;
                track += SfxUpdateKeyframeTracksStride;
            }
            while (index <= *(int*)(a2 + 8));
        }

        int peak = dword_1803231D8;
        *(int*)(a2 + 8) = lastActive;
        dword_180323168 = lastActive;
        if (peak < lastActive)
        {
            peak = lastActive;
        }

        dword_1803231D8 = peak;
        return *(uint*)(a2 + 4);
    }

    /// <summary>One active track; <paramref name="blend"/> is the scale value carried across tracks (ebp).</summary>
    private static void SfxUpdateKeyframeTracksStep(byte* track, int index, ref int blend)
    {
        Recompiler.Runtime.Tracer.Enter("SfxUpdateKeyframeTracksStep(track, index, blend)", (nint)track, index, blend);
        byte* key = ResolveGlobalPsxAddress(*(uint*)(track + 36));
        Key* key2 = (Key*)key;
        int frame = *(short*)(track + 56);
        int duration = key2->field_6;
        *(uint*)(track + 20) = (uint)PsxAddrMapperRegister(
            (long)unk_180576A10,
            (ulong)((long)unk_180587520 + SfxUpdateKeyframeTracksMapperStride * index));
        if (duration == 0)
        {
            return;
        }

        *(ushort*)(track + 50) = 0;
        int t = (frame << 12) / duration;
        if (frame >= duration)
        {
            if (*(ushort*)(track + 52) == 0 && *(ushort*)(track + 54) == 0)
            {
                *(ushort*)(track + 48) = 0xFFFF;
            }

            return;
        }

        SfxUpdateKeyframeTracksScale(track, key, frame, duration, t, ref blend);
        SfxUpdateKeyframeTracksCursor(track, t, ref blend);

        int bfc = key2->field_30;
        short ir3 = SfxUpdateKeyframeTracksTransform(track, key, t, bfc);
        ir3 = SfxUpdateKeyframeTracksLink(track, t, ir3);
        SfxUpdateKeyframeTracksColour(track, t, bfc, ir3);
        SfxUpdateKeyframeTracksSource(track, t);
        *(ushort*)(track + 56) = (ushort)(frame + 1);
    }

    /// <summary>Scale channel: curve kind *(short*)(key + 4) (0 INTPL, 1 sine, 2 table, 3 constant 256), then +58/+50 and the +92..+96 deltas.</summary>
    private static void SfxUpdateKeyframeTracksScale(byte* track, byte* key, int frame, int duration, int t, ref int blend)
    {
        Recompiler.Runtime.Tracer.Enter("SfxUpdateKeyframeTracksScale(track, key, frame, duration, t, blend)", (nint)track, (nint)key, frame, duration, t, blend);
        Key* key2 = (Key*)key;
        switch (key2->field_4)
        {
            case 0:
                blend = SfxUpdateKeyframeTracksIntpl1(t, key2->field_8, key2->field_10);
                break;
            case 1:
                double angle = (double)(t >> 1) / 4096.8 * 2.0 * 3.141592653589793;
                blend = unchecked(key2->field_8 * Recompiler.Runtime.X86.Cvttsd2si(Math.Sin(angle) * 4096.8)) >> 12;
                break;
            case 2:
                byte* curve = ResolveGlobalPsxAddress(*(uint*)(track + 36));
                short* table = *(int*)curve == -1 ? (short*)(curve + 64) : (short*)(curve + 80);
                blend = table[16 * frame / duration];
                break;
            case 3:
                blend = 256;
                break;
        }

        int low = *(short*)(track + 58) + (byte)blend;
        int high = blend >> 8;
        if (low >= 256)
        {
            low -= 256;
            high++;
        }

        *(ushort*)(track + 58) = (ushort)low;
        *(ushort*)(track + 50) = (ushort)high;
        if (frame != 0)
        {
            *(ushort*)(track + 92) += key2->field_52;
            *(ushort*)(track + 94) += key2->field_54;
            *(ushort*)(track + 96) += key2->field_56;
        }
        else
        {
            *(uint*)(track + 92) = key2->field_44;
            *(uint*)(track + 96) = key2->field_48;
        }
    }

    /// <summary>
    /// Cursor block at *(track + 24): when its word at -6 is 1, spreads 0x80 over two of the four bytes at -4 by the
    /// INTPL of 4 * count (-20); the quarter steps also land in <paramref name="blend"/>.
    /// </summary>
    private static void SfxUpdateKeyframeTracksCursor(byte* track, int t, ref int blend)
    {
        Recompiler.Runtime.Tracer.Enter("SfxUpdateKeyframeTracksCursor(track, t, blend)", (nint)track, t, blend);
        byte* cursor = ResolveGlobalPsxAddress(*(uint*)(track + 24));
        if (*(short*)(cursor - 6) != 1)
        {
            return;
        }

        int count = *(int*)(cursor - 20);
        *(int*)(cursor - 4) = 0;
        int mac = SfxUpdateKeyframeTracksIntpl1(t, 0, 4 * count);
        blend = mac >> 2;
        int fraction = 32 * (mac & 3);
        if (fraction != 0)
        {
            int next = blend + 1;
            if (next >= *(int*)(cursor - 20))
            {
                next = blend;
            }

            cursor[blend - 4] = (byte)(0x80 - fraction);
            cursor[next - 4] += (byte)fraction;
        }
        else
        {
            cursor[blend - 4] = 0x80;
        }
    }

    /// <summary>
    /// Position/rotation: INTPL of the cursor vectors, GPF + GPL of the key's two vectors, optional key vector
    /// (+32 flag). Returns the int16 MAC3 the colour block uses as IR3.
    /// </summary>
    private static short SfxUpdateKeyframeTracksTransform(byte* track, byte* key, int t, int bfc)
    {
        Recompiler.Runtime.Tracer.Enter("SfxUpdateKeyframeTracksTransform(track, key, t, bfc)", (nint)track, (nint)key, t, bfc);
        Key* key2 = (Key*)key;
        byte* cursor = ResolveGlobalPsxAddress(*(uint*)(track + 24));
        SfxUpdateKeyframeTracksIr(*(short*)(cursor - 14), *(short*)(cursor - 10), key2->field_22);
        SfxUpdateKeyframeTracksFc(*(short*)(cursor - 12), *(short*)(cursor - 8), bfc);
        dword_180211FE0 = t;
        GteIntpl();
        int mac1 = (short)dword_180212024;
        ushort mac2 = (ushort)dword_180212028;
        short mac3 = (short)dword_18021202C;

        dword_180211FE0 = 4096 - t;
        SfxUpdateKeyframeTracksIr(key2->field_16, key2->field_18, key2->field_20);
        GteGpf();
        *(int*)(track + 28) = mac1;
        *(ushort*)(track + 32) = mac2;
        *(ushort*)(track + 74) = (ushort)mac3;

        dword_180211FE0 = t;
        SfxUpdateKeyframeTracksIr(key2->field_24, key2->field_26, key2->field_28);
        GteGpl();
        int* ir2 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
        dword_180211FE4 = (short)dword_180211FE4;
        *(ushort*)(track + 68) = (ushort)dword_180211FE4;
        *ir2 = (short)*ir2;
        *(ushort*)(track + 70) = (ushort)*ir2;
        *(int*)word_180211FEC = word_180211FEC[0];
        *(ushort*)(track + 72) = (ushort)word_180211FEC[0];

        if (key2->field_32 != 0)
        {
            mac3 = SfxUpdateKeyframeTracksIntplVector(t, key + 36, key + 40);
            *(uint*)(track + 76) = SfxUpdateKeyframeTracksPackMac12();
        }

        return mac3;
    }

    /// <summary>Linked block at *(track + 4): vector INTPL into +80, scalar into +86, optional vector (-20 flag) into +88.</summary>
    private static short SfxUpdateKeyframeTracksLink(byte* track, int t, short ir3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxUpdateKeyframeTracksLink(track, t, ir3)", (nint)track, t, ir3);
        byte* link = ResolveGlobalPsxAddress(*(uint*)(track + 4));
        if (link == null)
        {
            return ir3;
        }

        GteIntplVector((short*)(link - 36), (short*)(link - 28), t, (ushort*)(track + 80));
        *(ushort*)(track + 86) = (ushort)SfxUpdateKeyframeTracksIntpl1(t, *(short*)(link - 30), *(short*)(link - 22));
        if (*(ushort*)(link - 20) != 0)
        {
            ir3 = SfxUpdateKeyframeTracksIntplVector(t, link - 16, link - 12);
            *(uint*)(track + 88) = SfxUpdateKeyframeTracksPackMac12();
        }

        return ir3;
    }

    /// <summary>Colour block by the kind dword at the key (0: one RGB, 1..3: two RGB, 4: RGB + third channel).</summary>
    private static void SfxUpdateKeyframeTracksColour(byte* track, int t, int bfc, short ir3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxUpdateKeyframeTracksColour(track, t, bfc, ir3)", (nint)track, t, bfc, ir3);
        uint address = *(uint*)(track + 36);
        int kind = *(int*)ResolveGlobalPsxAddress(address);
        if (kind == 0)
        {
            short* rgb = (short*)ResolveGlobalPsxAddress(address);
            SfxUpdateKeyframeTracksIr(rgb[30], rgb[32], ir3);
            SfxUpdateKeyframeTracksFc(rgb[31], rgb[33], bfc);
            dword_180211FE0 = t;
            GteIntpl();
            *(ushort*)(track + 60) = (ushort)dword_180212024;
            *(ushort*)(track + 62) = (ushort)dword_180212028;
        }
        else if (kind >= 1 && kind <= 3)
        {
            short* rgb = (short*)ResolveGlobalPsxAddress(address);
            SfxUpdateKeyframeTracksIr(rgb[30], rgb[31], ir3);
            SfxUpdateKeyframeTracksFc(rgb[32], rgb[33], bfc);
            dword_180211FE0 = t;
            GteIntpl();
            *(ushort*)(track + 60) = (ushort)dword_180212024;
            *(ushort*)(track + 62) = (ushort)dword_180212028;
            int mac3 = (short)dword_18021202C;
            SfxUpdateKeyframeTracksIr(rgb[34], rgb[35], mac3);
            SfxUpdateKeyframeTracksFc(rgb[36], rgb[37], bfc);
            dword_180211FE0 = t;
            GteIntpl();
            *(ushort*)(track + 64) = (ushort)dword_180212024;
            *(ushort*)(track + 66) = (ushort)dword_180212028;
        }
        else if (kind == 4)
        {
            short* rgb = (short*)ResolveGlobalPsxAddress(address);
            SfxUpdateKeyframeTracksIr(rgb[30], rgb[32], rgb[34]);
            SfxUpdateKeyframeTracksFc(rgb[31], rgb[33], rgb[35]);
            dword_180211FE0 = t;
            GteIntpl();
            *(ushort*)(track + 64) = (ushort)dword_18021202C;
            *(ushort*)(track + 60) = (ushort)dword_180212024;
            *(ushort*)(track + 62) = (ushort)dword_180212028;
        }
    }

    /// <summary>Source block at *(uint*)track by its type byte: one or two scalar INTPLs into +8 (+10).</summary>
    private static void SfxUpdateKeyframeTracksSource(byte* track, int t)
    {
        Recompiler.Runtime.Tracer.Enter("SfxUpdateKeyframeTracksSource(track, t)", (nint)track, t);
        byte* source = ResolveGlobalPsxAddress(*(uint*)track);
        Source* source2 = (Source*)source;
        switch (*source)
        {
            case 0:
            case 5:
            case 2:
            case 4:
                *(ushort*)(track + 8) = (ushort)SfxUpdateKeyframeTracksIntpl1(t, source2->field_8, source2->field_10);
                break;
            case 1:
                *(ushort*)(track + 8) = (ushort)SfxUpdateKeyframeTracksIntpl1(t, source2->field_24, source2->field_26);
                *(ushort*)(track + 10) = (ushort)SfxUpdateKeyframeTracksIntpl1(t, source2->field_32, source2->field_34);
                break;
            case 3:
                *(ushort*)(track + 8) = (ushort)SfxUpdateKeyframeTracksIntpl1(t, source2->field_24, source2->field_26);
                break;
        }
    }

    /// <summary>INTPL of three int16 values at <paramref name="ir"/> towards three at <paramref name="fc"/>; returns int16 MAC3.</summary>
    private static short SfxUpdateKeyframeTracksIntplVector(int t, byte* ir, byte* fc)
    {
        Recompiler.Runtime.Tracer.Enter("SfxUpdateKeyframeTracksIntplVector(t, ir, fc)", t, (nint)ir, (nint)fc);
        dword_180211FE0 = t;
        SfxUpdateKeyframeTracksIr(*(short*)ir, *(short*)(ir + 2), *(short*)(ir + 4));
        SfxUpdateKeyframeTracksFc(*(short*)fc, *(short*)(fc + 2), *(short*)(fc + 4));
        GteIntpl();
        return (short)dword_18021202C;
    }

    /// <summary>IR0 = t, IR1 = ir1, RFC = rfc, GTE INTPL; returns MAC1.</summary>
    private static int SfxUpdateKeyframeTracksIntpl1(int t, int ir1, int rfc)
    {
        Recompiler.Runtime.Tracer.Enter("SfxUpdateKeyframeTracksIntpl1(t, ir1, rfc)", t, ir1, rfc);
        dword_180211FE0 = t;
        dword_180211FE4 = ir1;
        *(int*)Unsafe.AsPointer(ref qword_180211F94) = rfc;
        GteIntpl();
        return dword_180212024;
    }

    private static void SfxUpdateKeyframeTracksIr(int ir1, int ir2, int ir3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxUpdateKeyframeTracksIr(ir1, ir2, ir3)", ir1, ir2, ir3);
        dword_180211FE4 = ir1;
        *(int*)Unsafe.AsPointer(ref qword_180211FE8) = ir2;
        *(int*)word_180211FEC = ir3;
    }

    /// <summary>Low halves of MAC1 (bits 0..15) and MAC2 (bits 16..31).</summary>
    private static uint SfxUpdateKeyframeTracksPackMac12()
    {
        Recompiler.Runtime.Tracer.Enter("SfxUpdateKeyframeTracksPackMac12()");
        return (ushort)dword_180212024 | ((uint)(ushort)dword_180212028 << 16);
    }
}
