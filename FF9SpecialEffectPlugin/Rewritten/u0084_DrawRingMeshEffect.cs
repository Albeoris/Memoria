using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Draws one frame of a ring-mesh effect. Header a1: u16 frame count, +4 flags (bit 1: per-frame u16 x/y scale
    /// pairs at +8, GPF-scaled by a4 when a4 &gt;= 0), +5 kind (1 textured, 2 gouraud), +6 rings, +7 segments; the
    /// track (u16 count at +2, colour/geometry data from +4) follows the scale table or starts at +8. a5 is the frame
    /// (wrapped modulo the frame count), a2 the VXY0/VZ0 position, a3 the s16 x/y/z rotation plus a flag at [3] that
    /// pre-multiplies the loaded rotation (null = zero rotation). The GTE RT/TR are loaded from the matrix at
    /// work-table[5], the position is transformed (RTV0TR) into the new translation, the rotation becomes
    /// (RT *) Ry Rx Rz identity with columns scaled by x/y/z. Kind 1/2 then builds the ring descriptor on the stack
    /// and tail-calls EmitConcentricRingQuads / EmitShadedRingQuads, reserving 40/44 bytes per quad from
    /// work-table[9] and using work-table[8] as the OT and [10] as the OTZ bias. Other kinds return the scaled
    /// R33 zero-extended.
    /// </summary>
    public static long DrawRingMeshEffect(byte* a1, long* a2, short* a3, int a4, int a5, int a6, int a7)
    {
        Recompiler.Runtime.Tracer.Enter("DrawRingMeshEffect(a1, a2, a3, a4, a5, a6, a7)", (nint)a1, (nint)a2, (nint)a3, a4, a5, a6, a7);
        long zeroAngles = 0;
        short* angles = a3 != null ? a3 : (short*)&zeroAngles;
        int frames = *(ushort*)a1;
        int frame = a5;
        if (a5 >= frames)
        {
            frame = a5 % frames;
        }

        int kind = a1[5];
        int rings = a1[6];
        int segments = a1[7];
        DrawRingMeshEffectScales(a1, a4, frames, frame, out int scaleX, out int scaleY, out int scaleZ, out ushort* track);

        uint* workTable = (uint*)off_180066C68;
        byte* rotation = ResolveGlobalPsxAddress(workTable[5]);
        Rotation* rotation2 = (Rotation*)rotation;
        uint* rt = (uint*)xmmword_180211F40;
        rt[0] = rotation2->field_0;
        rt[1] = rotation2->field_4;
        rt[2] = rotation2->field_8;
        rt[3] = rotation2->field_12;
        dword_180211F50 = rotation2->field_16;
        byte* translation = ResolveGlobalPsxAddress(workTable[5]);
        Rotation* rotation3 = (Rotation*)translation;
        *(int*)unk_180211F54 = rotation3->field_20;
        *(int*)unk_180211F58 = rotation3->field_24;
        dword_180211F5C = rotation3->field_28;
        qword_180211FC0 = *a2;

        int count = track[1];
        int step = frames > 1 ? frame * (count - 1) / (frames - 1) : 0;
        GteRtV0Tr();

        Stack20_DrawRingMeshEffect_v74 m = default;
        m.union_0_2 = 4096;
        m.union_2_2 = 0;
        m.field_4 = 0;
        m.union_6_2 = 0;
        m.union_8_2 = 4096;
        m.union_10_2 = 0;
        m.union_12_2 = 0;
        m.union_14_2 = 0;
        m.union_16_2 = 4096;
        short* r = (short*)RotMatrixZ(angles[2], ((short*)&m));
        r = (short*)RotMatrixX(angles[0], r);
        RotMatrixY(angles[1], r);
        int trY = dword_180212028;
        int trZ = dword_18021202C;
        for (int row = 0; row < 3; row++)
        {
            ((short*)&m)[row * 3] = (short)((scaleX * ((short*)&m)[row * 3]) >> 12);
            ((short*)&m)[row * 3 + 1] = (short)((scaleY * ((short*)&m)[row * 3 + 1]) >> 12);
            ((short*)&m)[row * 3 + 2] = (short)((scaleZ * ((short*)&m)[row * 3 + 2]) >> 12);
        }

        int trX = dword_180212024;

        // The descriptor area doubles as the copy of RT/TR for the optional pre-multiplication; the ring emitters
        // write radii scratch up to +59.
        byte* desc = stackalloc byte[64];
        if (angles[3] != 0)
        {
            uint* copy = (uint*)desc;
            copy[0] = rt[0];
            copy[1] = rt[1];
            copy[2] = rt[2];
            copy[3] = rt[3];
            copy[4] = (uint)dword_180211F50;
            copy[5] = *(uint*)unk_180211F54;
            copy[6] = *(uint*)unk_180211F58;
            copy[7] = (uint)dword_180211F5C;
            MulMatrix0((short*)desc, ((short*)&m), (ushort*)((short*)&m));
        }

        long result = (ushort)m.union_16_2;
        *(int*)unk_180211F54 = trX;
        *(int*)unk_180211F58 = trY;
        dword_180211F5C = trZ;
        *(int*)unk_180211FC4 = 0;
        System.Buffer.MemoryCopy(((short*)&m), rt, 16, 16);
        dword_180211F50 = (ushort)m.union_16_2;
        dword_180211FCC = 0;
        dword_180211FD4 = 0;

        if (kind == 1)
        {
            return DrawRingMeshEffectTextured(desc, track, frames, frame, rings, segments, count, step, a6, a7);
        }

        if (kind == 2)
        {
            return DrawRingMeshEffectShaded(desc, track, rings, segments, count, step, a6, a7);
        }

        return result;
    }

    /// <summary>
    /// Per-axis matrix scales: with flag bit 1 the u16 pair of <paramref name="frame"/> from the table at a1 + 8
    /// (GPF by IR0 = a4 with IR3 = 4096 when a4 &gt;= 0, the IR registers left sign-extended) and the track after the
    /// table; otherwise x = y = a4 (4096 when negative), z = 4096 and the track at a1 + 8.
    /// </summary>
    private static void DrawRingMeshEffectScales(byte* a1, int a4, int frames, int frame, out int scaleX, out int scaleY, out int scaleZ, out ushort* track)
    {
        Recompiler.Runtime.Tracer.Enter("DrawRingMeshEffectScales(a1, a4, frames, frame)", (nint)a1, a4, frames, frame);
        scaleZ = 4096;
        byte* table = a1 + 8;
        if ((a1[4] & 2) != 0)
        {
            byte* pair = table + unchecked(4 * frame);
            scaleX = *(ushort*)pair;
            scaleY = *(ushort*)(pair + 2);
            if (a4 >= 0)
            {
                int* ir2 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
                int* ir3 = (int*)word_180211FEC;
                dword_180211FE0 = a4;
                dword_180211FE4 = scaleX;
                *ir2 = scaleY;
                *ir3 = 4096;
                GteGpf();
                int z = *(short*)ir3;
                scaleX = (short)dword_180211FE4;
                scaleY = *(short*)ir2;
                dword_180211FE4 = scaleX;
                *ir2 = scaleY;
                scaleZ = z;
                *ir3 = z;
            }

            track = (ushort*)(table + unchecked(4 * frames));
        }
        else
        {
            scaleX = a4 < 0 ? 4096 : a4;
            scaleY = scaleX;
            track = (ushort*)table;
        }
    }

    /// <summary>
    /// Kind 1: colour of step <paramref name="step"/> (RGB scaled by a7 &gt;&gt; 12 unless a7 &lt; 0, staged in the IR0
    /// global like the original) and the textured ring descriptor over the PSX block after the track's colours:
    /// u16 columns, +4 tpage, +6 clut/uv, +8 uv table of 2 * (rings + 1) * (columns + 2) bytes, then per-frame data.
    /// </summary>
    private static long DrawRingMeshEffectTextured(byte* desc, ushort* track, int frames, int frame, int rings, int segments, int count, int step, int a6, int a7)
    {
        Recompiler.Runtime.Tracer.Enter("DrawRingMeshEffectTextured(desc, track, frames, frame, rings, segments, count, step, a6, a7)", (nint)desc, (nint)track, frames, frame, rings, segments, count, step, a6, a7);
        uint block = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)((byte*)track + unchecked(4 * count) + 4));
        *(uint*)(desc + 20) = block;
        byte* color = (byte*)track + unchecked(4 * step) + 4;
        byte* rgbc = (byte*)Unsafe.AsPointer(ref dword_180211FE0);
        if (a7 >= 0)
        {
            rgbc[0] = (byte)((color[0] * a7) >> 12);
            rgbc[1] = (byte)((color[1] * a7) >> 12);
            rgbc[2] = (byte)((color[2] * a7) >> 12);
        }
        else
        {
            rgbc[0] = (byte)((color[0] << 12) >> 12);
            rgbc[1] = (byte)((color[1] << 12) >> 12);
            rgbc[2] = (byte)((color[2] << 12) >> 12);
        }

        rgbc[3] = color[3];
        *(int*)desc = dword_180211FE0;

        int columns = *(ushort*)ResolveGlobalPsxAddress(block);
        *(int*)(desc + 40) = (*(ushort*)ResolveGlobalPsxAddress(unchecked(block + 4)) | (32 * (a6 & 3))) << 16;
        int clutUv = *(ushort*)ResolveGlobalPsxAddress(unchecked(block + 6)) << 16;
        uint uvTable = unchecked(block + 8);
        *(uint*)(desc + 20) = uvTable;
        *(int*)(desc + 4) = clutUv;
        int uvBytes = 2 * (rings + 1) * (columns + 2);
        uint frameData = unchecked(uvTable + (uint)uvBytes + (uint)(2 * frames));
        *(uint*)(desc + 8) = frameData;
        *(uint*)(desc + 12) = unchecked(frameData + 4);
        *(uint*)(desc + 16) = unchecked(frameData + 4 + (uint)(2 * rings) + 2);
        int uvOffset = *(ushort*)ResolveGlobalPsxAddress(unchecked(uvTable + (uint)uvBytes + (uint)(2 * frame)));
        *(int*)(desc + 32) = uvOffset;

        uint* workTable = (uint*)off_180066C68;
        uint packets = workTable[9];
        byte* packetPtr = ResolveGlobalPsxAddress(packets);
        workTable[9] = unchecked(packets + (uint)(40 * rings * segments));
        *(int*)(desc + 36) = rings;
        *(int*)(desc + 28) = segments;
        desc[3] = (byte)((a6 == 255 ? 0 : 2) | 0x4C);
        *(uint*)(desc + 24) = workTable[10];
        byte* ot = ResolveGlobalPsxAddress(workTable[8]);
        return EmitConcentricRingQuads(columns, (ulong)desc, (ulong)packetPtr, (ulong)ot);
    }

    /// <summary>
    /// Kind 2: gouraud ring descriptor; one or two RGB layers (track[0] != 0) of (rings + 1) entries per step, the
    /// second layer's table 4 bytes further; geometry after <paramref name="count"/> steps; fade = a7 (4096 when
    /// negative).
    /// </summary>
    private static long DrawRingMeshEffectShaded(byte* desc, ushort* track, int rings, int segments, int count, int step, int a6, int a7)
    {
        Recompiler.Runtime.Tracer.Enter("DrawRingMeshEffectShaded(desc, track, rings, segments, count, step, a6, a7)", (nint)desc, (nint)track, rings, segments, count, step, a6, a7);
        int layers = (track[0] != 0 ? 1 : 0) + 1;
        int stride = layers * (rings + 1);
        uint colors = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)((byte*)track + unchecked(4 * (step * stride)) + 4));
        *(uint*)(desc + 32) = colors;
        if (track[0] != 0)
        {
            colors = unchecked(colors + 4);
        }

        *(uint*)(desc + 20) = colors;
        int fade = a7 < 0 ? 4096 : a7;
        uint geometry = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)((byte*)track + unchecked(4 * (count * stride)) + 4));
        *(uint*)(desc + 8) = geometry;
        *(uint*)(desc + 12) = unchecked(geometry + 4);
        *(uint*)(desc + 16) = unchecked(geometry + 4 + (uint)(2 * rings) + 2);

        uint* workTable = (uint*)off_180066C68;
        uint packets = workTable[9];
        byte* packetPtr = ResolveGlobalPsxAddress(packets);
        workTable[9] = unchecked(packets + (uint)(44 * rings * segments));
        *(int*)(desc + 4) = fade;
        *(int*)(desc + 36) = rings;
        *(int*)(desc + 28) = segments;
        *(int*)(desc + 40) = 32 * (a6 & 3);
        *(int*)desc = ((a6 == 255 ? 0 : 2) | 0x38) << 24;
        *(uint*)(desc + 24) = workTable[10];
        byte* ot = ResolveGlobalPsxAddress(workTable[8]);
        return EmitShadedRingQuads(layers, (ulong)desc, (ulong)packetPtr, (ulong)ot);
    }
}
