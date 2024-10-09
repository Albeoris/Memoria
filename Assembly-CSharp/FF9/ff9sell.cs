﻿using Memoria.Data;
using System;

namespace FF9
{
    public class ff9sell
    {
        public static void FF9Sell_Sell(RegularItem itemId, UInt16 sell_ct)
        {
            // Dummied (a similar code is in ShopUI.OnKeyConfirm)
            FF9ITEM_DATA itemData = ff9item._FF9Item_Data[itemId];
            Int32 sellingPrice = itemData.selling_price;
            if (sellingPrice < 0)
                return;
            Int32 sellingCount = ff9item.FF9Item_Remove(itemId, sell_ct);
            if (sellingCount > 0)
            {
                FF9StateSystem.Common.FF9.party.gil += (UInt32)(sellingPrice * sellingCount);
                if (FF9StateSystem.Common.FF9.party.gil > 9999999u)
                    FF9StateSystem.Common.FF9.party.gil = 9999999u;
            }
        }
    }
}
