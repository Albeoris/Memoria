using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Effect script handler attached to an object. Phase <paramref name="a1"/> 0 stores the state size 48 in
    /// <paramref name="a5"/>; 1 initialises the state <paramref name="a2"/> from the script operands a5[0], a5[1] and the
    /// operand bytes dword_180620B84 / byte_180620B80 (object id, 0xFF = position from the block at PSX address
    /// a3 + 12, +56/+60, y - 192) / byte_180620B81 (z bias * 8): +0 mode (bits 6..7), +1 object, +2 bias, +4 speed from
    /// word_18004B678[low nibble], +24 frame 0, +32 position; per mode the sprite table entries of dword_1802228D0 and
    /// the frame count at +14. Other phases draw one frame (0 sprite cell, 1 sprite animation, 3 ring mesh scaled by a
    /// cosine, 2 a trail effect spawned once at the start frame, oriented to the player for object 16), set a3 + 40 to the
    /// depth bias of the object, and return 1 when the animation has ended; otherwise clear a3 + 40, advance the frame
    /// and return 0.
    /// </summary>
    public static long SfxObjectEffectHandler(int a1, long a2, long a3, long a4, int* a5)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjectEffectHandler(a1, a2, a3, a4, a5)", a1, a2, a3, a4, (nint)a5);
        State2* state2 = (State2*)a2;
        Shape_SfxObjectEffectHandler_a3* shape_SfxObjectEffectHandler_a3 = (Shape_SfxObjectEffectHandler_a3*)a3;
        if (a1 == 0)
        {
            *a5 = 48;
            return 0;
        }

        byte* state = (byte*)a2;
        byte* operands = (byte*)Unsafe.AsPointer(ref dword_180620B84);
        byte* spriteTable = (byte*)Unsafe.AsPointer(ref dword_1802228D0);
        if (a1 == 1)
        {
            long first = *a5;
            int second = a5[1];
            int packed = dword_180620B84;
            state2->union_24_4 = 0;
            state2->field_2 = (short)((sbyte)byte_180620B81 << 3);
            state[0] = (byte)(((sbyte)packed >> 6) & 3);
            state2->union_4_2 = ((short*)Unsafe.AsPointer(ref word_18004B678))[(sbyte)packed & 0xF];
            byte objectId = byte_180620B80;
            state[1] = objectId;
            if (objectId == 0xFF)
            {
                *(uint*)(state + 32) = *(uint*)(PsxAddrToPtr((long)unk_180576A10, shape_SfxObjectEffectHandler_a3->field_12) + 56);
                uint z = *(uint*)(PsxAddrToPtr((long)unk_180576A10, shape_SfxObjectEffectHandler_a3->field_12) + 60);
                *(ushort*)(state + 34) += 0xFF40;
                *(uint*)(state + 36) = z;
            }
            else
            {
                HostQueryObjAnchorPos(PsxGetContextQword(objectId), 0, a2 + 32);
                packed = dword_180620B84;
            }

            switch (state[0])
            {
                case 0:
                    SfxObjectEffectHandlerBindSprite(state, spriteTable, first, second, 2);
                    state2->union_28_2 = (short)((sbyte)operands[2] << 2);
                    state2->union_30_2 = (short)((sbyte)operands[3] << 2);
                    break;

                case 1:
                    SfxObjectEffectHandlerBindSprite(state, spriteTable, first, second, 4);
                    break;

                case 2:
                    state2->field_8 = (short)first;
                    state2->field_10 = (sbyte)operands[1];
                    state2->union_4_2 = (short)second;
                    state2->union_14_2 = (sbyte)(packed & 0xF);
                    break;

                case 3:
                    state2->field_8 = (short)first;
                    state2->union_14_2 = (sbyte)operands[1];
                    state2->field_6 = 0;
                    *(byte**)(state + 16) = spriteTable + ((int*)spriteTable)[(short)first];
                    break;
            }

            return 0;
        }

        // The original leaves this vector uninitialised when the object is 0xFF (divergence recorded).
        Span<short> anchor = stackalloc short[4];
        short* anchorAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(anchor));
        if (state[1] != 0xFF)
        {
            int bias = state2->field_2;
            shape_SfxObjectEffectHandler_a3->field_40 = (uint)(bias + (int)(*(uint*)(PsxGetContextQword(state[1]) + 56) / 6u));
            short sign = (short)ObjDepthOrderSignById(16, state[1]);
            anchor[3] = 0;
            anchor[0] = 0;
            anchor[1] = (short)(sign << 11);
            anchor[2] = (short)state2->field_6;
        }

        ref int frame = ref *(int*)(state + 24);
        int frames = state2->union_14_2;
        switch (state[0])
        {
            case 0:
                EmitSpriteCellFrame(*(ushort**)(state + 16), (long*)(state + 32), anchorAddress, state2->union_4_2, frame, 1, 4096, state2->union_28_2, state2->union_30_2);
                if (frame >= state2->union_14_2 - 1)
                {
                    return 1;
                }

                break;

            case 1:
                EmitSpriteAnimFrame(*(long*)(state + 16), (long*)(state + 32), anchorAddress, state2->union_4_2, frame, 1, 4096);
                if (frame >= state2->union_14_2 - 1)
                {
                    return 1;
                }

                break;

            case 2:
                if (frame == frames && SfxObjectEffectHandlerSpawnTrail(state, a2, anchor))
                {
                    return 1;
                }

                if (frame > state2->union_14_2)
                {
                    long slot = *(long*)(state + 16);
                    if (state[1] == 16 && slot != 0)
                    {
                        *(ushort*)(slot + 92) = *(ushort*)(state + 40);
                        *(ushort*)(slot + 94) = *(ushort*)(state + 42);
                        *(ushort*)(slot + 96) = *(ushort*)(state + 44);
                    }

                    if (*(ushort*)(slot + 48) == 0xFFFF)
                    {
                        return 1;
                    }
                }

                break;

            case 3:
                int current = frame;
                double angle = (double)((current << 10) / frames) / BitConverter.Int64BitsToDouble(qword_18004B6C8)
                    * BitConverter.Int64BitsToDouble(qword_18004B698) * BitConverter.Int64BitsToDouble(qword_18004B6A0);
                double scale = Math.Cos(angle) * BitConverter.Int64BitsToDouble(qword_18004B6C8);
                DrawRingMeshEffect(*(byte**)(state + 16), (long*)(state + 32), anchorAddress, state2->union_4_2, current, 1, Recompiler.Runtime.X86.Cvttsd2si(scale));
                if (frame >= state2->union_14_2 - 1)
                {
                    return 1;
                }

                break;
        }

        shape_SfxObjectEffectHandler_a3->field_40 = 0;
        frame++;
        return 0;
    }

    /// <summary>
    /// Modes 0 and 1 of the init phase: +12 = first operand, +8 = operand byte 1 (sprite index), +6 = second operand
    /// byte * 256, +16 = sprite entry (dword_1802228D0 offset table), entry + <paramref name="byteOffset"/> = palette
    /// word qword_180323260[first], +14 = entry word 0 &amp; 0x7FFF (frame count).
    /// </summary>
    private static void SfxObjectEffectHandlerBindSprite(byte* state, byte* spriteTable, long first, int second, int byteOffset)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjectEffectHandlerBindSprite(state, spriteTable, first, second, byteOffset)", (nint)state, (nint)spriteTable, first, second, byteOffset);
        State2* state2 = (State2*)state;
        long sprite = (sbyte)((byte*)Unsafe.AsPointer(ref dword_180620B84))[1];
        state2->union_12_2 = (ushort)((short)first);
        state2->field_8 = (short)sprite;
        state2->field_6 = (ushort)((short)((sbyte)second * 256));
        byte* entry = spriteTable + ((int*)spriteTable)[sprite];
        *(byte**)(state + 16) = entry;
        *(ushort*)(entry + byteOffset) = ((ushort*)&((Shape_qword_180323260*)qword_180323260)->field_0)[(short)first];
        state2->union_14_2 = (short)((ushort)(**(ushort**)(state + 16) & 0x7FFF));
    }

    /// <summary>
    /// Mode 2 at the start frame: refreshes the position (object 16: the player's host position, turned towards the
    /// player position written into the shared stack vector <paramref name="target"/> with LookAnglesBetween into +40; returns true when object 16 is the player itself and the
    /// effect id is 54), then spawns the trail effect of sprite entry +10 at +32 (kind = +2, sub-block entry +8, byte +4).
    /// </summary>
    private static bool SfxObjectEffectHandlerSpawnTrail(byte* state, long a2, Span<short> target)
    {
        Recompiler.Runtime.Tracer.Enter("SfxObjectEffectHandlerSpawnTrail(state, a2, target)", (nint)state, a2, target.Length);
        short* targetAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(target));
        State2* state2 = (State2*)state;
        byte objectId = state[1];
        if (objectId != 0xFF)
        {
            if (objectId == 16)
            {
                GetObjectHostPosition(16, 1, (ushort*)(state + 32));
                long player = PsxGetContextQword(0);
                var callback = (BattleCallbackFn)BattleCallbackAddress;
                bool queried = false;
                if (callback != null)
                {
                    ulong code = player != 0 ? (*(ushort*)(player + 24) | 0x16000000u) : 0x16000000u;
                    if ((uint)Native.InvokeBattleCallback((long)callback, code, 0, 0, 0, 0, 0) != 0 && *(byte*)(player + 16) == 0)
                    {
                        queried = true;
                    }
                }

                if (!queried)
                {
                    HostQueryObjPosition(player, (long)targetAddress);
                    queried = *(byte*)(player + 16) == 0;
                    if (!queried)
                    {
                        int lift = *(int*)(player + 60) >> 1;
                        if (HostCallbackSend(player, 0x200000) != 0)
                        {
                            lift -= 128;
                        }

                        target[1] -= (short)lift;
                    }
                }

                if (queried)
                {
                    ulong third = *(byte*)(player + 26);
                    callback = (BattleCallbackFn)BattleCallbackAddress;
                    if (callback != null)
                    {
                        Native.InvokeBattleCallback((long)callback, *(ushort*)(player + 24) | HostCommand.GetMatrix, 0, third, 0, 0, (ulong)targetAddress);
                    }
                }

                LookAnglesBetween((short*)(state + 32), targetAddress, (ushort*)(state + 40));
                long self = PsxGetContextQword(state[1]);
                if (PsxGetContextQword(0) == self && (ushort)state2->field_10 == 54)
                {
                    return true;
                }
            }
            else
            {
                HostQueryObjAnchorPos(PsxGetContextQword(objectId), 0, a2 + 32);
            }
        }

        byte* spriteTable = (byte*)Unsafe.AsPointer(ref dword_1802228D0);
        long slot = CreateTrailEffect((long)(spriteTable + ((int*)spriteTable)[state2->field_10]), a2 + 32);
        if (slot != 0)
        {
            *(ushort*)(slot + 34) = (ushort)state2->field_2;
        }

        ushort* subBlock = (ushort*)(spriteTable + ((int*)spriteTable)[state2->field_8]);
        if (slot != 0)
        {
            uint address = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)subBlock);
            *(uint*)(PsxAddrToPtr((long)unk_180576A10, *(uint*)(slot + 24)) + 28) = address;
            *(byte*)PsxAddrToPtr((long)unk_180576A10, *(uint*)(slot + 24)) = state[4];
        }

        ushort frameCount = (ushort)(*subBlock & 0x7FFF);
        *(ushort*)(PsxAddrToPtr((long)unk_180576A10, *(uint*)(slot + 24)) + 8) = frameCount;
        *(long*)(state + 16) = slot;
        return false;
    }
}
