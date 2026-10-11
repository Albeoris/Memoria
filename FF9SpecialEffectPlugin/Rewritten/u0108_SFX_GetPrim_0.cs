namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Returns the next primitive of the ordering table being walked (qword_180171DB8; header dword: low 24 bits =
    /// PSX address of the next entry, byte 3 = length in words) and stores 16 * OT index (dword_18006972C) into *a1.
    /// Links are OR-ed with dword_180069728 and resolved by the inlined PsxAddrToPtr over the module context
    /// unk_180576A10. Entries with length 0 are ordering-table links and are skipped while decrementing the index;
    /// 0xFFFFFF or an unresolvable link ends the walk.
    /// </summary>
    public static P_TAG* SFX_GetPrim_0(int* otz)
    {
        Recompiler.Runtime.Tracer.Enter("SFX_GetPrim_0(otz)", (nint)otz);
        uint* next = (uint*)qword_180171DB8;
        if (next == null)
        {
            return null;
        }

        int index = dword_18006972C;
        uint* current;
        while (true)
        {
            current = next;
            uint link = *next & 0xFFFFFF;
            if (link == 0xFFFFFF)
            {
                next = null;
            }
            else
            {
                uint address = (uint)dword_180069728 | link;
                next = (uint*)PsxAddrToPtr((long)unk_180576A10, address);
            }

            bool isLink = ((byte*)current)[3] == 0;
            qword_180171DB8 = (long)next;
            if (!isLink)
            {
                break;
            }

            dword_18006972C = --index;
            if (next == null)
            {
                return null;
            }
        }

        *otz = 16 * index;
        return (P_TAG*)current;
    }
}
