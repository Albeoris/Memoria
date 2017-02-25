using System;
using System.IO;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.Collections;
using Memoria.Prime.CSV;

namespace FF9
{
    public class ff9buy
    {
        public static readonly EntryCollection<ShopItems> ShopItems;

        static ff9buy()
        {
            ShopItems = LoadShopItems();
        }

        private static EntryCollection<ShopItems> LoadShopItems()
        {
            try
            {
                String inputPath = DataResources.Items.ShopItems;
                if (!File.Exists(inputPath))
                    throw new FileNotFoundException($"File with shop items not found: [{inputPath}]");

                ShopItems[] shopItems = CsvReader.Read<ShopItems>(inputPath);
                if (shopItems.Length < FF9BUY_SHOP_MAX)
                    throw new NotSupportedException($"You must set an assortment for {FF9BUY_SHOP_MAX} shops, but there {shopItems.Length}.");

                return EntryCollection.CreateWithDefaultElement(shopItems, e => e.Id);
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
            if (!ShopItems.TryGet(shopId, out assortiment))
                return ShopUI.ShopType.Synthesis;

            for (Int32 i = 0; i < assortiment.Length; i++)
            {
                Byte itemId = assortiment[i];
                Byte itemType = ff9item._FF9Item_Data[itemId].type;
                if ((itemType & FF9BUY_TYPE_WEAPON) == 0)
                    return ShopUI.ShopType.Item;
            }

            return ShopUI.ShopType.Weapon;
        }

        public const UInt16 FF9BUY_SHOP_MAX = 32;

        public const Byte FF9BUY_TYPE_WEAPON = 240;
    }
}