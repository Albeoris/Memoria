using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.Collections;
using Memoria.Prime.CSV;
using System;
using System.Collections.Generic;
using System.IO;

namespace FF9
{
    public class ff9buy
    {
        public static readonly Dictionary<Int32, ShopItems> ShopItems;

        static ff9buy()
        {
            ShopItems = LoadShopItems();
        }

        private static Dictionary<Int32, ShopItems> LoadShopItems()
        {
            try
            {
                String inputPath = DataResources.Items.PureDirectory + DataResources.Items.ShopItems;
                Dictionary<Int32, ShopItems> result = new Dictionary<Int32, ShopItems>();
                foreach (ShopItems[] shops in AssetManager.EnumerateCsvFromLowToHigh<ShopItems>(inputPath))
                    foreach (ShopItems shop in shops)
                        result[shop.Id] = shop;
                if (result.Count == 0)
                    throw new FileNotFoundException($"Cannot load shop items because a file does not exist: [{DataResources.Items.Directory + DataResources.Items.ShopItems}].", DataResources.Items.Directory + DataResources.Items.ShopItems);
                for (Int32 i = 0; i < 32; i++)
                    if (!result.ContainsKey(i))
                        throw new NotSupportedException($"You must define at least 32 shop items, with IDs between 0 and 31.");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9buy] Load shop items failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }

        public static ShopUI.ShopType FF9Buy_GetType(Int32 shopId)
        {
            ShopItems assortiment;
            if (!ShopItems.TryGetValue(shopId, out assortiment))
                return ShopUI.ShopType.Synthesis;

            for (Int32 i = 0; i < assortiment.Length; i++)
                if ((ff9item._FF9Item_Data[assortiment[i]].type & FF9BUY_TYPE_WEAPON) == 0)
                    return ShopUI.ShopType.Item;

            return ShopUI.ShopType.Weapon;
        }

        public const ItemType FF9BUY_TYPE_WEAPON = ItemType.Weapon | ItemType.Armlet | ItemType.Helmet | ItemType.Armor;
    }
}
