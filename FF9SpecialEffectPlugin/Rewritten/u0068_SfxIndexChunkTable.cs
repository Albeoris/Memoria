namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Walks the chunk directory at the start of the set's data: int16 group count, then per group an int16 pair
    /// (unused, entry count) and per entry {byte type, byte flag, int16 size in 2048-byte sectors}. Data starts at
    /// sector 1. Type 2 skips its size (plus one more int16 size when the flag is set); type 3 records the chunk in the
    /// group's 96-byte slot: host pointer, byte size, PSX base 0x801F0000 / 0x801F5000 (alternating by group) and 16
    /// offsets relocated against that base. Returns the last value the original left in its result register.
    /// </summary>
    public static long SfxIndexChunkTable(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxIndexChunkTable(a1)", a1);
        var set = (SfxChunkSet*)a1;
        short* cursor = set->directory;
        uint result = (uint)*cursor;
        cursor++;
        int offset = 2048;
        set->slotCount = result;
        int group = 0;
        if ((int)result > 0)
        {
            SfxChunkSlot* slot = &set->firstSlot;
            do
            {
                result = (uint)cursor[1];
                cursor += 2;
                if ((int)result > 0)
                {
                    long remaining = result;
                    do
                    {
                        result = (uint)(cursor[1] << 11);
                        byte type = *(byte*)cursor;
                        if (type == 2)
                        {
                            sbyte extended = *((sbyte*)cursor + 1);
                            offset += (int)result;
                            cursor += 2;
                            if (extended != 0)
                            {
                                result = (uint)(*cursor << 11);
                                offset += (int)result;
                                ++cursor;
                            }
                        }
                        else if (type == 3)
                        {
                            long start = offset;
                            offset += (int)result;
                            cursor += 2;
                            uint* data = (uint*)((long)set->directory + start);
                            slot->size = result;
                            slot->data = data;
                            int psxBase = 20480 * (group & 1) - 2145487104;
                            slot->psxBase = (uint)psxBase;
                            uint relativeBase = (uint)(psxBase & 0xFFFFFFF);
                            ulong first = (*data & 0xFFFFFFFu) - relativeBase;
                            slot->firstOffset = (uint)first;
                            uint* source = &data[(first >> 2) + 2];
                            uint* target = (uint*)((uint*)&slot->offsets);
                            uint count = 0;
                            for (int i = 0; i < 16; i++)
                            {
                                result = *source;
                                ++count;
                                if (result != 0)
                                {
                                    result = (result & 0xFFFFFFF) - relativeBase;
                                    *target = result;
                                }
                                else
                                {
                                    *target = 0;
                                }

                                ++target;
                                ++source;
                            }

                            slot->offsetCount = count;
                        }
                        else
                        {
                            offset += (int)result;
                            cursor += 2;
                        }

                        --remaining;
                    }
                    while (remaining != 0);
                }

                ++group;
                ++slot;
            }
            while ((uint)group < set->slotCount);
        }

        return result;
    }
}
