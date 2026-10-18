namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Resets the 32 effect-model slots at 0x180220230 (id = index, in-use flag and the four pointers cleared), then
    /// gives the first a2 slots the model records a1, a1 + 200, ... (no bound check against 32). Returns the slot
    /// after the last one assigned (the table itself when a2 &lt;= 0).
    /// </summary>
    public static ulong* InitEffModelSlots(long a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("InitEffModelSlots(a1, a2)", a1, a2);
        var slots = (EffModelSlot*)unk_180220230;
        for (int i = 0; i < 32; i++)
        {
            EffModelSlot* slot = slots + i;
            slot->id = (ushort)i;
            slot->inUse = 0;
            slot->model = 0;
            slot->field_8 = 0;
            slot->field_16 = 0;
            slot->field_24 = 0;
        }

        var result = (ulong*)slots;
        if ((int)a2 > 0)
        {
            for (uint n = (uint)a2; n != 0; n--)
            {
                *result = (ulong)a1;
                a1 += 200;
                result += 6;
            }
        }

        return result;
    }
}
