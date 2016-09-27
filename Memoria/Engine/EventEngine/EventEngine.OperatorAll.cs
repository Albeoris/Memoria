using System;

public partial class EventEngine
{
    public Int32 OperatorAll(Int32 op)
    {
        Int32 a = 0;
        Int32 valueAtOffset = this.gCP.getValueAtOffset(-3);
        Int32 num1;
        if ((valueAtOffset >> 26 & 7) == 5)
        {
            num1 = this.GetSysList(valueAtOffset);
        }
        else
        {
            this.gCP.retreatTopOfStack();
            this.gCP.retreatTopOfStack();
            num1 = this.eBin.getv();
            this.gCP.advanceTopOfStack();
            this.gCP.advanceTopOfStack();
            this.gCP.advanceTopOfStack();
        }
        Int32 num2 = this.eBin.getv();
        EBin.op_binary opBinary = (EBin.op_binary)op;
        for (Int32 index = 0; index < 8; ++index)
        {
            if ((num1 & 1) != 0)
            {
                this.gMemberTarget = this._objPtrList[index];
                switch (opBinary)
                {
                    case EBin.op_binary.B_LET_A:
                        a = num2;
                        break;
                    case EBin.op_binary.B_MULT_LET_A:
                        a = this.eBin.getv() * num2;
                        this.gCP.advanceTopOfStack();
                        break;
                    case EBin.op_binary.B_DIV_LET_A:
                        a = num2 == 0 ? this.eBin.getv() : this.eBin.getv() / num2;
                        this.gCP.advanceTopOfStack();
                        break;
                    case EBin.op_binary.B_REM_LET_A:
                        a = num2 == 0 ? this.eBin.getv() : this.eBin.getv() % num2;
                        this.gCP.advanceTopOfStack();
                        break;
                    case EBin.op_binary.B_PLUS_LET_A:
                        a = this.eBin.getv() + num2;
                        this.gCP.advanceTopOfStack();
                        break;
                    case EBin.op_binary.B_MINUS_LET_A:
                        a = this.eBin.getv() - num2;
                        this.gCP.advanceTopOfStack();
                        break;
                    case EBin.op_binary.B_SHIFT_LEFT_LET_A:
                        a = this.eBin.getv() << num2;
                        this.gCP.advanceTopOfStack();
                        break;
                    case EBin.op_binary.B_SHIFT_RIGHT_LET_A:
                        a = this.eBin.getv() >> num2;
                        this.gCP.advanceTopOfStack();
                        break;
                    case EBin.op_binary.B_AND_LET_A:
                        a = this.eBin.getv() & num2;
                        this.gCP.advanceTopOfStack();
                        break;
                    case EBin.op_binary.B_XOR_LET_A:
                        a = this.eBin.getv() ^ num2;
                        this.gCP.advanceTopOfStack();
                        break;
                    case EBin.op_binary.B_OR_LET_A:
                        a = this.eBin.getv() | num2;
                        this.gCP.advanceTopOfStack();
                        break;
                }
                this.eBin.putv(a);
                this.gCP.advanceTopOfStack();
            }
            num1 >>= 1;
        }
        this.gCP.retreatTopOfStack();
        this.gCP.retreatTopOfStack();
        return a;
    }

    private Int32 OperatorAll1(Int32 op)
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
        EBin.op_binary opBinary = (EBin.op_binary)op;
        for (Int32 index = 0; index < 8; ++index)
        {
            if ((num & 1) != 0)
            {
                this.gMemberTarget = this._objPtrList[index];
                switch (opBinary)
                {
                    case EBin.op_binary.B_POST_PLUS_A:
                        a = this.eBin.getv();
                        this.gCP.advanceTopOfStack();
                        this.eBin.putv(a + 1);
                        break;
                    case EBin.op_binary.B_POST_MINUS_A:
                        a = this.eBin.getv();
                        this.gCP.advanceTopOfStack();
                        this.eBin.putv(a - 1);
                        break;
                    case EBin.op_binary.B_PRE_PLUS_A:
                        a = this.eBin.getv() + 1;
                        this.gCP.advanceTopOfStack();
                        this.eBin.putv(a);
                        break;
                    case EBin.op_binary.B_PRE_MINUS_A:
                        a = this.eBin.getv() - 1;
                        this.gCP.advanceTopOfStack();
                        this.eBin.putv(a);
                        break;
                }
                this.gCP.advanceTopOfStack();
            }
            num >>= 1;
        }
        this.gCP.retreatTopOfStack();
        this.gCP.retreatTopOfStack();
        return a;
    }
}