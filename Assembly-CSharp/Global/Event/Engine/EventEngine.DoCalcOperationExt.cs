using System;

public partial class EventEngine
{
    public Int32 DoCalcOperationExt(Int32 code)
    {
        Int32 frameNdx = 0;
        Int16 triNdx1 = 0;
        Int16 floorNdx1 = 0;
        EBin.op_binary opBinary = (EBin.op_binary)code;
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
            default:
                switch (opBinary - 67)
                {
                    case EBin.op_binary.B_PAD0:
                    case EBin.op_binary.B_PAD1:
                    case EBin.op_binary.B_PAD2:
                        label_9:
                        frameNdx = this.OperatorExtractLet(code);
                        break;
                    case EBin.op_binary.B_POST_PLUS_A:
                    case EBin.op_binary.B_POST_MINUS_A:
                        label_8:
                        frameNdx = this.OperatorExtract1(code);
                        break;
                    default:
                        switch (opBinary - 8)
                        {
                            case EBin.op_binary.B_PAD0:
                            case EBin.op_binary.B_PAD1:
                            case EBin.op_binary.B_PAD2:
                            case EBin.op_binary.B_PAD3:
                                frameNdx = this.OperatorAll1(code);
                                break;
                            case EBin.op_binary.B_PRE_MINUS:
                                goto label_8;
                            default:
                                switch (opBinary - 90)
                                {
                                    case EBin.op_binary.B_PAD0:
                                    case EBin.op_binary.B_PAD1:
                                    case EBin.op_binary.B_PAD2:
                                        this.eBin.getv();
                                        break;
                                    default:
                                        if (opBinary != EBin.op_binary.B_CURHP)
                                        {
                                            if (opBinary != EBin.op_binary.B_MAXHP)
                                            {
                                                if (opBinary == EBin.op_binary.B_LET_E)
                                                    goto label_9;
                                                else
                                                    break;
                                            }
                                            else
                                            {
                                                frameNdx = (Int32)this._ff9.player[this.chr2slot(this.eBin.getv())].max.hp;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            frameNdx = (Int32)this._ff9.player[this.chr2slot(this.eBin.getv())].cur.hp;
                                            break;
                                        }
                                }
                                break;
                        }
                        break;
                }
                break;
        }
        return frameNdx;
    }
}