using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// Small handlers reached only through function-pointer tables in the data image: the PSX mapper start-up
/// initializer, the 0xE7 packet builder, the libgte-style math wrappers on 4096.8 units per turn (MATH-001),
/// the GTE rotation/translation setters and the MIPS instruction field decoders.
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>
    /// CRT dynamic initializer of the PsxVirtualAddrMapper64 at 0x180576A10 (PSX-MAPPER-001), kept as a plain
    /// function (STARTUP-001): clears the used byte of the 254 32-byte slots, marks slot 31 used and zeroes the count.
    /// </summary>
    public static long InitMapperTable()
    {
        Recompiler.Runtime.Tracer.Enter("InitMapperTable()");
        byte* slot = ((byte*)unk_180576A10);
        byte* end = (byte*)Unsafe.AsPointer(ref qword_1805789D0);
        do
        {
            *slot = 0;
            slot += 32;
        }
        while ((long)slot < (long)end);
        byte_180576DF0 = 1;
        qword_1805789D0 = 0;
        return 0;
    }

    /// <summary>
    /// Fills a 5-word 0xE7 packet: length byte 5, code 0xE7000000, then (a2[0] &lt;&lt; 16) | (ushort)a2[1],
    /// (a2[2] &lt;&lt; 16) | a2[3] and (a3 &lt;&lt; 16) | a4, the last two OR a sign-extended low half (GPU-014).
    /// </summary>
    public static long InitE7Packet(long a1, short* a2, short a3, short a4)
    {
        Recompiler.Runtime.Tracer.Enter("InitE7Packet(a1, a2, a3, a4)", a1, (nint)a2, a3, a4);
        PsgModel* psgModel = (PsgModel*)a1;
        byte* packet = (byte*)a1;
        packet[3] = 5;
        *(uint*)(packet + 4) = 0xE7000000;
        *(int*)(packet + 8) = (a2[0] << 16) | (ushort)a2[1];
        psgModel->Addr12 = (uint)((a2[2] << 16) | a2[3]);
        psgModel->PartsAddr = (uint)((a3 << 16) | a4);
        return (uint)(int)a4;
    }

    /// <summary>rcos: cos(a1 / 4096.8 * 2 * pi) * 4096.8, truncated with cvttsd2si.</summary>
    public static long PsxRcos(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("PsxRcos(a1)", a1);
        return (uint)Recompiler.Runtime.X86.Cvttsd2si(Math.Cos((double)a1 / 4096.8 * 2.0 * 3.141592653589793) * 4096.8);
    }

    /// <summary>rsin: sin(a1 / 4096.8 * 2 * pi) * 4096.8, truncated with cvttsd2si.</summary>
    public static long PsxRsin(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("PsxRsin(a1)", a1);
        return (uint)Recompiler.Runtime.X86.Cvttsd2si(Math.Sin((double)a1 / 4096.8 * 2.0 * 3.141592653589793) * 4096.8);
    }

    /// <summary>ratan2-style angle: 1024 - trunc(-atan2(a2, a1) / (2 pi) * -4096.0).</summary>
    public static long PsxRatan2(int a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("PsxRatan2(a1, a2)", a1, a2);
        return (uint)(1024 - Recompiler.Runtime.X86.Cvttsd2si(-Math.Atan2(a2, a1) / 6.283185307179586 * -4096.0));
    }

    /// <summary>
    /// SetRotMatrix: copies the first 16 bytes of the MATRIX to the GTE RT registers at 0x180211F40 (four dword
    /// stores) and the zero-extended RT33 halfword at +16 to the dword at 0x180211F50; returns that value.
    /// </summary>
    public static long GteSetRotMatrix(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("GteSetRotMatrix(a1)", a1);
        Shape_GteSetRotMatrix_a1* shape_GteSetRotMatrix_a1 = (Shape_GteSetRotMatrix_a1*)a1;
        int* m = (int*)a1;
        dword_180211F40 = m[0];
        dword_180211F44 = m[1];
        dword_180211F48 = m[2];
        dword_180211F4C = m[3];
        uint rt33 = shape_GteSetRotMatrix_a1->field_16;
        dword_180211F50 = (int)rt33;
        return rt33;
    }

    /// <summary>SetTransMatrix: TRX/TRY/TRZ (dword stores at 0x180211F54/58/5C) from MATRIX.t at +20/+24/+28.</summary>
    public static long GteSetTransMatrix(uint* a1)
    {
        Recompiler.Runtime.Tracer.Enter("GteSetTransMatrix(a1)", (nint)a1);
        dword_180211F54 = (int)a1[5];
        dword_180211F58 = (int)a1[6];
        dword_180211F5C = (int)a1[7];
        return a1[7];
    }

    /// <summary>SquareRoot0: trunc(sqrt((double)a1)) with cvttsd2si.</summary>
    public static long PsxSquareRoot0(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("PsxSquareRoot0(a1)", a1);
        return (uint)Recompiler.Runtime.X86.Cvttsd2si(Math.Sqrt(a1));
    }

    /// <summary>MIPS rs field: bits 21..25.</summary>
    public static long MipsDecodeRs(long a1, uint a2)
    {
        Recompiler.Runtime.Tracer.Enter("MipsDecodeRs(a1, a2)", a1, a2);
        return (a2 >> 21) & 0x1F;
    }

    /// <summary>MIPS rt field: bits 16..20.</summary>
    public static long MipsDecodeRt(long a1, uint a2)
    {
        Recompiler.Runtime.Tracer.Enter("MipsDecodeRt(a1, a2)", a1, a2);
        return (a2 >> 16) & 0x1F;
    }

    /// <summary>MIPS rd field: bits 11..15 (MIPS-003: the original masks the full edx with 0x1F).</summary>
    public static long MipsDecodeRd(long a1, ushort a2)
    {
        Recompiler.Runtime.Tracer.Enter("MipsDecodeRd(a1, a2)", a1, a2);
        return (a2 >> 11) & 0x1F;
    }

    /// <summary>MIPS shamt field: bits 6..10.</summary>
    public static long MipsDecodeShamt(long a1, uint a2)
    {
        Recompiler.Runtime.Tracer.Enter("MipsDecodeShamt(a1, a2)", a1, a2);
        return (a2 >> 6) & 0x1F;
    }

    /// <summary>MIPS 20-bit code field (syscall/break): bits 6..25.</summary>
    public static long MipsDecodeCode20(long a1, uint a2)
    {
        Recompiler.Runtime.Tracer.Enter("MipsDecodeCode20(a1, a2)", a1, a2);
        return (a2 >> 6) & 0xFFFFF;
    }

    /// <summary>MIPS signed 16-bit immediate, returned as a zero-extended 32-bit value.</summary>
    public static long MipsDecodeSimm16(long a1, ushort a2)
    {
        Recompiler.Runtime.Tracer.Enter("MipsDecodeSimm16(a1, a2)", a1, a2);
        return (uint)(int)(short)a2;
    }

    /// <summary>MIPS unsigned 16-bit immediate.</summary>
    public static long MipsDecodeImm16(long a1, ushort a2)
    {
        Recompiler.Runtime.Tracer.Enter("MipsDecodeImm16(a1, a2)", a1, a2);
        return a2;
    }

    /// <summary>MIPS 25-bit immediate (COP2 command field).</summary>
    public static long MipsDecodeImm25(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("MipsDecodeImm25(a1, a2)", a1, a2);
        return a2 & 0x1FFFFFF;
    }

    /// <summary>MIPS branch target: a1 + 4 * (simm16 + 1) in 32 bits, zero-extended.</summary>
    public static long MipsBranchTarget(int a1, ushort a2)
    {
        Recompiler.Runtime.Tracer.Enter("MipsBranchTarget(a1, a2)", a1, a2);
        return (uint)(a1 + 4 * ((short)a2 + 1));
    }

    /// <summary>Clears the in-use dword (+80) and the model pointer (+0) of the summon slot at 0x180220830.</summary>
    public static long ResetSummonSlot()
    {
        Recompiler.Runtime.Tracer.Enter("ResetSummonSlot()");
        dword_180220880 = 0;
        qword_180220830 = 0;
        return 0;
    }

    /// <summary>Empty handler.</summary>
    public static void nullsub_1()
    {
        Recompiler.Runtime.Tracer.Enter("nullsub_1()");
    }
}
