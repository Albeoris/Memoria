using System;
using System.Linq;

public static class ff9itemHelpers
{
    public static FF9ITEM FF9Item_GetPtr(Int32 id, FF9ITEM[] ff9Items)
    {
        return ff9Items.FirstOrDefault(x => x?.count != 0 && x?.id == id);
    }
}