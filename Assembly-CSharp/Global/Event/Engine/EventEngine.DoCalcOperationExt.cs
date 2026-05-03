using System;

public partial class EventEngine
{
    public Int32 DoCalcOperationExt(EBin.op_binary code)
    {
        Int32 result = 0;
        Int16 triNdx = -1;
        Int16 floorNdx = -1;
        EBin.op_binary opBinary = code;
        switch (opBinary)
        {
            case EBin.op_binary.B_HAVE_ITEM:
                Int32 id = this.eBin.getv();
                result = ff9item.FF9Item_GetCount_Generic(id);
                break;
            case EBin.op_binary.B_BAFRAME:
                this.fieldmap.walkMesh.BGI_animGetFrame((UInt32)this.eBin.getv(), ref result);
                break;
            case EBin.op_binary.B_FRAME:
                result = EventEngineUtils.GetCharAnimFrame(this.gCur.go, this.eBin.getv());
                break;
            case EBin.op_binary.B_SPS:
                this.eBin.getv();
                this.eBin.getv();
                break;
            case EBin.op_binary.B_CURMP:
                result = (Int32)this._ff9.GetPlayer(this.chr2slot(this.eBin.getv())).cur.mp;
                break;
            case EBin.op_binary.B_MAXMP:
                result = (Int32)this._ff9.GetPlayer(this.chr2slot(this.eBin.getv())).max.mp;
                break;
            case EBin.op_binary.B_BGIID:
                BGI.BGI_charGetInfo(this.eBin.getv(), ref triNdx, ref floorNdx);
                result = triNdx;
                break;
            case EBin.op_binary.B_BGIFLOOR:
                BGI.BGI_charGetInfo(this.eBin.getv(), ref triNdx, ref floorNdx);
                result = floorNdx;
                break;
            case EBin.op_binary.B_AND_LET_E:
            case EBin.op_binary.B_XOR_LET_E:
            case EBin.op_binary.B_OR_LET_E:
            case EBin.op_binary.B_LET_E:
                result = this.OperatorExtractLet(code);
                break;
            case EBin.op_binary.B_LMAX:
            case EBin.op_binary.B_LMIN:
            case EBin.op_binary.B_NOT_E:
                result = this.OperatorExtract1(code);
                break;
            case EBin.op_binary.B_POST_PLUS_A:
            case EBin.op_binary.B_POST_MINUS_A:
            case EBin.op_binary.B_PRE_PLUS_A:
            case EBin.op_binary.B_PRE_MINUS_A:
                result = this.OperatorAll1(code);
                break;
            case EBin.op_binary.B_KEYON2:
            case EBin.op_binary.B_KEYOFF2:
            case EBin.op_binary.B_KEY2:
                this.eBin.getv();
                break;
            case EBin.op_binary.B_CURHP:
                result = (Int32)this._ff9.GetPlayer(this.chr2slot(this.eBin.getv())).cur.hp;
                break;
            case EBin.op_binary.B_MAXHP:
                result = (Int32)this._ff9.GetPlayer(this.chr2slot(this.eBin.getv())).max.hp;
                break;
        }
        return result;
    }
}
