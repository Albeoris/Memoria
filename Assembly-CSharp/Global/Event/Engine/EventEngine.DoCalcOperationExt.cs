using System;

public partial class EventEngine
{
    public Int32 DoCalcOperationExt(EBin.op_binary code)
    {
        Int32 frameNdx = 0;
        Int16 triNdx1 = 0;
        Int16 floorNdx1 = 0;
        EBin.op_binary opBinary = code;
        switch (opBinary)
        {
            case EBin.op_binary.B_HAVE_ITEM:
                Int32 id = this.eBin.getv();
                frameNdx = id >= EventEngine.kSItemOfs ? (id >= EventEngine.kCItemOfs ? QuadMistDatabase.MiniGame_GetCardCount(id - EventEngine.kCItemOfs) : (!ff9item.FF9Item_IsExistImportant(id - EventEngine.kSItemOfs) ? 0 : 1)) : ff9item.FF9Item_GetCount(id);
                break;
            case EBin.op_binary.B_BAFRAME:
                this.fieldmap.walkMesh.BGI_animGetFrame((UInt32)this.eBin.getv(), ref frameNdx);
                break;
            case EBin.op_binary.B_FRAME:
                frameNdx = EventEngineUtils.GetCharAnimFrame(this.gCur.go, this.eBin.getv());
                break;
            case EBin.op_binary.B_SPS:
                this.eBin.getv();
                this.eBin.getv();
                break;
            case EBin.op_binary.B_CURMP:
                frameNdx = (Int32)this._ff9.player[this.chr2slot(this.eBin.getv())].cur.mp;
                break;
            case EBin.op_binary.B_MAXMP:
                frameNdx = (Int32)this._ff9.player[this.chr2slot(this.eBin.getv())].max.mp;
                break;
            case EBin.op_binary.B_BGIID:
                Int16 triNdx2 = -1;
                BGI.BGI_charGetInfo(this.eBin.getv(), ref triNdx2, ref floorNdx1);
                frameNdx = (Int32)triNdx2;
                break;
            case EBin.op_binary.B_BGIFLOOR:
                Int16 floorNdx2 = -1;
                BGI.BGI_charGetInfo(this.eBin.getv(), ref triNdx1, ref floorNdx2);
                frameNdx = (Int32)floorNdx2;
                break;
            case EBin.op_binary.B_AND_LET_E:
            case EBin.op_binary.B_XOR_LET_E:
            case EBin.op_binary.B_OR_LET_E: 
            case EBin.op_binary.B_LET_E:
                frameNdx = this.OperatorExtractLet(code);
                break;
            case EBin.op_binary.B_LMAX:
            case EBin.op_binary.B_LMIN:
            case EBin.op_binary.B_NOT_E:
                frameNdx = this.OperatorExtract1(code);
                break;
            case EBin.op_binary.B_POST_PLUS_A:
            case EBin.op_binary.B_POST_MINUS_A:
            case EBin.op_binary.B_PRE_PLUS_A:
            case EBin.op_binary.B_PRE_MINUS_A:
                frameNdx = this.OperatorAll1(code);
                break;
            case EBin.op_binary.B_KEYON2:
            case EBin.op_binary.B_KEYOFF2:
            case EBin.op_binary.B_KEY2:
                this.eBin.getv();
                break;
            case EBin.op_binary.B_CURHP:
                frameNdx = (Int32) this._ff9.player[this.chr2slot(this.eBin.getv())].cur.hp;
                break;
            case EBin.op_binary.B_MAXHP:
                frameNdx = (Int32) this._ff9.player[this.chr2slot(this.eBin.getv())].max.hp;
                break;
        }
        return frameNdx;
    }
}