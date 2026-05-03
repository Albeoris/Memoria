using System;

public class FF9FieldAttrState
{
    public FF9FieldAttrState()
    {
        this.ff9 = new UInt16[2, 4];
        this.field = new UInt16[2, 4];
        this.fmv = new UInt16[2, 4];
    }

    public const Int32 FF9FIELD_ATTR_STAGECOUNT = 4;

    public UInt16[,] ff9;

    public UInt16[,] field;

    public UInt16[,] fmv;
}
