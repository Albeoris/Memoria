using System;

public static class ff9itemHelpers
{

    public static FF9ITEM FF9Item_GetPtr(Int32 id, FF9ITEM[] ff9Items)
    {
        FF9ITEM[] ff9ItemArray = ff9Items;
        for (Int32 index = 0; index < 256; ++index)
        {
            FF9ITEM ff9Item = ff9ItemArray[index];
            if (ff9Item?.count != 0 && ff9Item?.id == id)
                return ff9Item;
        }
        return null;
    }
}