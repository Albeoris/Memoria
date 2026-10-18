using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Evaluates the 32-byte orbit/camera track <paramref name="a2"/> into <paramref name="a3"/> (angle x, angle y,
    /// roll, distance). Track bytes: +0 start (3 s16 angles, s16 distance), +8/+20 anchor id pairs for state 3,
    /// +12 end vector, +20/+21 anchor ids, +24 state, +25 per-axis wrap modes (2 bits each), +26 weight curve modes
    /// (2 bits per weight), +28 length, +29 elapsed. States 2/3 with elapsed 0 fall back to state 1. The two weights
    /// come from the curve (0 linear, 1 ease-out, 2 ease-in, 3 keep) with speed dword_18021FF0C; the start/end
    /// vectors are blended through GTE LoadAverageShort12. State 2: distance through GTE INTPL; state 3: blend of two
    /// orbit positions, with <paramref name="a1"/> != 0 converted to look angles and distance towards the anchor;
    /// state 4: cubic Bezier over the four control points at 0x180220060; other states: orbit position of the end
    /// vector. Returns the last value the original leaves in eax (see the claims).
    /// </summary>
    public static long EvalOrbitTrack(int a1, long a2, ushort* a3)
    {
        Recompiler.Runtime.Tracer.Enter("EvalOrbitTrack(a1, a2, a3)", a1, a2, (nint)a3);
        byte* track = (byte*)a2;
        Span<int> weights = stackalloc int[2];
        Span<int> complements = stackalloc int[2];
        weights[0] = 4096;
        weights[1] = 0;
        complements[0] = 0;
        complements[1] = 0;
        if ((byte)(track[24] - 2) <= 1 && track[29] == 0)
        {
            track[24] = 1;
        }

        if (track[24] > 1)
        {
            int modes = track[26];
            byte length = track[28];
            int speed = dword_18021FF0C;
            for (int i = 0; i < 2; i++)
            {
                byte elapsed;
                if (length != 0 && (elapsed = track[29]) != 0)
                {
                    uint factor = ((uint)(4096 - (length * speed)) << 17) / (uint)(length * length);
                    switch (modes & 3)
                    {
                        case 0:
                            weights[i] = (elapsed * (0x10000000 / length)) >> 16;
                            break;
                        case 1:
                            int remaining = length - elapsed;
                            weights[i] = 4096 - ((int)((uint)(remaining * remaining) * factor) >> 17) - (speed * remaining);
                            break;
                        case 2:
                            weights[i] = (elapsed * speed) + ((int)((uint)(elapsed * elapsed) * factor) >> 17);
                            break;
                    }
                }
                else
                {
                    weights[i] = 0;
                }

                complements[i] = 4096 - weights[i];
                modes >>= 2;
            }
        }

        int p = weights[0];
        int q = complements[0];
        bool interpolating = track[24] > 1;
        Span<short> v = stackalloc short[8];
        short* vAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(v));
        for (int k = 0; k < 4; k++)
        {
            v[k] = *(short*)(track + (2 * k));
            v[4 + k] = *(short*)(track + 12 + (2 * k));
        }

        if (interpolating)
        {
            EvalOrbitTrackWrapAngles(vAddress, track[25]);
        }

        Span<ushort> avg = stackalloc ushort[4];
        ushort* avgAddress = (ushort*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(avg));
        GteLoadAverageShort12(vAddress, vAddress + 4, p, q, avgAddress);
        switch (track[24])
        {
            case 2:
                dword_180211FE0 = complements[1];
                dword_180211FE4 = v[3];
                *(int*)Unsafe.AsPointer(ref qword_180211F94) = v[7];
                GteIntpl();
                avg[3] = (ushort)dword_180212024;
                if (a1 != 0)
                {
                    a3[3] = avg[3];
                    a3[0] = avg[0];
                    a3[1] = avg[1];
                    a3[2] = avg[2];
                    return avg[2];
                }

                ComputeOrbitPosition(track[20], (sbyte)track[21], (short*)avgAddress, a3);
                a3[3] = avg[2];
                if (track[29] == 0)
                {
                    track[24] = 1;
                }

                return avg[2];

            case 3:
                return EvalOrbitTrackBlend(a1, track, a3, vAddress, avg, p, q);

            case 4:
                return EvalOrbitTrackBezier(a3, avg, p, q);

            default:
                ComputeOrbitPosition(track[20], (sbyte)track[21], vAddress + 4, a3);
                a3[3] = (ushort)v[6];
                uint anchor = (uint)((track[20] & 0x1F) - 25);
                if (anchor <= 1)
                {
                    byte_18022011B++;
                }

                return anchor;
        }
    }

    /// <summary>Per axis (2 bits of <paramref name="modes"/>): moves the start or end angle by 4096 so the blend takes the intended way round.</summary>
    private static void EvalOrbitTrackWrapAngles(short* v, int modes)
    {
        Recompiler.Runtime.Tracer.Enter("EvalOrbitTrackWrapAngles(v, modes)", (nint)v, modes);
        for (int k = 0; k < 3; k++)
        {
            short* start = v + k;
            short* end = v + k + 4;
            switch (modes & 3)
            {
                case 0:
                    if (*start != *end)
                    {
                        if ((((ushort)*start - (ushort)*end) & 0x800) != 0)
                        {
                            if (*start >= *end)
                            {
                                *end = (short)(*end + 4096);
                            }
                        }
                        else if (*start <= *end)
                        {
                            *start = (short)(*start + 4096);
                        }
                    }

                    break;
                case 1:
                    if (*start >= *end)
                    {
                        *end = (short)(*end + 4096);
                    }

                    break;
                case 2:
                    if (*start <= *end)
                    {
                        *start = (short)(*start + 4096);
                    }

                    break;
            }

            modes >>= 2;
        }
    }

    /// <summary>State 3: blends the orbit positions of the anchor pairs at +8 and +20; see <see cref="EvalOrbitTrack"/>.</summary>
    private static long EvalOrbitTrackBlend(int a1, byte* track, ushort* a3, short* v, ReadOnlySpan<ushort> avg, int p, int q)
    {
        Recompiler.Runtime.Tracer.Enter("EvalOrbitTrackBlend(a1, track, a3, v, avg, p, q)", a1, (nint)track, (nint)a3, (nint)v, avg.Length, p, q);
        Span<short> orbit = stackalloc short[8];
        short* orbitAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(orbit));
        for (int k = 0; k < 2; k++)
        {
            byte* ids = track + 8 + (12 * k);
            ComputeOrbitPosition(ids[0], (sbyte)ids[1], v + (4 * k), (ushort*)(orbitAddress + (4 * k)));
        }

        if (a1 == 0)
        {
            GteLoadAverageShort12(orbitAddress, orbitAddress + 4, p, q, a3);
            a3[3] = avg[2];
            if (track[29] == 0)
            {
                track[24] = 1;
            }

            return avg[2];
        }

        GteLoadAverageShort12(orbitAddress, orbitAddress + 4, p, q, (ushort*)(orbitAddress + 4));
        byte first = track[20];
        byte second = track[21];
        uint* bias = stackalloc uint[2];
        GetAnchorTransform((sbyte)first, (long)orbitAddress, (long)v, bias);
        short ax;
        short ay;
        short az;
        if ((second & 0x1F) != 0)
        {
            Span<short> position = stackalloc short[4];
            short* positionAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(position));
            short* rotation = stackalloc short[4];
            GetAnchorTransform((sbyte)((second & 0x3F) | (first & 0xC0)), (long)positionAddress, (long)rotation, bias);
            ax = (short)((short)(orbit[0] + position[0]) >> 1);
            ay = (short)((short)(orbit[1] + position[1]) >> 1);
            az = (short)((short)(orbit[2] + position[2]) >> 1);
        }
        else
        {
            ax = orbit[0];
            ay = orbit[1];
            az = orbit[2];
        }

        int dx = orbit[4] - ax;
        int dy = orbit[5] - ay;
        int dz = orbit[6] - az;
        int horizontal2 = (dx * dx) + (dz * dz);
        int yaw = Cvttsd2si(-Math.Atan2(dz, dx) / 6.283185307179586 * -4096.0);
        a3[1] = (ushort)((-1024 - yaw) & 0xFFF);
        double horizontal = Cvttsd2si(Math.Sqrt(horizontal2));
        double pitch = Math.Atan2(horizontal, dy);
        int distance2 = (dy * dy) + horizontal2;
        int pitchUnits = Cvttsd2si(-pitch / 6.283185307179586 * -4096.0);
        a3[2] = avg[2];
        a3[0] = (ushort)((1024 - pitchUnits) & 0xFFF);
        int distance = Cvttsd2si(Math.Sqrt(distance2));
        a3[3] = (ushort)distance;
        track[21] |= 0xC0;
        return (uint)distance;
    }

    /// <summary>State 4: cubic Bezier (weights p/16, q/16) over the four s16 control points at 0x180220060 (stride 8).</summary>
    private static long EvalOrbitTrackBezier(ushort* a3, ReadOnlySpan<ushort> avg, int p, int q)
    {
        Recompiler.Runtime.Tracer.Enter("EvalOrbitTrackBezier(a3, avg, p, q)", (nint)a3, avg.Length, p, q);
        int s = p >> 4;
        int t = q >> 4;
        *(uint*)a3 = 0;
        a3[2] = 0;
        Span<int> basis = stackalloc int[4];
        basis[0] = (s * s * s) >> 12;
        basis[1] = (6 * t * s * s) >> 12;
        basis[2] = (6 * s * t * t) >> 12;
        basis[3] = (t * t * t) >> 12;
        a3[3] = avg[2];
        short* points = (short*)(unk_180220062 - 2);
        ushort y = 0;
        ushort z = 0;
        int last = 0;
        for (int k = 0; k < 4; k++)
        {
            int w = basis[k];
            short* point = points + (4 * k);
            a3[0] = (ushort)(a3[0] + ((w * point[0]) >> 12));
            y = (ushort)(y + ((w * point[1]) >> 12));
            a3[1] = y;
            last = (w * point[2]) >> 12;
            z = (ushort)(z + last);
            a3[2] = z;
        }

        return (uint)last;
    }

    /// <summary>
    /// Starts track block <paramref name="a1"/> with timer <paramref name="a2"/> (dword_180211E4C): clears bit-1
    /// flags of byte_1802200BC, sets byte_180220119, clears byte_1802200B2 and the track speed dword_18021FF0C,
    /// snaps both running tracks (state 2/3 with elapsed != 0) to their end and sets them to state 1, then returns
    /// SfxSelectBlockVariant(a1).
    /// </summary>
    public static long SfxStartTrackBlock(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("SfxStartTrackBlock(a1, a2)", a1, a2);
        dword_180211E4C = a2;
        byte flags = byte_1802200BC;
        if ((flags & 2) != 0)
        {
            flags = 0;
        }

        byte_180220119 = 1;
        byte_1802200B2 = 0;
        dword_18021FF0C = 0;
        byte_1802200BC = flags;
        byte* state = (byte*)Unsafe.AsPointer(ref byte_1802200E0);
        byte* track = unk_1802200C8;
        for (int i = 0; i < 2; i++)
        {
            if (state[5] != 0 && (byte)(state[0] - 2) <= 1)
            {
                EvalOrbitTrack(1, (long)track, (ushort*)(track + 12));
                state[0] = 1;
            }

            track += 32;
            state += 32;
        }

        return SfxSelectBlockVariant((short*)a1);
    }

    /// <summary>
    /// Qword 8 (+152) of the context at off_180066C68[3] when its signed count at +39 is positive, else
    /// PsxGetContextQword(0).
    /// </summary>
    public static long PsxGetContextQword8Or0()
    {
        Recompiler.Runtime.Tracer.Enter("PsxGetContextQword8Or0()");
        byte* context = ResolveGlobalPsxAddress(ContextAddress(3));
        if ((sbyte)context[39] <= 0)
        {
            return PsxGetContextQword(0);
        }

        if (*(sbyte*)(ResolveGlobalPsxAddress(ContextAddress(3)) + 39) <= 0)
        {
            PsxCompatibilityAssert();
        }

        return *(long*)(ResolveGlobalPsxAddress(ContextAddress(3)) + 152);
    }

    /// <summary>
    /// Starts a reaction block for event <paramref name="a1"/> (not 0xFFFF, low byte high nibble not 0xF) when
    /// dword_180323268 is set and the context flags at +64 lack bit 1, and the context object (qword 16) is neither
    /// in host state 0x10000000 nor 0x4000000. The block is chosen from the table qword_1803231E0 (offsets +72 for
    /// context flag bit 2, +52/+56 for host state 0x4000 by owner byte +16, +44/+48 for nibbles 1/7 by object byte
    /// +17 == 2), from Block (+6/+8 offsets by owner byte +16), or the object's pointer at +48 when its byte +16 is
    /// 0; then SfxStartTrackBlock(block, 80).
    /// </summary>
    public static void SfxTriggerReaction(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxTriggerReaction(a1)", a1);
        if (a1 == 0xFFFF || dword_180323268 == 0)
        {
            return;
        }

        byte code = (byte)a1;
        if ((a1 & 0xF0) == 0xF0 || (*(byte*)(PsxAddrToPtr((long)unk_180576A10, ContextAddress(3)) + 64) & 2) != 0)
        {
            return;
        }

        long obj = PsxGetContextQword(16);
        if (HostCallbackSend(obj, 0x10000000u) != 0 || HostCallbackSend(obj, 0x4000000u) != 0)
        {
            return;
        }

        long owner = PsxGetContextQword8Or0();
        long table = qword_1803231E0;
        Table2* table2 = (Table2*)table;
        byte* block;
        if ((*(byte*)(PsxAddrToPtr((long)unk_180576A10, ContextAddress(3)) + 64) & 4) != 0)
        {
            block = (byte*)(table + table2->field_72);
        }
        else if (*(byte*)(obj + 16) != 0)
        {
            if (HostCallbackSend(obj, 0x4000u) != 0)
            {
                long offset = *(byte*)(owner + 16) != 0 ? table2->field_52 : table2->field_56;
                block = (byte*)(table + offset);
            }
            else
            {
                int group = code >> 4;
                if (group == 1 || group == 7)
                {
                    long offset = *(byte*)(obj + 17) == 2 ? table2->field_44 : table2->field_48;
                    block = (byte*)(table + offset);
                }
                else
                {
                    byte* blocks = (byte*)Block;
                    if (blocks == null)
                    {
                        return;
                    }

                    block = *(byte*)(owner + 16) != 0 ? blocks + ((short*)blocks)[3] : blocks + ((short*)blocks)[4];
                }
            }
        }
        else
        {
            block = *(byte**)(obj + 48);
        }

        if (block != null)
        {
            SfxStartTrackBlock((long)block, 80);
        }
    }
}
