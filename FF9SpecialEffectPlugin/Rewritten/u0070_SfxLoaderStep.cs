using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>PSX address of the 0x1802208D0 mirror: script entry addresses are relative to it.</summary>
    private const long SfxLoaderStepMirrorBase = 0x801E7700;

    /// <summary>Host callback message that uploads a RECT of pixels.</summary>
    private const ulong SfxLoaderStepUpload = 0x64000000;

    /// <summary>Record types after which the loader waits for the next call (bits 0, 1, 4, 9 = 0x213).</summary>
    private const int SfxLoaderStepWaitTypes = 0x213;

    /// <summary>
    /// One step of the SFX data loader state machine (state = low word of dword_1803231FC): 1 starts the script
    /// context, 2 maps the record list at unk_1803208D2, 3 reads the next block header (type count, records), 4 copies
    /// the next record's sectors from unk_1803678F0 to its destination, 5 processes the record by type, 6 resumes
    /// at 4, 7 parks at 6. Returns the 16-bit value left in ax by the path taken.
    /// </summary>
    public static short SfxLoaderStep()
    {
        Recompiler.Runtime.Tracer.Enter("SfxLoaderStep()");
        int state = (ushort)dword_1803231FC;
        if (state == 0)
        {
            return 0;
        }

        while (true)
        {
            int index = (short)state - 1;
            switch (index)
            {
                case 0:
                    InitSfxScriptContext((uint)dword_1803231F4, 4, dword_1803231F0);
                    ++dword_1803231F0;
                    return SfxLoaderStepSetState(2);
                case 1:
                    word_180323208 = word_1803208D0;
                    dword_18032320C = (int)PsxAddrMapperRegister((long)unk_180576A10, (ulong)unk_1803208D2);
                    SfxLoaderStepSetState(3);
                    state = 3;
                    break;
                case 2:
                    if ((ushort)word_18032320A >= (ushort)word_180323208)
                    {
                        SfxLoaderStepSetState(0);
                        return word_180323208;
                    }

                    short* block = (short*)ResolveGlobalPsxAddress((uint)dword_18032320C);
                    word_180323200 = block[0];
                    dword_18032320C += 4;
                    word_180323202 = block[1];
                    SfxLoaderStepSetState(4);
                    state = 4;
                    break;
                case 3:
                    return SfxLoaderStepLoadRecord();
                case 4:
                    if (!SfxLoaderStepProcessRecord(out short result))
                    {
                        return result;
                    }

                    state = 4;
                    break;
                case 5:
                    SfxLoaderStepSetState(4);
                    state = 4;
                    break;
                case 6:
                    *unk_180323257 = 0;
                    return SfxLoaderStepSetState(6);
                default:
                    return (short)index;
            }
        }
    }

    private static short SfxLoaderStepSetState(short state)
    {
        Recompiler.Runtime.Tracer.Enter("SfxLoaderStepSetState(state)", state);
        *(short*)Unsafe.AsPointer(ref dword_1803231FC) = state;
        return state;
    }

    /// <summary>Record buffer: unk_1802258D0 for a block of kind 1 with dword_1803231F8 != 0, else unk_1802208D0.</summary>
    private static byte* SfxLoaderStepBuffer()
    {
        Recompiler.Runtime.Tracer.Enter("SfxLoaderStepBuffer()");
        return word_180323200 == 1 && dword_1803231F8 != 0 ? unk_1802258D0 : unk_1802208D0;
    }

    /// <summary>The configuration block at PSX address *(uint*)(off_180066C68 + 12); bit 3 of +64 selects the host buffer.</summary>
    private static bool SfxLoaderStepHostBuffer()
    {
        Recompiler.Runtime.Tracer.Enter("SfxLoaderStepHostBuffer()");
        return (ResolveGlobalPsxAddress((uint)((Table*)off_180066C68)->field_12)[64] & 8) != 0;
    }

    /// <summary>
    /// State 4: reads the record header at dword_18032320C (type, flags, u16 sector count, then data), picks the
    /// destination by type, maps the next header and copies the sectors from the read buffer.
    /// </summary>
    private static short SfxLoaderStepLoadRecord()
    {
        Recompiler.Runtime.Tracer.Enter("SfxLoaderStepLoadRecord()");
        byte* header = ResolveGlobalPsxAddress((uint)dword_18032320C);
        Block2* block2 = (Block2*)header;
        byte type = header[0];
        byte_180323206 = type;
        byte flags = header[1];
        byte_180323207 = flags;
        if (SfxLoaderStepHostBuffer() && (type == 4 || type == 9))
        {
            HostCountPendingObjects();
            flags = byte_180323207;
            type = byte_180323206;
        }

        byte* destination = SfxLoaderStepBuffer();
        ushort* next = (ushort*)(header + 4);
        uint bytes = (uint)(ushort)block2->field_2 << 11;
        switch (type)
        {
            case 1:
                destination = ResolveGlobalPsxAddress((uint)dword_18032323C);
                break;
            case 2:
                destination = SfxLoaderStepRing(destination, flags, ref next, ref bytes);
                break;
            case 3:
                *(ushort*)unk_1803231FE = 2;
                break;
            case 4:
                destination = (byte*)qword_180069720;
                *(ushort*)Unsafe.AsPointer(ref dword_180323240) = 1;
                dword_180323244 = (int)PsxAddrMapperRegister((long)unk_180576A10, (ulong)qword_180069720);
                *(uint*)(ResolveGlobalPsxAddress((uint)dword_180323244) + 76) = 0;
                for (int offset = 80; offset < 144; offset += 4)
                {
                    *(uint*)(ResolveGlobalPsxAddress((uint)dword_180323244) + offset) = 0;
                }

                break;
            case 5:
                byte* model = ResolveGlobalPsxAddress((uint)dword_180323244);
                destination = model + ((Block3*)model)->field_0;
                break;
            case 6:
                *unk_180323242 = 1;
                byte* summon = ResolveGlobalPsxAddress((uint)dword_180323244);
                uint area = ((Block3*)summon)->field_68;
                dword_180323248 = (int)area;
                dword_18032324C = ((Block3*)summon)->field_72;
                byte* target = ResolveGlobalPsxAddress(area);
                ((Block3*)summon)->field_80 = area;
                dword_18032324C -= (int)bytes;
                dword_180323248 += (int)bytes;
                destination = target;
                break;
            case 7:
                *(byte*)Unsafe.AsPointer(ref word_180323254) = 1;
                destination = unk_180171DC0;
                break;
            case 8:
                *unk_180323258 = 1;
                destination = unk_180171DC0;
                break;
            case 9:
                destination = SfxLoaderStepHostBuffer() ? (byte*)qword_180069720 : unk_180171DC0;
                break;
            case 10:
                uint stream = (uint)dword_180323218;
                destination = ResolveGlobalPsxAddress(stream);
                dword_180323218 = (int)(stream + bytes);
                break;
        }

        dword_18032320C = (int)PsxAddrMapperRegister((long)unk_180576A10, (ulong)next);
        uint sectors = bytes >> 11;
        long size = (int)(sectors << 11);
        byte* source = unk_1803678F0 + (int)((dword_1803231F0 - dword_1803678EC) << 11);
        System.Buffer.MemoryCopy(source, destination, size, size);
        dword_1803231F0 += (int)sectors;
        return SfxLoaderStepSetState(5);
    }

    /// <summary>
    /// Type 2: the ring buffer at dword_18022A8D0. With flags the record restarts it (u16 sector offset follows the
    /// header); the write cursor dword_180323218 advances and wraps to dword_180323210 past dword_180323214 bytes.
    /// </summary>
    private static byte* SfxLoaderStepRing(byte* destination, byte flags, ref ushort* next, ref uint bytes)
    {
        Recompiler.Runtime.Tracer.Enter("SfxLoaderStepRing(destination, flags, next, bytes)", (nint)destination, flags, (nint)next, bytes);
        uint first = bytes;
        *(ushort*)unk_1803231FE = 1;
        int* starts = (int*)Unsafe.AsPointer(ref dword_180323220);
        if (flags != 0)
        {
            destination = (byte*)Unsafe.AsPointer(ref dword_18022A8D0);
            uint skip = (uint)*next << 11;
            next++;
            bytes += skip;
            int start = (int)PsxAddrMapperRegister((long)unk_180576A10, (ulong)(destination + skip));
            dword_180323218 = start;
            dword_180323210 = start;
            dword_180323214 = (int)(0xC800 - skip);
            starts[dword_1803231F8] = start;
        }
        else
        {
            starts[dword_1803231F8] = dword_180323218;
            destination = ResolveGlobalPsxAddress((uint)dword_180323218);
        }

        dword_180323218 = (int)((uint)dword_180323218 + first);
        byte* cursor = ResolveGlobalPsxAddress((uint)dword_180323218);
        byte* ring = ResolveGlobalPsxAddress((uint)dword_180323210);
        if (cursor >= ring + (uint)dword_180323214)
        {
            dword_180323218 = dword_180323210;
        }

        return destination;
    }

    /// <summary>
    /// State 5: handles the loaded record by type, then counts it off the block. Returns false with the value of ax
    /// when the call ends here; true to continue at state 4 in the same call.
    /// </summary>
    private static bool SfxLoaderStepProcessRecord(out short result)
    {
        Recompiler.Runtime.Tracer.Enter("SfxLoaderStepProcessRecord()");
        if (byte_180323206 == 9 && (byte_180323207 & 0xF) != 0 && *unk_180323195 == 0)
        {
            result = 4;
            return false;
        }

        ulong rect = 0;
        switch (byte_180323206)
        {
            case 0:
                SfxLoaderStepImages((short*)&rect);
                break;
            case 1:
                SfxUploadTexturePages((short*)&rect, PsxAddrToPtr((long)unk_180576A10, (uint)dword_18032323C));
                break;
            case 2:
                SfxLoaderStepArena();
                break;
            case 3:
                SfxLoaderStepScript();
                break;
            case 4:
                SfxLoaderStepModelTextures((short*)&rect);
                break;
            case 5:
                SfxLoaderStepRegisterModel();
                break;
            case 6:
                *unk_180323242 = 2;
                break;
            case 7:
                bool first = *unk_180323255 == 0;
                *(byte*)Unsafe.AsPointer(ref word_180323254) = 2;
                *(byte*)Unsafe.AsPointer(ref dword_180323256) = 1;
                if (first)
                {
                    *(ushort*)Unsafe.AsPointer(ref dword_180323256) = 0x101;
                }

                break;
            case 8:
                *unk_180323258 = 2;
                break;
            case 9:
                SfxLoaderStepVramPages((short*)&rect);
                break;
        }

        short remaining = (short)(word_180323202 - 1);
        word_180323202 = remaining;
        if (remaining <= 0)
        {
            SfxLoaderStepSetState(0);
            result = remaining;
            return false;
        }

        byte type = byte_180323206;
        if (type == 7)
        {
            short state = *unk_180323255 != 0 ? (short)7 : (short)6;
            ++*unk_180323255;
            result = SfxLoaderStepSetState(state);
            return false;
        }

        bool wait = type <= 9 && ((SfxLoaderStepWaitTypes >> type) & 1) != 0;
        result = SfxLoaderStepSetState(4);
        return !wait;
    }

    /// <summary>Calls the host callback with a RECT upload (coordinates sign-extended from 16 bits).</summary>
    private static void SfxLoaderStepUploadRect(short x, short y, short w, short h, byte* pixels)
    {
        Recompiler.Runtime.Tracer.Enter("SfxLoaderStepUploadRect(x, y, w, h, pixels)", x, y, w, h, (nint)pixels);
        HostObjCallback(SfxLoaderStepUpload, (uint)(int)x, (uint)(int)y, (uint)(int)w, (uint)(int)h, (ulong)pixels);
    }

    /// <summary>
    /// Type 0: image list in the record buffer (+4 offset of the 8-byte RECT headers, +8 count, +0 offset of the
    /// texture table), each RECT uploaded with its pixels; then the texture pages from the table.
    /// </summary>
    private static void SfxLoaderStepImages(short* rect)
    {
        Recompiler.Runtime.Tracer.Enter("SfxLoaderStepImages(rect)", (nint)rect);
        byte* buffer = SfxLoaderStepBuffer();
        ushort* image = (ushort*)(buffer + *(int*)(buffer + 4));
        int count = *(int*)(buffer + 8);
        long tableOffset = *(int*)buffer;
        for (int i = 0; i < count; i++)
        {
            ushort w = image[2];
            ushort h = image[3];
            rect[0] = (short)image[0];
            rect[1] = (short)image[1];
            rect[2] = (short)w;
            rect[3] = (short)h;
            int pixels = unchecked(h * w);
            image += 4;
            if (w != 0 && h != 0 && BattleCallbackAddress != null)
            {
                SfxLoaderStepUploadRect(rect[0], rect[1], (short)w, (short)h, (byte*)image);
            }

            image += pixels;
        }

        byte* table = buffer + *(int*)(buffer + tableOffset);
        dword_18032323C = (int)PsxAddrMapperRegister((long)unk_180576A10, (ulong)table);
        dword_180323234 = (int)PsxAddrMapperRegister((long)unk_180576A10, (ulong)(buffer + 8 + tableOffset));
        word_180323238 = 0;
        SfxUploadTexturePages(rect, (long)table);
    }

    /// <summary>Type 2: maps the optional table entry (0, 64) and the arena of table dword_1803231F8 (entries 0..1).</summary>
    private static void SfxLoaderStepArena()
    {
        Recompiler.Runtime.Tracer.Enter("SfxLoaderStepArena()");
        uint ring = (uint)Unsafe.AsPointer(ref dword_18022A8D0);
        if (byte_180323207 != 0 && (uint)PsxGetTableEntry(0, 65) - ring != (uint)dword_18022A8D0)
        {
            *(int*)Unsafe.AsPointer(ref qword_180323228) =
                (int)PsxAddrMapperRegister((long)unk_180576A10, (ulong)PsxGetTableEntry(0, 64));
        }

        int table = dword_1803231F8;
        uint* arenas = (uint*)unk_18032322C;
        int size = (int)((uint)PsxGetTableEntry(table, 1) - (uint)PsxGetTableEntry(table, 0));
        if (size != 0)
        {
            byte* arena = PsxGetTableEntry(table, 0);
            arenas[dword_1803231F8] = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)arena);
            SfxArenaInit((long)arena, size);
        }
        else
        {
            arenas[table] = 0;
        }
    }

    /// <summary>
    /// Type 3: script entry of the record buffer (PSX address relative to the 0x801E7700 mirror): stores the entry
    /// data pointer, calls its 32-bit init pointer when set (asserting the arena bounds), then decodes the MIPS code
    /// of script slot word_18032320A into the double-buffered decoder context.
    /// </summary>
    private static void SfxLoaderStepScript()
    {
        Recompiler.Runtime.Tracer.Enter("SfxLoaderStepScript()");
        byte* buffer = SfxLoaderStepBuffer();
        long table = dword_1803231F8;
        long offset = (long)*(uint*)buffer - SfxLoaderStepMirrorBase;
        byte* mirror = unk_1802208D0;
        uint init = *(uint*)(mirror + offset + 4);
        ((long*)Unsafe.AsPointer(ref qword_1803233E0))[table] = (long)(mirror + 8 + offset);
        if (init != 0)
        {
            uint needed = *(uint*)(mirror + offset);
            if (needed != 0)
            {
                if ((uint)qword_180323228 == 0)
                {
                    PsxCompatibilityAssert();
                }

                uint ring = (uint)Unsafe.AsPointer(ref dword_18022A8D0);
                if (needed > (uint)PsxGetTableEntry(0, 65) - ring - (uint)dword_18022A8D0)
                {
                    PsxCompatibilityAssert();
                }
            }

            var entry = (delegate* unmanaged<uint, nint, uint, long, void>)(nint)init;
            entry((uint)qword_180323228, off_180066C68, *(uint*)(off_180066C68 + 16), -2149480192L);
        }

        ushort slot = (ushort)word_18032320A;
        long decoder = qword_1803678E0;
        PsxCallContext* psxCallContext = (PsxCallContext*)decoder;
        long script = decoder + 24 + slot * 96L;
        int bank = slot & 1;
        ((ulong*)&psxCallContext->field_DA8)[bank] = (ulong)script;
        uint length = *(uint*)(script + 88);
        uint address = *(uint*)script & 0xFFFFFFF;
        long context = decoder + 3096 + ((long)bank << 6);
        *(long*)(context + 8) = *(long*)(script + 8);
        *(uint*)context = address;
        *(uint*)(context + 4) = length;
        MipsDecodeAll(context);
        *(ushort*)unk_1803231FE = 3;
        ((ushort*)Unsafe.AsPointer(ref dword_18032321C))[dword_1803231F8] = (ushort)word_18032320A;
    }

    /// <summary>
    /// Type 4: model texture block at dword_180323244: the 256-wide CLUT strip to (256, 230), then one 64x128 page per
    /// s16 count at +4 (x/y from the u16 pairs at +24/+48); each page's RECT x/y word pair is kept twice in
    /// dword_180620B90.
    /// </summary>
    private static void SfxLoaderStepModelTextures(short* rect)
    {
        Recompiler.Runtime.Tracer.Enter("SfxLoaderStepModelTextures(rect)", (nint)rect);
        byte* block = ResolveGlobalPsxAddress((uint)dword_180323244);
        Block3* block3 = (Block3*)block;
        *(uint*)rect = 0x00E60100;
        short h = block3->field_6;
        byte* pixels = block + block3->field_0;
        rect[2] = 256;
        rect[3] = h;
        if (h != 0 && BattleCallbackAddress != null)
        {
            SfxLoaderStepUploadRect(256, 230, 256, h, pixels + block3->field_8);
        }

        long count = block3->field_4;
        if (count <= 0)
        {
            return;
        }

        *(uint*)(rect + 2) = 0x00800040;
        int* pages = (int*)Unsafe.AsPointer(ref dword_180620B90);
        ushort* entry = (ushort*)(block + 48);
        for (long i = 0; i < count; ++entry)
        {
            ushort attributes = *(ushort*)((byte*)entry - 24);
            short y = (short)(((attributes & 0x10) << 4) + *entry);
            short x = (short)((attributes & 0xF) << 6);
            rect[1] = y;
            rect[0] = x;
            if (BattleCallbackAddress != null)
            {
                SfxLoaderStepUploadRect(x, y, 64, 128, pixels);
            }

            int position = *(int*)rect;
            ++i;
            pixels += 0x4000;
            pages[2 * i - 1] = position;
            pages[2 * i - 2] = position;
        }
    }

    /// <summary>
    /// Type 5: relocates the summon model header at dword_180323244 (mapped addresses at +60/+64/+68, the s16 count
    /// at +2 of offsets at +384 rebased), sets the model area dword_180323248/4C, inits the model table at +144 and
    /// registers the summon model (texture positions only when +384 and +64 differ).
    /// </summary>
    private static void SfxLoaderStepRegisterModel()
    {
        Recompiler.Runtime.Tracer.Enter("SfxLoaderStepRegisterModel()");
        long ctx = (long)unk_180576A10;
        byte* header = ResolveGlobalPsxAddress((uint)dword_180323244);
        Block3* block3 = (Block3*)header;
        block3->field_60 = (uint)PsxAddrMapperRegister(ctx, (ulong)(header + block3->field_0));
        block3->field_64 = (uint)PsxAddrMapperRegister(ctx, (ulong)(header + block3->field_20));
        uint* offsets = (uint*)(header + 384);
        for (int i = 0; i < block3->field_2; i++)
        {
            offsets[i] += (uint)PsxPtrToAddr(ctx, (ulong)header);
        }

        block3->field_68 = (uint)PsxAddrMapperRegister(ctx, (ulong)(header + block3->field_16));
        dword_180323248 = (int)PsxAddrMapperRegister(ctx, (ulong)(header + block3->field_16));
        int area = (int)(0x50000 - block3->field_16);
        block3->field_72 = area;
        dword_18032324C = area;
        InitModelTable88((long)(header + 144), 1);
        long parts = PsxAddrToPtr(ctx, block3->field_384);
        long textures = PsxAddrToPtr(ctx, block3->field_64);
        long positions = parts == textures ? 0 : (long)Unsafe.AsPointer(ref dword_180620B90);
        dword_180323250 = (int)RegisterSummonModel((long)header, positions, ctx);
        *(ushort*)Unsafe.AsPointer(ref dword_180323240) = 2;
    }

    /// <summary>
    /// Type 9: up to eight 64x128 VRAM pages selected by the bits of byte_180323207 (bit k for page pairs 0/1 and 2/3,
    /// then one bit per page 4..7) from the host or the local buffer; a page inside x 768..896, y 256..512 sets bit 0
    /// of dword_18032318C.
    /// </summary>
    private static void SfxLoaderStepVramPages(short* rect)
    {
        Recompiler.Runtime.Tracer.Enter("SfxLoaderStepVramPages(rect)", (nint)rect);
        *unk_180323243 = 1;
        byte* pixels = SfxLoaderStepHostBuffer() ? (byte*)qword_180069720 : unk_180171DC0;
        *(uint*)(rect + 2) = 0x00800040;
        int mask = 1;
        for (int i = 0; i < 8; i++)
        {
            if (((byte)mask & byte_180323207) != 0)
            {
                short y = (short)(((i & 1) + 2) * 128);
                short x = i < 4
                    ? (short)(((i & 0xFFFE) + 24) << 5)
                    : (short)((((i << 5) - 97) & 0xFFC0) + 320);
                rect[0] = x;
                rect[1] = y;
                if (BattleCallbackAddress != null)
                {
                    SfxLoaderStepUploadRect(x, y, 64, 128, pixels);
                }

                if (x >= 768 && x + 64 <= 896 && y >= 256 && y + 128 <= 512)
                {
                    dword_18032318C |= 1;
                }

                pixels += 0x4000;
            }

            if (i >= 4 || (i & 1) != 0)
            {
                mask += mask;
            }
        }

        ++*unk_180323259;
    }
}
