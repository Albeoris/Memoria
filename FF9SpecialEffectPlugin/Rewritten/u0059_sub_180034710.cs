using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Moves object a2 to the position a3 (3 shorts): every active attachment in list a1 owned by a2 is shifted by the
    /// delta (in 20.12 fixed point), then the object's position (PSX address at a2+40) is overwritten. Returns (ushort)a3[2].
    /// </summary>
    public static long MoveObjectWithAttachments(long a1, long a2, short* a3)
    {
        Recompiler.Runtime.Tracer.Enter("MoveObjectWithAttachments(a1, a2, a3)", a1, a2, (nint)a3);
        short* position = (short*)PsxAddrToPtr((long)unk_180576A10, *(uint*)(a2 + 40));
        Position* position2 = (Position*)position;
        int dx = (a3[0] - position[0]) << 12;
        SfxAttachmentList* list = (SfxAttachmentList*)a1;
        SfxAttachment* entry = list->Entries;
        int dy = (a3[1] - position[1]) << 12;
        int dz = (a3[2] - position[2]) << 12;
        if (list->Count > 0)
        {
            int index = 0;
            do
            {
                if (entry->Active != 0 && entry->Owner == a2)
                {
                    entry->X += dx;
                    entry->Y += dy;
                    entry->Z += dz;
                }

                index++;
                entry++;
            }
            while ((uint)index < (uint)list->Count);
        }

        position2->union_0_4 = *(uint*)a3;
        long result = (ushort)a3[2];
        position[2] = (short)result;
        return result;
    }

    /// <summary>Moves object a1 to a2 within the global attachment list (qword_180211E68); a null object does nothing.</summary>
    public static long MoveSfxObject(long a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("MoveSfxObject(a1, a2)", a1, a2);
        if (a1 != 0)
        {
            return MoveObjectWithAttachments((long)Unsafe.AsPointer(ref qword_180211E68), a1, (short*)a2);
        }

        // The original returns whatever rax held (undefined); see the divergence recorded for sub_180030710.
        return 0;
    }

    /// <summary>
    /// Detaches object a1: every active entry of the global attachment list owned by a1 gets State = DetachedState,
    /// then the object's ushort at +48 is set to 0xFFFF. A null object does nothing.
    /// </summary>
    public static void DetachSfxObject(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("DetachSfxObject(a1)", a1);
        if (a1 == 0)
        {
            return;
        }

        SfxAttachmentList* list = (SfxAttachmentList*)Unsafe.AsPointer(ref qword_180211E68);
        int count = list->Count;
        SfxAttachment* entry = (SfxAttachment*)qword_180211E78;
        for (int index = 0; index < count; index++, entry++)
        {
            if (entry->Active != 0 && entry->Owner == a1)
            {
                entry->State = entry->DetachedState;
                count = list->Count;
            }
        }

        *(ushort*)(a1 + 48) = 0xFFFF;
    }

    /// <summary>Bytes of a bump arena holding a2 blocks of a1 bytes: 16-byte header + a2 * (align4(a1) + 4), 32-bit product.</summary>
    public static long ComputeArenaBytes(int a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("ComputeArenaBytes(a1, a2)", a1, a2);
        uint aligned = (uint)a1 & 0xFFFFFFFC;
        if ((a1 & 3) != 0)
        {
            aligned += 4;
        }

        return (uint)(a2 * (aligned + 4)) + 16L;
    }

    /// <summary>
    /// Clears the first dword of every slot of the PSX pool at address a1: stride (short) at +0, slot count (short)
    /// at +2, slots from +16.
    /// </summary>
    public static void ClearPoolSlotHeaders(uint a1)
    {
        Recompiler.Runtime.Tracer.Enter("ClearPoolSlotHeaders(a1)", a1);
        byte* pool = (byte*)PsxAddrToPtr((long)unk_180576A10, a1);
        Pool* pool2 = (Pool*)pool;
        byte* slot = pool + 16;
        for (int i = 0; i < pool2->field_2; i++)
        {
            *(uint*)slot = 0;
            slot += pool2->field_0;
        }
    }
}
