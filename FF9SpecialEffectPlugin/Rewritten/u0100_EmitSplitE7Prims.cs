using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

#pragma warning disable CS0675 // sign-extended s16 operands are ORed as in the original

internal static unsafe partial class Native
{
    /// <summary>
    /// Emits a horizontally wrapped span as up to two 6-word 0xE7 packets taken from the packet cursor (context word
    /// 9, advanced by 48) and links them at the last entry (4095) of the ordering table (context word 8). With
    /// q = a1 % a5 and r = a5 - q: packet 1 (only when q != 0) covers width q at x = a3 + r, u = a7; packet 2 covers
    /// width r at x = a3, u = a7 + q. Words: tag, 0xE7000000, (a6 &lt;&lt; 16) | u, (a4 &lt;&lt; 16) | width,
    /// (a2 &lt;&lt; 16) | x. Returns the xor mask applied to the OT entry by the last link (a1 / a5 when r is 0).
    /// </summary>
    public static long EmitSplitE7Prims(int a1, short a2, short a3, short a4, int a5, short a6, ushort a7)
    {
        Recompiler.Runtime.Tracer.Enter("EmitSplitE7Prims(a1, a2, a3, a4, a5, a6, a7)", a1, a2, a3, a4, a5, a6, a7);
        uint* context = (uint*)off_180066C68;
        long memory = (long)unk_180576A10;
        uint cursor = context[9];
        byte* packet = (byte*)PsxAddrToPtr(memory, cursor);
        context[9] = cursor + 48;
        long result = (uint)(a1 / a5);
        int remainder = a1 % a5;
        int rest = a5 - remainder;
        if (remainder != 0)
        {
            packet[3] = 5;
            ((uint*)packet)[1] = 0xE7000000;
            ((uint*)packet)[2] = (uint)(a7 | (a6 << 16));
            ((uint*)packet)[3] = (uint)((short)remainder | (a4 << 16));
            ((uint*)packet)[4] = (uint)((a2 << 16) | (short)(a3 + rest));
            result = EmitSplitE7PrimsLink(memory, (uint*)PsxAddrToPtr(memory, context[8]), packet);
        }

        if (rest == 0)
        {
            return result;
        }

        byte* second = packet + 24;
        second[3] = 5;
        ((uint*)packet)[7] = 0xE7000000;
        ((uint*)packet)[8] = (uint)((ushort)(a7 + remainder) | (a6 << 16));
        ((uint*)packet)[9] = (uint)((short)rest | (a4 << 16));
        ((uint*)packet)[10] = (uint)(a3 | (a2 << 16));
        return EmitSplitE7PrimsLink(memory, (uint*)PsxAddrToPtr(memory, context[8]), second);
    }

    /// <summary>Links <paramref name="packet"/> in front of OT entry 4095 of <paramref name="ot"/>; returns the xor mask applied to the entry.</summary>
    private static long EmitSplitE7PrimsLink(long memory, uint* ot, byte* packet)
    {
        Recompiler.Runtime.Tracer.Enter("EmitSplitE7PrimsLink(memory, ot, packet)", memory, (nint)ot, (nint)packet);
        uint* head = ot + 4095;
        *(uint*)packet ^= (*(uint*)packet ^ *head) & 0xFFFFFF;
        uint address = (uint)PsxPtrToAddr(memory, (ulong)packet);
        uint mask = (*head ^ address) & 0xFFFFFF;
        *head ^= mask;
        return mask;
    }

    /// <summary>
    /// Saves the 32-byte GTE matrix (rotation s16[9], pad, translation s32[3]) at context word 5 into
    /// xmmword_180620BC0/BD0 and replaces it with Ry(a1[1]) * Rx(a1[0]) * Rz(a1[2]) and translation (0, 0,
    /// dword_180211FA8); a null <paramref name="a1"/> means all angles 0. Returns the register value left by the
    /// inlined address resolution of context word 5.
    /// </summary>
    public static long PushRotTransMatrix(short* a1)
    {
        Recompiler.Runtime.Tracer.Enter("PushRotTransMatrix(a1)", (nint)a1);
        short* zeroAngles = stackalloc short[4];
        short* angles = a1 != null ? a1 : zeroAngles;
        byte* matrix = stackalloc byte[32];
        new Span<byte>(matrix, 32).Clear();
        short* rotation = (short*)matrix;
        rotation[0] = 4096;
        rotation[4] = 4096;
        rotation[8] = 4096;
        short* m = (short*)RotMatrixZ(angles[2], rotation);
        m = (short*)RotMatrixX(angles[0], m);
        RotMatrixY(angles[1], m);
        ((int*)matrix)[7] = dword_180211FA8;
        ((int*)matrix)[5] = 0;
        ((int*)matrix)[6] = 0;

        uint address = ((uint*)off_180066C68)[5];
        long memory = (long)unk_180576A10;
        byte* current = (byte*)PsxAddrToPtr(memory, address);
        long result = EmitSplitE7PrimsResolveRax(address);
        System.Buffer.MemoryCopy(current, xmmword_180620BC0, 16, 16);
        System.Buffer.MemoryCopy(current + 16, xmmword_180620BD0, 16, 16);
        byte* target = (byte*)PsxAddrToPtr(memory, address);
        System.Buffer.MemoryCopy(matrix, target, 32, 32);
        return result;
    }

    /// <summary>
    /// rax after the original's inlined PSX address resolution of a non-zero <paramref name="address"/>: the masked
    /// address for RAM (0x80 segment), 32 * segment for a mapped segment, otherwise address - 0x1F800000.
    /// </summary>
    private static long EmitSplitE7PrimsResolveRax(uint address)
    {
        Recompiler.Runtime.Tracer.Enter("EmitSplitE7PrimsResolveRax(address)", address);
        uint segment = address >> 24;
        if (segment == 128)
        {
            uint masked = address & 0xFFFFFFF;
            if (masked < 0x200000)
            {
                return masked;
            }
        }
        else if ((address & 0xC00000) == 0xC00000)
        {
            return 32L * segment;
        }

        return address - 0x1F800000u;
    }

    /// <summary>
    /// Restores the 32-byte matrix saved by PushRotTransMatrix to context word 5. Returns the register value left by
    /// the inlined address resolution (the scratchpad base pointer when the matrix lives in the scratchpad).
    /// </summary>
    public static long PopRotTransMatrix()
    {
        Recompiler.Runtime.Tracer.Enter("PopRotTransMatrix()");
        uint address = ((uint*)off_180066C68)[5];
        long memory = (long)unk_180576A10;
        byte* target = (byte*)PsxAddrToPtr(memory, address);
        long result = EmitSplitE7PrimsResolveRax(address);
        bool ram = address >> 24 == 128 && (address & 0xFFFFFFF) < 0x200000;
        bool segment = address >> 24 != 128 && (address & 0xC00000) == 0xC00000;
        if (!ram && !segment && address - 0x1F800000u <= 0x3FF)
        {
            result = (long)unk_1805789E8;
        }

        System.Buffer.MemoryCopy(xmmword_180620BC0, target, 16, 16);
        System.Buffer.MemoryCopy(xmmword_180620BD0, target + 16, 16, 16);
        return result;
    }

    /// <summary>Sets the matrix depth dword_180211FA8 to <paramref name="a1"/>, saving the old value in dword_180620B88; returns the old value.</summary>
    public static long SetMatrixDepth(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("SetMatrixDepth(a1)", a1);
        uint previous = (uint)dword_180211FA8;
        dword_180211FA8 = a1;
        dword_180620B88 = (int)previous;
        return previous;
    }

    /// <summary>Restores dword_180211FA8 from dword_180620B88; returns that value.</summary>
    public static long RestoreMatrixDepth()
    {
        Recompiler.Runtime.Tracer.Enter("RestoreMatrixDepth()");
        dword_180211FA8 = dword_180620B88;
        return (uint)dword_180620B88;
    }

    /// <summary>
    /// Sends CreateTexture with seven int32 values: packed texture coordinates, capture dimensions and camera shift.
    /// The fifth native parameter is unused. Memoria reads only the first seven payload values; the extra eighth
    /// slot is uninitialised in the original and zero here. Returns the callback result, 0 without a callback.
    /// </summary>
    public static long HostCaptureBackgroundTexture(int textureX, int textureY, int captureWidth, int captureHeight, int unused, int cameraX, int cameraY, int cameraZ)
    {
        Recompiler.Runtime.Tracer.Enter("HostCaptureBackgroundTexture(textureX, textureY, captureWidth, captureHeight, unused, cameraX, cameraY, cameraZ)", textureX, textureY, captureWidth, captureHeight, unused, cameraX, cameraY, cameraZ);
        Span<uint> values = stackalloc uint[8];
        uint* valuesAddress = (uint*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(values));
        values[0] = (uint)textureX;
        values[1] = (uint)textureY;
        values[2] = (uint)captureWidth;
        values[3] = (uint)captureHeight;
        values[4] = (uint)cameraX;
        values[5] = (uint)cameraY;
        values[6] = (uint)cameraZ;
        values[7] = 0;
        var callback = HostCallback;
        if (callback == null)
        {
            return 0;
        }

        return Native.InvokeBattleCallback((long)callback, HostCommand.CreateTexture, 0, 0, 0, 0, (ulong)valuesAddress);
    }

    /// <summary>
    /// Resets the 6128-byte work area at unk_180620BF0 to 0xFF (base qword_180620BE0, size and free size 6128),
    /// clears dword_180323270 and the first dword of 11 32-byte records at unk_18032327C. Returns the pointer past
    /// the last record.
    /// </summary>
    public static uint* ResetSfxWorkArea()
    {
        Recompiler.Runtime.Tracer.Enter("ResetSfxWorkArea()");
        byte* area = unk_180620BF0;
        dword_180620BE8 = 6128;
        qword_180620BE0 = (long)area;
        dword_180323270 = 0;
        dword_180620BEC = 6128;
        new Span<byte>(area, 6128).Fill(0xFF);
        uint* record = (uint*)unk_18032327C;
        for (int i = 0; i < 11; i++)
        {
            *record = 0;
            record += 8;
        }

        return record;
    }

    /// <summary>
    /// Sets o32 argument <paramref name="a2"/> of a PSX call to <paramref name="a3"/>: a0..a3 of the current GPR bank
    /// for 0..3, otherwise the stack word at sp + 4 * a2. Returns the bank offset (bank &lt;&lt; 7) or the host
    /// address of the PSX stack pointer.
    /// </summary>
    public static long PsxSetCallArg(long a1, int a2, int a3)
    {
        Recompiler.Runtime.Tracer.Enter("PsxSetCallArg(a1, a2, a3)", a1, a2, a3);
        var context = (PsxCallContext*)a1;
        long result = (long)context->bank << 7;
        if ((uint)a2 <= 3)
        {
            CurrentGprBank(context)[RegA0 + a2] = (uint)a3;
            return result;
        }

        result = PsxAddrToPtr((long)context->memory, CurrentGprBank(context)[RegSp]);
        *(int*)(result + (uint)(4 * a2)) = a3;
        return result;
    }
}
