using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Step <paramref name="a3"/> of moving object <paramref name="a1"/> to the point 400 units along its turned-around
    /// facing: returns 1 at once when the context object <paramref name="a2"/> (255 = 0) has flag +16 set. Otherwise
    /// the point is SfxRotTransPointZ(position, rotation + 2048 yaw, 400); on step 0 a flagged object gets command
    /// 0x0C(0x1D); duration = HostObjCmd09; before the last step the two 0x200000 sends lower the start and target y by
    /// 128; GTE INTPL with IR0 = min((step + 1) * 4096 / duration, 4096) gives the new position (host 0x02). Returns
    /// step &gt;= duration.
    /// </summary>
    public static long SfxObjStepToFacingPoint(int a1, uint a2, int a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjStepToFacingPoint(a1, a2, a3)", a1, a2, a3);
        Stack8_SfxObjStepToFacingPoint_moved start = default;
        Stack8_SfxObjStepToFacingPoint_moved moved = default;
        Span<short> rotation = stackalloc short[4];
        short* rotationAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(rotation));
        Span<short> target = stackalloc short[4];
        short* targetAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(target));

        HostGetObjectPositionById(a1, (long)((short*)&start));
        moved.union_0_4 = start.union_0_4;
        moved.union_4_4 = start.union_4_4;
        if (a2 == 255)
        {
            a2 = 0;
        }

        if (*(byte*)(PsxGetContextQword((int)a2) + 16) != 0)
        {
            return 1;
        }

        long obj = PsxGetContextQword(a1);
        if (BattleCallbackAddress != null)
        {
            HostObjCallback(ObjCommand(obj, HostCommand.GetRotate), 1, 0, 0, 0, (ulong)rotationAddress);
        }

        rotation[1] = unchecked((short)(rotation[1] + 2048));
        SfxRotTransPointZ(((short*)&moved), rotationAddress, 400, (ushort*)targetAddress);
        if (a3 == 0)
        {
            HostObjCmd0CIfFlag((uint)a1, 0x1D);
        }

        int duration = (int)HostObjCmd09(a1);
        short targetY = target[1];
        if (a3 < duration)
        {
            if (HostCallbackSend(PsxGetContextQword(a1), 0x200000) != 0)
            {
                start.union_2_2 = unchecked((short)(start.union_2_2 - 128));
            }

            if (HostCallbackSend(PsxGetContextQword(a1), 0x200000) != 0)
            {
                targetY = unchecked((short)(target[1] - 128));
            }
        }

        SfxObjStepToFacingPointInterpolate(a3, duration, start.union_0_2, start.union_2_2, start.union_4_2, target[0], targetY, target[2]);
        moved.union_0_2 = (short)dword_180212024;
        moved.union_2_2 = (short)dword_180212028;
        moved.union_4_2 = (short)dword_18021202C;
        HostSetObjectPositionById(a1, (long)((short*)&moved));
        return a3 >= duration ? 1 : 0;
    }

    /// <summary>
    /// Turns object <paramref name="a1"/> towards point <paramref name="a2"/> by step a6 of a7: returns when a6 &gt; a7;
    /// t = (a6 &lt;&lt; 12) / (a7 or 1). The current rotation is read from the host (0x03) into a3 when a6 is 0 or a3 is
    /// null (then a local buffer and t = 4096). The goal is the current rotation when the object already stands on
    /// a2's x/z, else LookAnglesBetween(position, a2) with pitch cleared unless a4 bit 0, yaw + 2048 unless bit 1;
    /// roll = a5; SfxTurnTowards(rotation, goal, 4096 - t) and HostSetObjRotation(goal), whose result is returned.
    /// </summary>
    public static long SfxObjTurnTowardsPoint(int a1, short* a2, ushort* a3, sbyte a4, short a5, int a6, int a7)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjTurnTowardsPoint(a1, a2, a3, a4, a5, a6, a7)", a1, (nint)a2, (nint)a3, a4, a5, a6, a7);
        if (a6 > a7)
        {
            return 0;
        }

        int divisor = a7 == 0 ? 1 : a7;
        int t = unchecked(a6 << 12) / divisor;
        short* fallback = stackalloc short[4];
        ushort* rotation = a3;
        if (a6 == 0 || a3 == null)
        {
            if (a3 == null)
            {
                rotation = (ushort*)fallback;
                t = 4096;
            }

            HostGetObjectRotationById(a1, (long)rotation);
        }

        Stack8_SfxObjTurnTowardsPoint_goal goal = default;
        Stack8_SfxObjStepToPoint_position position = default;
        if (a2 != null)
        {
            HostQueryObjPosition(PsxGetContextQword(a1), (long)((short*)&position));
            if (position.field_0 == *(int*)a2 && position.field_4 == a2[2])
            {
                *(uint*)((short*)&goal) = *(uint*)rotation;
                goal.union_4_4 = *(uint*)(rotation + 2);
            }
            else
            {
                LookAnglesBetween(((short*)&position), a2, (ushort*)((short*)&goal));
                if ((a4 & 1) == 0)
                {
                    goal.union_0_2 = 0;
                }

                if ((a4 & 2) == 0)
                {
                    goal.union_2_2 = unchecked((short)(goal.union_2_2 + 2048));
                }
            }
        }
        else
        {
            goal.union_0_4 = 0;
        }

        goal.union_4_2 = a5;
        SfxTurnTowards(rotation, (ushort*)((short*)&goal), 4096 - t);
        return HostSetObjRotation(a1, (long)((short*)&goal));
    }

    /// <summary>Host 0x09 | id (result ignored), then SfxObjTurnTowardsPoint(a1, a2, a3, 2, 0, a4, 3).</summary>
    public static long SfxObjFaceTowards(int a1, short* a2, ushort* a3, int a4)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjFaceTowards(a1, a2, a3, a4)", a1, (nint)a2, (nint)a3, a4);
        HostObjCmd09(a1);
        return SfxObjTurnTowardsPoint(a1, a2, a3, 2, 0, a4, 3);
    }

    /// <summary>
    /// Step <paramref name="a2"/> of moving object <paramref name="a1"/> to its anchor (host 0x01) starting at
    /// <paramref name="a3"/>: on step 0 a3 = current position; returns 0xFFFFFFFF when it already has the anchor's x/z;
    /// a flagged object gets command 0x0C(0x1E) unless a4; it turns to face the anchor (pitch 0, yaw + 2048, roll 0,
    /// immediately). Duration is 24 with a4, else HostObjCmd09; before the last step the two 0x200000 sends lower
    /// a3[1] and the anchor y by 128; INTPL a3 → anchor gives the new position (host 0x02). Returns step &gt;= duration.
    /// </summary>
    public static long SfxObjStepToPoint(int a1, int a2, short* a3, int a4)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjStepToPoint(a1, a2, a3, a4)", a1, a2, (nint)a3, a4);
        Stack8_SfxObjStepToPoint_anchor anchor = default;
        Span<int> rotation = stackalloc int[2];
        short* rotationAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(rotation));
        Stack8_SfxObjStepToPoint_position position = default;
        Stack8_SfxObjStepToFacingPoint_moved goal = default;
        Span<short> moved = stackalloc short[4];
        short* movedAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(moved));

        HostGetObjectPositionById(a1, (long)((short*)&anchor));
        if (a2 == 0)
        {
            HostQueryObjPosition(PsxGetContextQword(a1), (long)a3);
            if (a3[0] == anchor.union_0_2 && a3[2] == anchor.field_4)
            {
                return 0xFFFFFFFFL;
            }

            if (a4 == 0)
            {
                HostObjCmd0CIfFlag((uint)a1, 0x1E);
            }

            HostGetObjectRotationById(a1, (long)rotationAddress);
            HostQueryObjPosition(PsxGetContextQword(a1), (long)((short*)&position));
            if (position.field_0 == anchor.union_0_4 && position.field_4 == anchor.field_4)
            {
                goal.union_0_4 = rotation[0];
                goal.union_4_4 = rotation[1];
            }
            else
            {
                LookAnglesBetween(((short*)&position), ((short*)&anchor), (ushort*)((short*)&goal));
                goal.union_0_2 = 0;
                goal.union_2_2 = unchecked((short)(goal.union_2_2 + 2048));
            }

            goal.union_4_2 = 0;
            SfxTurnTowards((ushort*)rotationAddress, (ushort*)((short*)&goal), 0);
            HostSetObjRotation(a1, (long)((short*)&goal));
        }

        int duration = a4 != 0 ? 24 : (int)HostObjCmd09(a1);
        if (a2 < duration)
        {
            if (HostCallbackSend(PsxGetContextQword(a1), 0x200000) != 0)
            {
                a3[1] = unchecked((short)(a3[1] - 128));
            }

            if (HostCallbackSend(PsxGetContextQword(a1), 0x200000) != 0)
            {
                anchor.union_2_2 = unchecked((short)(anchor.union_2_2 - 128));
            }
        }

        SfxObjStepToFacingPointInterpolate(a2, duration, a3[0], a3[1], a3[2], anchor.union_0_2, anchor.union_2_2, anchor.field_4);
        moved[0] = (short)dword_180212024;
        moved[1] = (short)dword_180212028;
        moved[2] = (short)dword_18021202C;
        HostSetObjectPositionById(a1, (long)movedAddress);
        return a2 >= duration ? 1 : 0;
    }

    /// <summary>Script op: on phase 1 stores a5[0] and a5[4] into bytes 0 and 1 of 0x180620B84. Returns 1.</summary>
    public static long SfxOpSetParamBytes01(int a1, long a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpSetParamBytes01(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        if (a1 == 1)
        {
            *(byte*)Unsafe.AsPointer(ref dword_180620B84) = a5[0];
            *unk_180620B85 = a5[4];
        }

        return 1;
    }

    /// <summary>Script op: on phase 1 stores a5[0] and a5[4] into bytes 2 and 3 of 0x180620B84. Returns 1.</summary>
    public static long SfxOpSetParamBytes23(int a1, long a2, long a3, long a4, byte* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpSetParamBytes23(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        if (a1 == 1)
        {
            *unk_180620B86 = a5[0];
            *unk_180620B87 = a5[4];
        }

        return 1;
    }

    /// <summary>Script op: on phase 1 sends host 0x7D000000 (60 / (int)*a5) when a callback exists. Returns 1.</summary>
    public static long SfxOpSetHostFrameRate(int a1, long a2, long a3, long a4, uint* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxOpSetHostFrameRate(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        if (a1 == 1 && BattleCallbackAddress != null)
        {
            HostObjCallback(HostCommand.SetFps, (uint)(60 / (int)*a5), 0, 0, 0, 0);
        }

        return 1;
    }

    /// <summary>
    /// Three times: takes the first free of the six 8-byte voice slots at 0x1803231A4 (u16 mask, s16 age, u32 sound),
    /// else the one with the greatest age (&gt; -1); the slot gets mask 1 &lt;&lt; index, sound a2 and age 0, and the host is
    /// told 0x20000000 (1, a1, mask, SfxSourcePan(a2) &amp; 0xFF, 0). Returns the last callback result (0 without one).
    /// </summary>
    public static short* SfxAllocVoices(uint a1, uint a2)
    {
        Recompiler.Runtime.Tracer.Enter("SfxAllocVoices(a1, a2)", a1, a2);
        short* slots = (short*)Unsafe.AsPointer(ref word_1803231A4);
        long result = 0;
        for (int pass = 3; pass != 0; pass--)
        {
            int bestAge = -1;
            short* bestSlot = null;
            int bestMask = 0;
            short* slot = slots;
            int index = 0;
            int mask = 1;
            bool free = false;
            while (true)
            {
                if (*slot == 0)
                {
                    free = true;
                    break;
                }

                if (bestAge < slot[1])
                {
                    bestAge = slot[1];
                    bestSlot = slot;
                    bestMask = mask;
                }

                index++;
                slot += 4;
                mask += mask;
                if (index >= 6)
                {
                    break;
                }
            }

            if (!free)
            {
                mask = bestMask;
                slot = bestSlot;
            }

            if (slot != null)
            {
                *slot = (short)mask;
                *(uint*)(slot + 2) = a2;
                slot[1] = 0;
            }

            if (mask != 0)
            {
                uint pan = (byte)SfxSourcePan(a2);
                result = (long)BattleCallbackAddress;
                if (result != 0)
                {
                    result = HostObjCallback(HostCommand.SfxPlay, 1, a1, (uint)mask & 0xFFFFFF, pan, 0);
                }
            }
            else
            {
                result = (long)slot;
            }
        }

        return (short*)result;
    }

    /// <summary>
    /// Object a1 of the list at +152 of the block at PSX address off_180066C68[3]: returns 1 when a1 &gt;= its count
    /// (s8 at +39); else its anchor position goes to a2 (HostQueryObjAnchorPos mode 0) and its rotation to a3 (host
    /// 0x03, when a callback exists). Returns 0.
    /// </summary>
    public static long SfxQueryListObjPose(int a1, long a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxQueryListObjPose(a1, a2, a3)", a1, a2, a3);
        uint address = ((uint*)off_180066C68)[3];
        long obj = *(long*)(ResolveGlobalPsxAddress(address) + 8L * a1 + 152);
        if (a1 >= (sbyte)ResolveGlobalPsxAddress(address)[39])
        {
            return 1;
        }

        HostQueryObjAnchorPos(obj, 0, a2);
        if (BattleCallbackAddress != null)
        {
            HostObjCallback(ObjCommand(obj, HostCommand.GetRotate), 1, 0, 0, 0, (ulong)a3);
        }

        return 0;
    }

    /// <summary>
    /// When 0x180323268 is set and the block at off_180066C68[3] has objects (+39), starts the track block at
    /// qword_1803231E0 + offset (+36 when context object 8 has flag +16, else +68) with SfxStartTrackBlock(.., 0).
    /// </summary>
    public static void SfxStartIdleTrack()
    {
        Recompiler.Runtime.Tracer.Enter("SfxStartIdleTrack()");
        if (dword_180323268 == 0)
        {
            return;
        }

        if (ResolveGlobalPsxAddress(((uint*)off_180066C68)[3])[39] == 0)
        {
            return;
        }

        bool flagged = *(byte*)(PsxGetContextQword(8) + 16) != 0;
        long table = qword_1803231E0;
        Table2* table2 = (Table2*)table;
        if (flagged)
        {
            SfxStartTrackBlock(table + table2->field_36, 0);
        }
        else
        {
            SfxStartTrackBlock(table + table2->field_68, 0);
        }
    }

    /// <summary>
    /// GTE INTPL from (x0, y0, z0) to (x1, y1, z1) with IR0 = min((step + 1) * 4096 / duration, 4096) (idiv, signed);
    /// every register gets the sign-extended 16-bit value as a full dword.
    /// </summary>
    private static void SfxObjStepToFacingPointInterpolate(int step, int duration, short x0, short y0, short z0, short x1, short y1, short z1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjStepToFacingPointInterpolate(step, duration, x0, y0, z0, x1, y1, z1)", step, duration, x0, y0, z0, x1, y1, z1);
        int t = unchecked((step + 1) << 12) / duration;
        if (t > 4096)
        {
            t = 4096;
        }

        dword_180211FE0 = t;
        dword_180211F9C = z1;
        dword_180211FE4 = x0;
        *(int*)Unsafe.AsPointer(ref qword_180211FE8) = y0;
        *(int*)unk_180211FEC = z0;
        *(int*)Unsafe.AsPointer(ref qword_180211F94) = x1;
        *(int*)unk_180211F98 = y1;
        GteIntpl();
    }
}
