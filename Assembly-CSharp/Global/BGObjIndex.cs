using System;

public class BGObjIndex
{
    public BGObjIndex(Int32[] groupIndex, Int32[] materialIndex)
    {
        this.groupIndex = groupIndex;
        this.materialIndex = materialIndex;
    }

    public Int32[] groupIndex;

    public Int32[] materialIndex;
}
