using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// The recompiled-PSX (MIPS R3000 + GTE) interpreter loop of a script context. Instructions are 16-byte records:
/// u16 opcode at +0, u16 at +2 (non-zero for branches, i.e. "the next instruction is a delay slot"), then three
/// 32-bit operands at +4, +8 and +12 (register numbers, immediates or branch targets depending on the opcode).
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>Per-bank thread state, stride 64: +0 code base (u64), +8 pc (s32), +16 next instruction record (u64).</summary>
    private const int PsxInterpreterRunThreads = 3120;

    /// <summary>Per-bank LO (stride 4); HI is 8 bytes further.</summary>
    private const int PsxInterpreterRunLo = 11704;

    private const int PsxInterpreterRunHi = 11712;

    /// <summary>Per-bank pending-branch flag (bytes at 11720) and target (dwords at 11724).</summary>
    private const int PsxInterpreterRunBranchFlag = 11720;

    private const int PsxInterpreterRunBranchTarget = 11724;

    /// <summary>
    /// Runs instructions of the current bank's thread of context <paramref name="a1"/> until its pc becomes negative.
    /// The register bank is fixed at entry (the original computes it once), the thread state, LO/HI and the
    /// pending branch are looked up through the current bank on every access. Returns bank &lt;&lt; 6 of the bank
    /// that ended the loop.
    /// </summary>
    public static long PsxInterpreterRun(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("PsxInterpreterRun(a1)", a1);
        var ctx = (PsxCallContext*)a1;
        long bank = ctx->bank;
        uint* gpr = (uint*)((byte*)((uint*)&ctx->gprBanks) + (bank << 7));
        while (true)
        {
            byte* thread = (byte*)a1 + PsxInterpreterRunThreads + (bank << 6);
            byte* insn = *(byte**)(thread + 16);
            *(int*)(thread + 8) += 4;
            *(byte**)(thread + 16) = insn + 16;

            PsxInterpreterRunStep(a1, gpr, insn);

            if (*(ushort*)(insn + 2) == 0)
            {
                PsxInterpreterRunTakeBranch(a1);
            }

            bank = ctx->bank;
            if (*(int*)((byte*)a1 + PsxInterpreterRunThreads + (bank << 6) + 8) < 0)
            {
                return bank << 6;
            }
        }
    }

    /// <summary>Executes one instruction record; every opcode not listed (0, 0x43..0x58, &gt; 0x5A) does nothing.</summary>
    private static void PsxInterpreterRunStep(long a1, uint* r, byte* insn)
    {
        Recompiler.Runtime.Tracer.Enter("PsxInterpreterRunStep(a1, r, insn)", a1, (nint)r, (nint)insn);
        int a = *(int*)(insn + 4);
        int b = *(int*)(insn + 8);
        int c = *(int*)(insn + 12);
        switch (*(ushort*)insn)
        {
            case 0x01: // sll
                if (a != 0) r[a] = r[b] << c;
                break;
            case 0x02: // sra
                if (a != 0) r[a] = (uint)((int)r[b] >> c);
                break;
            case 0x03: // srl
                if (a != 0) r[a] = r[b] >> c;
                break;
            case 0x04: // move
                if (a != 0) r[a] = r[b];
                break;
            case 0x05: // jalr: link first, then read the target register (rd == rs reads the link)
                if (a != 0) r[a] = PsxInterpreterRunLink(a1);
                PsxInterpreterRunSetBranch(a1, r[b]);
                break;
            case 0x06: // jr
                PsxInterpreterRunSetBranch(a1, r[a]);
                break;
            case 0x07: // mfhi
                if (a != 0) r[a] = *PsxInterpreterRunMulDiv(a1, PsxInterpreterRunHi);
                break;
            case 0x08: // mthi
                *PsxInterpreterRunMulDiv(a1, PsxInterpreterRunHi) = r[a];
                break;
            case 0x09: // mflo
                if (a != 0) r[a] = *PsxInterpreterRunMulDiv(a1, PsxInterpreterRunLo);
                break;
            case 0x0A: // mtlo
                *PsxInterpreterRunMulDiv(a1, PsxInterpreterRunLo) = r[a];
                break;
            case 0x0B: // mult
            {
                long product = (long)(int)r[b] * (int)r[a];
                *PsxInterpreterRunMulDiv(a1, PsxInterpreterRunLo) = (uint)product;
                *PsxInterpreterRunMulDiv(a1, PsxInterpreterRunHi) = (uint)(product >> 32);
                break;
            }
            case 0x0C: // multu
            {
                ulong product = (ulong)r[b] * r[a];
                *PsxInterpreterRunMulDiv(a1, PsxInterpreterRunLo) = (uint)product;
                *PsxInterpreterRunMulDiv(a1, PsxInterpreterRunHi) = (uint)(product >> 32);
                break;
            }
            case 0x0D: // div (idiv: a zero divisor or int.MinValue / -1 faults like the original)
            {
                int dividend = (int)r[a];
                int divisor = (int)r[b];
                int quotient = dividend / divisor;
                int remainder = dividend % divisor;
                *PsxInterpreterRunMulDiv(a1, PsxInterpreterRunLo) = (uint)quotient;
                *PsxInterpreterRunMulDiv(a1, PsxInterpreterRunHi) = (uint)remainder;
                break;
            }
            case 0x0E: // divu
            {
                uint dividend = r[a];
                uint divisor = r[b];
                uint quotient = dividend / divisor;
                uint remainder = dividend % divisor;
                *PsxInterpreterRunMulDiv(a1, PsxInterpreterRunLo) = quotient;
                *PsxInterpreterRunMulDiv(a1, PsxInterpreterRunHi) = remainder;
                break;
            }
            case 0x0F: // add
            case 0x10: // addu
                if (a != 0) r[a] = r[b] + r[c];
                break;
            case 0x11: // and
                if (a != 0) r[a] = r[b] & r[c];
                break;
            case 0x12: // xor
                if (a != 0) r[a] = r[b] ^ r[c];
                break;
            case 0x13: // srlv
                if (a != 0) r[a] = r[b] >> (int)r[c];
                break;
            case 0x14: // sub
            case 0x15: // subu
                if (a != 0) r[a] = r[b] - r[c];
                break;
            case 0x16: // nor
                if (a != 0) r[a] = ~(r[b] | r[c]);
                break;
            case 0x17: // or
                if (a != 0) r[a] = r[b] | r[c];
                break;
            case 0x18: // sllv
                if (a != 0) r[a] = r[b] << (int)r[c];
                break;
            case 0x19: // slt (setl)
                if (a != 0) r[a] = (int)r[b] < (int)r[c] ? 1u : 0u;
                break;
            case 0x1A: // sltu (setb)
                if (a != 0) r[a] = r[b] < r[c] ? 1u : 0u;
                break;
            case 0x1B: // srav
                if (a != 0) r[a] = (uint)((int)r[b] >> (int)r[c]);
                break;
            case 0x1C: // addi
            case 0x1D: // addiu
                if (a != 0) r[a] = r[b] + (uint)c;
                break;
            case 0x1E: // slti (setl)
                if (a != 0) r[a] = (int)r[b] < c ? 1u : 0u;
                break;
            case 0x1F: // sltiu (setb)
                if (a != 0) r[a] = r[b] < (uint)c ? 1u : 0u;
                break;
            case 0x20: // andi
                if (a != 0) r[a] = r[b] & (uint)c;
                break;
            case 0x21: // ori
                if (a != 0) r[a] = r[b] | (uint)c;
                break;
            case 0x22: // xori
                if (a != 0) r[a] = r[b] ^ (uint)c;
                break;
            case 0x23: // li
                if (a != 0) r[a] = (uint)b;
                break;
            case 0x24: // li (u16)
                if (a != 0) r[a] = *(ushort*)(insn + 8);
                break;
            case 0x25: // lui
                if (a != 0) r[a] = (uint)b << 16;
                break;
            case 0x26: // lb
                if (a != 0) r[a] = (uint)*(sbyte*)PsxInterpreterRunMem(a1, b, r[c]);
                break;
            case 0x27: // lh
                if (a != 0) r[a] = (uint)*(short*)PsxInterpreterRunMem(a1, b, r[c]);
                break;
            case 0x28: // lwl
            case 0x29: // lw
            case 0x2C: // lwr (all three are a plain 32-bit load in the original)
                if (a != 0) r[a] = *(uint*)PsxInterpreterRunMem(a1, b, r[c]);
                break;
            case 0x2A: // lbu
                if (a != 0) r[a] = *(byte*)PsxInterpreterRunMem(a1, b, r[c]);
                break;
            case 0x2B: // lhu
                if (a != 0) r[a] = *(ushort*)PsxInterpreterRunMem(a1, b, r[c]);
                break;
            case 0x2D: // sb (address first, then the source register)
            {
                byte* p = PsxInterpreterRunMem(a1, b, r[c]);
                *p = (byte)r[a];
                break;
            }
            case 0x2E: // sh
            {
                byte* p = PsxInterpreterRunMem(a1, b, r[c]);
                Site_PsxAddrToPtr2* site_PsxAddrToPtr2 = (Site_PsxAddrToPtr2*)p;
                site_PsxAddrToPtr2->union_0_2 = (short)((ushort)r[a]);
                break;
            }
            case 0x2F: // swl
            case 0x30: // sw
            case 0x31: // swr
            {
                byte* p = PsxInterpreterRunMem(a1, b, r[c]);
                Site_PsxAddrToPtr2* site_PsxAddrToPtr22 = (Site_PsxAddrToPtr2*)p;
                site_PsxAddrToPtr22->union_0_4 = (int)r[a];
                break;
            }
            case 0x32: // j
            case 0x3C:
                PsxInterpreterRunSetBranch(a1, (uint)a);
                break;
            case 0x33: // jal
                r[31] = PsxInterpreterRunLink(a1);
                PsxInterpreterRunSetBranch(a1, (uint)a);
                break;
            case 0x34: // beq
                if (r[a] == r[b]) PsxInterpreterRunSetBranch(a1, (uint)c);
                break;
            case 0x35: // bne
                if (r[a] != r[b]) PsxInterpreterRunSetBranch(a1, (uint)c);
                break;
            case 0x36: // bgez
                if ((int)r[a] >= 0) PsxInterpreterRunSetBranch(a1, (uint)b);
                break;
            case 0x37: // bltz
                if ((int)r[a] < 0) PsxInterpreterRunSetBranch(a1, (uint)b);
                break;
            case 0x38: // bltzal
                if ((int)r[a] < 0)
                {
                    r[31] = PsxInterpreterRunLink(a1);
                    PsxInterpreterRunSetBranch(a1, (uint)b);
                }

                break;
            case 0x39: // bgezal
                if ((int)r[a] >= 0)
                {
                    r[31] = PsxInterpreterRunLink(a1);
                    PsxInterpreterRunSetBranch(a1, (uint)b);
                }

                break;
            case 0x3A: // blez
                if ((int)r[a] <= 0) PsxInterpreterRunSetBranch(a1, (uint)b);
                break;
            case 0x3B: // bgtz
                if ((int)r[a] > 0) PsxInterpreterRunSetBranch(a1, (uint)b);
                break;
            case 0x3D: // swc2: address first, then the GTE data register
            {
                byte* p = PsxInterpreterRunMem(a1, b, r[c]);
                Site_PsxAddrToPtr2* site_PsxAddrToPtr23 = (Site_PsxAddrToPtr2*)p;
                site_PsxAddrToPtr23->union_0_4 = (int)((uint)GteReadData(a));
                break;
            }
            case 0x3E: // lwc2
            {
                byte* p = PsxInterpreterRunMem(a1, b, r[c]);
                Site_PsxAddrToPtr2* site_PsxAddrToPtr24 = (Site_PsxAddrToPtr2*)p;
                PsxInterpreterRunGteWrite(a, site_PsxAddrToPtr24->union_0_4);
                break;
            }
            case 0x3F: // cfc2
                if (a != 0) r[a] = ((uint*)xmmword_180211F40)[b];
                break;
            case 0x40: // ctc2
                ((uint*)xmmword_180211F40)[b] = r[a];
                break;
            case 0x41: // cop2 command
                PsxInterpreterRunGteCommand(a);
                break;
            case 0x42: // restart the thread at its code base
            {
                byte* thread = (byte*)a1 + PsxInterpreterRunThreads + ((long)((PsxCallContext*)a1)->bank << 6);
                long codeBase = *(long*)thread;
                *(int*)(thread + 8) = -4;
                *(long*)(thread + 16) = codeBase;
                break;
            }
            case 0x59: // mfc2 (the GTE read happens even for r0)
            {
                uint value = (uint)GteReadData(b);
                if (a != 0) r[a] = value;
                break;
            }
            case 0x5A: // mtc2
                PsxInterpreterRunGteWrite(b, (int)r[a]);
                break;
        }
    }

    /// <summary>The current bank's LO or HI word (<paramref name="offset"/> = PsxInterpreterRunLo / Hi).</summary>
    private static uint* PsxInterpreterRunMulDiv(long a1, int offset)
    {
        Recompiler.Runtime.Tracer.Enter("PsxInterpreterRunMulDiv(a1, offset)", a1, offset);
        return (uint*)((byte*)a1 + offset + 4L * ((PsxCallContext*)a1)->bank);
    }

    /// <summary>Return address of a linking jump: the current bank's pc + 8.</summary>
    private static uint PsxInterpreterRunLink(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("PsxInterpreterRunLink(a1)", a1);
        return (uint)(*(int*)((byte*)a1 + PsxInterpreterRunThreads + ((long)((PsxCallContext*)a1)->bank << 6) + 8) + 8);
    }

    /// <summary>Host pointer of PSX address <paramref name="offset"/> + <paramref name="baseValue"/> (32-bit sum).</summary>
    private static byte* PsxInterpreterRunMem(long a1, int offset, uint baseValue)
    {
        Recompiler.Runtime.Tracer.Enter("PsxInterpreterRunMem(a1, offset, baseValue)", a1, offset, baseValue);
        return (byte*)PsxAddrToPtr((long)((PsxCallContext*)a1)->memory, baseValue + (uint)offset);
    }

    /// <summary>Records a branch taken after the delay slot: flag = 1, then the target, both for the current bank.</summary>
    private static void PsxInterpreterRunSetBranch(long a1, uint target)
    {
        Recompiler.Runtime.Tracer.Enter("PsxInterpreterRunSetBranch(a1, target)", a1, target);
        var ctx = (PsxCallContext*)a1;
        ((byte*)&ctx->field_2DC8)[ctx->bank] = 1;
        ((uint*)&ctx->field_11724)[ctx->bank] = target;
    }

    /// <summary>
    /// After a non-branch instruction: a pending branch of the current bank is cleared and taken. Targets with the
    /// top byte 0xFF are native calls (SfxScriptDispatch with the low 22 bits); other targets with bit 31 set are
    /// absolute PSX addresses (low 28 bits) relative to the bank's code block at field_DA8 (outside it: offset 0);
    /// the rest are code offsets.
    /// </summary>
    private static void PsxInterpreterRunTakeBranch(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("PsxInterpreterRunTakeBranch(a1)", a1);
        var ctx = (PsxCallContext*)a1;
        long bank = ctx->bank;
        if (((byte*)&ctx->field_2DC8)[bank] == 0)
        {
            return;
        }

        uint target = ((uint*)&ctx->field_11724)[bank];
        ((byte*)&ctx->field_2DC8)[bank] = 0;
        if ((target & 0xFF000000) == 0xFF000000)
        {
            // The original leaves r8 (third argument) as whatever the instruction left there; it is not read.
            SfxScriptDispatch(a1, (int)(target & 0x3FFFFF), 0);
        }
        else if ((int)target >= 0)
        {
            PsxInterpreterRunJump(a1, ctx->bank, target);
        }
        else
        {
            bank = ctx->bank;
            uint address = target & 0xFFFFFFF;
            uint* block = (uint*)((ulong*)&ctx->field_DA8)[bank];
            uint start = *block & 0xFFFFFFF;
            uint offset = address < start || address >= start + block[4] ? 0u : address - start;
            PsxInterpreterRunJump(a1, bank, offset);
        }
    }

    /// <summary>Sets the bank's next instruction record to code base + 16 * (offset / 4) and its pc to offset - 4.</summary>
    private static void PsxInterpreterRunJump(long a1, long bank, uint offset)
    {
        Recompiler.Runtime.Tracer.Enter("PsxInterpreterRunJump(a1, bank, offset)", a1, bank, offset);
        byte* thread = (byte*)a1 + PsxInterpreterRunThreads + (bank << 6);
        *(long*)(thread + 16) = *(long*)thread + (long)(16 * (ulong)(offset >> 2));
        *(uint*)(thread + 8) = offset - 4;
    }

    /// <summary>
    /// Writes GTE data register <paramref name="index"/> (array at 0x180211FC0). Writing LZCS (30) first stores the
    /// leading-sign-bit count (leading zeros of a non-negative value, leading ones of a negative one) in LZCR.
    /// </summary>
    private static void PsxInterpreterRunGteWrite(int index, int value)
    {
        Recompiler.Runtime.Tracer.Enter("PsxInterpreterRunGteWrite(index, value)", index, value);
        if (index == 30)
        {
            int bits = value >= 0 ? ~value : value;
            int count = 0;
            while (bits < 0)
            {
                count++;
                bits <<= 1;
            }

            dword_18021203C = count;
        }

        ((int*)Unsafe.AsPointer(ref qword_180211FC0))[index] = value;
    }

    /// <summary>
    /// COP2 command word <paramref name="command"/>: DPCS, RTPS, RTPT, RTV0TR, NCLIP and AVSZ3; anything else asserts
    /// (PsxEmulator.cpp line 0x4E7). After RTPT MAC0 is set to 1.
    /// </summary>
    private static void PsxInterpreterRunGteCommand(int command)
    {
        Recompiler.Runtime.Tracer.Enter("PsxInterpreterRunGteCommand(command)", command);
        switch (command)
        {
            case 0x780010:
                GteDpcs();
                break;
            case 0x180001:
                GteRtps(0);
                break;
            case 0x280030:
                GteRtps(0);
                GteRtps(1);
                GteRtps(2);
                break;
            case 0x480012:
                GteRtV0Tr();
                break;
            case 0x1400006:
            {
                // NCLIP: MAC0 = SX0*SY1 + SX1*SY2 + SX2*SY0 - SX0*SY2 - SX1*SY0 - SX2*SY1 (32-bit wrap).
                int sx0 = (short)dword_180211FF0, sy0 = *(short*)unk_180211FF2;
                int sx1 = (short)dword_180211FF4, sy1 = *(short*)unk_180211FF6;
                int sx2 = (short)dword_180211FF8, sy2 = *(short*)unk_180211FFA;
                dword_180212020 = unchecked(sy2 * sx1 - sy2 * sx0 - sx1 * sy0 - sy1 * sx2 + sx0 * sy1 + sy0 * sx2);
                break;
            }
            case 0x158002D:
                GteAvsz3();
                break;
            default:
                PsxInterpreterRunAssert();
                break;
        }

        if (command == 0x280030)
        {
            dword_180212020 = 1;
        }
    }

    /// <summary><c>_wassert(L"0", L"..\\..\\SpecialEffectCode\\sonoda\\PsxEmulator.cpp", 0x4E7)</c> (see DIV-004).</summary>
    private static void PsxInterpreterRunAssert()
    {
        Recompiler.Runtime.Tracer.Enter("PsxInterpreterRunAssert()");
        System.Environment.FailFast(
            "Assertion failed: 0, file ..\\..\\SpecialEffectCode\\sonoda\\PsxEmulator.cpp, line " + 0x4E7);
    }
}
