using System;

public class ArrayUtil
{
    public static Int32 GetIndex(Int32 x, Int32 y, Int32 arrayW, Int32 arrayH)
    {
        return x + y * arrayW;
    }
}
