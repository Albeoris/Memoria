using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>Size of one tracked host-object entry in the table at unk_1800694F8 (8 entries up to qword_1800696F8).</summary>
    private const int SfxSendIntDataEntrySize = 0x40;

    /// <summary>
    /// Starts the effect track a2 of the block a1 for a host entry: clears the 0x68-byte effect state, draws the random
    /// side bit, refreshes the vectors of the two selected host objects, then starts the track block.
    /// </summary>
    public static long SfxStartTrackForEntry(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("SfxStartTrackForEntry(a1, a2)", a1, a2);
        new Span<byte>(Unsafe.AsPointer(ref qword_18021FF00), 0x68).Clear();
        dword_1803231DC = 1103515245 * dword_1803231DC + 12345;
        byte_1802200B3 = (byte)(((uint)dword_1803231DC >> 16) & 1);
        HostQueryObjectVector(CameraCaster, (ushort*)unk_18021FF4A);
        HostQueryObjectVector(CameraTarget, (ushort*)unk_18021FF5E);
        long result = SfxStartTrackBlock(a1, a2);
        byte_180220119 = 2;
        byte_1802200BC = 0;
        return result;
    }

    /// <summary>Returns the entry of the host-object table whose id (u16 at +0x18) is id, or null when none matches.</summary>
    private static byte* SfxSendIntDataFindEntry(uint id)
    {
        Recompiler.Runtime.Tracer.Enter("SfxSendIntDataFindEntry(id)", id);
        byte* end = (byte*)Unsafe.AsPointer(ref CameraCaster);
        for (byte* entry = BattleObjects; entry < end; entry += SfxSendIntDataEntrySize)
        {
            if (*(ushort*)(entry + 0x18) == (ushort)id)
            {
                return entry;
            }
        }

        return null;
    }

    /// <summary>Host pointer of the PSX address in slot `index` of the PSX register table off_180066C68.</summary>
    private static byte* SfxSendIntDataSlot(int index)
    {
        Recompiler.Runtime.Tracer.Enter("SfxSendIntDataSlot(index)", index);
        return (byte*)PsxAddrToPtr((long)unk_180576A10, ((uint*)off_180066C68)[index]);
    }

    /// <summary>
    /// Integer command channel from Memoria: camera caster/target, camera selection and effect/camera state.
    /// arg2 is part of the four-argument host ABI and is unused by this binary.
    /// Evidence: ref:R001/Assembly-CSharp/Global/SFX/SFX.cs#L1989-L2091.
    /// </summary>
    public static int SFX_SendIntData_0(int type, int arg0, int arg1, int arg2)
    {
        Recompiler.Runtime.Tracer.Enter("SFX_SendIntData_0(type, arg0, arg1, arg2)", type, arg0, arg1, arg2);
        switch ((SfxIntCommand)type)
        {
            case SfxIntCommand.SetCameraCaster:
            {
                byte* entry = SfxSendIntDataFindEntry((uint)arg0);
                CameraCaster = (long)(entry != null ? entry : BattleObjects);
                return 0;
            }
            case SfxIntCommand.SetCameraTarget:
            {
                byte* entry = SfxSendIntDataFindEntry((uint)arg0);
                CameraTarget = (long)(entry != null ? entry : BattleObjects);
                return 0;
            }
            case SfxIntCommand.SetCamera:
            {
                byte* block = (byte*)Block;
                if (block != null)
                {
                    SfxResetAndSelectBlock((long)(block + *((short*)block + (uint)arg0)), arg1);
                }
                else
                {
                    SfxResetAndSelectBlock(0, (int)arg1);
                }

                return 0;
            }
            case SfxIntCommand.SetEnemyCamera:
            {
                byte* entry = SfxSendIntDataFindEntry((uint)arg0);
                if (entry == null)
                {
                    entry = BattleObjects;
                }

                SfxStartTrackForEntry(*(long*)(entry + 0x30), (int)arg1);
                return 0;
            }
            case SfxIntCommand.GetEffectOvRun:
            {
                byte* state = SfxSendIntDataSlot(0);
                State4* state4 = (State4*)state;
                uint notTerminated = state4->field_0 != 0xFFFF ? 1u : 0u;
                return (int)((~(state4->field_28 >> 15) & notTerminated) & 1);
            }
            case SfxIntCommand.GetEffCamTrigger:
                return ((State4*)SfxSendIntDataSlot(11))->field_82;
            case SfxIntCommand.SetEffCamTrigger:
                *(SfxSendIntDataSlot(11) + 82) = 0;
                return 0;
            case SfxIntCommand.GetTaskMonsteraStartOk:
                return *(SfxSendIntDataSlot(0) + 35);
            case SfxIntCommand.SetTaskMonsteraStart:
                *(SfxSendIntDataSlot(0) + 15) = 1;
                return 0;
            case SfxIntCommand.GetCameraPhase:
                return CameraPhase;
            case SfxIntCommand.SetCameraPhase:
                CameraPhase = (byte)arg0;
                return 0;
            case SfxIntCommand.GetEffectJTexUsed:
            {
                byte* state = SfxSendIntDataSlot(0);
                State4* state42 = (State4*)state;
                uint flags = state42->field_28;
                uint notTerminated = state42->field_0 != 0xFFFF ? 1u : 0u;
                return (int)((~(flags >> 15) & notTerminated & flags) & 1);
            }
            default:
                return 0;
        }
    }

    /// <summary>MIPS J/JAL target: the 26-bit instruction index shifted left by 2 (zero-extended 32-bit result).</summary>
    public static long MipsJumpTargetOffset(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("MipsJumpTargetOffset(a1, a2)", a1, a2);
        return (uint)((a2 & 0x3FFFFFF) << 2);
    }

    /// <summary>
    /// MIPS I-type decode: returns the sign-extended 16-bit immediate as a zero-extended 32-bit value and stores the rs
    /// field (bits 21..25) in *a3.
    /// </summary>
    public static long MipsDecodeImm16Rs(long a1, int a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("MipsDecodeImm16Rs(a1, a2, a3)", a1, a2, (nint)a3);
        uint result = (ushort)a2;
        if ((a2 & 0x8000) != 0)
        {
            result |= 0xFFFF0000;
        }

        *a3 = (a2 >> 21) & 0x1F;
        return result;
    }

    /// <summary>Clears the qwords at +8 and +0x18 of the buffer object and returns it (constructor-style).</summary>
    public static ulong* SfxBufferReset(ulong* a1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxBufferReset(a1)", (nint)a1);
        a1[1] = 0;
        a1[3] = 0;
        return a1;
    }

    /// <summary>Frees the data pointer at +0x18 with operator delete[] (CRT free).</summary>
    public static void SfxBufferFreeData(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("SfxBufferFreeData(a1)", a1);
        NativeMemory.Free(*(void**)(a1 + 24));
    }
}
