using System;

public partial class EventEngine
{
    public Int32 OperatorExtract(Int32 op)
    {
        Int32 num1 = 0;
        Int32 num2 = 0;
        Int32 valueAtOffset = this.gCP.getValueAtOffset(-3);
        Int32 num3;
        if ((valueAtOffset >> 26 & 7) == 5)
        {
            num3 = this.GetSysList(valueAtOffset);
        }
        else
        {
            this.gCP.retreatTopOfStack();
            this.gCP.retreatTopOfStack();
            num3 = this.eBin.getv();
            this.gCP.advanceTopOfStack();
            this.gCP.advanceTopOfStack();
            this.gCP.advanceTopOfStack();
        }
        Int32 num4 = this.eBin.getv();
        for (Int32 index = 0; index < 8; ++index)
        {
            num1 >>= 1;
            if ((num3 & 1) != 0)
            {
                this.gMemberTarget = this._objPtrList[index];
                EBin.op_binary opBinary = (EBin.op_binary)op;
                switch (opBinary)
                {
                    case EBin.op_binary.B_LT_E:
                        num2 = this.eBin.getv() >= num4 ? 0 : 1;
                        break;
                    case EBin.op_binary.B_GT_E:
                        num2 = this.eBin.getv() <= num4 ? 0 : 1;
                        break;
                    case EBin.op_binary.B_LE_E:
                        num2 = this.eBin.getv() > num4 ? 0 : 1;
                        break;
                    case EBin.op_binary.B_GE_E:
                        num2 = this.eBin.getv() < num4 ? 0 : 1;
                        break;
                    case EBin.op_binary.B_EQ_E:
                        num2 = this.eBin.getv() != num4 ? 0 : 1;
                        break;
                    case EBin.op_binary.B_NE_E:
                        num2 = this.eBin.getv() == num4 ? 0 : 1;
                        break;
                    default:
                        switch (opBinary - 84)
                        {
                            case EBin.op_binary.B_PAD0:
                                num2 = (this.eBin.getv() & num4) == 0 ? 0 : 1;
                                break;
                            case EBin.op_binary.B_PAD1:
                                num2 = (this.eBin.getv() & num4) != 0 ? 0 : 1;
                                break;
                            case EBin.op_binary.B_PAD2:
                                num2 = (this.eBin.getv() ^ num4) == 0 ? 0 : 1;
                                break;
                            case EBin.op_binary.B_PAD3:
                                num2 = (this.eBin.getv() | num4) == 0 ? 0 : 1;
                                break;
                        }
                        break;
                }
                num1 |= num2 << 7;
                this.gCP.advanceTopOfStack();
            }
            num3 >>= 1;
        }
        this.gCP.retreatTopOfStack();
        this.gCP.retreatTopOfStack();
        return num1;
    }

    private Int32 OperatorExtract1(Int32 op)
    {
        Int32 num1 = 0;
        Int32 num2 = 0;
        Int32 valueAtOffset = this.gCP.getValueAtOffset(-2);
        Int32 num3;
        if ((valueAtOffset >> 26 & 7) == 5)
        {
            num3 = this.GetSysList(valueAtOffset);
        }
        else
        {
            this.gCP.retreatTopOfStack();
            num3 = this.eBin.getv();
            this.gCP.advanceTopOfStack();
            this.gCP.advanceTopOfStack();
        }
        switch ((EBin.op_binary)op)
        {
            case EBin.op_binary.B_LMAX:
                Int32 num4 = (Int32)Int16.MinValue;
                for (Int32 index = 0; index < 8; ++index)
                {
                    if ((num3 & 1) != 0)
                    {
                        this.gMemberTarget = this._objPtrList[index];
                        Int32 num5 = this.eBin.getv();
                        this.gCP.advanceTopOfStack();
                        if (num4 < num5)
                        {
                            num4 = num5;
                            num2 = index;
                        }
                    }
                    num3 >>= 1;
                }
                num1 = 1 << num2;
                break;
            case EBin.op_binary.B_LMIN:
                Int32 num6 = (Int32)Int16.MaxValue;
                for (Int32 index = 0; index < 8; ++index)
                {
                    if ((num3 & 1) != 0)
                    {
                        this.gMemberTarget = this._objPtrList[index];
                        Int32 num5 = this.eBin.getv();
                        this.gCP.advanceTopOfStack();
                        if (num6 > num5)
                        {
                            num6 = num5;
                            num2 = index;
                        }
                    }
                    num3 >>= 1;
                }
                num1 = 1 << num2;
                break;
            case EBin.op_binary.B_NOT_E:
                for (Int32 index = 0; index < 8; ++index)
                {
                    num1 >>= 1;
                    if ((num3 & 1) != 0)
                    {
                        this.gMemberTarget = this._objPtrList[index];
                        num1 |= (this.eBin.getv() != 0 ? 0 : 1) << 7;
                        this.gCP.advanceTopOfStack();
                    }
                    num3 >>= 1;
                }
                break;
        }
        this.gCP.retreatTopOfStack();
        this.gCP.retreatTopOfStack();
        return num1;
    }

    private Int32 OperatorExtractLet(Int32 op)
    {
        Int32 a = 0;
        Int32 valueAtOffset = this.gCP.getValueAtOffset(-2);
        Int32 num;
        if ((valueAtOffset >> 26 & 7) == 5)
        {
            num = this.GetSysList(valueAtOffset);
        }
        else
        {
            this.gCP.retreatTopOfStack();
            num = this.eBin.getv();
            this.gCP.advanceTopOfStack();
            this.gCP.advanceTopOfStack();
        }
        for (Int32 index = 0; index < 8; ++index)
        {
            a >>= 1;
            if ((num & 1) != 0)
            {
                this.gMemberTarget = this._objPtrList[index];
                a |= (this.eBin.getv() == 0 ? 0 : 1) << 7;
                this.gCP.advanceTopOfStack();
            }
            num >>= 1;
        }
        this.gCP.retreatTopOfStack();
        this.gCP.retreatTopOfStack();
        switch (op)
        {
            case 67:
                a &= this.eBin.getv();
                this.gCP.advanceTopOfStack();
                break;
            case 68:
                a ^= this.eBin.getv();
                this.gCP.advanceTopOfStack();
                break;
            case 69:
                a |= this.eBin.getv();
                this.gCP.advanceTopOfStack();
                break;
        }
        this.eBin.putv(a);
        return a;
    }
}